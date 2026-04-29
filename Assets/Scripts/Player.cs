using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private CharacterController _controller = null;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private InputActionReference _moveRef = null;
    [SerializeField] private InputActionReference _interactRef = null;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private Transform _rayStartPoint = null;
    [SerializeField] private float _rayDistance = 1.2f;

    private bool _locked = false;

    private void Start()
    {
        if (PlayerTeam.Instance != null && !PlayerTeam.Instance.HasStarter)
            SceneManager.LoadScene("StarterSelection");
    }

    private void Update()
    {
        if (Time.timeScale == 0f || _locked)
        {
            if (_animator != null) _animator.SetBool("Walking", false);
            return;
        }
        UpdateMovement();
        UpdateInteraction();
    }

    private void UpdateMovement()
    {
        Vector2 move = _moveRef.action.ReadValue<Vector2>();
        bool walking = move.magnitude >= 0.1f;

        if (_animator != null) _animator.SetBool("Walking", walking);
        if (!walking) return;

        if (move.y >= 0.1f)       transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        else if (move.x >= 0.1f)  transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        else if (move.y <= -0.1f) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        else if (move.x <= -0.1f) transform.rotation = Quaternion.Euler(0f, 270f, 0f);

        _controller.SimpleMove(transform.forward * _speed);
    }

    private void UpdateInteraction()
    {
        if (!_interactRef.action.WasPerformedThisFrame()) return;

        // Fermer dialogue ouvert
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsOpen)
        {
            DialogueManager.Instance.HideDialogue();
            return;
        }

        if (_rayStartPoint == null) return;
        Ray ray = new Ray(_rayStartPoint.position, _rayStartPoint.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, _rayDistance)) return;

        PNJ pnj = hit.collider.GetComponent<PNJ>();
        if (pnj == null) return;

        if (pnj.IsTrainer && !pnj.IsDefeated && pnj.TrainerWishemon != null)
        {
            pnj.SetDefeated();
            FightManager.Instance.StartTrainerFight(pnj.TrainerName, pnj.TrainerWishemon);
        }
        else
        {
            DialogueManager.Instance?.ShowDialogue(pnj.dialogue);
        }
    }

    public void SetLocked(bool locked) => _locked = locked;
}
