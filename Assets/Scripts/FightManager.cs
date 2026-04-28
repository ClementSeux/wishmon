using UnityEngine;

public class FightManager : MonoBehaviour
{
    private static FightManager _instance = null;
    public static FightManager Instance => _instance;

    [SerializeField] private Player _player = null;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    public WishemonCard GetPlayerWishemon()
    {
        if (_player == null)
        {
            Debug.LogWarning("[FightManager] Player pas assigne !");
            return null;
        }
        return _player.Starter;
    }

    public void StartFight(WishemonCard playerCard, WishemonCard wildCard)
    {
        // TODO : lancer la scene de combat
        // - placer les deux wishemons
        // - afficher le menu de combat
        // - gerer les tours
        Debug.Log($"Combat : {playerCard?.Name} vs {wildCard?.Name}");
    }
}
