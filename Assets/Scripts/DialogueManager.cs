using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioClip _openClip;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        _panel.SetActive(false);
    }

    public void ShowDialogue(string message)
    {
        IsOpen = true;
        _panel.SetActive(true);
        _text.text = message;
        if (_openClip && _sfxSource) _sfxSource.PlayOneShot(_openClip);
    }

    public void HideDialogue()
    {
        IsOpen = false;
        _panel.SetActive(false);
    }
}
