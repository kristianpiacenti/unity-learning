using UnityEditor;
using UnityEngine;

public class PIANewImageWindow : EditorWindow
{

    static PIANewImageWindow window;
    int width = 16;
    int height = 16;

    public static void ShowWindow()
    {

        window = GetWindow<PIANewImageWindow>();
        window.maxSize = new Vector2(200, 200);
        window.Show();

    }

    private void OnGUI()
    {
        width = EditorGUILayout.IntField("Width: ", width);
        height = EditorGUILayout.IntField("Height: ", height);
        if (GUILayout.Button("Create"))
        {
            LoadNewAsset();
        }
    }
    private void LoadNewAsset()
    {
        PIASession.Instance.LoadNewAsset(width, height);
        PIAEditorWindow.window.Repaint();
        window.Close();
    }
}

