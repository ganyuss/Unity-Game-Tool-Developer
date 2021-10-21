using System.IO;
using EditorPlus;
using EditorPlus.Editor;
using UnityEditor.Animations;
using UnityEngine;

public class CharacterCreationParameters : ScriptableObject {

    public static CharacterCreationParameters Instance => AssetDatabaseUtils.GetSingle<CharacterCreationParameters>();

    [Header("Folders")]
    [SerializeField]
    private string characterPrefabFolderId;
    [SerializeField]
    private string characterPreviewFolderId;
    [SerializeField]
    private string characterDataFolderId;

    [Header("References")]
    public GameObject CharacterLogicPrefab;
    public AnimatorController CharacterAnimatorController;
    public string PhotoBoothSceneName;
    
    [Header("Export settings")]
    public bool IncludeNonUsedCharacterInSpreadsheet;
    [Dropdown(nameof(CreateDropdownList))]
    public string Separator = ",";

    private string _characterPrefabFolder;
    public string CharacterPrefabFolder => 
        _characterPrefabFolder ??
        (_characterPrefabFolder = FolderLocator.GetFolder(characterPrefabFolderId));
    
    
    private string _characterPreviewFolder;
    public string CharacterPreviewFolder =>
        _characterPreviewFolder ??
        (_characterPreviewFolder = FolderLocator.GetFolder(characterPreviewFolderId));

    private string _characterDataFolder;
    public string CharacterDataFolder => 
        _characterDataFolder ??
        (_characterDataFolder = FolderLocator.GetFolder(characterDataFolderId));

    private DropdownList<string> CreateDropdownList() {
        return new DropdownList<string> {
            ["Regular CSV (,)"] = ",",
            ["Excel (;)"] = ";",
        };
    }
}
