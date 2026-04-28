using System.Collections.Generic;
using UnityEngine;

public class TallGrass : MonoBehaviour
{
    [Header("Parametres encounter")]
    [SerializeField] private float _checkInterval = 0.5f;
    [SerializeField] private float _probabilityIncrement = 0.05f;
    [SerializeField, Range(0f, 1f)] private float _maxProbability = 1f;
    [SerializeField] private List<WishemonCard> _potentialWishemons = null;

    private float _currentProbability = 0f;
    private float _checkTimer = 0f;
    private bool _isPlayerInside = false;

    private void Update()
    {
        if (_isPlayerInside)
        {
            _checkTimer += Time.deltaTime;
            if (_checkTimer >= _checkInterval)
            {
                _checkTimer -= _checkInterval;
                TryEncounter();
            }
        }
        else if (_currentProbability > 0f || _checkTimer > 0f)
        {
            _currentProbability = 0f;
            _checkTimer = 0f;
        }
    }

    private void TryEncounter()
    {
        _currentProbability = Mathf.Min(_currentProbability + _probabilityIncrement, _maxProbability);

        if (Random.value >= _currentProbability) return;

        int idx = Random.Range(0, _potentialWishemons.Count);
        WishemonCard wildCard = _potentialWishemons[idx];
        _currentProbability = 0f;

        Debug.Log("Combat ! Rencontre avec " + wildCard.Name);
        FightManager.Instance.StartFight(FightManager.Instance.GetPlayerWishemon(), wildCard);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player _))
            _isPlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player _))
            _isPlayerInside = false;
    }
}
