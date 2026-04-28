using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Wishemons", fileName = "WishemonCard")]
public class WishemonCard : ScriptableObject
{
    [SerializeField] private string _name = String.Empty;
    [SerializeField] private GameObject _prefab = null;
    [SerializeField] private int _pv = 10;
    [SerializeField] private int _attack = 5;
    [SerializeField] private int _defense = 2;

    public string Name => _name;
    public GameObject Prefab => _prefab;
    public int Pv => _pv;
    public int Attack => _attack;
    public int Defense => _defense;
}
