using UnityEngine;

// Ajoute ce composant directement sur le PNJ.
// Il crée automatiquement la zone de déclenchement et lance le combat.
public class PNJTrainer : MonoBehaviour
{
    [Header("Dresseur")]
    [SerializeField] private string _trainerName = "Dresseur";
    [SerializeField] private WishemonCard _trainerWishemon;
    [SerializeField] private float _triggerRadius = 2.5f;

    [Header("Dialogue après défaite")]
    [SerializeField] private string _defeatDialogue = "Tu m'as battu !";

    private bool _defeated = false;
    private SphereCollider _trigger;

    private void Awake()
    {
        // Crée le trigger sur ce même objet
        _trigger = gameObject.GetComponent<SphereCollider>();
        if (_trigger == null)
            _trigger = gameObject.AddComponent<SphereCollider>();
        _trigger.isTrigger = true;
        _trigger.radius = _triggerRadius;
        _trigger.center = new Vector3(0, 1f, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_defeated) return;
        if (_trainerWishemon == null) return;

        Player player = other.GetComponent<Player>() ?? other.GetComponentInParent<Player>();
        if (player == null) return;

        // Regarder le joueur
        Vector3 dir = other.transform.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        _defeated = true;

        if (FightManager.Instance == null)
        {
            Debug.LogError("[PNJTrainer] FightManager introuvable dans la scène !");
            return;
        }

        FightManager.Instance.StartTrainerFight(_trainerName, _trainerWishemon);
    }
}
