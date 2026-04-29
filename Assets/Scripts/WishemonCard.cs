using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Wishmon/WishemonCard", fileName = "WishemonCard")]
public class WishemonCard : ScriptableObject
{
    [SerializeField] private string _name = String.Empty;
    [SerializeField] private GameObject _prefab = null;
    [SerializeField] private Sprite _sprite = null;
    [SerializeField] private AttackType _type = AttackType.Normal;
    [SerializeField] private int _maxPv = 30;
    [SerializeField] private int _attack = 10;
    [SerializeField] private int _defense = 5;
    [SerializeField] private int _speed = 5;
    [SerializeField, Range(0.1f, 1f)] private float _catchRate = 0.5f;
    [SerializeField] private Attack[] _attacks = Array.Empty<Attack>();

    public string Name => _name;
    public GameObject Prefab => _prefab;
    public Sprite Sprite => _sprite;
    public AttackType Type => _type;
    public int MaxPv => _maxPv;
    public int Attack => _attack;
    public int Defense => _defense;
    public int Speed => _speed;
    public float CatchRate => _catchRate;
    public Attack[] Attacks => _attacks;

    // Compat avec ancien code
    public int Pv => _maxPv;
}
