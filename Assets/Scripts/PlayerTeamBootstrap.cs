using UnityEngine;

// Garantit que PlayerTeam existe quand StarterSelection se charge.
public class PlayerTeamBootstrap : MonoBehaviour
{
    [SerializeField] private GameObject _playerTeamPrefab;

    private void Awake()
    {
        if (PlayerTeam.Instance == null && _playerTeamPrefab != null)
            Instantiate(_playerTeamPrefab);
    }
}
