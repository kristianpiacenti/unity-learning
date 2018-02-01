﻿using System.IO;
using UnityEditor;
using UnityEngine;

public class PIASession {
    #region Fields

    private PIAImageData _imageData;
    private static PIASession _instance;

    #endregion

    #region Properties

    public static PIASession Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PIASession();
            }
            return _instance;
        }
    }
    public PIAImageData ImageData { get { return _imageData; } set { _imageData = value; } }
    private bool isNew
    {
        get
        {
            string path = AssetDatabase.GetAssetPath(ImageData);
            path = Path.GetFileNameWithoutExtension(path);
            return string.IsNullOrEmpty(path) ? true : false;
        }
    }
    public string ProjectName
    {
        get
        {
            string path = AssetDatabase.GetAssetPath(ImageData);
            path = Path.GetFileNameWithoutExtension(path);
            return string.IsNullOrEmpty(path) ? "NewImage" : path;
        }
    }
    #endregion

    #region Methods

    public Texture2D LoadAsset()
    {
        OpenAsset(ref _imageData);
        if (ImageData.CurrentFrame == null)
            ImageData.Init(16,16);
        return ImageData.CurrentFrame.GetCurrentImage().Texture;
    }
    public PIASession()
    {
        LoadNewAsset(16,16);
    }
    public Texture2D LoadImageFromFile()
    {

        string path = EditorUtility.OpenFilePanelWithFilters("Select Texture", "", new string[] { "PNG", "png", "JPG", "jpg" });

        //string path = EditorUtility.OpenFilePanel("Select Texture", "", "Image Files;*.jpg;*.png");

        if (string.IsNullOrEmpty(path))
            return null;

        Texture2D texture = new Texture2D(2, 2);
        texture.filterMode = FilterMode.Point;
        byte[] fileData = File.ReadAllBytes(path);
        texture.LoadImage(fileData);
        //ImageData.Width = texture.width;
        //ImageData.Height = texture.height;
        ImageData.CurrentFrame.GetCurrentImage().Texture = texture;
        return ImageData.CurrentFrame.GetCurrentImage().Texture;
    }

    private void OpenAsset(ref PIAImageData output)
    {
        string path = EditorUtility.OpenFilePanelWithFilters("Select Asset", "Assets/", new string[] { "ASSET", "asset" });
        if (string.IsNullOrEmpty(path))
            return;
        output = AssetDatabase.LoadAssetAtPath<PIAImageData>(FileUtil.GetProjectRelativePath(path));

    }

    public void ExportImage(Texture2D tex)
    {
        byte[] encodedBytes;
        string path = EditorUtility.SaveFilePanel("Save Image", "", ProjectName, "png");
        if (string.IsNullOrEmpty(path))
            return;
        string extension = Path.GetExtension(path);
        switch (extension)
        {
            case ".png":
                encodedBytes = tex.EncodeToPNG();
                break;
            case ".jpg":
                encodedBytes = tex.EncodeToJPG();
                break;
            default:
                encodedBytes = tex.EncodeToPNG();
                break;
        }
        File.WriteAllBytes(path, encodedBytes);
        AssetDatabase.Refresh();

    }
    public void SaveAsset()
    {
        string path;
        if (isNew)
        {
            path = EditorUtility.SaveFilePanel("Save Asset", "", ProjectName, "asset");
            path = FileUtil.GetProjectRelativePath(path);
            if (string.IsNullOrEmpty(path))
                return;
            AssetDatabase.CreateAsset(ImageData, path);
        }
        ImageData.CurrentFrame.GetCurrentImage().Save();
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(_imageData);

    }
    public void LoadNewAsset(int width, int height)
    {
        ImageData = ScriptableObject.CreateInstance<PIAImageData>();
        ImageData.Init(width,height);
    }

    #endregion






}
