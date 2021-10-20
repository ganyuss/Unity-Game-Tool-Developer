using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreviewTest : EditorWindow
{
    [MenuItem("Preview test/test")]
    public static void ShowWindow() {
        GetWindow<PreviewTest>().Show();
    }

    private const string SceneName = "Assets/CharacterPhotoBooth/CharacterPhotoShootBooth.unity";

    private string targetFilePath = "Assets/";
    private GameObject objectToPreview;
    
    private void OnEnable() {
        
    }

    private void OnGUI() {

        targetFilePath = EditorGUILayout.TextField("Target file path", targetFilePath);
        objectToPreview = (GameObject)EditorGUILayout.ObjectField("Object to create preview of", objectToPreview, typeof(GameObject), false);
        
        if (GUILayout.Button("Create Preview")) {

            Scene[] loadedScenes = GetAllLoadedScenes().ToArray();
            
            if (loadedScenes.Any(s => s.isDirty)) {
                if (EditorUtility.DisplayDialog("Edited scenes found", "Do you want to save the opened scenes ?", "Yes",
                    "No")) {
                    EditorSceneManager.SaveScenes(loadedScenes);
                }
            }
            
            Scene shootBoothScene = EditorSceneManager.OpenScene(SceneName, OpenSceneMode.Single);
            GetComponentInScene<CharacterPreviewCreator>(shootBoothScene).CreatePreview(objectToPreview, targetFilePath);
        }
    }

    private T GetComponentInScene<T>(Scene scene, bool includeInactive = false) where T : UnityEngine.Object {
        
        foreach (var gameObject in scene.GetRootGameObjects()) {
            T component = gameObject.GetComponentInChildren<T>(includeInactive);

            if (component != null) {
                return component;
            }
        }

        return null;
    }

    private IEnumerable<Scene> GetAllLoadedScenes() {
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            yield return SceneManager.GetSceneAt(i);
        }
    } 
}
