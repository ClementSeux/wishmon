using UnityEngine;
using UnityEngine.UI;

// Affiche un modèle 3D wishemon dans une RawImage via RenderTexture.
// Placé dans la scène avec une Camera enfant et un WishemonCombatModel enfant.
public class WishemonModelViewer : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private WishemonCombatModel _model;
    [SerializeField] private RawImage _rawImage;

    public void Init(WishemonCard card)
    {
        if (card == null) return;

        var rt = new RenderTexture(512, 512, 16, RenderTextureFormat.ARGB32);
        rt.Create();
        _cam.targetTexture = rt;
        _rawImage.texture = rt;
        _model.Init(card);
    }
}
