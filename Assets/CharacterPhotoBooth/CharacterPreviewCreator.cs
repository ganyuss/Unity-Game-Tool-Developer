using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CharacterPreviewCreator : MonoBehaviour {
    [SerializeField]
    private GameObject CharacterPreview;
    [SerializeField]
    private Transform CharacterParent;
    [SerializeField]
    private Camera RenderCamera;

    public void CreatePreview(GameObject previewTargetPrefab, string targetFile) {
        CharacterPreview.SetActive(false);
        GameObject previewTarget = Instantiate(previewTargetPrefab, CharacterParent, false);
        previewTarget.transform.position = Vector3.zero;

        Texture2D outputTexture = RenderFrameToTexture();

        // For testing purposes, also write to a file in the project folder
        File.WriteAllBytes(targetFile, outputTexture.EncodeToPNG());

        DestroyImmediate(outputTexture);
        //DestroyImmediate(previewTarget);
        CharacterPreview.SetActive(true);

        ChangeImportSettings(targetFile);
    }

    // source: https://docs.unity3d.com/ScriptReference/Camera.Render.html
    private Texture2D RenderFrameToTexture() {
        // The Render Texture in RenderTexture.active is the one
        // that will be read by ReadPixels.
        var currentRT = RenderTexture.active;
        RenderTexture.active = RenderCamera.targetTexture;

        // Render the camera's view.
        RenderCamera.Render();

        // Make a new texture and read the active Render Texture into it.
        var targetTexture = RenderCamera.targetTexture;
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
}
