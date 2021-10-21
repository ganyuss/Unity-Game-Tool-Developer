using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterPreviewCreator : MonoBehaviour {
    [SerializeField]
    private GameObject characterPreview;
    [SerializeField]
    private Transform characterParent;
    [SerializeField]
    private Camera renderCamera;
    
#if UNITY_EDITOR
#region static interface
    public static void CreatePreview(GameObject previewTargetPrefab, string photoBoothSceneName, string targetFile) {
        Scene[] loadedScenes = GetAllLoadedScenes().ToArray();
        if (loadedScenes.Any(s => s.isDirty)) {
            if (EditorUtility.DisplayDialog("Edited scenes found", "Do you want to save the opened scenes ?", "Yes",
                "No")) {
                EditorSceneManager.SaveScenes(loadedScenes);
            }
        }
        string[] scenePaths = loadedScenes.Select(s => s.path).ToArray();
            
        Scene shootBoothScene = EditorSceneManager.OpenScene(photoBoothSceneName, OpenSceneMode.Single);
        GetComponentInScene<CharacterPreviewCreator>(shootBoothScene).InSceneCreatePreview(previewTargetPrefab, targetFile);

        // reload back the previous scenes
        EditorSceneManager.OpenScene(scenePaths.First(), OpenSceneMode.Single);
        for (int i = 1; i < loadedScenes.Length; i++) {
            EditorSceneManager.OpenScene(scenePaths[i], OpenSceneMode.Additive);
        }
    }
    
    private static T GetComponentInScene<T>(Scene scene, bool includeInactive = false) where T : UnityEngine.Object {
        
        foreach (var gameObject in scene.GetRootGameObjects()) {
            T component = gameObject.GetComponentInChildren<T>(includeInactive);

            if (component != null) {
                return component;
            }
        }

        return null;
    }

    private static IEnumerable<Scene> GetAllLoadedScenes() {
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            yield return SceneManager.GetSceneAt(i);
        }
    } 
#endregion
    

    private void InSceneCreatePreview(GameObject previewTargetPrefab, string targetFile) {
        characterPreview.SetActive(false);
        GameObject previewTarget = Instantiate(previewTargetPrefab, characterParent, false);
        previewTarget.transform.position = Vector3.zero;

        Texture2D outputTexture = RenderFrameToTexture();

        // For testing purposes, also write to a file in the project folder
        File.WriteAllBytes(targetFile, outputTexture.EncodeToPNG());

        DestroyImmediate(outputTexture);
        DestroyImmediate(previewTarget);
        characterPreview.SetActive(true);
        
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        ChangeImportSettings(targetFile);
    }

    // source: https://docs.unity3d.com/ScriptReference/Camera.Render.html
    private Texture2D RenderFrameToTexture() {
        // The Render Texture in RenderTexture.active is the one
        // that will be read by ReadPixels.
        var currentRT = RenderTexture.active;
        RenderTexture.active = renderCamera.targetTexture;

        // Render the camera's view.
        renderCamera.Render();

        // Make a new texture and read the active Render Texture into it.
        var targetTexture = renderCamera.targetTexture;
        Texture2D image = new Texture2D(targetTexture.width, targetTexture.height);
        image.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0);
        image.Apply();

        // Replace the original active Render Texture.
        RenderTexture.active = currentRT;

        return image;
    }
    
    private void ChangeImportSettings(string targetFile) {
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(targetFile);
        importer.textureType = TextureImporterType.Sprite;
        
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
    }
#endif
}
