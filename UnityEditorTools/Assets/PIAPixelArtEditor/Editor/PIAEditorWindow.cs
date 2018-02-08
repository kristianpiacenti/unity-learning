using UnityEditor;
using UnityEngine;
using Piacenti.EditorTools;
using System.Collections.Generic;


public class PIAEditorWindow : EditorWindow {
    #region Fields
    
    private static PIAEditorWindow _instance;
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
    PIAGrid grid;

    float scaleMultiplier = INIT_SCALE_MULTIPLIER;
    float imageOffsetX = 0;
    float imageOffsetY = 0;
    
    Vector2Int mouseCellCoordinate;

    
    #endregion

    #region Properties

    public static PIAEditorWindow Instance
    {
        get
        {
            return _instance;
        }
    }
    public Rect BodyRect { get { return body.GetRect(); } }
    #endregion


    #region Methods
    [MenuItem("EditorTools/PIAPixelArtEditor")]
    public static void ShowWindow()
    {

        window = GetWindow<PIAEditorWindow>();
        Vector2 windowSize = new Vector2(995, 800);
        window.position = new Rect(Screen.width / 2 - windowSize.x / 2, 100, windowSize.x, windowSize.y);
        window.minSize = new Vector2(400, 200);
        window.Show();
    }

    private void OnEnable()
    {
        if(_instance==null)
            _instance = this;

        //DRAWER
        drawer = new PIADrawer();
        grid = new PIAGrid();
        
        //INIT INPUT AREAS 
        bodyInputArea = new PIAInputArea();
        imageInputArea = new PIAInputArea();
        bodyInputArea.OnGUIUpdate += ChangeImageScaleMultiplier;
        bodyInputArea.OnGUIUpdate += (e) => drawer.OnGUIExecute(e,mouseCellCoordinate);
        bodyInputArea.OnGUIUpdate += ChangeImageOffset;
        
        //INIT WINDOW SECTIONS
        InitializeSections();

        //INIT GRAPHICS
        skin = Resources.Load<GUISkin>("Skins/PIAPixelArtEditorSkin");

    }
    private void Update()
    {
        PIAAnimator.Instance.Update();
    }
    private void OnGUI()
    {
       
        DrawLayouts();

        bodyInputArea.GUIUpdate(body.GetRect());


      //  drawer.CurrentMousePosition = PIACanvas.GridPixelToWorldPosition(pixelCoordinate, imageRect, PIASession.Instance.ImageData.Canvas.GetFinalImage());
        DrawHeader();
        DrawLeftSide();
        DrawRightSide();
        DrawBody();

        mouseCellCoordinate = grid.WorldToCellPosition(PIAInputArea.MousePosition);
        if (mouseCellCoordinate.x < 0 || mouseCellCoordinate.y < 0 || mouseCellCoordinate.x >= PIASession.Instance.ImageData.Width || mouseCellCoordinate.y >= PIASession.Instance.ImageData.Height)
            mouseCellCoordinate = new Vector2Int(-1, -1);


    }


    private void InitializeSections()
    {
        sections = new List<WindowSection>();
        header = new WindowSection(new Rect(0, 0, position.width, 40), new Color(0.0735f, 0.0735f, 0.0735f, 1.0000f));
        leftSide = new WindowSection(new Rect(0, header.GetRect().height, 220, position.height), new Color(0.1137f, 0.1137f, 0.1137f, 1.0000f));
        rightSide = new WindowSection(new Rect(position.width - 220, header.GetRect().height, 220, position.height), new Color(0.1137f, 0.1137f, 0.1137f, 1.0000f));
        
        body = new WindowSection(new Rect(leftSide.GetRect().width, header.GetRect().height, position.width - leftSide.GetRect().width - rightSide.GetRect().width,
            position.height - header.GetRect().height ), new Color(0.6275f, 0.6275f, 0.6275f, 1.0000f));
        sections.Add(header);
        sections.Add(leftSide);
        sections.Add(rightSide);
        sections.Add(body);
    }
    private void DrawLayouts()
    {
        header.SetRect(0, 0, position.width, 45);
        leftSide.SetRect(0, header.GetRect().height, 220, position.height);
        rightSide.SetRect(position.width - 220, header.GetRect().height, 220, position.height);
      
        body.SetRect(leftSide.GetRect().width, header.GetRect().y+header.GetRect().height, position.width - leftSide.GetRect().width - rightSide.GetRect().width,
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
        Texture2D square = PIATextureDatabase.Instance.GetTexture("square");

        Texture2D[] icons = new Texture2D[] { brush, pen, pen, pen, pen, pen };
            
        float iconsWidth = 40;
        int iconsPerLine = 2;
        DrawPixelCoordinates();


        Vector2 rectOffset = new Vector2(20, 20);
        Rect leftRect = leftSide.GetRect();
        Rect drawToolsRect = new Rect(leftRect.x + rectOffset.x, leftRect.y + rectOffset.y, leftRect.width - (rectOffset.x * 2), 300);

        GUILayout.BeginArea(drawToolsRect);
        {

            
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                drawer.DrawerType = (PIADrawerType)GUILayout.SelectionGrid((int)drawer.DrawerType,
                    icons, iconsPerLine, skin.GetStyle("editorbutton"),
                    GUILayout.MaxWidth(iconsWidth * iconsPerLine), GUILayout.MaxHeight(iconsWidth * icons.Length / 2));
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginArea(new Rect(0,170, leftSide.GetRect().width, 200));
            {
                
                drawer.FirstColor = EditorGUI.ColorField(new Rect(0, 0, 50, 50), new GUIContent(""), drawer.FirstColor, false, true, false, null);
                drawer.SecondColor = EditorGUI.ColorField(new Rect(25, 25, 50, 50), new GUIContent(""), drawer.SecondColor, false, true, false, null);
                
                
            }
            GUILayout.EndArea();

            
        }
        GUILayout.EndArea();



        Rect sessionRect = new Rect(leftRect.x + rectOffset.x, drawToolsRect.yMax + rectOffset.y, leftRect.width - (rectOffset.x * 2), 90);

        GUILayout.BeginArea(sessionRect);
        {
            GUILayout.BeginVertical(skin.GetStyle("buttonsrect"));
            {
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("New", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(40)))
                    {
                        PIANewImageWindow.ShowWindow();
                    }
                    if (GUILayout.Button("Load", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(40)))
                    {
                        PIASession.Instance.LoadAsset();

                    }
                    if (GUILayout.Button("Save", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(40)))
                    {
                        PIASession.Instance.SaveAsset();
                    }

                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Import", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(40)))
                    {
                        PIASession.Instance.LoadImageFromFile();
                    }
                    if (GUILayout.Button("Export", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(40)))
                    {
                        PIASession.Instance.ExportImage(PIASession.Instance.ImageData.CurrentFrame.GetFrameTexture());


                    }
                    GUILayout.FlexibleSpace();

                }
                GUILayout.EndHorizontal();

            }
            GUILayout.EndVertical();

        }
        GUILayout.EndArea();

