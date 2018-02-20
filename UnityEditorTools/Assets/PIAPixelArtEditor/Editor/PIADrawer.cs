﻿using UnityEngine;
using ProtoTurtle.BitmapDrawing;
public enum PIAToolType {
    Paint,
    Erase,
    Rectangle,
    RectangleFilled,
    Selection
}

public class PIADrawer{

    #region Static

    public static Color ClearColor { get { return new Color(255, 255, 255, 0); } }

    private static void TransformToLeftTop(ref Vector2Int point, int height) {
        point = new Vector2Int(point.x, height - point.y-1);
    }
    private static void SwapX(ref Vector2Int point0, ref Vector2Int point1)
    {
        Vector2Int tmp = point0;
        point0 = new Vector2Int(point1.x, point0.y);
        point1 = new Vector2Int(tmp.x, point1.y);
    }
    private static void SwapY(ref Vector2Int point0, ref Vector2Int point1)
    {
        Vector2Int tmp = point0;
        point0 = new Vector2Int(point0.x, point1.y);
        point1 = new Vector2Int(point1.x, tmp.y);
    }
    public static RectInt DrawFilledRectangle(PIATexture tex, Vector2Int startingPoint, Vector2Int finalPoint, Color color) {
        RectInt rectangle = DrawRectangle(tex, startingPoint, finalPoint, color);
        for (int x = rectangle.x+1; x < rectangle.xMax; x++)
        {
            for (int y = rectangle.y; y > rectangle.y - rectangle.height - 1; y--)
            {
                tex.Paint(x,y,color);
            }
        }
        tex.Texture.Apply();

        return rectangle;
    }
    public static RectInt DrawRectangle(PIATexture tex, Vector2Int startingPoint, Vector2Int finalPoint,Color color) {
        // 0,0 on upper left corner
        RectInt rectangle;
        TransformToLeftTop(ref startingPoint, tex.Texture.height);
        TransformToLeftTop(ref finalPoint, tex.Texture.height);

        if (startingPoint.x > finalPoint.x)
            SwapX(ref startingPoint, ref finalPoint);
        if (startingPoint.y < finalPoint.y)
            SwapY(ref startingPoint, ref finalPoint);

        rectangle = new RectInt(startingPoint.x, startingPoint.y, Mathf.Abs(finalPoint.x - startingPoint.x), Mathf.Abs(finalPoint.y - startingPoint.y));

        // Upper segment
        
        for (int x = startingPoint.x; x <= finalPoint.x; x++)
        {
            tex.Paint(x, startingPoint.y, color,true,false);
        }
        // Downer segment

        for (int x = startingPoint.x; x <= finalPoint.x; x++)
        {
            tex.Paint(x, finalPoint.y, color,true,false);
        }

        // Left segment   

        for (int y = startingPoint.y; y >= finalPoint.y; y--)
        {
            tex.Paint(startingPoint.x, y, color,true,false);
        }

        // Right segment

        for (int y = startingPoint.y; y >= finalPoint.y; y--)
        {
            tex.Paint(finalPoint.x, y, color,true,false);

        }

        tex.Texture.Apply();

        return rectangle;
    }
    public static void ClearRect(PIATexture tex, RectInt rectangle) {
        for (int x = rectangle.x; x <= rectangle.xMax; x++)
        {
            for (int y = rectangle.y; y >= rectangle.y - rectangle.height; y--)
            {
                tex.Paint(x, y, ClearColor);
            }
        }
        tex.Texture.Apply();
        
    }
    #endregion

    #region Fields

    private float dragDistance;
    private Vector2Int downPoint;
    private Vector2Int upPoint;
    private RectInt selectedRect;

    #endregion

    #region Properties

    public PIAToolType ToolType { get; set; }
    public Color FirstColor { get; set; }
    public Color SecondColor { get; set; }
    public Vector2Int CurrentMousePosition { get; set; }
    #endregion


    #region Methods

    public PIADrawer()
    {
        ToolType = PIAToolType.Paint;
        FirstColor = Color.black;
        SecondColor = ClearColor;
    }

