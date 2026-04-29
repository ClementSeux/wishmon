using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private GameObject _newGameButton;

    private void Start()
    {
        bool hasSave = SaveSystem.HasSave();
        if (_continueButton != null) _continueButton.SetActive(hasSave);
        if (_newGameButton != null) _newGameButton.SetActive(true);
    }

    public void NewGame()
    {
        SaveSystem.DeleteSave();
        Time.timeScale = 1f;
        SceneManager.LoadScene("StarterSelection");
    }

    public void ContinueGame()
    {
        if (!SaveSystem.HasSave()) return;
        SaveSystem.Load();
        Time.timeScale = 1f;
        SceneManager.LoadScene("World");
    }

    // Legacy
    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StarterSelection");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("World");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
