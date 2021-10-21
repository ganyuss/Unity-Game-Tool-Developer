using System.IO;
using EditorPlus;
using EditorPlus.Editor;
using UnityEditor.Animations;
using UnityEngine;

public class CharacterCreationParameters : ScriptableObject {

    public static CharacterCreationParameters Instance => AssetDatabaseUtils.GetSingle<CharacterCreationParameters>();
    
    [SerializeField]
    private string characterFolder = "Assets/";
    
    [Header("Folder Names")]
    [SerializeField]
    private string characterPrefabFolderName;
    [SerializeField]
    private string characterPreviewFolderName;
    [SerializeField]
    private string characterDataFolderName;

    [Header("References")]
    public GameObject CharacterLogicPrefab;
    public AnimatorController CharacterAnimatorController;
    public string PhotoBoothSceneName;
    
    [Header("Export settings")]
    public bool IncludeNonUsedCharacterInSpreadsheet;
    [Dropdown(nameof(CreateDropdownList))]
    public string Separator = ",";

    public string CharacterPrefabFolder => Path.Combine(characterFolder, characterPrefabFolderName);
    public string CharacterPreviewFolder => Path.Combine(characterFolder, characterPreviewFolderName);
    public string CharacterDataFolder => Path.Combine(characterFolder, characterDataFolderName);

    private DropdownList<string> CreateDropdownList() {
        return new DropdownList<string> {
            ["Regular CSV (,)"] = ",",
            ["Excel (;)"] = ";",
        };
    }
}
