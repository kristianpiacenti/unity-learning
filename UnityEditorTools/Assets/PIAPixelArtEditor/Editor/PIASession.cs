using System.IO;
using UnityEditor;
using UnityEngine;

public class PIASession {
    private PIAImageData _imageData;

    public PIAImageData ImageData { get { return _imageData; } set { _imageData = value; } }
    private bool isNew {
        get {
            string path = AssetDatabase.GetAssetPath(ImageData);
            path = Path.GetFileNameWithoutExtension(path);
            return string.IsNullOrEmpty(path) ? true : false;
        }
    }
    public string ProjectName {
        get {
            string path = AssetDatabase.GetAssetPath(ImageData);
            path= Path.GetFileNameWithoutExtension(path);
            return string.IsNullOrEmpty(path) ? "NewImage": path;
        }
    }
    public Texture2D LoadAsset()
    {
        OpenAsset(ref _imageData);
        if (ImageData.Canvas == null)
            ImageData.Init();
        return ImageData.Canvas.GetFinalImage();
    }
    public PIASession() {
        LoadNewAsset();
    }
    public Texture2D LoadImageFromFile()
    {
        string path = EditorUtility.OpenFilePanelWithFilters("Select Texture", "", new string[] { "png", "png" });
        if (string.IsNullOrEmpty(path))
            return null;

        Texture2D texture = new Texture2D(2, 2);
        texture.filterMode = FilterMode.Point;
        byte[] fileData = File.ReadAllBytes(path);
        texture.LoadImage(fileData);
        ImageData.Canvas.GetCurrentLayer().Texture= texture;
        return ImageData.Canvas.GetFinalImage();
    }

    private void OpenAsset(ref PIAImageData output) {
        string path = EditorUtility.OpenFilePanelWithFilters("Select Asset", "Assets/", new string[] { "asset", "asset" });
        if (string.IsNullOrEmpty(path))
            return;
        output = AssetDatabase.LoadAssetAtPath<PIAImageData>(FileUtil.GetProjectRelativePath(path));

    }
    
    public void ExportImage(Texture2D tex)
    {

        string path = EditorUtility.SaveFilePanel("Save Image", "", ProjectName, "png");
        if (string.IsNullOrEmpty(path))
            return;
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        

    }
    public void SaveAsset() {
        string path;
        if (isNew) {
            path = EditorUtility.SaveFilePanel("Save Asset", "", ProjectName, "asset");
            path = FileUtil.GetProjectRelativePath(path);
            if (string.IsNullOrEmpty(path))
                return;
            AssetDatabase.CreateAsset(ImageData, path);
        }

        AssetDatabase.Refresh();
        
    }
    public void LoadNewAsset()
    {
        ImageData = ScriptableObject.CreateInstance<PIAImageData>();
        ImageData.Init();
    }

}
