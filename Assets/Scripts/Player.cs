using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private CharacterController _controller = null;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private InputActionReference _moveRef = null;
    [SerializeField] private InputActionReference _interactRef = null;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private Transform _rayStartPoint = null;
    [SerializeField] private float _rayDistance = 1f;

    [SerializeField] private Wishemon _wishemonPrefab = null;
    [SerializeField] private WishemonCard _starter = null;

    public WishemonCard Starter => _starter;

    [Header("Wishemon qui suit le joueur")]
    [SerializeField] private Vector3 _wishemonOffset = new Vector3(0f, 0f, -2.5f);
    [SerializeField, Range(0.05f, 1f)] private float _wishemonScale = 0.25f;

    private void Start()
    {
        Wishemon w = Instantiate(_wishemonPrefab, transform);
        w.transform.localPosition = _wishemonOffset;
        w.transform.localScale = Vector3.one * _wishemonScale;
        w.Initialize(_starter);
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            _animator.SetBool("Walking", false);
            return;
        }

        UpdateMovement();
        UpdateInteraction();
    }

    private void UpdateMovement()
    {
        Vector2 move = _moveRef.action.ReadValue<Vector2>();
        bool isWalking = move.magnitude >= 0.1f;

        _animator.SetBool("Walking", isWalking);

        if (!isWalking) return;

        if (move.y >= 0.1f)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        else if (move.x >= 0.1f)
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        else if (move.y <= -0.1f)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        else if (move.x <= -0.1f)
            transform.rotation = Quaternion.Euler(0f, 270f, 0f);

        _controller.SimpleMove(transform.forward * _speed);
    }

    private void UpdateInteraction()
    {
        if (!_interactRef.action.WasPerformedThisFrame()) return;

        Ray ray = new Ray(_rayStartPoint.position, _rayStartPoint.forward);
        if (Physics.Raycast(ray, _rayDistance))
            Debug.Log("Touche !");
    }
}
