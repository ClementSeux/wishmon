using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PNJTrigger : MonoBehaviour
{
    [SerializeField] private PNJ _pnj;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        if (_pnj == null)
            _pnj = GetComponentInParent<PNJ>();

        if (_pnj == null)
            Debug.LogError($"[PNJTrigger] Pas de composant PNJ trouvé sur {transform.parent?.name}", this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[PNJTrigger] OnTriggerEnter avec {other.gameObject.name}");

        // Cherche Player sur le GO lui-même OU sur sa racine
        Player player = other.GetComponent<Player>() ?? other.GetComponentInParent<Player>();
        if (player == null)
        {
            Debug.Log($"[PNJTrigger] Pas de composant Player sur {other.gameObject.name}");
            return;
        }

        if (_pnj == null)
        {
            Debug.LogError("[PNJTrigger] _pnj est null !");
            return;
        }

        Debug.Log($"[PNJTrigger] PNJ={_pnj.name} IsTrainer={_pnj.IsTrainer} IsDefeated={_pnj.IsDefeated} Wishemon={_pnj.TrainerWishemon}");

        if (_pnj.IsTrainer && !_pnj.IsDefeated && _pnj.TrainerWishemon != null)
        {
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
        else
        {
            Debug.LogWarning($"[PNJTrigger] Combat non déclenché : IsTrainer={_pnj.IsTrainer} IsDefeated={_pnj.IsDefeated} Wishemon={_pnj.TrainerWishemon}");
        }
    }
}
