using System.IO;
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

    [Space]
    public GameObject CharacterPrefabBase;
    public AnimatorController CharacterAnimatorController;
    public string PhotoBoothSceneName;

    public string CharacterPrefabFolder => Path.Combine(characterFolder, characterPrefabFolderName);
    public string CharacterPreviewFolder => Path.Combine(characterFolder, characterPreviewFolderName);
    public string CharacterDataFolder => Path.Combine(characterFolder, characterDataFolderName);
}
