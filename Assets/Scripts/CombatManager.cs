using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CombatState { Start, PlayerTurn, EnemyTurn, Busy, End }

public class CombatManager : MonoBehaviour
{
    private static CombatManager _instance;
    public static CombatManager Instance => _instance;

    [SerializeField] private CombatUI _ui;

    private WishemonInstance _player;
    private WishemonInstance _enemy;
    private CombatState _state;

    public WishemonInstance Player => _player;
    public WishemonInstance Enemy => _enemy;

    private void Awake()
    {
        if (_instance != null) { Destroy(gameObject); return; }
        _instance = this;
    }

    private void Start()
    {
        _player = CombatData.PlayerWishemon;
        if (_player == null)
        {
            Debug.LogError("[CombatManager] PlayerWishemon null ! Retour au monde.");
            SceneManager.LoadScene(CombatData.SceneToReturn);
            return;
        }
        _enemy = new WishemonInstance(CombatData.WildCard);

        AudioManager.Instance?.PlayCombatMusic();
        _ui.Setup(_player, _enemy);
        StartCoroutine(StartCombat());
    }

    private IEnumerator StartCombat()
    {
        _state = CombatState.Start;
        string intro = CombatData.IsTrainerBattle
            ? $"{CombatData.TrainerName} veut se battre !"
            : $"Un {_enemy.Card.Name} sauvage apparaît !";
        yield return _ui.ShowMessage(intro);
        yield return _ui.ShowMessage($"Allez, {_player.Card.Name} !");
        PlayerTurn();
    }

    private void PlayerTurn()
    {
        _state = CombatState.PlayerTurn;
        _ui.ShowActionMenu();
    }

    // --- Actions joueur ---

    public void OnAttack(int attackIndex)
    {
        if (_state != CombatState.PlayerTurn) return;
        StartCoroutine(PlayerAttackRoutine(attackIndex));
    }

    public void OnCapture()
    {
        if (_state != CombatState.PlayerTurn) return;
        if (CombatData.IsTrainerBattle)
        { StartCoroutine(RejectAndReturn("Impossible de capturer le wishemon d'un dresseur !")); return; }
        if (!PlayerTeam.Instance.HasItem("pokeball"))
        { StartCoroutine(RejectAndReturn("Tu n'as plus de Pokeball !")); return; }
        StartCoroutine(CaptureRoutine());
    }

    public void OnRun()
    {
        if (_state != CombatState.PlayerTurn) return;
        if (CombatData.IsTrainerBattle)
        { StartCoroutine(RejectAndReturn("Impossible de fuir un combat de dresseur !")); return; }
        StartCoroutine(RunRoutine());
    }

    public void OnUsePotion()
    {
        if (_state != CombatState.PlayerTurn) return;
        if (!PlayerTeam.Instance.HasItem("potion"))
        { StartCoroutine(RejectAndReturn("Tu n'as plus de Potion !")); return; }
        StartCoroutine(PotionRoutine());
    }

    // --- Routines ---

    private IEnumerator PlayerAttackRoutine(int idx)
    {
        _state = CombatState.Busy;
        _ui.HideMenus();

        if (idx >= _player.Card.Attacks.Length)
        { PlayerTurn(); yield break; }

        var atk = _player.Card.Attacks[idx];
        if (_player.CurrentPP[idx] <= 0)
        {
            yield return _ui.ShowMessage("Plus de PP pour cette attaque !");
            PlayerTurn();
            yield break;
        }
        _player.CurrentPP[idx]--;

        int dmg = CalcDamage(_player, _enemy, atk);
        _enemy.CurrentPv = Mathf.Max(0, _enemy.CurrentPv - dmg);
        yield return _ui.ShowMessage($"{_player.Card.Name} utilise {atk.Name} ! ({dmg} dégâts)");
        _ui.UpdateEnemyHP();

        if (_enemy.IsFainted) { yield return WinRoutine(); yield break; }

        yield return EnemyAttackRoutine();
    }

