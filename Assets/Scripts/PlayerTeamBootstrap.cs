using UnityEngine;

// Garantit que PlayerTeam existe quand on lance directement depuis StarterSelection.
// Charge les 3 premiers wishemons de WishemonDatabase comme starters par défaut.
public class PlayerTeamBootstrap : MonoBehaviour
{
    private void Awake()
    {
        if (PlayerTeam.Instance != null) return;

        var go = new GameObject("PlayerTeam");
        var pt = go.AddComponent<PlayerTeam>();

        var db = WishemonDatabase.Instance;
        if (db != null && db.All.Length > 0)
        {
            int count = Mathf.Min(3, db.All.Length);
            var choices = new WishemonCard[count];
            for (int i = 0; i < count; i++)
                choices[i] = db.All[i];
            pt.SetStarterChoices(choices);
        }
        else
        {
            Debug.LogWarning("[PlayerTeamBootstrap] WishemonDatabase introuvable ou vide dans Resources.");
        }
    }
}
