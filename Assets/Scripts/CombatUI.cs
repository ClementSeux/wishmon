using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [Header("Joueur")]
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private Slider _playerHPBar;
    [SerializeField] private TextMeshProUGUI _playerHPText;
    [SerializeField] private Image _playerSprite;

    [Header("Ennemi")]
    [SerializeField] private TextMeshProUGUI _enemyName;
    [SerializeField] private Slider _enemyHPBar;
    [SerializeField] private TextMeshProUGUI _enemyHPText;
    [SerializeField] private Image _enemySprite;

    [Header("Message")]
    [SerializeField] private GameObject _messagePanel;
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private float _messageDelay = 1.4f;

    [Header("Menus")]
    [SerializeField] private GameObject _actionMenu;
    [SerializeField] private GameObject _attackMenu;

    [Header("Boutons attaques (4 max)")]
    [SerializeField] private Button[] _attackButtons = new Button[4];
    [SerializeField] private TextMeshProUGUI[] _attackLabels = new TextMeshProUGUI[4];

    private WishemonInstance _player;
    private WishemonInstance _enemy;

    public void Setup(WishemonInstance player, WishemonInstance enemy)
    {
        _player = player;
        _enemy = enemy;

        _playerName.text = player.Card.Name;
        _enemyName.text = enemy.Card.Name;

        if (player.Card.Sprite != null) _playerSprite.sprite = player.Card.Sprite;
        if (enemy.Card.Sprite != null) _enemySprite.sprite = enemy.Card.Sprite;

        _playerHPBar.maxValue = player.Card.MaxPv;
        _enemyHPBar.maxValue = enemy.Card.MaxPv;
        UpdatePlayerHP();
        UpdateEnemyHP();

        SetupAttackButtons();
        HideMenus();
        _messagePanel.SetActive(false);
    }

    private void SetupAttackButtons()
    {
        for (int i = 0; i < _attackButtons.Length; i++)
        {
            bool hasAtk = i < _player.Card.Attacks.Length;
            _attackButtons[i].gameObject.SetActive(hasAtk);
            if (hasAtk)
            {
                int idx = i;
                _attackLabels[i].text = _player.Card.Attacks[i].Name;
                _attackButtons[i].onClick.RemoveAllListeners();
                _attackButtons[i].onClick.AddListener(() => CombatManager.Instance.OnAttack(idx));
            }
        }
    }

    public void UpdatePlayerHP()
    {
        _playerHPBar.value = _player.CurrentPv;
        _playerHPText.text = $"{_player.CurrentPv}/{_player.Card.MaxPv}";
    }

    public void UpdateEnemyHP()
    {
        _enemyHPBar.value = _enemy.CurrentPv;
        _enemyHPText.text = $"{_enemy.CurrentPv}/{_enemy.Card.MaxPv}";
    }

    public void ShowActionMenu()
    {
        _messagePanel.SetActive(false);
        _actionMenu.SetActive(true);
        _attackMenu.SetActive(false);
    }

    public void ShowAttackMenu()
    {
        _actionMenu.SetActive(false);
        _attackMenu.SetActive(true);
    }

    public void HideMenus()
    {
        _actionMenu.SetActive(false);
        _attackMenu.SetActive(false);
        _messagePanel.SetActive(true);
    }

    public IEnumerator ShowMessage(string msg)
    {
        _actionMenu.SetActive(false);
        _attackMenu.SetActive(false);
        _messagePanel.SetActive(true);
        _messageText.text = msg;
        yield return new WaitForSeconds(_messageDelay);
    }

    // Appele depuis les boutons UI en Inspector
    public void OnClickAttaque() => ShowAttackMenu();
    public void OnClickRetour() => ShowActionMenu();
    public void OnClickCapture() => CombatManager.Instance.OnCapture();
    public void OnClickFuir() => CombatManager.Instance.OnRun();
    public void OnClickPotion() => CombatManager.Instance.OnUsePotion();
}
