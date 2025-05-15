using System.Drawing;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Paps.UnityToolbarExtenderUIToolkit;

[MainToolbarElement(id: "PlayFromGameStart", ToolbarAlign.Right)]
public class PlayFromGameStartButton : Button
{
    private const string GameStartScenePath = "Assets/Scenes/MainScene.unity";
    private const string PreviousSceneKey = "KEY_PREVIOUS_SCENE_PATH";

    public void InitializeElement()
    {
        text = "#";
        tooltip = "Click to play from GameStart scene";

        style.width = 40;
        style.height = 20;
        style.marginLeft = 6;
        style.marginRight = 6;
        style.paddingLeft = 8;
        style.paddingRight = 8;
        style.borderBottomWidth = 2;
        style.borderTopLeftRadius = 6;
        style.borderTopRightRadius = 6;
        style.borderBottomLeftRadius = 6;
        style.borderBottomRightRadius = 6;
        style.alignItems = Align.Center;
        style.justifyContent = Justify.Center;

        clicked += OnPlayClicked;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }


    private void OnPlayClicked()
    {
        if (EditorApplication.isPlaying)
            return;

        string currentScene = SceneManager.GetActiveScene().path;
        EditorPrefs.SetString(PreviousSceneKey, currentScene);

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(GameStartScenePath);
            EditorApplication.EnterPlaymode();
        }
    }

    private void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            string prev = EditorPrefs.GetString(PreviousSceneKey, "");
            if (!string.IsNullOrEmpty(prev))
            {
                EditorPrefs.DeleteKey(PreviousSceneKey);
                EditorSceneManager.OpenScene(prev);
            }
        }
    }
}