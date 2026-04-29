using UnityEngine;
using UnityEngine.SceneManagement;

public class FightManager : MonoBehaviour
{
    private static FightManager _instance;
    public static FightManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null) { Destroy(gameObject); return; }
        _instance = this;
    }

    public void StartWildFight(WishemonCard wildCard)
    {
        var playerW = PlayerTeam.Instance?.GetFirstAlive();
        if (playerW == null) { Debug.LogWarning("[FightManager] Pas de wishemon disponible !"); return; }

        CombatData.PlayerWishemon = playerW;
        CombatData.WildCard = wildCard;
        CombatData.IsTrainerBattle = false;
        CombatData.SceneToReturn = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Combat");
    }

    public void StartTrainerFight(string trainerName, WishemonCard trainerWishemon)
    {
        var playerW = PlayerTeam.Instance?.GetFirstAlive();
        if (playerW == null) { Debug.LogWarning("[FightManager] Pas de wishemon disponible !"); return; }

        CombatData.PlayerWishemon = playerW;
        CombatData.WildCard = trainerWishemon;
        CombatData.IsTrainerBattle = true;
        CombatData.TrainerName = trainerName;
        CombatData.SceneToReturn = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Combat");
    }

    // Compat TallGrass
    public void StartFight(WishemonCard playerCard, WishemonCard wildCard) => StartWildFight(wildCard);
    public WishemonCard GetPlayerWishemon() => PlayerTeam.Instance?.GetFirstAlive()?.Card;
}
