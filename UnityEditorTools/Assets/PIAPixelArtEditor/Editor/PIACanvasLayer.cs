using UnityEngine;
using UnityEditor;
public class PIACanvasLayer{

    public static int index;

    public string Name { get; set; }
    public Texture2D Texture { get; set; }


    public PIACanvasLayer() {
        index++;
        Name = "Layer" + index;
        Texture = new Texture2D(16, 16);
        ClearTexture();
        Texture.filterMode = FilterMode.Point;
    }
    public void ClearTexture() {
        for (int x = 0; x < Texture.width; x++)
        {
            for (int y = 0; y < Texture.height; y++)
            {
                Paint(x, y, Color.clear,false);
            }
        }
    }
    public void Paint(int x, int y, Color color,bool registerUndo=true) {
        if (registerUndo)
            Undo.RegisterCompleteObjectUndo(Texture, "Paint");
        Texture.SetPixel(x, y, color);
        Texture.Apply();
        
    }
    public void Erase(int x, int y, Color color) {
        Paint(x, y, Color.clear);
    }
}
