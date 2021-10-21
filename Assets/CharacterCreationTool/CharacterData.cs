using EditorPlus;
#if UNITY_EDITOR
using EditorPlus.Editor;
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu]
public class CharacterData : ScriptableObject
{
    public int Price;
    public Sprite Icon;
    public GameObject Prefab;
    
    public StoreItem ToStoreItem() {
        return new StoreItem {
            Name = name, 
            Price = Price, 
            Icon = Icon, 
            Prefab = Prefab
        };
    }
    
#if UNITY_EDITOR
    [Button("Delete Character Completely")]
    private void DeleteCharacterCompletely() {
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(Icon));
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(Prefab));

        AssetDatabaseUtils.GetSingle<CharacterList>()?.Characters.Remove(this);
        
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this));
    }
#endif
}
