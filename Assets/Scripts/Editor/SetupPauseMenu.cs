using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine.InputSystem;

public class SetupPauseMenu
{
	[MenuItem("Tools/Setup Pause Menu in World")]
	public static void SetupPauseMenuScene()
	{
		// Create Canvas
		GameObject canvasObj = new GameObject("PauseMenuCanvas");
		Canvas canvas = canvasObj.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvasObj.AddComponent<CanvasScaler>();
		canvasObj.AddComponent<GraphicRaycaster>();

		RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
		canvasRect.anchorMin = Vector2.zero;
		canvasRect.anchorMax = Vector2.one;
		canvasRect.offsetMin = Vector2.zero;
		canvasRect.offsetMax = Vector2.zero;

		// Create Panel (background)
		GameObject panelObj = new GameObject("PausePanel");
		panelObj.transform.SetParent(canvasRect);
		RectTransform panelRect = panelObj.AddComponent<RectTransform>();
		panelRect.anchorMin = Vector2.zero;
		panelRect.anchorMax = Vector2.one;
		panelRect.offsetMin = Vector2.zero;
		panelRect.offsetMax = Vector2.zero;

		Image panelImage = panelObj.AddComponent<Image>();
		panelImage.color = new Color(0, 0, 0, 0.8f);

		// Create Title Text
		GameObject titleObj = new GameObject("Title");
		titleObj.transform.SetParent(panelRect);
		RectTransform titleRect = titleObj.AddComponent<RectTransform>();
		titleRect.anchoredPosition = new Vector2(0, 200);
		titleRect.sizeDelta = new Vector2(400, 100);

		Text titleText = titleObj.AddComponent<Text>();
		titleText.text = "PAUSED";
		titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		titleText.fontSize = 60;
		titleText.fontStyle = FontStyle.Bold;
		titleText.alignment = TextAnchor.MiddleCenter;
		titleText.color = Color.white;

		// Add PauseManager to a GameObject (created BEFORE buttons so listeners can wire up)
		GameObject pauseManagerObj = new GameObject("PauseManager");
		PauseManager pauseManager = pauseManagerObj.AddComponent<PauseManager>();
		pauseManager._pauseCanvas = canvas;

		// Create Resume Button
		Button resumeBtn = CreateButton(panelRect, "ResumeButton", "RESUME", new Vector2(0, 80), new Color(0.2f, 0.8f, 0.2f), pauseManager.Resume);

		// Create Back to Menu Button
		Button backBtn = CreateButton(panelRect, "BackButton", "BACK TO MENU", new Vector2(0, -40), new Color(0.8f, 0.6f, 0.2f), pauseManager.GoToMainMenu);

		// Configure explicit vertical navigation between the two buttons
		Navigation resumeNav = new Navigation { mode = Navigation.Mode.Explicit, selectOnDown = backBtn, selectOnUp = backBtn };
		Navigation backNav = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = resumeBtn, selectOnDown = resumeBtn };
		resumeBtn.navigation = resumeNav;
		backBtn.navigation = backNav;

		pauseManager._firstSelected = resumeBtn.gameObject;

		// Find and assign the Pause InputActionReference
		var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Inputs/InputSystem_Actions.inputactions");
		if (inputActions != null)
		{
			pauseManager._pauseRef = InputActionReference.Create(inputActions["Player/Pause"]);
		}

		// Hide canvas initially
		canvasObj.SetActive(false);

		EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		EditorUtility.DisplayDialog("Success", "Pause menu configured in World scene!\n\nPress ESC in game to pause.", "OK");
	}

	private static Button CreateButton(RectTransform parent, string name, string buttonText, Vector2 position, Color color, UnityEngine.Events.UnityAction onClick)
	{
		GameObject btnObj = new GameObject(name);
		btnObj.transform.SetParent(parent);

		RectTransform btnRect = btnObj.AddComponent<RectTransform>();
		btnRect.anchoredPosition = position;
		btnRect.sizeDelta = new Vector2(250, 70);

		Image btnImage = btnObj.AddComponent<Image>();
		btnImage.color = color;

		Button btn = btnObj.AddComponent<Button>();
		btn.targetGraphic = btnImage;

		// Add text
		GameObject textObj = new GameObject("Text");
		textObj.transform.SetParent(btnRect);

		Text text = textObj.AddComponent<Text>();
		text.text = buttonText;
		text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		text.alignment = TextAnchor.MiddleCenter;
		text.fontSize = 35;
		text.fontStyle = FontStyle.Bold;
		text.color = Color.white;

		RectTransform textRect = textObj.GetComponent<RectTransform>();
		textRect.anchorMin = Vector2.zero;
		textRect.anchorMax = Vector2.one;
		textRect.offsetMin = Vector2.zero;
		textRect.offsetMax = Vector2.zero;

		UnityEventTools.AddPersistentListener(btn.onClick, onClick);

		return btn;
	}
}
