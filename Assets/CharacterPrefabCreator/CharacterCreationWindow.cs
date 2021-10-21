﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CharacterCreationWindow : EditorWindow {

    [MenuItem("Avatar Creator/Open Window")]
    public static void ShowWindow() {
        GetWindow<CharacterCreationWindow>().Show();
    }
    
    private GameObject characterRig;
    private GameObject characterModel;
    private Avatar characterAvatar;

    private Renderer modelRenderer;
    private Material[] materials;
    
    private GameObject characterBasePrefab;
    private string targetPrefabPath = "Assets/";
    
    void OnGUI() {

        EditorGUILayout.LabelField("Character", EditorStyles.boldLabel);
        characterRig = ObjectField("Character Rig", characterRig, false);
        EditorGUI.BeginChangeCheck();
        characterModel = ObjectField("Character Model", characterModel, false);
        if (EditorGUI.EndChangeCheck()) UpdateMaterialList();
        characterAvatar = ObjectField("Character Avatar", characterAvatar, false);
        
        EditorGUILayout.Space();

        if (materials != null) {
            EditorGUILayout.LabelField("Model's Materials", EditorStyles.boldLabel);

            for (int i = 0; i < materials.Length; i++) {
                materials[i] = ObjectField($"material {i}", materials[i], false);
            }
        }
        
        EditorGUILayout.Space();
        
        characterBasePrefab = ObjectField("Character Avatar", characterBasePrefab, false);
        targetPrefabPath = EditorGUILayout.TextField("Target prefab path", targetPrefabPath);
        
        GUI.enabled = IsCharacterCreationPossible();
        if (GUILayout.Button("Create Character")) {
            GameObject characterPrefabVariant = CreatePrefabVariant(characterBasePrefab, targetPrefabPath);
            characterPrefabVariant.GetComponent<Animator>().avatar = characterAvatar;

            AddGameObjectsAsChildrenToPrefab(characterPrefabVariant, characterRig, characterModel);

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
            });

        }
    }

    private void UpdateMaterialList() {
        if (characterModel is null) {
            modelRenderer = null;
            materials = null;
            
            return;
        }
        
        if (modelRenderer is null)
            modelRenderer = characterModel.GetComponent<Renderer>();

        int modelMaterialCount = modelRenderer.sharedMaterials.Length;
        if (materials == null) {
            materials = new Material[modelMaterialCount];
        }
        else if (materials.Length != modelMaterialCount) {
            Array.Resize(ref materials, modelMaterialCount);
        }
    }

    private GameObject CreatePrefabVariant(GameObject prefabBase, string targetPath) {
        GameObject prefabBaseInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefabBase);
        GameObject prefabVariant = PrefabUtility.SaveAsPrefabAsset(prefabBaseInstance, targetPath);
        DestroyImmediate(prefabBaseInstance);

        return prefabVariant;
    }

    private void AddGameObjectsAsChildrenToPrefab(GameObject prefab, params GameObject[] childrenToAdd) {
        GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        foreach (GameObject child in childrenToAdd) {
            GameObject childInstance = Instantiate(child, prefabInstance.transform);
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
        //DestroyImmediate(instance);
    }

    private bool IsCharacterCreationPossible() =>
        characterRig != null
        && characterModel != null
        && characterAvatar != null
        && characterBasePrefab != null;

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
}
