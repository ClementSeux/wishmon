using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] public Canvas _pauseCanvas = null;
    [SerializeField] public InputActionReference _pauseRef = null;
    [SerializeField] public GameObject _firstSelected = null;

    private bool _isPaused = false;

    private void Start()
    {
        _pauseCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_pauseRef.action.WasPerformedThisFrame())
        {
            if (_isPaused) Resume();
            else Pause();
        }
    }

    private void Pause()
    {
        _isPaused = true;
        _pauseCanvas.gameObject.SetActive(true);
        Time.timeScale = 0f;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstSelected);
    }

    public void Resume()
    {
        _isPaused = false;
        _pauseCanvas.gameObject.SetActive(false);
        Time.timeScale = 1f;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
