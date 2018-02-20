using UnityEditor;
using UnityEngine;

public class PIAExportSettingsWindow : EditorWindow
{

    static PIAExportSettingsWindow window;
    int width = 16;
    int height = 16;

    public static void ShowWindow()
    {

        window = GetWindow<PIAExportSettingsWindow>();
        window.maxSize = new Vector2(400, 200);
        window.Show();

    }
    public static void CloseWindow()
    {
        if(window!=null)
            window.Close();

    }
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Export This Frame"))
            {
                PIASession.Instance.ExportImage(PIASession.Instance.ImageData.CurrentFrame.GetFrameTexture());
            }
            if (GUILayout.Button("Export All Frames"))
            {
                PIASession.Instance.ExportAll();
            }
            if (GUILayout.Button("Export Sprite Sheet"))
            {
                PIASession.Instance.ExportSpriteSheet();
            }
        }
        GUILayout.EndHorizontal();
        
    }

    private void LoadNewAsset()
    {
        PIASession.Instance.LoadNewAsset(width, height);
        PIAEditorWindow.window.Repaint();
        window.Close();
    }
}