using UnityEngine;

public class PNJ : MonoBehaviour
{
    [TextArea]
    public string dialogue = "Bonjour, je suis un PNJ !";

    [Header("Combat dresseur")]
    [SerializeField] private bool _isTrainer = false;
    [SerializeField] private string _trainerName = "Dresseur";
    [SerializeField] private WishemonCard _trainerWishemon = null;
    [SerializeField] private string _defeatDialogue = "Tu m'as battu !";

    private bool _defeated = false;

    public bool IsTrainer => _isTrainer;
    public string TrainerName => _trainerName;
    public WishemonCard TrainerWishemon => _trainerWishemon;
    public bool IsDefeated => _defeated;

    public void SetDefeated()
    {
        _defeated = true;
        dialogue = _defeatDialogue;
    }
}
