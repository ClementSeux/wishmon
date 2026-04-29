using UnityEngine;

public enum AttackType { Normal, Feu, Eau, Plante, Electrik, Roche, Spectre }

[CreateAssetMenu(menuName = "Wishmon/Attaque", fileName = "NewAttaque")]
public class Attack : ScriptableObject
{
    [SerializeField] private string _name = "Charge";
    [SerializeField] private AttackType _type = AttackType.Normal;
    [SerializeField] private int _power = 40;
    [SerializeField] private int _maxPP = 35;
    [SerializeField, TextArea] private string _description = "";

    public string Name => _name;
    public AttackType Type => _type;
    public int Power => _power;
    public int MaxPP => _maxPP;
    public string Description => _description;
}
