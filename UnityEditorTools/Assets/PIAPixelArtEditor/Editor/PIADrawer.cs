using UnityEngine;
using UnityEditor;
public enum PIADrawerType {
    Paint,
    Erase
}

public class PIADrawer{

    public PIADrawerType DrawerType { get; set; }
    public Color FirstColor { get; set; }
    public Color SecondColor { get; set; }
    public Vector2Int CurrentMousePosition { get; set; }
    
    

    public PIADrawer() {
        DrawerType = PIADrawerType.Paint;
        FirstColor = Color.black;
        SecondColor = Color.clear;
    }
    public void OnGUIExecute(Event e,Vector2Int pixelCoordinate) {
        if (pixelCoordinate.x < 0 || pixelCoordinate.y < 0)
            return;
        if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
        {
            PIACanvasLayer canvasLayer = PIASession.Instance.ImageData.Canvas.GetCurrentLayer();
            switch (DrawerType)
            {
                case PIADrawerType.Paint:
                    if(e.button==0)
                        canvasLayer.Paint(pixelCoordinate.x, canvasLayer.Texture.height - pixelCoordinate.y - 1, FirstColor);
                    if(e.button==1)
                        canvasLayer.Paint(pixelCoordinate.x, canvasLayer.Texture.height - pixelCoordinate.y - 1, SecondColor);
                    break;
                case PIADrawerType.Erase:
                    canvasLayer.Paint(pixelCoordinate.x, canvasLayer.Texture.height - pixelCoordinate.y - 1, Color.clear);
                    break;
            }

        }
    }
    public void DrawCurrentPixelBox(float cellWidth) {
        GUI.Box(new Rect(CurrentMousePosition, Vector2.one * cellWidth), PIATextureDatabase.Instance.GetTexture("brush"));
    }

}

