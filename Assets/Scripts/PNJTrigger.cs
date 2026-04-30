using UnityEngine;

// Ajoute ce composant + un Collider Trigger autour du PNJ.
// Quand le joueur s'approche : dialogue ou combat automatique.
[RequireComponent(typeof(Collider))]
public class PNJTrigger : MonoBehaviour
{
    [SerializeField] private PNJ _pnj;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        if (_pnj == null)
            _pnj = GetComponentInParent<PNJ>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Player>(out _)) return;
        if (_pnj == null) return;

        if (_pnj.IsTrainer && !_pnj.IsDefeated && _pnj.TrainerWishemon != null)
        {
            // Regarder le joueur avant le combat
            Vector3 dir = other.transform.position - _pnj.transform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
                _pnj.transform.rotation = Quaternion.LookRotation(dir);

            _pnj.SetDefeated();
            FightManager.Instance?.StartTrainerFight(_pnj.TrainerName, _pnj.TrainerWishemon);
        }
        else if (!_pnj.IsTrainer)
        {
            DialogueManager.Instance?.ShowDialogue(_pnj.dialogue);
        }
    }
}
