using HomaGames.Internal.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class StoreItem
{
    public string Name;
    public int Price;
    public Sprite Icon;
    public GameObject Prefab;
}

public class Store : Singleton<Store>
{
    public List<StoreItem> StoreItems;
    public Action<StoreItem> OnItemSelected;

    [SerializeField]
    private CharacterList characterList;

    void Awake() {
        StoreItems.AddRange(characterList.Characters.Select(c => c.ToStoreItem()));
    }
    
    public void SelectItem(StoreItem item)
    {
        OnItemSelected?.Invoke(item);
    }
}
