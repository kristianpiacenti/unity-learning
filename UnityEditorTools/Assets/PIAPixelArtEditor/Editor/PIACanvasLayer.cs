using UnityEngine;

public class PIACanvasLayer{

    public static int index;

    public string Name { get; set; }
    public Texture2D Texture { get; set; }


    public PIACanvasLayer() {
        index++;
        Name = "Layer" + index;
        Texture = new Texture2D(2, 2);
        Texture.filterMode = FilterMode.Point;
    }
   
    public void Paint(int x, int y, Color color) {
        Texture.SetPixel(x, y, color);
    }
    public void Erase(int x, int y, Color color) {
        Paint(x, y, Color.clear);
    }
}
