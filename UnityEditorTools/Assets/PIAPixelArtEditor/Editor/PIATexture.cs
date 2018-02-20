using UnityEditor;
using UnityEngine;

[System.Serializable]
public class PIATexture
{

    #region Static

    public static void Wipe(Texture2D tex)
    {
        Color[] colors = new Color[tex.width * tex.height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = PIADrawer.ClearColor;
        }
        tex.SetPixels(colors);

    }
    public static Texture2D CreateBlank(int width, int height)
    {
        Texture2D tex = new Texture2D(width, height);
        Wipe(tex);
        tex.Apply();
        return tex;

    }

    public static void Crop(ref Color[] map, ref Texture2D texture, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[(y * width) + x] = texture.GetPixel(x, y);
            }
        }
        texture.Resize(width, height);
        texture.SetPixels(map);
        texture.Apply();
    }

    #endregion

    #region Fields
    [SerializeField]
    private Color[] map;

    private Texture2D texture;
    int width = 16;
    int height = 16;
    #endregion

    #region Properties
    public int LayerIndex { get; set; }

    public Texture2D Texture
    {
        get
        {
            if (texture == null)
                texture = LoadFromMap();
            return texture;
        }
        set
        {
            Init(value);
        }
    }

    #endregion

    #region Methods

    public void Init(int _width, int _height,int _layerIndex)
    {
        width = _width;
        height = _height;
        LayerIndex = _layerIndex;
        texture = CreateBlank(width, height);
        texture.filterMode = FilterMode.Point;
        map = new Color[width * height];
    }
    public void Init(Texture2D tex)
    {
        map = new Color[width * height];
        texture = tex;
        Crop(ref map, ref tex, width, height);

    }
    public void ClearTexture(bool apply=false)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Paint(x, y, PIADrawer.ClearColor, false);
            }
        }
        if(apply)
            Texture.Apply();
    }


    public void Paint(int x, int y, Color color, bool registerUndo = true, bool apply = false)
    {
        if (registerUndo)
            Undo.RegisterCompleteObjectUndo(texture, "Paint");
       
        texture.SetPixel(x, y, color);
        if (apply)
            texture.Apply();
    }
    public void Paint(Color[] _map)
    {

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Paint(x, y, _map[(y * width) + x], false);
            }
        }
    }
    public void Erase(int x, int y, Color color)
    {
        Paint(x, y, PIADrawer.ClearColor);
    }

    public void Save()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[(y * width) + x] = texture.GetPixel(x, y);
            }
        }

    }
    private Texture2D LoadFromMap()
    {
        if (map.Length == 0)
        {
            Init(16, 16,0);
            return texture;
        }

        texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;

        Paint(map);
        return texture;
    }

    #endregion


   
}