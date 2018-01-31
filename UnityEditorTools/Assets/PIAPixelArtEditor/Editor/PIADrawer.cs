using UnityEngine;
using UnityEditor;
public enum PIADrawerType {
    Paint,
    Erase
}

public class PIADrawer{

    #region Fields

    #endregion

    #region Properties

    public PIADrawerType DrawerType { get; set; }
    public Color FirstColor { get; set; }
    public Color SecondColor { get; set; }
    public Vector2Int CurrentMousePosition { get; set; }

    #endregion


    #region Methods

    public PIADrawer()
    {
        DrawerType = PIADrawerType.Paint;
        FirstColor = Color.black;
        SecondColor = Color.clear;
    }

    public void OnGUIExecute(Event e, Vector2Int pixelCoordinate)
    {
        if (pixelCoordinate.x < 0 || pixelCoordinate.y < 0)
            return;
        PIACanvasLayer canvasLayer = PIASession.Instance.ImageData.Canvas.GetCurrentLayer();

        switch (DrawerType)
        {
            case PIADrawerType.Paint:
                if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
                {
                    if (e.button == 0)
                    {
                        canvasLayer.Image.Paint(pixelCoordinate.x, canvasLayer.Image.Texture.height - pixelCoordinate.y - 1, FirstColor);
                    }
                    if (e.button == 1)
                        canvasLayer.Image.Paint(pixelCoordinate.x, canvasLayer.Image.Texture.height - pixelCoordinate.y - 1, SecondColor);
                }

                break;
            case PIADrawerType.Erase:
                if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
                {
                    canvasLayer.Image.Paint(pixelCoordinate.x, canvasLayer.Image.Texture.height - pixelCoordinate.y - 1, Color.clear);
                }
                break;
        }


    }
    public void DrawCurrentPixelBox(float cellWidth)
    {
        GUI.Box(new Rect(CurrentMousePosition, Vector2.one * cellWidth), PIATextureDatabase.Instance.GetTexture("brush"));
    }

    #endregion





}

