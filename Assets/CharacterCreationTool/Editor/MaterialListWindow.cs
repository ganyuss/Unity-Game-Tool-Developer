using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EditorPlus.Editor;
using UnityEditor;
using UnityEngine;

public class MaterialListWindow : EditorWindow {
    private List<Material> materials;

    [MenuItem("TEst/test")]
    private static void a() {
        GetWindow<MaterialListWindow>().Show();
    }

    private static readonly string[] ExcludedPaths = {
        "Assets/Standard Assets/"
    };

    private static readonly string TargetShaderName = "Toon/Lit";

    void Awake() {
        RefreshMaterialList();
    }

    private void RefreshMaterialList() {
        materials = AssetDatabaseUtils.GetAll<Material>().Where(ShouldDisplayMaterial).ToList();
    }
    
    void OnGUI() {

        EditorGUILayout.LabelField("Toon material list", EditorStyles.boldLabel);

        for (int i = 0; i < materials.Count; i++) {
            materials[i] = MaterialField(materials[i]);
        }

        if (materials.Count == 0) {
            EditorGUILayout.LabelField($"No \"{TargetShaderName}\" related material found.");
        }
        
        if (GUILayout.Button("Refresh", GUILayout.ExpandWidth(false))) {
            RefreshMaterialList();
        }
    }


    private Material MaterialField(Material material) {
        EditorGUILayout.BeginHorizontal();
        GUI.enabled = false;
        EditorGUILayout.ObjectField(material, typeof(Material), false);
        GUI.enabled = true;
        material.color = EditorGUILayout.ColorField(material.color);
        EditorGUILayout.EndHorizontal();

        return material;
    }

    private bool ShouldDisplayMaterial(Material material) {
        if (material.shader.name != TargetShaderName)
            return false;

        string materialPath = AssetDatabase.GetAssetPath(material);
        return ExcludedPaths.All(excludedPath => !materialPath.Contains(excludedPath));
    }
}
