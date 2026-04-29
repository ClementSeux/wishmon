using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StarterSelectionManager : MonoBehaviour
{
    [SerializeField] private Button[] _buttons = new Button[3];
    [SerializeField] private Image[] _sprites = new Image[3];
    [SerializeField] private TextMeshProUGUI[] _names = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI _infoText;

    private void Start()
    {
        var team = PlayerTeam.Instance;
        if (team == null) return;

        for (int i = 0; i < _buttons.Length; i++)
        {
            int idx = i;
            var card = i < team.StarterChoices.Length ? team.StarterChoices[i] : null;

            if (card == null) { _buttons[i].gameObject.SetActive(false); continue; }

            if (card.Sprite != null) _sprites[i].sprite = card.Sprite;
            _names[i].text = card.Name;
            _buttons[i].onClick.AddListener(() => Choose(idx));
            int captured = i; // for hover info
            var trigger = _buttons[i].gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            var entry = new UnityEngine.EventSystems.EventTrigger.Entry
                { eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter };
            entry.callback.AddListener(_ => ShowInfo(team.StarterChoices[captured]));
            trigger.triggers.Add(entry);
        }
    }

    private void ShowInfo(WishemonCard card)
    {
        if (_infoText == null || card == null) return;
        _infoText.text = $"{card.Name}  |  Type : {card.Type}  |  PV : {card.MaxPv}  |  ATK : {card.Attack}";
    }

    private void Choose(int idx)
    {
        PlayerTeam.Instance.ChooseStarter(idx);
        SceneManager.LoadScene("World");
    }
}
