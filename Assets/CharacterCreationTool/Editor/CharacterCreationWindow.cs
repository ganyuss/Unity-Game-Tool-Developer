using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EditorPlus.Editor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreationWindow : EditorWindow {


    
    private string characterName;
    private int characterPrice;
    
    private GameObject characterFBXAsset;

    private Renderer modelRenderer;
    private Material[] materials;

    void OnGUI() {

        characterName = EditorGUILayout.TextField("Character Name", characterName);
        characterPrice = EditorGUILayout.IntField("Character Price", characterPrice);
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("3D Model", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        characterFBXAsset = ObjectField("Character FBX Asset", characterFBXAsset, false);
        if (EditorGUI.EndChangeCheck()) UpdateMaterialList();
        
        EditorGUILayout.Space();

        if (materials != null) {
            EditorGUILayout.LabelField("Model's Materials", EditorStyles.boldLabel);

            for (int i = 0; i < materials.Length; i++) {
                materials[i] = ObjectField($"material {i}", materials[i], false);
            }
        }
        
        EditorGUILayout.Space();

        GUI.enabled = IsCharacterCreationPossible();
        if (GUILayout.Button("Create Character")) {

            var parameters = CharacterCreationParameters.Instance;
            var characterPrefabPath = Path.Combine(parameters.CharacterPrefabFolder, characterName + ".prefab");
            var characterPreviewPath = Path.Combine(parameters.CharacterPreviewFolder, characterName + ".png");
            var characterDataPath = Path.Combine(parameters.CharacterDataFolder, characterName + ".asset");
            
            GameObject characterPrefab = CreateCharacterPrefab(parameters.CharacterPrefabBase,
                characterPrefabPath, parameters);
            
            CharacterPreviewCreator.CreatePreview(characterPrefab, parameters.PhotoBoothSceneName,
                characterPreviewPath);
            Sprite characterPreview = AssetDatabase.LoadAssetAtPath<Sprite>(characterPreviewPath);

            CharacterData characterData = CreateInstance<CharacterData>();
            characterData.Icon = characterPreview;
            characterData.Prefab = characterPrefab;
            characterData.Price = characterPrice;
            
            AssetDatabase.CreateAsset(characterData, characterDataPath);
            
            var characterList = AssetDatabaseUtils.GetSingle<CharacterList>();
            if (characterList != null) {
                characterList.Characters.Add(characterData);
                EditorUtility.SetDirty(characterList);
            }
        }
    }

    private void UpdateMaterialList() {
        if (characterFBXAsset is null) {
            modelRenderer = null;
            materials = null;
            
            return;
        }
        
        if (modelRenderer is null)
            modelRenderer = characterFBXAsset.GetComponentInChildren<Renderer>();

        int modelMaterialCount = modelRenderer.sharedMaterials.Length;
        if (materials == null) {
            materials = new Material[modelMaterialCount];
        }
        else if (materials.Length != modelMaterialCount) {
            Array.Resize(ref materials, modelMaterialCount);
        }
    }
    
#region Prefab Creation
    private GameObject CreateCharacterPrefab(GameObject logicGameObject, string targetPrefabPath, CharacterCreationParameters parameters) {
        GameObject characterPrefabVariant = CreateSeparatePrefab(characterFBXAsset, targetPrefabPath);

        AddGameObjectsAsChildrenToPrefab(characterPrefabVariant, targetPrefabPath, logicGameObject);

        EditPrefabValue(characterPrefabVariant, prefab => {
            Renderer renderer = prefab.GetComponentInChildren<Renderer>();
                
            // We must use a cloned array, because Renderer.sharedMaterials returns a copy of the array.
            // source: bottom of https://docs.unity3d.com/ScriptReference/Renderer-sharedMaterials.html
            Material[] materialsClone = new Material[materials.Length];
            Array.Copy(materials, materialsClone, materials.Length);
                
            renderer.sharedMaterials = materialsClone;

            if (renderer is SkinnedMeshRenderer skinnedMeshRenderer) {
                skinnedMeshRenderer.rootBone = FindRecursive(prefab.transform, skinnedMeshRenderer.rootBone.name);
            }

            prefab.GetComponentInChildren<Animator>().runtimeAnimatorController =
                parameters.CharacterAnimatorController;
        });

        return characterPrefabVariant;
    }

    private GameObject CreateSeparatePrefab(GameObject prefabBase, string targetPrefabPath) {
        GameObject prefabBaseInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefabBase);
        PrefabUtility.UnpackPrefabInstance(prefabBaseInstance, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
        GameObject prefabVariant = PrefabUtility.SaveAsPrefabAsset(prefabBaseInstance, targetPrefabPath);
        DestroyImmediate(prefabBaseInstance);

        return prefabVariant;
    }

    private void AddGameObjectsAsChildrenToPrefab(GameObject prefab, string targetPrefabPath, params GameObject[] prefabChildrenToAdd) {
        GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        foreach (GameObject child in prefabChildrenToAdd) {
            GameObject childInstance = (GameObject)PrefabUtility.InstantiatePrefab(child, prefabInstance.transform);
            PrefabUtility.ApplyAddedGameObject(childInstance, targetPrefabPath, InteractionMode.AutomatedAction);
        }
        
        DestroyImmediate(prefabInstance);
    }

    /// <summary>
    /// This method can be used to edit a prefab's properties.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="valuesModifier"></param>
    private void EditPrefabValue(GameObject prefab, Action<GameObject> valuesModifier) {
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        valuesModifier.Invoke(instance);
        
        PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);
        DestroyImmediate(instance);
    }

    private bool IsCharacterCreationPossible() =>
        characterFBXAsset != null;

    /// <summary>
    /// Works just like <see cref="Transform.Find">Transform.Find(string)</see>, but recursively.
    /// </summary>
    /// <param name="parent">The transform to start from.</param>
    /// <param name="targetName">the name of the transform to find, as child of parent.</param>
    /// <returns>A child transform of parent with the name targetName, or null if none can be found.</returns>
    private Transform FindRecursive(Transform parent, string targetName) {
        List<Transform> transformsToLookAt = GetAllDirectChildrenOf(parent);

        while (transformsToLookAt.Count > 0) {
            foreach (Transform child in transformsToLookAt) {
                if (child.name == targetName)
                    return child;
            }

            List<Transform> nextLayer = new List<Transform>();
            foreach (Transform child in transformsToLookAt) {
                nextLayer.AddRange(GetAllDirectChildrenOf(child));
            }

            transformsToLookAt = nextLayer;
        }

        return null;
    }

    private List<Transform> GetAllDirectChildrenOf(Transform parent) =>
        Enumerable.Range(0, parent.childCount).Select(parent.GetChild).ToList();

        private T ObjectField<T>(string fieldLabel, T obj, bool allowSceneObjects) where T : UnityEngine.Object {
        return (T) EditorGUILayout.ObjectField(fieldLabel, obj, typeof(T), allowSceneObjects);
    }
#endregion
}
