using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance => _instance;

    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Musiques")]
    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private AudioClip _worldMusic;
    [SerializeField] private AudioClip _combatMusic;

    private void Awake()
    {
        if (_instance != null) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || _musicSource.clip == clip) return;
        _musicSource.clip = clip;
        _musicSource.loop = true;
        _musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null) _sfxSource.PlayOneShot(clip);
    }

    public void PlayMenuMusic() => PlayMusic(_menuMusic);
    public void PlayWorldMusic() => PlayMusic(_worldMusic);
    public void PlayCombatMusic() => PlayMusic(_combatMusic);
}
