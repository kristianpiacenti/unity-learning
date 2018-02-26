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
            CurrentLayer = Mathf.Max(0,index - 1);
            foreach (var item in frames)
            {
                item.RemoveTexture(index);
            }
        }
    }
    public PIAFrame AddFrame() {
        PIAFrame frame = new PIAFrame();
        frame.Init(this);
        frames.Add(frame);
        CurrentFrameIndex = frames.Count - 1;
        return frame;
    }
    public void RemoveFrame(int index) {
        if (frames.Contains(frames[index]))
        {

            frames.Remove(frames[index]);
            CurrentFrameIndex = Mathf.Max(0, index-1);
            
        }
    }
    public void MoveFrameUp(int currentIndex) {
        PIAFrame tmp = frames[currentIndex-1];
        frames[currentIndex - 1] = frames[currentIndex];
        frames[currentIndex] = tmp;
    }
    public void MoveFrameDown(int currentIndex) {
        PIAFrame tmp = frames[currentIndex + 1];
        frames[currentIndex + 1] = frames[currentIndex];
        frames[currentIndex] = tmp;
    }
    #endregion
    
}

[System.Serializable]
public struct PIALayer {
    [SerializeField]
    private int _index;
    [SerializeField]
    private string _name;
    [SerializeField]
    private bool _hidden;

    public bool Hidden { get { return _hidden; } set { _hidden = value; } }
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
    public PIATexture AddTexture(PIAImageData imageData)
    {
        PIATexture texture = new PIATexture();
        texture.Init(imageData.Width, imageData.Height, imageData.CurrentLayer);
        textures.Add(texture);
        return texture;
    }
    public PIATexture AddTexture() {
        PIAImageData imageData = PIASession.Instance.ImageData;
        return AddTexture(imageData);
    }
    public void CopyFrom(PIAFrame source) {
        textures.Clear();
        foreach (var item in source.textures)
        {
            byte[] itemData = item.Texture.EncodeToPNG();
            PIATexture piaTexture = AddTexture();
            piaTexture.LayerIndex = item.LayerIndex;
            Texture2D texture = piaTexture.Texture;
            texture.LoadImage(itemData);
            texture.Apply();
        }
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
    public Texture2D GetFrameTextureWithLayerFilters() {
        PIAImageData imageData = PIASession.Instance.ImageData;
        Texture2D finalTexture = PIATexture.CreateBlank(PIASession.Instance.ImageData.Width, PIASession.Instance.ImageData.Height);
        finalTexture.filterMode = FilterMode.Point;
        Color nativeColor = finalTexture.GetPixel(0, 0);

        List<int> hiddenLayersIndex = new List<int>();
        foreach (var item in imageData.Layers)
        {
            if (item.Hidden == true)
                hiddenLayersIndex.Add(item.Index);
        }
        textures = textures.OrderBy(x => x.LayerIndex).ToList();
        foreach (var item in textures)
        {
            if (hiddenLayersIndex.Contains(item.LayerIndex))
                continue;

            if (imageData.CurrentLayer == item.LayerIndex)
            {
                for (int x = 0; x < PIASession.Instance.ImageData.Width; x++)
                {
                    for (int y = 0; y < PIASession.Instance.ImageData.Height; y++)
                    {
                        Color pixelColor = item.Texture.GetPixel(x, y);
                        if (pixelColor.a>0)
                        {
                            finalTexture.SetPixel(x, y, pixelColor);
                        }

                    }
                }
            }
            else
            {
                for (int x = 0; x < PIASession.Instance.ImageData.Width; x++)
                {
                    for (int y = 0; y < PIASession.Instance.ImageData.Height; y++)
                    {
                        Color pixelColor = item.Texture.GetPixel(x, y);
                        pixelColor.a = pixelColor.a / 2;
                        if (pixelColor.a > 0 && nativeColor.Equals(finalTexture.GetPixel(x, y)))
                        {
                            finalTexture.SetPixel(x, y, pixelColor);
                        }
                    }
                }
            }
        }
        finalTexture.Apply();

        return finalTexture;
    }
    public Texture2D GetFrameTexture() {
        Texture2D finalTexture = PIATexture.CreateBlank(PIASession.Instance.ImageData.Width, PIASession.Instance.ImageData.Height);
        finalTexture.filterMode = FilterMode.Point;
        textures = textures.OrderBy(x => x.LayerIndex).ToList();
        foreach (var item in textures)
        {
            for (int x = 0; x < PIASession.Instance.ImageData.Width; x++)
            {
                for (int y = 0; y < PIASession.Instance.ImageData.Height; y++)
                {
                    Color pixelColor = item.Texture.GetPixel(x, y);
                    if (pixelColor.a>0)
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





