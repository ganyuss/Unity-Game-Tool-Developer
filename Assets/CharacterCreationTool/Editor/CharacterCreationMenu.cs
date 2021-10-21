using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EditorPlus.Editor;
using UnityEditor;
using UnityEngine;

public class CharacterCreationMenu
{
    private const string MenuTitle = "Character Creator/";
    
    [MenuItem(MenuTitle + "Open Window")]
    private static void ShowWindow() {
        EditorWindow.GetWindow<CharacterCreationWindow>().Show();
    }
    
    [MenuItem(MenuTitle + "Open Settings")]
    private static void OpenSettings() {
        Selection.activeObject = AssetDatabaseUtils.GetSingle<CharacterCreationParameters>();
    }

    [MenuItem(MenuTitle + "Export Spreadsheet")]
    private static void ExportSpreadsheet() {
        List<CharacterData> charactersToExport;
        var parameters = CharacterCreationParameters.Instance;

        if (parameters.IncludeNonUsedCharacterInSpreadsheet) {
            charactersToExport = AssetDatabaseUtils.GetAll<CharacterData>().ToList();
        }
        else {
            charactersToExport = AssetDatabaseUtils.GetSingle<CharacterList>()?.Characters;

            if (charactersToExport is null) {
                Debug.LogError("Error while exporting characters: Could not find the character list.");
                return;
            }
        }

        string spreadsheetPath = EditorUtility.SaveFilePanel("Spreadsheet file location", Application.dataPath,
            "Character list.csv", "csv");

        if (string.IsNullOrEmpty(spreadsheetPath)) return;

        File.WriteAllLines(spreadsheetPath,
            charactersToExport.Select(c => string.Join(parameters.Separator, new[] { $"\"{c.name}\"", c.Price.ToString() })));
    }
}
