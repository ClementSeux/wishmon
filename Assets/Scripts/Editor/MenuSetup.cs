using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

public class MenuSetup
{
	[MenuItem("Tools/Setup Menu Scene")]
	public static void SetupMenu()
	{
		Canvas canvas = Object.FindFirstObjectByType<Canvas>();
		if (canvas == null)
		{
			EditorUtility.DisplayDialog("Error", "Canvas not found in scene", "OK");
			return;
		}

		RectTransform canvasRect = canvas.GetComponent<RectTransform>();

		// Clear existing buttons
		for (int i = canvasRect.childCount - 1; i >= 0; i--)
		{
			Transform child = canvasRect.GetChild(i);
			if (child.name.Contains("Button"))
				Object.DestroyImmediate(child.gameObject);
		}

		// Create Play Button
		CreateButton(canvasRect, "PlayButton", "PLAY", new Vector2(0, 100), new Color(0.2f, 0.8f, 0.2f));

		// Create Resume Button
		CreateButton(canvasRect, "ResumeButton", "RESUME", new Vector2(0, 20), new Color(0.2f, 0.6f, 0.8f));

		// Create Quit Button
		CreateButton(canvasRect, "QuitButton", "QUIT", new Vector2(0, -60), new Color(0.8f, 0.2f, 0.2f));

		// Add MenuManager to Canvas
		if (canvas.GetComponent<MenuManager>() == null)
			canvas.gameObject.AddComponent<MenuManager>();

		EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
		EditorUtility.DisplayDialog("Success", "Menu scene configured!", "OK");
	}

	private static void CreateButton(RectTransform parent, string name, string buttonText, Vector2 position, Color color)
	{
		GameObject btnObj = new GameObject(name);
		btnObj.transform.SetParent(parent, false);

		RectTransform btnRect = btnObj.AddComponent<RectTransform>();
		btnRect.anchoredPosition = position;
		btnRect.sizeDelta = new Vector2(200, 60);

		Image btnImage = btnObj.AddComponent<Image>();
		btnImage.color = color;

		Button btn = btnObj.AddComponent<Button>();
		btn.targetGraphic = btnImage;

		// Add text
		GameObject textObj = new GameObject("Text");
		textObj.transform.SetParent(btnRect, false);

		Text text = textObj.AddComponent<Text>();
		text.text = buttonText;
		text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		text.alignment = TextAnchor.MiddleCenter;
		text.fontSize = 30;
		text.fontStyle = FontStyle.Bold;

		RectTransform textRect = textObj.GetComponent<RectTransform>();
		textRect.anchorMin = Vector2.zero;
		textRect.anchorMax = Vector2.one;
		textRect.offsetMin = Vector2.zero;
		textRect.offsetMax = Vector2.zero;

		// Connect button to MenuManager methods
		MenuManager manager = parent.GetComponent<MenuManager>();
		if (manager != null)
		{
			switch (name)
			{
				case "PlayButton":
					btn.onClick.AddListener(() => manager.PlayGame());
					break;
				case "ResumeButton":
					btn.onClick.AddListener(() => manager.ResumeGame());
					break;
				case "QuitButton":
					btn.onClick.AddListener(() => manager.QuitGame());
					break;
			}
		}
	}
}
