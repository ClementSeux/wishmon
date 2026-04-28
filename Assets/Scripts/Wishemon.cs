using UnityEngine;

public class Wishemon : MonoBehaviour
{
    [SerializeField] private WishemonCard _card = null;

    public WishemonCard Card => _card;

    public void Initialize(WishemonCard card)
    {
        _card = card;
        GameObject model = Instantiate(_card.Prefab, transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
    }
}
