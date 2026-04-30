using System.Collections.Generic;
using UnityEngine;

public class PlayerTeam : MonoBehaviour
{
    private static PlayerTeam _instance;
    public static PlayerTeam Instance => _instance;

    [Header("Starters disponibles (3)")]
    [SerializeField] private WishemonCard[] _starterChoices = new WishemonCard[3];

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (_instance != null) return;
        var go = new GameObject("PlayerTeam [auto]");
        var pt = go.AddComponent<PlayerTeam>();
        // Starters chargés depuis la DB après le chargement de scène
        DontDestroyOnLoad(go);
        // Charge les starters depuis WishemonDatabase (dispo après BeforeSceneLoad)
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += pt.OnFirstSceneLoaded;
    }

    private void OnFirstSceneLoaded(UnityEngine.SceneManagement.Scene scene,
        UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnFirstSceneLoaded;
        if (_starterChoices == null || _starterChoices.Length == 0 ||
            System.Array.TrueForAll(_starterChoices, c => c == null))
        {
            var db = WishemonDatabase.Instance;
            if (db != null && db.All.Length > 0)
            {
                int count = Mathf.Min(3, db.All.Length);
                _starterChoices = new WishemonCard[count];
                for (int i = 0; i < count; i++)
                    _starterChoices[i] = db.All[i];
            }
        }
    }

    public List<WishemonInstance> Team { get; } = new List<WishemonInstance>();
    public List<string> CaughtWishemon { get; } = new List<string>();
    public WishemonCard[] StarterChoices => _starterChoices;
    public bool HasStarter => Team.Count > 0;

    private Dictionary<string, int> _inventory = new Dictionary<string, int>();

    private void Awake()
    {
        if (_instance != null) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        _inventory["pokeball"] = 5;
        _inventory["potion"] = 3;
    }

    public void ChooseStarter(int index)
    {
        if (index < 0 || index >= _starterChoices.Length || _starterChoices[index] == null) return;
        Team.Clear();
        CaughtWishemon.Clear();
        var inst = new WishemonInstance(_starterChoices[index]);
        Team.Add(inst);
        if (!CaughtWishemon.Contains(inst.Card.Name))
            CaughtWishemon.Add(inst.Card.Name);
        SaveSystem.Save();
    }

    public WishemonInstance GetFirstAlive()
    {
        foreach (var w in Team)
            if (!w.IsFainted) return w;
        return null;
    }

    public bool AddToTeam(WishemonInstance w)
    {
        if (Team.Count >= 6) return false;
        Team.Add(w);
        if (!CaughtWishemon.Contains(w.Card.Name))
            CaughtWishemon.Add(w.Card.Name);
        return true;
    }

    public bool IsAllFainted()
    {
        if (Team.Count == 0) return false;
        foreach (var w in Team)
            if (!w.IsFainted) return false;
        return true;
    }

    public void HealAll()
    {
        foreach (var w in Team) w.Heal();
    }

    // Inventaire
    public int GetItemCount(string item) => _inventory.TryGetValue(item, out int v) ? v : 0;

    public bool HasItem(string item) => GetItemCount(item) > 0;

    public bool UseItem(string item)
    {
        if (!HasItem(item)) return false;
        _inventory[item]--;
        return true;
    }

    public void AddItem(string item, int count = 1)
    {
        if (_inventory.ContainsKey(item)) _inventory[item] += count;
        else _inventory[item] = count;
    }

    public void SetItem(string item, int count) => _inventory[item] = count;

    public void SetStarterChoices(WishemonCard[] choices) => _starterChoices = choices;
}
