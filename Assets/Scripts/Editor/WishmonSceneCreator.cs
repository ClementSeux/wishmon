// Script éditeur - accessible via le menu Unity "Wishmon/..."
// Lance-le une seule fois pour créer les scènes Combat et StarterSelection.
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public static class WishmonSceneCreator
{
    // ==================== COMBAT ====================
    [MenuItem("Wishmon/1. Créer scène Combat")]
    public static void CreateCombatScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "Combat";

        // Camera - vue 3/4 style Pokemon
        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.53f, 0.81f, 0.98f);
        cam.transform.position = new Vector3(0, 4, -6);
        cam.transform.rotation = Quaternion.Euler(20, 0, 0);
        camGO.AddComponent<AudioListener>();

        // Sol de combat
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(2, 1, 2);
        var groundMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        groundMat.color = new Color(0.4f, 0.6f, 0.3f);
        ground.GetComponent<Renderer>().material = groundMat;

        // Anchors 3D modeles
        var enemyAnchorGO = new GameObject("EnemyModelAnchor");
        enemyAnchorGO.transform.position = new Vector3(2.5f, 0, 2f);
        enemyAnchorGO.transform.rotation = Quaternion.Euler(0, 200, 0);
        var enemyModel = enemyAnchorGO.AddComponent<WishemonCombatModel>();

        var playerAnchorGO = new GameObject("PlayerModelAnchor");
        playerAnchorGO.transform.position = new Vector3(-2.5f, 0, -1f);
        playerAnchorGO.transform.rotation = Quaternion.Euler(0, 20, 0);
        var playerModel = playerAnchorGO.AddComponent<WishemonCombatModel>();

        // Canvas principal
        var canvasGO = new GameObject("CombatCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // EventSystem
        var evGO = new GameObject("EventSystem");
        evGO.AddComponent<EventSystem>();
        evGO.AddComponent<InputSystemUIInputModule>();

        // --- Zone ennemi (haut droite) ---
        var enemyZone = CreatePanel(canvasGO.transform, "EnemyZone", new Color(0f, 0f, 0f, 0.75f));
        SetRect(enemyZone, new Vector2(0.52f, 0.60f), new Vector2(1f, 1f), new Vector2(0, 0), new Vector2(-20, -20));

        var enemyNameTxt = CreateTMP(enemyZone.transform, "EnemyName", "Ennemi", 42, TextAlignmentOptions.Left);
        SetRect(enemyNameTxt, new Vector2(0, 1), new Vector2(1, 1), new Vector2(20, -70), new Vector2(-20, -10));

        var enemyHPBar = CreateSlider(enemyZone.transform, "EnemyHPBar");
        SetRect(enemyHPBar, new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(20, -20), new Vector2(-20, 20));

        var enemyHPText = CreateTMP(enemyZone.transform, "EnemyHPText", "30/30", 32, TextAlignmentOptions.Right);
        SetRect(enemyHPText, new Vector2(0, 0), new Vector2(1, 0), new Vector2(20, 10), new Vector2(-20, 55));

        // --- Zone joueur (haut gauche) ---
        var playerZone = CreatePanel(canvasGO.transform, "PlayerZone", new Color(0f, 0f, 0f, 0.75f));
        SetRect(playerZone, new Vector2(0f, 0.60f), new Vector2(0.48f, 1f), new Vector2(20, 0), new Vector2(0, -20));

        var playerNameTxt = CreateTMP(playerZone.transform, "PlayerName", "Joueur", 42, TextAlignmentOptions.Left);
        SetRect(playerNameTxt, new Vector2(0, 1), new Vector2(1, 1), new Vector2(20, -70), new Vector2(-20, -10));

        var playerHPBar = CreateSlider(playerZone.transform, "PlayerHPBar");
        SetRect(playerHPBar, new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(20, -20), new Vector2(-20, 20));

        var playerHPText = CreateTMP(playerZone.transform, "PlayerHPText", "30/30", 32, TextAlignmentOptions.Right);
        SetRect(playerHPText, new Vector2(0, 0), new Vector2(1, 0), new Vector2(20, 10), new Vector2(-20, 55));

        // --- Panel message (bande du bas) ---
        var msgPanel = CreatePanel(canvasGO.transform, "MessagePanel", new Color(0.08f, 0.08f, 0.15f, 0.97f));
        SetRect(msgPanel, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 200));
        var msgText = CreateTMP(msgPanel.transform, "MessageText", "Que va faire ton Wishemon ?", 36);
        SetRectFull(msgText, 30, 20);

        // --- Menu Action (moitie droite de la bande bas) ---
        var actionMenu = CreatePanel(canvasGO.transform, "ActionMenu", new Color(0.05f, 0.12f, 0.25f, 0.98f));
        SetRect(actionMenu, new Vector2(0.5f, 0), new Vector2(1f, 0), new Vector2(0, 0), new Vector2(0, 200));
        var hLayout = actionMenu.AddComponent<HorizontalLayoutGroup>();
        hLayout.spacing = 8; hLayout.padding = new RectOffset(16, 16, 16, 16);
        hLayout.childForceExpandWidth = true; hLayout.childForceExpandHeight = true;

        CreateButton(actionMenu.transform, "BtnAttaque", "Attaque", 28, () => { });
        CreateButton(actionMenu.transform, "BtnPotion", "Potion", 28, () => { });
        CreateButton(actionMenu.transform, "BtnCapture", "Capture", 28, () => { });
        CreateButton(actionMenu.transform, "BtnFuir", "Fuir", 28, () => { });

        // --- Menu Attaques ---
        var attackMenu = CreatePanel(canvasGO.transform, "AttackMenu", new Color(0.05f, 0.12f, 0.25f, 0.98f));
        SetRect(attackMenu, new Vector2(0, 0), new Vector2(1f, 0), new Vector2(0, 0), new Vector2(0, 200));
        var atkLayout = attackMenu.AddComponent<GridLayoutGroup>();
        atkLayout.cellSize = new Vector2(420, 75); atkLayout.spacing = new Vector2(10, 10);
        atkLayout.padding = new RectOffset(16, 16, 16, 16);
        atkLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        atkLayout.constraintCount = 2;

        var atkButtons = new Button[4];
        var atkLabels = new TextMeshProUGUI[4];
        for (int i = 0; i < 4; i++)
        {
            var btnGO = CreateButton(attackMenu.transform, $"AtkBtn{i}", $"Attaque {i + 1}", 26, () => { });
            atkButtons[i] = btnGO.GetComponent<Button>();
            atkLabels[i] = btnGO.GetComponentInChildren<TextMeshProUGUI>();
        }
        CreateButton(attackMenu.transform, "BtnRetour", "Retour", 24, () => { });

        // --- CombatManager ---
        var mgrGO = new GameObject("CombatManager");
        var mgr = mgrGO.AddComponent<CombatManager>();
        var ui = mgrGO.AddComponent<CombatUI>();

        var mgrSO = new SerializedObject(mgr);
        mgrSO.FindProperty("_ui").objectReferenceValue = ui;
        mgrSO.ApplyModifiedProperties();

        var uiSO = new SerializedObject(ui);
        uiSO.FindProperty("_playerName").objectReferenceValue = playerNameTxt.GetComponent<TextMeshProUGUI>();
        uiSO.FindProperty("_playerHPBar").objectReferenceValue = playerHPBar.GetComponent<Slider>();
        uiSO.FindProperty("_playerHPText").objectReferenceValue = playerHPText.GetComponent<TextMeshProUGUI>();
        uiSO.FindProperty("_enemyName").objectReferenceValue = enemyNameTxt.GetComponent<TextMeshProUGUI>();
        uiSO.FindProperty("_enemyHPBar").objectReferenceValue = enemyHPBar.GetComponent<Slider>();
        uiSO.FindProperty("_enemyHPText").objectReferenceValue = enemyHPText.GetComponent<TextMeshProUGUI>();
        uiSO.FindProperty("_playerModel").objectReferenceValue = playerModel;
        uiSO.FindProperty("_enemyModel").objectReferenceValue = enemyModel;
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

        WireButtonToCombatUI(actionMenu.transform.Find("BtnAttaque"), ui, "OnClickAttaque");
        WireButtonToCombatUI(actionMenu.transform.Find("BtnPotion"), ui, "OnClickPotion");
        WireButtonToCombatUI(actionMenu.transform.Find("BtnCapture"), ui, "OnClickCapture");
        WireButtonToCombatUI(actionMenu.transform.Find("BtnFuir"), ui, "OnClickFuir");
        WireButtonToCombatUI(attackMenu.transform.Find("BtnRetour"), ui, "OnClickRetour");

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Combat.unity");
        Debug.Log("[WishmonSceneCreator] Scene Combat creee : Assets/Scenes/Combat.unity");
        EditorUtility.DisplayDialog("Wishmon", "Scene Combat creee!\nN'oublie pas Build Settings.", "OK");
    }

    // ==================== STARTER SELECTION ====================
    [MenuItem("Wishmon/2. Créer scène StarterSelection")]
    public static void CreateStarterScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "StarterSelection";

        // Camera principale (voit la scene UI, PAS les modeles 3D)
        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.07f, 0.07f, 0.18f);
        cam.transform.position = new Vector3(0, 0, -10);
        cam.cullingMask = ~0; // tout voir sauf les modeles (on separera par position)
        camGO.AddComponent<AudioListener>();

        var evGO = new GameObject("EventSystem");
        evGO.AddComponent<EventSystem>();
        evGO.AddComponent<InputSystemUIInputModule>();

        // Canvas
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // Fond
        var bg = canvasGO.AddComponent<Image>();
        bg.color = new Color(0.07f, 0.07f, 0.18f);

        // Titre
        var title = CreateTMP(canvasGO.transform, "Title", "Choisis ton Wishemon !", 60);
        SetRect(title, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -130), new Vector2(0, -10));

        // Sous-titre
        var info = CreateTMP(canvasGO.transform, "Info", "Clique sur un wishemon pour commencer l'aventure", 30);
        SetRect(info, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -210), new Vector2(0, -140));

        // Conteneur 3 starters
        var container = new GameObject("StarterContainer");
        container.transform.SetParent(canvasGO.transform, false);
        var containerRT = container.AddComponent<RectTransform>();
        containerRT.anchorMin = new Vector2(0.05f, 0.15f);
        containerRT.anchorMax = new Vector2(0.95f, 0.85f);
        containerRT.offsetMin = containerRT.offsetMax = Vector2.zero;
        var layout = container.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 40; layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childForceExpandWidth = true; layout.childForceExpandHeight = true;
        layout.padding = new RectOffset(20, 20, 0, 0);

        var buttons = new Button[3];
        var rawImages = new RawImage[3];
        var names = new TextMeshProUGUI[3];

        Color[] cardColors = new Color[]
        {
            new Color(0.1f, 0.15f, 0.35f),
            new Color(0.1f, 0.25f, 0.15f),
            new Color(0.3f, 0.12f, 0.12f),
        };
        Color[] bgColors = new Color[]
        {
            new Color(0.12f, 0.16f, 0.36f),
            new Color(0.12f, 0.26f, 0.16f),
            new Color(0.32f, 0.14f, 0.14f),
        };

        for (int i = 0; i < 3; i++)
        {
            var card = CreatePanel(container.transform, $"Starter{i}", cardColors[i]);
            var outline = card.AddComponent<Outline>();
            outline.effectColor = new Color(0.4f, 0.6f, 1f);
            outline.effectDistance = new Vector2(3, 3);
            card.AddComponent<LayoutElement>();
            var vl = card.AddComponent<VerticalLayoutGroup>();
            vl.childAlignment = TextAnchor.MiddleCenter;
            vl.spacing = 20; vl.padding = new RectOffset(20, 20, 30, 30);
            vl.childForceExpandWidth = true; vl.childForceExpandHeight = false;

            // RawImage pour la RenderTexture du modele 3D
            var rawGO = new GameObject("ModelView");
            rawGO.transform.SetParent(card.transform, false);
            var rawRT = rawGO.AddComponent<RectTransform>();
            rawRT.sizeDelta = new Vector2(220, 220);
            var le = rawGO.AddComponent<LayoutElement>();
            le.preferredHeight = 220; le.preferredWidth = 220; le.flexibleWidth = 0;
            rawImages[i] = rawGO.AddComponent<RawImage>();
            rawImages[i].color = bgColors[i]; // couleur de fond en attendant

            var nameGO = CreateTMP(card.transform, $"Name{i}", $"Starter {i + 1}", 36);
            var nameTMP = nameGO.GetComponent<TextMeshProUGUI>();
            nameTMP.fontStyle = FontStyles.Bold;
            names[i] = nameTMP;
            var namLE = nameGO.AddComponent<LayoutElement>();
            namLE.preferredHeight = 50;

            var btnGO = CreateButton(card.transform, $"Btn{i}", "Choisir !", 32, () => { });
            var btnLE = btnGO.AddComponent<LayoutElement>();
            btnLE.preferredHeight = 75; btnLE.minHeight = 75;
            buttons[i] = btnGO.GetComponent<Button>();
        }

        // Info panel en bas
        var infoPanel = CreatePanel(canvasGO.transform, "InfoPanel", new Color(0, 0, 0, 0.5f));
        SetRect(infoPanel, new Vector2(0.1f, 0), new Vector2(0.9f, 0), new Vector2(0, 10), new Vector2(0, 120));
        var infoTxt = CreateTMP(infoPanel.transform, "InfoText", "", 28);
        SetRectFull(infoTxt, 15, 10);

        // --- Viewers 3D (cameras + modeles hors champ de la camera principale) ---
        // Chaque viewer est positionné très loin en X pour isoler les cameras
        var viewers = new WishemonModelViewer[3];
        float[] xPositions = new float[] { -100f, 0f, 100f };

        for (int i = 0; i < 3; i++)
        {
            var viewerGO = new GameObject($"ModelViewer{i}");
            viewerGO.transform.position = new Vector3(xPositions[i], 0, 0);
            var viewer = viewerGO.AddComponent<WishemonModelViewer>();

            // Camera du viewer - rend uniquement sa zone locale
            var viewCamGO = new GameObject("Camera");
            viewCamGO.transform.SetParent(viewerGO.transform, false);
            viewCamGO.transform.localPosition = new Vector3(0, 1.2f, -3f);
            viewCamGO.transform.localRotation = Quaternion.Euler(10, 0, 0);
            var viewCam = viewCamGO.AddComponent<Camera>();
            viewCam.clearFlags = CameraClearFlags.SolidColor;
            viewCam.backgroundColor = bgColors[i];
            viewCam.fieldOfView = 35f;
            viewCam.nearClipPlane = 0.1f;
            viewCam.farClipPlane = 8f; // ne voit que 8 unités devant elle
            viewCam.enabled = true;
            // Désactiver l'audio listener sur ces cameras
            var al = viewCamGO.GetComponent<AudioListener>();
            if (al != null) Object.DestroyImmediate(al);

            // Modele 3D
            var modelGO = new GameObject("Model");
            modelGO.transform.SetParent(viewerGO.transform, false);
            modelGO.transform.localPosition = new Vector3(0, 0, 0);
            var model = modelGO.AddComponent<WishemonCombatModel>();

            // Wiring via SerializedObject
            var vSO = new SerializedObject(viewer);
            vSO.FindProperty("_cam").objectReferenceValue = viewCam;
            vSO.FindProperty("_model").objectReferenceValue = model;
            vSO.FindProperty("_rawImage").objectReferenceValue = rawImages[i];
            vSO.ApplyModifiedProperties();

            viewers[i] = viewer;
        }

        // Manager
        var mgrGO = new GameObject("StarterSelectionManager");
        var mgr = mgrGO.AddComponent<StarterSelectionManager>();
        mgrGO.AddComponent<PlayerTeamBootstrap>();

        var so = new SerializedObject(mgr);
        var btnsProp = so.FindProperty("_buttons");
        btnsProp.arraySize = 3;
        for (int i = 0; i < 3; i++) btnsProp.GetArrayElementAtIndex(i).objectReferenceValue = buttons[i];

        // _sprites laissé vide (on utilise les viewers)
        var sprProp = so.FindProperty("_sprites");
        sprProp.arraySize = 3;

        var nameProp = so.FindProperty("_names");
        nameProp.arraySize = 3;
        for (int i = 0; i < 3; i++) nameProp.GetArrayElementAtIndex(i).objectReferenceValue = names[i];

        so.FindProperty("_infoText").objectReferenceValue = infoTxt.GetComponent<TextMeshProUGUI>();

        var viewersProp = so.FindProperty("_viewers");
        viewersProp.arraySize = 3;
        for (int i = 0; i < 3; i++) viewersProp.GetArrayElementAtIndex(i).objectReferenceValue = viewers[i];

        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/StarterSelection.unity");
        Debug.Log("[WishmonSceneCreator] Scene StarterSelection creee !");
        EditorUtility.DisplayDialog("Wishmon", "Scene StarterSelection creee!\nN'oublie pas Build Settings.", "OK");
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
        Debug.Log("[WishmonSceneCreator] Build Settings mis a jour !");
        EditorUtility.DisplayDialog("Wishmon", "Build Settings mis a jour !\n" + string.Join("\n", scenes), "OK");
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

        var fillArea = new GameObject("Fill Area"); fillArea.transform.SetParent(go.transform, false);
        var faRT = fillArea.AddComponent<RectTransform>(); faRT.anchorMin = Vector2.zero; faRT.anchorMax = Vector2.one; faRT.offsetMin = faRT.offsetMax = Vector2.zero;

        var fill = new GameObject("Fill"); fill.transform.SetParent(fillArea.transform, false);
        var fillRT = fill.AddComponent<RectTransform>(); fillRT.anchorMin = Vector2.zero; fillRT.anchorMax = Vector2.one; fillRT.offsetMin = fillRT.offsetMax = Vector2.zero;
        var fillImg = fill.AddComponent<Image>(); fillImg.color = new Color(0.2f, 0.85f, 0.2f);

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
        img.color = new Color(0.18f, 0.38f, 0.72f);
        var btn = go.AddComponent<Button>();
        var colors = btn.colors;
        colors.highlightedColor = new Color(0.28f, 0.52f, 0.95f);
        colors.pressedColor = new Color(0.1f, 0.25f, 0.55f);
        btn.colors = colors;
        if (onClick != null) btn.onClick.AddListener(onClick);

        var lblGO = new GameObject("Label");
        lblGO.transform.SetParent(go.transform, false);
        var lblRT = lblGO.AddComponent<RectTransform>();
        lblRT.anchorMin = Vector2.zero; lblRT.anchorMax = Vector2.one;
        lblRT.offsetMin = new Vector2(8, 4); lblRT.offsetMax = new Vector2(-8, -4);
        var tmp = lblGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label; tmp.fontSize = size;
        tmp.alignment = TextAlignmentOptions.Center; tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;

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

    private static void SetRectFull(GameObject go, float paddingH = 0, float paddingV = 0)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(paddingH, paddingV);
        rt.offsetMax = new Vector2(-paddingH, -paddingV);
    }

    private static void SetRect(GameObject go, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin; rt.offsetMax = offsetMax;
    }
}

// Helper pour s'assurer que PlayerTeam existe dans StarterSelection
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
