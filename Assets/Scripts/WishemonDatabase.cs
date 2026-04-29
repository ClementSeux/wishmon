using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Wishmon/Database", fileName = "WishemonDatabase")]
public class WishemonDatabase : ScriptableObject
{
    [SerializeField] private WishemonCard[] _all = Array.Empty<WishemonCard>();

    private static WishemonDatabase _instance;
    public static WishemonDatabase Instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<WishemonDatabase>("WishemonDatabase");
            return _instance;
        }
    }

    public WishemonCard[] All => _all;

    public WishemonCard GetByName(string name)
    {
        foreach (var c in _all)
            if (c != null && c.Name == name) return c;
        return null;
    }
}
