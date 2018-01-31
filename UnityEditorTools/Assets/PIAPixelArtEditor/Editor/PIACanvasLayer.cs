using UnityEngine;
using UnityEditor;
[System.Serializable]
public class PIACanvasLayer{
    #region Fields

    public int index;

    [SerializeField]
    private string _name;
    [SerializeField]
    private PIATexture _image;

    #endregion

    #region Properties

    public string Name { get { return _name; } set { _name = value; } }
    public PIATexture Image { get { return _image; } set { _image = value; } }


    #endregion

    #region Methods

    public void Init()
    {
        index++;
        Name = "Layer" + index;
        Image = new PIATexture();
        Image.Init(16, 16);
    }

    #endregion





}
[System.Serializable]
public class PIATexture {
    #region Fields
    [SerializeField]
    private Color[] map;
    [SerializeField]
    private int width;
    [SerializeField]
    private int height;

    private Texture2D texture;

    #endregion

    #region Properties

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

    public void Init(int _width, int _height)
    {
        width = _width;
        height = _height;

        texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        map = new Color[width * height];
        ClearTexture();
    }
    public void Init(Texture2D tex)
    {
        texture = tex;
        width = tex.width;
        height = tex.height;

        map = new Color[width * height];

    }

    public void ClearTexture()
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Paint(x, y, Color.clear, false);
            }
        }
    }

    public void Paint(int x, int y, Color color, bool registerUndo = true)
    {
        if (registerUndo)
            Undo.RegisterCompleteObjectUndo(texture, "Paint");
        texture.SetPixel(x, y, color);
        texture.Apply();
    }
    public void Paint(Color[] _map)
    {

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Paint(x, y, _map[(y * texture.width) + x], false);
            }
        }
    }
    public void Erase(int x, int y, Color color)
    {
        Paint(x, y, Color.clear);
    }

    public void Save()
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                map[(y * texture.width) + x] = texture.GetPixel(x, y);
            }
        }

    }
    private Texture2D LoadFromMap()
    {
        if (map.Length == 0)
        {
            Init(16, 16);
            return texture;
        }

        texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;

        Paint(map);
        return texture;
    }

    #endregion



}

