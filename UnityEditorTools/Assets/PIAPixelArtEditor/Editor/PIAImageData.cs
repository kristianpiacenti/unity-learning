using UnityEngine;
using System.Linq;
using System.Collections.Generic;
[CreateAssetMenu(fileName ="NewImageData",menuName ="Create ImageData",order =0)]
public class PIAImageData : ScriptableObject {
    #region Fields

  
    [SerializeField]
    private List<PIALayer> layers;
    [SerializeField]
    private List<PIAFrame> frames;

    [SerializeField]
    private int _width;
    [SerializeField]
    private int _height;

    #endregion

    #region Properties

    public int CurrentFrameIndex { get; set; }
    public int CurrentLayer { get; set; }
    public List<PIAFrame> Frames { get { return frames; } set { frames = value; } }
    public int Width { get { return _width; }  set { _width = value; } }
    public int Height { get { return _height; } set { _height = value; } }
    public PIAFrame CurrentFrame { get { return frames[CurrentFrameIndex]; } }

    #endregion

    #region Methods

    public void Init(int width, int height)
    {
        Width = width;
        Height = height;
        frames = new List<PIAFrame>();
        AddFrame();
    }
    public void AddFrame() {
        PIAFrame frame = new PIAFrame();
        frame.Init(this);
        frames.Add(frame);
        CurrentFrameIndex = frames.Count - 1;
    }
    #endregion
    
}
public struct PIALayer {
    public int Index { get; set; }
    public string Name { get; set; }
}
[System.Serializable]
public class PIAFrame {
    [SerializeField]
    private List<PIATexture> textures;
    public void Init(PIAImageData _imageData) {
        textures = new List<PIATexture>();
        PIATexture texture = new PIATexture();
        texture.Init(_imageData.Width, _imageData.Height, _imageData.CurrentLayer);
        textures.Add(texture);
    }
    public PIATexture GetCurrentImage()
    {
        return textures[PIASession.Instance.ImageData.CurrentLayer];
    }
    public Texture2D GetFrameTexture() {
        Texture2D finalTexture = new Texture2D(PIASession.Instance.ImageData.Width, PIASession.Instance.ImageData.Height);
        finalTexture.filterMode = FilterMode.Point;
        textures = textures.OrderBy(x => x.LayerIndex).ToList();
        foreach (var item in textures)
        {
            for (int x = 0; x < PIASession.Instance.ImageData.Width; x++)
            {
                for (int y = 0; y < PIASession.Instance.ImageData.Height; y++)
                {
                    Color pixelColor = item.Texture.GetPixel(x, y);
                    if (!pixelColor.Equals(Color.clear))
                    {
                        finalTexture.SetPixel(x, y, pixelColor);
                    }
                }
            }

        }
        finalTexture.Apply();

        return finalTexture;

    }
}





