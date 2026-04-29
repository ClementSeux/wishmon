using System;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    private const string KEY_HAS_SAVE = "wishmon_has_save";

    public static bool HasSave() => PlayerPrefs.GetInt(KEY_HAS_SAVE, 0) == 1;

    public static void Save()
    {
        var pt = PlayerTeam.Instance;
        if (pt == null) return;

        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);

        var team = pt.Team;
        PlayerPrefs.SetInt("team_count", team.Count);
        for (int i = 0; i < team.Count; i++)
        {
            PlayerPrefs.SetString($"team_{i}_name", team[i].Card.Name);
            PlayerPrefs.SetInt($"team_{i}_pv", team[i].CurrentPv);
        }

        var caught = pt.CaughtWishemon;
        PlayerPrefs.SetInt("caught_count", caught.Count);
        for (int i = 0; i < caught.Count; i++)
            PlayerPrefs.SetString($"caught_{i}", caught[i]);

        PlayerPrefs.SetInt("inv_pokeball", pt.GetItemCount("pokeball"));
        PlayerPrefs.SetInt("inv_potion", pt.GetItemCount("potion"));

        PlayerPrefs.Save();
    }

    public static void Load()
    {
        if (!HasSave()) return;
        var db = WishemonDatabase.Instance;
        if (db == null) { Debug.LogError("[SaveSystem] Pas de WishemonDatabase dans Resources !"); return; }

        var pt = PlayerTeam.Instance;
        if (pt == null) return;

        pt.Team.Clear();
        int count = PlayerPrefs.GetInt("team_count", 0);
        for (int i = 0; i < count; i++)
        {
            string name = PlayerPrefs.GetString($"team_{i}_name", "");
            int pv = PlayerPrefs.GetInt($"team_{i}_pv", -1);
            var card = db.GetByName(name);
            if (card == null) continue;
            var inst = new WishemonInstance(card);
            if (pv >= 0) inst.CurrentPv = pv;
            pt.Team.Add(inst);
        }

        pt.CaughtWishemon.Clear();
        int ccount = PlayerPrefs.GetInt("caught_count", 0);
        for (int i = 0; i < ccount; i++)
            pt.CaughtWishemon.Add(PlayerPrefs.GetString($"caught_{i}", ""));

        pt.SetItem("pokeball", PlayerPrefs.GetInt("inv_pokeball", 5));
        pt.SetItem("potion", PlayerPrefs.GetInt("inv_potion", 3));
    }

    public static void DeleteSave()
    {
        PlayerPrefs.DeleteAll();
    }
}
