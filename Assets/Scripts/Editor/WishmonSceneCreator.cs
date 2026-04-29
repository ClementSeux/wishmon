// Script éditeur - accessible via le menu Unity "Wishmon/..."
// Lance-le une seule fois pour créer les scènes Combat et StarterSelection.
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public static class WishmonSceneCreator
{
    // ==================== COMBAT ====================
    [MenuItem("Wishmon/1. Créer scène Combat")]
    public static void CreateCombatScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "Combat";

        // Caméra
        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.08f, 0.08f, 0.12f);
        camGO.AddComponent<AudioListener>();

        // Canvas principal
        var canvasGO = new GameObject("CombatCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.AddComponent<GraphicRaycaster>();

        // EventSystem
        var evGO = new GameObject("EventSystem");
        evGO.AddComponent<EventSystem>();
        evGO.AddComponent<StandaloneInputModule>();

        // --- Fond de combat (placeholder coloré) ---
        var bg = CreatePanel(canvasGO.transform, "Background", new Color(0.15f, 0.25f, 0.15f));
        SetRectFull(bg);

        // --- Zone ennemi (haut) ---
        var enemyZone = CreatePanel(canvasGO.transform, "EnemyZone", Color.clear);
        SetRect(enemyZone, new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.55f), new Vector2(-250, -60), new Vector2(250, 60));

        var enemyNameTxt = CreateTMP(enemyZone.transform, "EnemyName", "Wishemon ennemi", 20, TextAlignmentOptions.Left);
        SetRect(enemyNameTxt, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, -30), new Vector2(200, 0));

        var enemyHPLabel = CreateTMP(enemyZone.transform, "EnemyHPLabel", "PV", 14);
        SetRect(enemyHPLabel, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 5), new Vector2(30, 25));

        var enemyHPBar = CreateSlider(enemyZone.transform, "EnemyHPBar");
        SetRect(enemyHPBar, new Vector2(0, 0), new Vector2(1, 0), new Vector2(35, 5), new Vector2(-5, 25));

        var enemyHPText = CreateTMP(enemyZone.transform, "EnemyHPText", "30/30", 13);
        SetRect(enemyHPText, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-60, 5), new Vector2(0, 25));

        var enemyImg = CreateImage(enemyZone.transform, "EnemySprite");
        SetRect(enemyImg, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-50, -100), new Vector2(50, 0));

        // --- Zone joueur (bas-gauche) ---
        var playerZone = CreatePanel(canvasGO.transform, "PlayerZone", Color.clear);
        SetRect(playerZone, new Vector2(0f, 0.35f), new Vector2(0f, 0.35f), new Vector2(20, -50), new Vector2(280, 50));

        var playerNameTxt = CreateTMP(playerZone.transform, "PlayerName", "Mon Wishemon", 20, TextAlignmentOptions.Left);
        SetRect(playerNameTxt, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, -30), new Vector2(200, 0));

        var playerHPLabel = CreateTMP(playerZone.transform, "PlayerHPLabel", "PV", 14);
        SetRect(playerHPLabel, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 5), new Vector2(30, 25));

        var playerHPBar = CreateSlider(playerZone.transform, "PlayerHPBar");
        SetRect(playerHPBar, new Vector2(0, 0), new Vector2(1, 0), new Vector2(35, 5), new Vector2(-5, 25));

        var playerHPText = CreateTMP(playerZone.transform, "PlayerHPText", "30/30", 13);
        SetRect(playerHPText, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-60, 5), new Vector2(0, 25));

        var playerImg = CreateImage(playerZone.transform, "PlayerSprite");
        SetRect(playerImg, new Vector2(0f, 0.35f), new Vector2(0f, 0.35f), new Vector2(40, -80), new Vector2(140, 0));

        // --- Panel message ---
        var msgPanel = CreatePanel(canvasGO.transform, "MessagePanel", new Color(0.1f, 0.1f, 0.1f, 0.92f));
        SetRect(msgPanel, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 120));
        var msgText = CreateTMP(msgPanel.transform, "MessageText", "…", 18);
        SetRectFull(msgText);

        // --- Menu Action ---
        var actionMenu = CreatePanel(canvasGO.transform, "ActionMenu", new Color(0.05f, 0.05f, 0.1f, 0.95f));
        SetRect(actionMenu, new Vector2(0.5f, 0), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 120));

        CombatUI uiRef = null; // assigned below

        CreateButton(actionMenu.transform, "BtnAttaque", "⚔ Attaque", 16, () => { });
        CreateButton(actionMenu.transform, "BtnPotion", "🧪 Potion", 16, () => { });
        CreateButton(actionMenu.transform, "BtnCapture", "🔵 Capture", 16, () => { });
        CreateButton(actionMenu.transform, "BtnFuir", "💨 Fuir", 16, () => { });
        var hLayout = actionMenu.GetComponent<HorizontalLayoutGroup>();
        if (hLayout == null) { hLayout = actionMenu.AddComponent<HorizontalLayoutGroup>(); }
        hLayout.spacing = 10; hLayout.padding = new RectOffset(10, 10, 10, 10);
        hLayout.childForceExpandWidth = true; hLayout.childForceExpandHeight = true;

        // --- Menu Attaques ---
        var attackMenu = CreatePanel(canvasGO.transform, "AttackMenu", new Color(0.05f, 0.05f, 0.1f, 0.95f));
        SetRect(attackMenu, new Vector2(0, 0), new Vector2(0.5f, 0), new Vector2(0, 0), new Vector2(0, 120));
        var atkLayout = attackMenu.AddComponent<GridLayoutGroup>();
        atkLayout.cellSize = new Vector2(180, 45); atkLayout.spacing = new Vector2(5, 5);
        atkLayout.padding = new RectOffset(8, 8, 8, 8);

        var atkButtons = new Button[4];
        var atkLabels = new TextMeshProUGUI[4];
        for (int i = 0; i < 4; i++)
        {
            var btnGO = CreateButton(attackMenu.transform, $"AtkBtn{i}", $"Attaque {i + 1}", 14, () => { });
            atkButtons[i] = btnGO.GetComponent<Button>();
            atkLabels[i] = btnGO.GetComponentInChildren<TextMeshProUGUI>();
        }
        CreateButton(attackMenu.transform, "BtnRetour", "← Retour", 14, () => { });

        // --- CombatManager GameObject ---
        var mgrGO = new GameObject("CombatManager");
        var mgr = mgrGO.AddComponent<CombatManager>();
        var ui = mgrGO.AddComponent<CombatUI>();

        // Wiring via SerializedObject
        var mgrSO = new SerializedObject(mgr);
        mgrSO.FindProperty("_ui").objectReferenceValue = ui;
        mgrSO.ApplyModifiedProperties();

        var uiSO = new SerializedObject(ui);
        uiSO.FindProperty("_playerName").objectReferenceValue = playerNameTxt.GetComponent<TextMeshProUGUI>();
        uiSO.FindProperty("_playerHPBar").objectReferenceValue = playerHPBar.GetComponent<Slider>();
        uiSO.FindProperty("_playerHPText").objectReferenceValue = playerHPText.GetComponent<TextMeshProUGUI>();
        uiSO.FindProperty("_playerSprite").objectReferenceValue = playerImg.GetComponent<Image>();
        uiSO.FindProperty("_enemyName").objectReferenceValue = enemyNameTxt.GetComponent<TextMeshProUGUI>();
        uiSO.FindProperty("_enemyHPBar").objectReferenceValue = enemyHPBar.GetComponent<Slider>();
        uiSO.FindProperty("_enemyHPText").objectReferenceValue = enemyHPText.GetComponent<TextMeshProUGUI>();
        uiSO.FindProperty("_enemySprite").objectReferenceValue = enemyImg.GetComponent<Image>();
        uiSO.FindProperty("_messagePanel").objectReferenceValue = msgPanel;
        uiSO.FindProperty("_messageText").objectReferenceValue = msgText.GetComponent<TextMeshProUGUI>();
        uiSO.FindProperty("_actionMenu").objectReferenceValue = actionMenu;
        uiSO.FindProperty("_attackMenu").objectReferenceValue = attackMenu;

        var atkBtnsProp = uiSO.FindProperty("_attackButtons");
        atkBtnsProp.arraySize = 4;
        for (int i = 0; i < 4; i++) atkBtnsProp.GetArrayElementAtIndex(i).objectReferenceValue = atkButtons[i];

        var atkLabelsProp = uiSO.FindProperty("_attackLabels");
        atkLabelsProp.arraySize = 4;
        for (int i = 0; i < 4; i++) atkLabelsProp.GetArrayElementAtIndex(i).objectReferenceValue = atkLabels[i];

        uiSO.ApplyModifiedProperties();

        // Wirer les boutons action
        WireButtonToCombatUI(actionMenu.transform.Find("BtnAttaque"), ui, "OnClickAttaque");
        WireButtonToCombatUI(actionMenu.transform.Find("BtnPotion"), ui, "OnClickPotion");
        WireButtonToCombatUI(actionMenu.transform.Find("BtnCapture"), ui, "OnClickCapture");
        WireButtonToCombatUI(actionMenu.transform.Find("BtnFuir"), ui, "OnClickFuir");
        WireButtonToCombatUI(attackMenu.transform.Find("BtnRetour"), ui, "OnClickRetour");

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Combat.unity");
        Debug.Log("[WishmonSceneCreator] Scène Combat créée dans Assets/Scenes/Combat.unity");
        EditorUtility.DisplayDialog("Wishmon", "Scène Combat créée ✓\nPense à l'ajouter dans Build Settings !", "OK");
    }

    // ==================== STARTER SELECTION ====================
    [MenuItem("Wishmon/2. Créer scène StarterSelection")]
    public static void CreateStarterScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "StarterSelection";

        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.backgroundColor = new Color(0.05f, 0.05f, 0.15f);
        camGO.AddComponent<AudioListener>();

        var evGO = new GameObject("EventSystem");
        evGO.AddComponent<EventSystem>();
        evGO.AddComponent<StandaloneInputModule>();

        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // Titre
        var title = CreateTMP(canvasGO.transform, "Title", "Choisis ton Wishemon !", 36);
        SetRect(title, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -100), new Vector2(0, 0));

        // Info
        var info = CreateTMP(canvasGO.transform, "Info", "Passe la souris sur un wishemon pour voir ses stats", 18);
        SetRect(info, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -160), new Vector2(0, -110));

        // Conteneur 3 starters
        var container = new GameObject("StarterContainer");
        container.transform.SetParent(canvasGO.transform, false);
        var rt = container.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.1f, 0.25f);
        rt.anchorMax = new Vector2(0.9f, 0.8f);
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        var layout = container.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 30; layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childForceExpandWidth = true; layout.childForceExpandHeight = true;

        var buttons = new Button[3];
        var sprites = new Image[3];
        var names = new TextMeshProUGUI[3];

        for (int i = 0; i < 3; i++)
        {
            var card = CreatePanel(container.transform, $"Starter{i}", new Color(0.15f, 0.15f, 0.25f));
            card.AddComponent<LayoutElement>();
            var vl = card.AddComponent<VerticalLayoutGroup>();
            vl.childAlignment = TextAnchor.MiddleCenter;
            vl.spacing = 8; vl.padding = new RectOffset(10, 10, 10, 10);
            vl.childForceExpandWidth = true; vl.childForceExpandHeight = false;

            var imgGO = new GameObject("Sprite");
            imgGO.transform.SetParent(card.transform, false);
            var imgRT = imgGO.AddComponent<RectTransform>();
            imgRT.sizeDelta = new Vector2(120, 120);
            sprites[i] = imgGO.AddComponent<Image>();
            sprites[i].color = new Color(0.5f + i * 0.2f, 0.3f, 0.7f - i * 0.2f);

            var nameGO = CreateTMP(card.transform, $"Name{i}", $"Starter {i + 1}", 20);
            names[i] = nameGO.GetComponent<TextMeshProUGUI>();

            var btnGO = CreateButton(card.transform, $"Btn{i}", "Choisir", 18, () => { });
            buttons[i] = btnGO.GetComponent<Button>();
        }

        // Manager
        var mgrGO = new GameObject("StarterSelectionManager");
        var mgr = mgrGO.AddComponent<StarterSelectionManager>();
        mgrGO.AddComponent<PlayerTeamBootstrap>();

        var so = new SerializedObject(mgr);
        var btnsProp = so.FindProperty("_buttons");
        btnsProp.arraySize = 3;
        for (int i = 0; i < 3; i++) btnsProp.GetArrayElementAtIndex(i).objectReferenceValue = buttons[i];
        var sprProp = so.FindProperty("_sprites");
        sprProp.arraySize = 3;
        for (int i = 0; i < 3; i++) sprProp.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
        var nameProp = so.FindProperty("_names");
        nameProp.arraySize = 3;
        for (int i = 0; i < 3; i++) nameProp.GetArrayElementAtIndex(i).objectReferenceValue = names[i];
        so.FindProperty("_infoText").objectReferenceValue = info.GetComponent<TextMeshProUGUI>();
        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/StarterSelection.unity");
        Debug.Log("[WishmonSceneCreator] Scène StarterSelection créée !");
        EditorUtility.DisplayDialog("Wishmon", "Scène StarterSelection créée ✓\nPense à l'ajouter dans Build Settings !", "OK");
    }

    [MenuItem("Wishmon/3. Ajouter toutes les scènes au Build")]
    public static void AddScenesToBuild()
    {
        var scenes = new[]
        {
            "Assets/Scenes/Menu.unity",
            "Assets/Scenes/StarterSelection.unity",
            "Assets/Scenes/World.unity",
            "Assets/Scenes/Combat.unity",
        };

        var list = new System.Collections.Generic.List<EditorBuildSettingsScene>();
        foreach (var path in scenes)
            list.Add(new EditorBuildSettingsScene(path, true));

        EditorBuildSettings.scenes = list.ToArray();
        Debug.Log("[WishmonSceneCreator] Build Settings mis à jour !");
        EditorUtility.DisplayDialog("Wishmon", "Build Settings mis à jour ✓\n" + string.Join("\n", scenes), "OK");
    }

    // ==================== HELPERS ====================

    private static GameObject CreatePanel(Transform parent, string name, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        var img = go.AddComponent<Image>();
        img.color = color;
        return go;
    }

    private static GameObject CreateTMP(Transform parent, string name, string text, int size,
        TextAlignmentOptions align = TextAlignmentOptions.Center)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.alignment = align;
        tmp.color = Color.white;
        return go;
    }

    private static GameObject CreateImage(Transform parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        var img = go.AddComponent<Image>();
        img.color = new Color(0.6f, 0.6f, 0.8f, 0.5f);
        return go;
    }

    private static GameObject CreateSlider(Transform parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        var slider = go.AddComponent<Slider>();
        slider.maxValue = 100; slider.value = 80;

        var bg = new GameObject("Background"); bg.transform.SetParent(go.transform, false);
        var bgRT = bg.AddComponent<RectTransform>(); bgRT.anchorMin = Vector2.zero; bgRT.anchorMax = Vector2.one; bgRT.offsetMin = bgRT.offsetMax = Vector2.zero;
        var bgImg = bg.AddComponent<Image>(); bgImg.color = new Color(0.3f, 0.1f, 0.1f);

        var fill = new GameObject("Fill"); fill.transform.SetParent(go.transform, false);
        var fillRT = fill.AddComponent<RectTransform>(); fillRT.anchorMin = Vector2.zero; fillRT.anchorMax = Vector2.one; fillRT.offsetMin = fillRT.offsetMax = Vector2.zero;
        var fillImg = fill.AddComponent<Image>(); fillImg.color = new Color(0.2f, 0.8f, 0.2f);

        var fillArea = new GameObject("Fill Area"); fillArea.transform.SetParent(go.transform, false);
        var faRT = fillArea.AddComponent<RectTransform>(); faRT.anchorMin = Vector2.zero; faRT.anchorMax = Vector2.one; faRT.offsetMin = faRT.offsetMax = Vector2.zero;
        fill.transform.SetParent(fillArea.transform, false);
        slider.fillRect = fillRT;

        return go;
    }

    private static GameObject CreateButton(Transform parent, string name, string label, int size,
        UnityEngine.Events.UnityAction onClick)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        var img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.4f, 0.7f);
        var btn = go.AddComponent<Button>();
        var colors = btn.colors;
        colors.highlightedColor = new Color(0.3f, 0.5f, 0.9f);
        btn.colors = colors;
        if (onClick != null) btn.onClick.AddListener(onClick);

        var lblGO = new GameObject("Label");
        lblGO.transform.SetParent(go.transform, false);
        var lblRT = lblGO.AddComponent<RectTransform>();
        lblRT.anchorMin = Vector2.zero; lblRT.anchorMax = Vector2.one;
        lblRT.offsetMin = lblRT.offsetMax = Vector2.zero;
        var tmp = lblGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label; tmp.fontSize = size;
        tmp.alignment = TextAlignmentOptions.Center; tmp.color = Color.white;

        return go;
    }

    private static void WireButtonToCombatUI(Transform btnTransform, CombatUI ui, string methodName)
    {
        if (btnTransform == null) return;
        var btn = btnTransform.GetComponent<Button>();
        if (btn == null) return;
        btn.onClick.RemoveAllListeners();
        UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(
            btn.onClick,
            (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(
                typeof(UnityEngine.Events.UnityAction), ui,
                typeof(CombatUI).GetMethod(methodName)));
    }

    private static void SetRectFull(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    private static void SetRect(GameObject go, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin; rt.offsetMax = offsetMax;
    }
}

// Petit helper pour s'assurer que PlayerTeam existe dans StarterSelection
public class PlayerTeamBootstrap : MonoBehaviour
{
    [SerializeField] private GameObject _playerTeamPrefab;

    private void Awake()
    {
        if (PlayerTeam.Instance == null && _playerTeamPrefab != null)
            Instantiate(_playerTeamPrefab);
    }
}
#endif