        Rect layerRect = new Rect(leftRect.x + rectOffset.x, sessionRect.yMax + rectOffset.y, leftRect.width - (rectOffset.x * 2), 90);

        GUILayout.BeginArea(layerRect);
        {
            GUILayout.BeginVertical(skin.GetStyle("buttonsrect"));
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Layers", skin.GetStyle("editorbutton2"), GUILayout.MaxHeight(40));


                }
                GUILayout.EndHorizontal();

                GUILayout.Label("", skin.GetStyle("horizontalsplitter"), GUILayout.MaxHeight(1));

                for (int i = 0; i < PIASession.Instance.ImageData.Layers.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(PIASession.Instance.ImageData.Layers[i].Name, skin.GetStyle("editorbutton2"));
                        if (GUILayout.Button("->", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(40)))
                        {
                            PIASession.Instance.ImageData.CurrentLayer = i;

                        }
                        if (i > 0)
                            if (GUILayout.Button("-", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(40)))
                            {
                                PIASession.Instance.ImageData.RemoveLayer(i);

                            }

                    }
                    GUILayout.EndHorizontal();

                }



                GUILayout.Label("", skin.GetStyle("horizontalsplitter"), GUILayout.MaxHeight(1));

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("+", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(40)))
                    {
                        PIASession.Instance.ImageData.AddLayer();
                    }

                    GUILayout.FlexibleSpace();

                }
                GUILayout.EndHorizontal();

            }
            GUILayout.EndVertical();

        }
        GUILayout.EndArea();

        Rect palettesRect = new Rect(leftRect.x + rectOffset.x, layerRect.yMax + rectOffset.y, leftRect.width - (rectOffset.x * 2), 90);

        GUILayout.BeginArea(palettesRect);
        {
            GUILayout.BeginVertical(skin.GetStyle("buttonsrect"));
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Palettes", skin.GetStyle("editorbutton2"), GUILayout.MaxHeight(40));


                }
                GUILayout.EndHorizontal();
                GUILayout.Label("", skin.GetStyle("horizontalsplitter"), GUILayout.MaxHeight(1));
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("+", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(40)))
                    {

                    }
                    if (GUILayout.Button("-", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(40)))
                    {

                    }
                    GUILayout.FlexibleSpace();

                }
                GUILayout.EndHorizontal();

            }
            GUILayout.EndVertical();

        }
        GUILayout.EndArea();

    }
    private void DrawRightSide()
    {
        Vector2 offset = new Vector2(20, 20);

        Rect animationPreviewRect = new Rect(rightSide.GetRect().x + offset.x, rightSide.GetRect().y, rightSide.GetRect().width, 220);
        GUILayout.BeginArea(animationPreviewRect);
        {
            EditorGUI.DrawTextureTransparent(new Rect(0, offset.y, rightSide.GetRect().width - (offset.x * 2), 200 - offset.y),
                PIAAnimator.Instance.GetFrameOrFirst().GetFrameTexture());
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            {
                // GUILayout.Label(PIAAnimator.Instance.Speed.ToString() + " FPS",skin.GetStyle("editorbutton2"),GUILayout.MaxWidth(40));
                PIAAnimator.Instance.Speed = (int)GUILayout.HorizontalSlider(PIAAnimator.Instance.Speed, 1, 24, GUILayout.MaxWidth(rightSide.GetRect().width - (offset.x * 2)));

            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();

        PIAImageData imageData = PIASession.Instance.ImageData;
        int space = 10;

        Rect framesRect = new Rect(rightSide.GetRect().x + offset.x, animationPreviewRect.yMax + offset.y, rightSide.GetRect().width, rightSide.GetRect().height-animationPreviewRect.height);

        GUILayout.BeginArea(framesRect);
        {

            for (int i = 0; i < imageData.Frames.Count; i++)
            {
                Rect previewRect = new Rect(20, space, 100, 100);
                EditorGUI.DrawTextureTransparent(previewRect, imageData.Frames[i].GetFrameTexture());
                if (GUI.Button(new Rect(previewRect.x, previewRect.y, previewRect.width / 5, previewRect.height / 5), "->"))
                {
                    imageData.CurrentFrameIndex = i;
                }
                if (i > 0)
                    if (GUI.Button(new Rect(previewRect.max.x - (previewRect.width / 5), previewRect.y, previewRect.width / 5, previewRect.height / 5), "-"))
                    {
                        imageData.RemoveFrame(i);
                    }
                space += 110;
            }

            if (GUI.Button(new Rect(50, space + 35, 25, 25), "+"))
            {
                imageData.AddFrame();
            }

        }
        GUILayout.EndArea();
    }
    private void DrawBody()
    {
        float scale = body.GetRect().width * scaleMultiplier;
        grid.Grid= new Rect((body.GetRect().width / 2 - scale / 2) + imageOffsetX, (BodyRect.center.y - scale / 2) - header.GetRect().height+ imageOffsetY, scale, scale);
        
        GUILayout.BeginArea(body.GetRect());
        {
            EditorGUI.DrawTextureTransparent(grid.Grid, PIASession.Instance.ImageData.CurrentFrame.GetCurrentImage().Texture);
            
            DrawGrid(grid.Grid);
        }
        GUILayout.EndArea();


    }
    
    private void DrawGrid(Rect rect)
    {
       
        if (grid.CellWidth <= 10 || grid.CellHeight <= 10)
            return;

        for (float offsetX = 0; offsetX <= rect.width+1; offsetX += grid.CellWidth)
        {
            EditorGUI.DrawRect(new Rect(new Vector2(rect.x + offsetX, rect.y), new Vector2(1, rect.height)), Color.black);
        }
        for (float offsetY = 0; offsetY <= rect.height+1; offsetY += grid.CellHeight)
        {
            EditorGUI.DrawRect(new Rect(new Vector2(rect.x , rect.y + offsetY), new Vector2(rect.width, 1)), Color.black);

        }
        //Handles.BeginGUI();
        //{
        //    Handles.color = Color.black;

        //    for (float offsetX = 0; offsetX <= rect.width; offsetX += grid.CellWidth)
        //    {
        //        Handles.DrawLine(new Vector2(rect.x + offsetX, rect.y), new Vector2(rect.x + offsetX, rect.y + rect.height));
        //    }
        //    for (float offsetY = 0; offsetY <= rect.height; offsetY += grid.CellHeight)
        //    {
        //        Handles.DrawLine(new Vector2(rect.x, rect.y + offsetY), new Vector2(rect.x + rect.width, rect.y + +offsetY));

        //    }
        //    Handles.color = Color.white;
        //}
        //Handles.EndGUI();
    }
    private void DrawPixelCoordinates()
    {
        //EditorGUILayout.LabelField(PIAInputArea.MousePosition.ToString(), skin.GetStyle("editorbutton2"));
        PIAImageData imageData = PIASession.Instance.ImageData;

        Rect leftRect = leftSide.GetRect();
        GUILayout.BeginArea(new Rect(leftRect.x, leftRect.height - 175,leftRect.width,175));
        {
            EditorGUILayout.LabelField("[" + imageData.Width + "x" + imageData.Height + "]", skin.GetStyle("editorbutton2"));
            EditorGUILayout.LabelField(mouseCellCoordinate.ToString(), skin.GetStyle("editorbutton2"));
            GUILayout.Space(30);
            

            if (GUILayout.Button("Reset", skin.GetStyle("editorbutton2")))
            {
                scaleMultiplier = INIT_SCALE_MULTIPLIER;
                imageOffsetX = 0;
                imageOffsetY = 0;
            }
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("+", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
                {
                    scaleMultiplier += 0.2f;
                }
                if (GUILayout.Button("-", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
                {
                    scaleMultiplier -= 0.2f;
                }
                GUILayout.FlexibleSpace();

            }
            EditorGUILayout.EndHorizontal();
            
            if(window!=null)
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
            PIASession.Instance.LoadNewAsset(width,height);
            PIAEditorWindow.window.Repaint();
            window.Close();
        }
    }

}
