using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    [SerializeField] private Player _player = null;
    [SerializeField] private Vector3 _offset = Vector3.zero;

    private void Update()
    {
        transform.position = _player.transform.position + _offset;
    }
}
