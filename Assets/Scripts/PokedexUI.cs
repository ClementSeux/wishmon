using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PokedexUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Transform _listContainer;
    [SerializeField] private GameObject _entryPrefab;
    [SerializeField] private InputActionReference _toggleRef;

    private bool _open;

    private void Update()
    {
        if (_toggleRef != null && _toggleRef.action.WasPerformedThisFrame())
            Toggle();
    }

    public void Toggle()
    {
        _open = !_open;
        _panel.SetActive(_open);
        if (_open) Refresh();
        Time.timeScale = _open ? 0f : 1f;
    }

    public void Close()
    {
        _open = false;
        _panel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void Refresh()
    {
        foreach (Transform t in _listContainer)
            Destroy(t.gameObject);

        var caught = PlayerTeam.Instance?.CaughtWishemon;
        if (caught == null) return;

        var db = WishemonDatabase.Instance;
        foreach (var name in caught)
        {
            var go = Instantiate(_entryPrefab, _listContainer);
            var label = go.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                var card = db?.GetByName(name);
                label.text = card != null
                    ? $"◆ {name}  [{card.Type}]  PV:{card.MaxPv}"
                    : $"◆ {name}";
            }
        }
    }
}
