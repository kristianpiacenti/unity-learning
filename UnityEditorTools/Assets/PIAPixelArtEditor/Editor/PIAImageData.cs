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
    public PIAFrame CurrentFrame { get { return frames[CurrentFrameIndex]; } }

    public List<PIAFrame> Frames { get { return frames; } set { frames = value; } }
    public List<PIALayer> Layers { get { return layers; } set { layers = value; } }

    public int Width { get { return _width; }  set { _width = value; } }
    public int Height { get { return _height; } set { _height = value; } }

    #endregion

    #region Methods

    public void Init(int width, int height)
    {
        Width = width;
        Height = height;
        frames = new List<PIAFrame>();
        layers = new List<PIALayer>();
        AddLayer();
    }
    public void AddLayer()
    {
        PIALayer layer = new PIALayer();
        layer.Index = layers.Count;
        layer.Name = "Layer" + layer.Index;
        layers.Add(layer);

        CurrentLayer = layers.Count - 1;
        if (frames.Count == 0)
            AddFrame();
        else
            foreach (var item in frames)
            {
                item.AddTexture();
            }
            
    }
    public void RemoveLayer(int index)
    {
        if (layers.Contains(layers[index]))
        {
            layers.Remove(layers[index]);
            CurrentLayer = index - 1;
            foreach (var item in frames)
            {
                item.RemoveTexture(index);
            }
        }
    }
    public void AddFrame() {
        PIAFrame frame = new PIAFrame();
        frame.Init(this);
        frames.Add(frame);
        CurrentFrameIndex = frames.Count - 1;
    }
    public void RemoveFrame(int index) {
        if (frames.Contains(frames[index]))
        {

            frames.Remove(frames[index]);
            CurrentFrameIndex = index - 1;
        }
    }
    #endregion
    
}

[System.Serializable]
public struct PIALayer {
    [SerializeField]
    private int _index;
    [SerializeField]
    private string _name;

    public int Index { get { return _index; } set { _index = value; } }
    public string Name { get { return _name; } set { _name = value; } }
}

[System.Serializable]
public class PIAFrame {
    [SerializeField]
    private List<PIATexture> textures;
    public void Init(PIAImageData _imageData) {
        textures = new List<PIATexture>();
        foreach (var item in _imageData.Layers)
        {
            AddTexture(_imageData);
        }
    }
    public void AddTexture(PIAImageData imageData)
    {
        PIATexture texture = new PIATexture();
        texture.Init(imageData.Width, imageData.Height, imageData.CurrentLayer);
        textures.Add(texture);
    }
    public void AddTexture() {
        PIAImageData imageData = PIASession.Instance.ImageData;
        AddTexture(imageData);
    }
    public void RemoveTexture(int index)
    {
        if (textures.Contains(textures[index]))
            textures.Remove(textures[index]);
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





