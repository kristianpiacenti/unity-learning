using UnityEditor;
using UnityEngine;
using Piacenti.EditorTools;
using System.Collections.Generic;
using System.IO;
public class PIAEditorWindow : EditorWindow {
    #region Fields

    public static PIAEditorWindow window;

    const float INIT_SCALE_MULTIPLIER = 0.9f;

    WindowSection header;
    WindowSection leftSide;
    WindowSection body;
    WindowSection rightSide;
    List<WindowSection> sections;

    PIAInputArea bodyInputArea;
    PIAInputArea imageInputArea;

    PIADrawer drawer;

    GUISkin skin;


    float scaleMultiplier = INIT_SCALE_MULTIPLIER;
    float imageOffsetX = 0;
    float imageOffsetY = 0;
    Rect imageRect;
    Vector2Int pixelCoordinate;
    #endregion

    #region Methods
    [MenuItem("EditorTools/PIAPixelArtEditor")]
    public static void ShowWindow()
    {

        window = GetWindow<PIAEditorWindow>();
        Vector2 windowSize = new Vector2(800, 800);
        window.position = new Rect(Screen.width / 2 - windowSize.x / 2, 100, windowSize.x, windowSize.y);
        window.minSize = new Vector2(400, 200);
        window.Show();
    }

    private void OnEnable()
    {
        //DRAWER
        drawer = new PIADrawer();

        //INIT INPUT AREAS 
        bodyInputArea = new PIAInputArea();
        imageInputArea = new PIAInputArea();
        bodyInputArea.OnGUIUpdate += ChangeImageScaleMultiplier;
        bodyInputArea.OnGUIUpdate += (e) => drawer.OnGUIExecute(e,pixelCoordinate);
        bodyInputArea.OnGUIUpdate += ChangeImageOffset;
        //INIT WINDOW SECTIONS
        InitializeSections();

        //INIT GRAPHICS
        skin = Resources.Load<GUISkin>("Skins/PIAPixelArtEditorSkin");

    }

    private void OnGUI()
    {
       
        DrawLayouts();
        bodyInputArea.GUIUpdate(body.GetRect());

        pixelCoordinate = PIACanvas.WorldPositionToGridPixel(PIAInputArea.MousePosition, imageRect,
                   body.GetRect().position, PIASession.Instance.ImageData.Canvas.GetFinalImage());

      //  drawer.CurrentMousePosition = PIACanvas.GridPixelToWorldPosition(pixelCoordinate, imageRect, PIASession.Instance.ImageData.Canvas.GetFinalImage());
        DrawHeader();
        DrawLeftSide();
        DrawRightSide();
        DrawBody();



    }


    private void InitializeSections()
    {
        sections = new List<WindowSection>();
        header = new WindowSection(new Rect(0, 0, position.width, 50), new Color(0.0735f, 0.0735f, 0.0735f, 1.0000f));
        leftSide = new WindowSection(new Rect(0, header.GetRect().height, 125, position.height), new Color(0.1137f, 0.1137f, 0.1137f, 1.0000f));
        rightSide = new WindowSection(new Rect(position.width - 125, header.GetRect().height, 125, position.height), new Color(0.1137f, 0.1137f, 0.1137f, 1.0000f));
        body = new WindowSection(new Rect(leftSide.GetRect().width, header.GetRect().height, position.width - leftSide.GetRect().width - rightSide.GetRect().width,
            position.height - header.GetRect().height), new Color(0.6275f, 0.6275f, 0.6275f, 1.0000f));
        sections.Add(header);
        sections.Add(leftSide);
        sections.Add(rightSide);
        sections.Add(body);
    }
    private void DrawLayouts()
    {
        header.SetRect(0, 0, position.width, 50);
        leftSide.SetRect(0, header.GetRect().height, 125, position.height);
        rightSide.SetRect(position.width - 125, header.GetRect().height, 125, position.height);
        body.SetRect(leftSide.GetRect().width, header.GetRect().height, position.width - leftSide.GetRect().width - rightSide.GetRect().width,
            position.height - header.GetRect().height);

        foreach (var item in sections)
        {
            GUI.DrawTexture(item.GetRect(), item.GetTexture());
        }
    }

