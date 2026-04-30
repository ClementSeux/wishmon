using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StarterSelectionManager : MonoBehaviour
{
    [SerializeField] private Button[] _buttons = new Button[3];
    [SerializeField] private Image[] _sprites = new Image[3];       // gardé pour compatibilité
    [SerializeField] private TextMeshProUGUI[] _names = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private WishemonCombatModel[] _models = new WishemonCombatModel[3];

    private void Start()
    {
        var team = PlayerTeam.Instance;
        if (team == null) return;

        for (int i = 0; i < _buttons.Length; i++)
        {
            int idx = i;
            var card = i < team.StarterChoices.Length ? team.StarterChoices[i] : null;

            if (card == null) { _buttons[i].gameObject.SetActive(false); continue; }

            if (i < _models.Length && _models[i] != null)
                _models[i].Init(card);

            _names[i].text = card.Name;
            _buttons[i].onClick.AddListener(() => Choose(idx));

            var trigger = _buttons[i].gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            var entry = new UnityEngine.EventSystems.EventTrigger.Entry
                { eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter };
            entry.callback.AddListener(_ => ShowInfo(team.StarterChoices[idx]));
            trigger.triggers.Add(entry);
        }
    }

    private void ShowInfo(WishemonCard card)
    {
        if (_infoText == null || card == null) return;
        string atks = card.Attacks != null && card.Attacks.Length > 0
            ? string.Join("  /  ", System.Array.ConvertAll(card.Attacks, a => a.Name))
            : "—";
        _infoText.text = $"{card.Name}   •   Type : {card.Type}   •   PV : {card.MaxPv}   •   ATK : {card.Attack}   •   Attaques : {atks}";
    }

    private void Choose(int idx)
    {
        PlayerTeam.Instance.ChooseStarter(idx);
        SceneManager.LoadScene("World");
    }
}