    private IEnumerator EnemyAttackRoutine()
    {
        _state = CombatState.EnemyTurn;

        if (_enemy.Card.Attacks.Length == 0)
        {
            yield return _ui.ShowMessage($"{_enemy.Card.Name} ne sait rien faire…");
        }
        else
        {
            int idx = Random.Range(0, _enemy.Card.Attacks.Length);
            var atk = _enemy.Card.Attacks[idx];
            int dmg = CalcDamage(_enemy, _player, atk);
            _player.CurrentPv = Mathf.Max(0, _player.CurrentPv - dmg);
            yield return _ui.ShowMessage($"{_enemy.Card.Name} utilise {atk.Name} ! ({dmg} dégâts)");
            _ui.UpdatePlayerHP();
        }

        if (_player.IsFainted)
        {
            if (PlayerTeam.Instance.IsAllFainted()) { yield return LoseRoutine(); yield break; }
        }

        PlayerTurn();
    }

    private IEnumerator CaptureRoutine()
    {
        _state = CombatState.Busy;
        _ui.HideMenus();
        PlayerTeam.Instance.UseItem("pokeball");

        yield return _ui.ShowMessage("Lance une Pokeball !");

        float hpRatio = (float)_enemy.CurrentPv / _enemy.Card.MaxPv;
        float chance = _enemy.Card.CatchRate * (1f - hpRatio * 0.5f);

        yield return new WaitForSeconds(0.8f);

        if (Random.value < chance)
        {
            bool added = PlayerTeam.Instance.AddToTeam(_enemy);
            string msg = added
                ? $"{_enemy.Card.Name} a été capturé !"
                : $"{_enemy.Card.Name} capturé, mais l'équipe est pleine !";
            yield return _ui.ShowMessage(msg);
            SaveSystem.Save();
            yield return new WaitForSeconds(0.5f);
            ReturnToWorld();
        }
        else
        {
            yield return _ui.ShowMessage($"{_enemy.Card.Name} s'échappe !");
            PlayerTurn();
        }
    }

    private IEnumerator RunRoutine()
    {
        _state = CombatState.Busy;
        _ui.HideMenus();
        yield return _ui.ShowMessage("Tu prends la fuite !");
        ReturnToWorld();
    }

    private IEnumerator PotionRoutine()
    {
        _state = CombatState.Busy;
        _ui.HideMenus();
        PlayerTeam.Instance.UseItem("potion");
        int heal = 20;
        _player.CurrentPv = Mathf.Min(_player.CurrentPv + heal, _player.Card.MaxPv);
        _ui.UpdatePlayerHP();
        yield return _ui.ShowMessage($"Utilise une Potion ! +{heal} PV");
        yield return EnemyAttackRoutine();
    }

    private IEnumerator WinRoutine()
    {
        _state = CombatState.End;
        yield return _ui.ShowMessage($"{_enemy.Card.Name} est K.O. !");
        if (CombatData.IsTrainerBattle)
            yield return _ui.ShowMessage($"Tu as battu {CombatData.TrainerName} !");
        SaveSystem.Save();
        yield return new WaitForSeconds(0.5f);
        ReturnToWorld();
    }

    private IEnumerator LoseRoutine()
    {
        _state = CombatState.End;
        yield return _ui.ShowMessage("Tous tes Wishemons sont K.O. !");
        yield return _ui.ShowMessage("Tu retournes au Pokecenter…");
        PlayerTeam.Instance.HealAll();
        SaveSystem.Save();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(CombatData.SceneToReturn);
    }

    private IEnumerator RejectAndReturn(string msg)
    {
        _state = CombatState.Busy;
        _ui.HideMenus();
        yield return _ui.ShowMessage(msg);
        PlayerTurn();
    }

    private void ReturnToWorld()
    {
        AudioManager.Instance?.PlayWorldMusic();
        SceneManager.LoadScene(CombatData.SceneToReturn);
    }

    private int CalcDamage(WishemonInstance attacker, WishemonInstance defender, Attack atk)
    {
        float raw = (float)atk.Power * attacker.Card.Attack / Mathf.Max(1, defender.Card.Defense);
        return Mathf.Max(1, Mathf.RoundToInt(raw * Random.Range(0.85f, 1f)));
    }
}
