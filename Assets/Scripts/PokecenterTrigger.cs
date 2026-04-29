using UnityEngine;

public class PokecenterTrigger : MonoBehaviour
{
    [SerializeField] private string _message = "Tes Wishemons ont été soignés ! Bonne chance !";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Player>(out _)) return;
        PlayerTeam.Instance?.HealAll();
        SaveSystem.Save();
        DialogueManager.Instance?.ShowDialogue(_message);
    }
}