    private void DrawHeader()
    {
        GUILayout.BeginArea(header.GetRect());
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                GUILayout.Label(PIASession.Instance.ProjectName, skin.GetStyle("header"));
                GUILayout.FlexibleSpace();

            }
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();

        }
        GUILayout.EndArea();
    }
    private void DrawLeftSide()
    {
        Texture2D brush = PIATextureDatabase.Instance.GetTexture("brush");
        Texture2D pen = PIATextureDatabase.Instance.GetTexture("pen");
        Texture2D[] icons = new Texture2D[] { brush, pen, pen, pen, pen, pen };
            
        float iconsWidth = 48;
        int iconsPerLine = 2;

        GUILayout.BeginArea(new Rect(leftSide.GetRect().x + 10, leftSide.GetRect().y + 10, leftSide.GetRect().width - 20, leftSide.GetRect().height - 20));
        {
            drawer.DrawerType = (PIADrawerType)GUILayout.SelectionGrid((int)drawer.DrawerType, 
                new string[] { "Paint", "Erase", "Box", "Line", "Color", "Select" }, iconsPerLine, 
                skin.GetStyle("editorbutton"), GUILayout.MaxWidth(iconsWidth * iconsPerLine), GUILayout.MaxHeight(iconsWidth * icons.Length / 2));

            //drawer.DrawerType = (PIADrawerType)GUILayout.SelectionGrid((int)drawer.DrawerType,
            //    icons, iconsPerLine, skin.GetStyle("editorbutton"), 
            //    GUILayout.MaxWidth(iconsWidth * iconsPerLine), GUILayout.MaxHeight(iconsWidth * icons.Length/2));

            GUILayout.Space(10);
            drawer.FirstColor = EditorGUILayout.ColorField(drawer.FirstColor);
            GUILayout.Space(5);
            drawer.SecondColor= EditorGUILayout.ColorField(drawer.SecondColor);

            //GUILayout.SelectionGrid(0, new Texture2D[] { penTexture,penTexture,penTexture,penTexture,penTexture,penTexture }, 2, GUILayout.MaxWidth(55*2), GUILayout.MaxHeight(55*3));


            //GUILayout.BeginHorizontal();
            //{
            //    GUILayout.Button("Hello", GUILayout.MaxWidth(55), GUILayout.MaxHeight(55));
            //    GUILayout.Button("Hello", GUILayout.MaxWidth(55), GUILayout.MaxHeight(55));

            //}
            //GUILayout.EndHorizontal();
            //GUILayout.Space(5);
            //GUILayout.BeginHorizontal();
            //{
            //    GUILayout.Button("Hello", GUILayout.MaxWidth(55), GUILayout.MaxHeight(55));
            //    GUILayout.Button("Hello", GUILayout.MaxWidth(55), GUILayout.MaxHeight(55));

            //}
            //GUILayout.EndHorizontal();
            //GUILayout.Space(5);
            //GUILayout.BeginHorizontal();
            //{

            //    GUILayout.Button("hello", GUILayout.MaxWidth(55), GUILayout.MaxHeight(55));

            //    GUILayout.Button("Hello", GUILayout.MaxWidth(55), GUILayout.MaxHeight(55));

            //}
            //GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }
    private void DrawRightSide()
    {
        DrawPixelCoordinates();
        GUILayout.BeginArea(new Rect(rightSide.GetRect().x + 60, rightSide.GetRect().height / 3, rightSide.GetRect().width / 2, 55 * 5));
        {
            if (GUILayout.Button("Reset", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(55))) {
                scaleMultiplier = INIT_SCALE_MULTIPLIER;
                imageOffsetX = 0;
                imageOffsetY = 0;
            }
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("+", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(55)))
                {
                    scaleMultiplier += 0.2f;
                }
                if (GUILayout.Button("-", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(55)))
                {
                    scaleMultiplier -= 0.2f;
                }
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("New", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(55)))
            {
                PIANewImageWindow.ShowWindow();
            }
            if (GUILayout.Button("Load", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(55)))
            {
                PIASession.Instance.LoadAsset();

            }
            if (GUILayout.Button("Save", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(55)))
            {
                PIASession.Instance.SaveAsset();
            }
            if (GUILayout.Button("Import", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(55)))
            {
                PIASession.Instance.LoadImageFromFile();
            }
            if (GUILayout.Button("Export", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(55)))
            {
                PIASession.Instance.ExportImage(PIASession.Instance.ImageData.Canvas.GetFinalImage());

            }

        }
        GUILayout.EndArea();
    }
    private void DrawBody()
    {
        float scale = body.GetRect().width * scaleMultiplier;
        imageRect = new Rect((body.GetRect().width / 2 - scale / 2) + imageOffsetX, (body.GetRect().center.y - scale / 2)-header.GetRect().height + imageOffsetY, scale, scale);
        Texture2D tex = PIASession.Instance.ImageData.Canvas.GetFinalImage();
       
        GUILayout.BeginArea(body.GetRect());
        {
            if (tex != null)
            {
                EditorGUI.DrawTextureTransparent(imageRect, tex);
                DrawGrid(imageRect, new Vector2(tex.width, tex.height));
                drawer.DrawCurrentPixelBox(imageRect.width / tex.width);
            }
        }
        GUILayout.EndArea();


    }

    private void DrawGrid(Rect rect, Vector2 texDimension)
    {
        float cellWidth = (rect.width / texDimension.x);
        float cellHeight = (rect.height / texDimension.y);

        if (cellWidth <= 10 || cellHeight <= 10)
            return;

        Handles.BeginGUI();
        {
            Handles.color = Color.black;

            for (float offsetX = 0; offsetX <= rect.width; offsetX += cellWidth)
            {
                Handles.DrawLine(new Vector2(rect.x + offsetX, rect.y), new Vector2(rect.x + offsetX, rect.y + rect.height));

            }
            for (float offsetY = 0; offsetY <= rect.height; offsetY += cellHeight)
            {
                Handles.DrawLine(new Vector2(rect.x, rect.y + offsetY), new Vector2(rect.x + rect.width, rect.y + +offsetY));

            }
            Handles.color = Color.white;
        }
        Handles.EndGUI();
    }
    private void DrawPixelCoordinates()
    {
        //EditorGUILayout.LabelField(PIAInputArea.MousePosition.ToString(), skin.GetStyle("editorbutton2"));

        Texture tex = PIASession.Instance.ImageData.Canvas.GetFinalImage();
        

        if (pixelCoordinate.x < 0 || pixelCoordinate.y < 0 || pixelCoordinate.x > tex.width || pixelCoordinate.y > tex.height)
            pixelCoordinate = new Vector2Int(-1, -1);
        GUILayout.BeginArea(rightSide.GetRect());
        {
            
            EditorGUILayout.LabelField("[" + tex.width + "x" + tex.height + "]", skin.GetStyle("editorbutton2"));
            EditorGUILayout.LabelField(pixelCoordinate.ToString(), skin.GetStyle("editorbutton2"));

            window.Repaint();
        }
        GUILayout.EndArea();
    }

    public void ChangeImageScaleMultiplier(Event e)
    {
        if (e.type == EventType.ScrollWheel)
        {
            float deltaY = e.delta.y * (-1) * Time.fixedDeltaTime;
            scaleMultiplier += 0.7f * deltaY;
            window.Repaint();
        }
    }
    public void ChangeImageOffset(Event e) {
        if (e.type == EventType.KeyDown)
        {
            switch (e.keyCode)
            {
                case KeyCode.W:
                    imageOffsetY -= 480f * Time.fixedDeltaTime;
                    break;

                case KeyCode.S:
                    imageOffsetY += 480f * Time.fixedDeltaTime;

                    break;
                case KeyCode.A:
                    imageOffsetX -= 480f * Time.fixedDeltaTime;

                    break;

                case KeyCode.D:
                    imageOffsetX += 480f * Time.fixedDeltaTime;

                    break;
            }
        }
    }

    #endregion

    private class PIANewImageWindow: EditorWindow{

        static PIANewImageWindow window;
        int width = 16;
        int height = 16;
        
        public static void ShowWindow() {
           
            window = GetWindow<PIANewImageWindow>();
            window.maxSize = new Vector2(200, 200);
            window.Show();

        }

        private void OnGUI()
        {
            width = EditorGUILayout.IntField("Width: ",width);
            height = EditorGUILayout.IntField("Height: ", height);
            if (GUILayout.Button("Create")) {
                LoadNewAsset();
            }
        }
        private void LoadNewAsset() {
            PIASession.Instance.LoadNewAsset();
            PIASession.Instance.ImageData.Canvas.GetCurrentLayer().Texture.Resize(width,height);
            PIAEditorWindow.window.Repaint();
            window.Close();
        }
    }

}