    public void OnGUIExecute(Event e, Vector2Int pixelCoordinate)
    {
        if (pixelCoordinate.x < 0 || pixelCoordinate.y < 0)
            return;

        PIATexture helper = PIAEditorWindow.Instance.HelpToolTexture;
        PIAFrame frame = PIASession.Instance.ImageData.CurrentFrame;
        int width = PIASession.Instance.ImageData.Width;
        int height = PIASession.Instance.ImageData.Height;
        switch (ToolType)
        {
            case PIAToolType.Paint:

                if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
                {
                    if (e.button == 0)
                    {
                        frame.GetCurrentImage().Paint(pixelCoordinate.x, height - pixelCoordinate.y - 1, FirstColor);
                    }
                    if (e.button == 1)
                    {
                        frame.GetCurrentImage().Paint(pixelCoordinate.x, height - pixelCoordinate.y - 1, SecondColor);
                    }
                }
                else {
                    helper.Paint(pixelCoordinate.x, height - pixelCoordinate.y - 1, new Color(Color.black.r, Color.black.g, Color.black.b, 0.2f),false,true);

                }

                break;
            case PIAToolType.Erase:
                if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
                {
                    frame.GetCurrentImage().Paint(pixelCoordinate.x, height - pixelCoordinate.y - 1, ClearColor);
                }
                else {
                    helper.Paint(pixelCoordinate.x, height - pixelCoordinate.y - 1, new Color(Color.white.r, Color.white.g, Color.white.b, 0.5f), false,true);
                }
                break;
            case PIAToolType.Rectangle:

                if (e.type == EventType.MouseDown)
                {

                    downPoint = new Vector2Int(pixelCoordinate.x, pixelCoordinate.y);
                    if (e.button == 0)
                    {
                        DrawRectangle(helper, downPoint, pixelCoordinate, new Color(FirstColor.r, FirstColor.g, FirstColor.b, 0.5f));
                    }
                    if (e.button == 1)
                        DrawRectangle(helper, downPoint, pixelCoordinate, new Color(SecondColor.r, SecondColor.g, SecondColor.b, 0.5f));

                }
                if (e.type == EventType.MouseDrag)
                {
                    if (e.button == 0)
                    {
                        DrawRectangle(helper, downPoint, pixelCoordinate, new Color(FirstColor.r, FirstColor.g, FirstColor.b, 0.5f));
                    }
                    if (e.button == 1)
                        DrawRectangle(helper, downPoint, pixelCoordinate, new Color(SecondColor.r, SecondColor.g, SecondColor.b, 0.5f));

                }
                if (e.type == EventType.MouseUp)
                {
                    upPoint = new Vector2Int(pixelCoordinate.x, pixelCoordinate.y);
                    if (e.button == 0)
                    {
                        DrawRectangle(frame.GetCurrentImage(), downPoint, upPoint, FirstColor);
                    }
                    if (e.button == 1)
                        DrawRectangle(frame.GetCurrentImage(), downPoint, upPoint, SecondColor);

                    helper.ClearTexture(true);
                }
                break;
            case PIAToolType.RectangleFilled:

                if (e.type == EventType.MouseDown)
                {

                    downPoint = new Vector2Int(pixelCoordinate.x, pixelCoordinate.y);
                    if (e.button == 0)
                    {
                        DrawFilledRectangle(helper, downPoint, pixelCoordinate, new Color(FirstColor.r, FirstColor.g, FirstColor.b, 0.5f));
                    }
                    if (e.button == 1)
                        DrawFilledRectangle(helper, downPoint, pixelCoordinate, new Color(SecondColor.r, SecondColor.g, SecondColor.b, 0.5f));

                }
                if (e.type == EventType.MouseDrag)
                {
                    if (e.button == 0)
                    {
                        DrawFilledRectangle(helper, downPoint, pixelCoordinate, new Color(FirstColor.r, FirstColor.g, FirstColor.b, 0.5f));
                    }
                    if (e.button == 1)
                        DrawFilledRectangle(helper, downPoint, pixelCoordinate, new Color(SecondColor.r, SecondColor.g, SecondColor.b, 0.5f));

                }
                if (e.type == EventType.MouseUp)
                {
                    upPoint = new Vector2Int(pixelCoordinate.x, pixelCoordinate.y);
                    if (e.button == 0)
                    {
                        DrawFilledRectangle(frame.GetCurrentImage(), downPoint, upPoint, FirstColor);
                    }
                    if (e.button == 1)
                        DrawFilledRectangle(frame.GetCurrentImage(), downPoint, upPoint, SecondColor);

                    helper.ClearTexture(true);
                }
                break;
            case PIAToolType.Selection:
                if (e.type == EventType.MouseDown)
                {

                    downPoint = new Vector2Int(pixelCoordinate.x, pixelCoordinate.y);
                    DrawFilledRectangle(helper, downPoint, pixelCoordinate, new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.5f));

                }
                if (e.type == EventType.MouseDrag)
                {
                    selectedRect = DrawFilledRectangle(helper, downPoint, pixelCoordinate, new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.5f));
                }
                if (e.keyCode == KeyCode.Delete) {
                    ClearRect(frame.GetCurrentImage(), selectedRect);
                    helper.ClearTexture(true);
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

