using UnityEngine;
using UnityEngine.SceneManagement;

// Attacher sur un GameObject dans la scene Menu uniquement.
// Gere le chargement de la sauvegarde au demarrage.
public class GameBootstrap : MonoBehaviour
{
    private void Start()
    {
        if (SaveSystem.HasSave())
            SaveSystem.Load();
    }
}
