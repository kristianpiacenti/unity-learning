using UnityEditor;
using UnityEngine;
using Piacenti.EditorTools;
using System.Collections.Generic;
using System;

public class PIAEditorWindow : EditorWindow {
    #region Fields
    
    private static PIAEditorWindow _instance;
    public static PIAEditorWindow window;

    const float INIT_SCALE_MULTIPLIER = 0.95f;

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
    Color buttonsBG = new Color(0.1294f, 0.1294f, 0.1294f, 1.0000f);

    Vector2 framesSlider;
    Vector2 layersSlider;

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
    public PIATexture HelpToolTexture { get; set; }
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
        HelpToolTexture = new PIATexture();
        HelpToolTexture.Init(PIASession.Instance.ImageData.Width, PIASession.Instance.ImageData.Height, 0);
        //INIT WINDOW SECTIONS
        InitializeSections();

        //INIT GRAPHICS
        skin = Resources.Load<GUISkin>("Skins/PIAPixelArtEditorSkin");

    }
    private void OnDisable()
    {
        PIAExportSettingsWindow.CloseWindow();
        PIAExtendedPreviewWindow.CloseWindow();
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
      // DrawLeftSide();
        DrawLeftSection();
      //  DrawRightSide();
        DrawRightSection();
        DrawBody();

        mouseCellCoordinate = grid.WorldToCellPosition(PIAInputArea.MousePosition);
        if (mouseCellCoordinate.x < 0 || mouseCellCoordinate.y < 0 || mouseCellCoordinate.x >= PIASession.Instance.ImageData.Width || mouseCellCoordinate.y >= PIASession.Instance.ImageData.Height)
            mouseCellCoordinate = new Vector2Int(-1, -1);

        window.Repaint();
    }


    private void InitializeSections()
    {
        Texture2D sides = PIATextureDatabase.Instance.GetTexture("sides");
        
        sections = new List<WindowSection>();
        header = new WindowSection(new Rect(0, 0, position.width, 40), buttonsBG);
        //leftSide = new WindowSection(new Rect(0, header.GetRect().height, 220, position.height), new Color(0.1137f, 0.1137f, 0.1137f, 1.0000f));
        leftSide = new WindowSection(new Rect(0, header.GetRect().height, 220, position.height - header.GetRect().height), sides);

        rightSide = new WindowSection(new Rect(position.width - 220, header.GetRect().height, 220, position.height - header.GetRect().height), sides);
        
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
        leftSide.SetRect(0, header.GetRect().height, 220, position.height - header.GetRect().height);
        rightSide.SetRect(position.width - 220, header.GetRect().height, 220, position.height - header.GetRect().height);
      
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
            //DrawProjectName();
            DrawToolbar();
        }
        GUILayout.EndArea();
    }
    private void DrawLeftSection() {
        Rect leftRect = leftSide.GetRect();

        GUILayout.BeginArea(leftRect);
        {

            Rect preview  = DrawPreview(leftRect);
            DrawLayers(preview);
            DrawProjectName(leftRect);

        }
        GUILayout.EndArea();

    }
    private void DrawRightSection() {
        Rect rightRect = rightSide.GetRect();

        GUILayout.BeginArea(rightRect);
        {
            DrawFrames(rightRect);
            DrawSessionBar(rightRect);
            DrawGridInfo(rightRect);

        }
        GUILayout.EndArea();
       // Debug.Log(PIAInputArea.MousePosition);

    }
    private void DrawBody()
    {
        float scale = body.GetRect().width * scaleMultiplier;
        grid.Grid= new Rect((body.GetRect().width / 2 - scale / 2) + imageOffsetX, (BodyRect.center.y - scale / 2) - header.GetRect().height+ imageOffsetY, scale, scale);
        
        GUILayout.BeginArea(body.GetRect());
        {

            EditorGUI.DrawTextureTransparent(grid.Grid, PIASession.Instance.ImageData.CurrentFrame.GetFrameTextureWithLayerFilters());
            GUI.DrawTexture(grid.Grid, HelpToolTexture.Texture);
            DrawGrid(grid.Grid);
            HelpToolTexture.ClearTexture();
        }
        GUILayout.EndArea();


    }

    private void DrawToolbar()
    {
        Texture2D pen = PIATextureDatabase.Instance.GetTexture("pen");
        Texture2D eraser = PIATextureDatabase.Instance.GetTexture("eraser");
        Texture2D squareTool = PIATextureDatabase.Instance.GetTexture("squaretool");
        Texture2D filledSquareTool = PIATextureDatabase.Instance.GetTexture("filledsquaretool");
        Texture2D selectionBox = PIATextureDatabase.Instance.GetTexture("selectionbox");

        Texture2D[] icons = new Texture2D[] { pen, eraser, squareTool, filledSquareTool, selectionBox };

        float iconWidth = 36;
        int iconsPerLine = icons.Length;
        int spaceBetweenIcons = 8;
        float toolbarMaxWidth = iconWidth * iconsPerLine + spaceBetweenIcons * iconsPerLine;
        float toolbarMaxHeight = iconWidth;

        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Space(15);
            drawer.ToolType = (PIAToolType)GUILayout.SelectionGrid((int)drawer.ToolType,
            icons, iconsPerLine, skin.GetStyle("editorbutton"),
            GUILayout.MaxWidth(toolbarMaxWidth), GUILayout.MaxHeight(toolbarMaxHeight));


        }
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();


        Rect firstColorRect = new Rect(toolbarMaxWidth + 35, 8, 60, 20);
        Rect secondColorRect = new Rect(firstColorRect.x + 35, 18, 60, 20);

        drawer.FirstColor = EditorGUI.ColorField(firstColorRect, new GUIContent(""), drawer.FirstColor, false, true, false, null);
        drawer.SecondColor = EditorGUI.ColorField(secondColorRect, new GUIContent(""), drawer.SecondColor, false, true, false, null);

    }
    private void DrawProjectName(Rect parent)
    {
        float projectNameRectHeight = 40;
        Rect projectNameRect = new Rect(0, parent.height - projectNameRectHeight, parent.width, projectNameRectHeight);

        Texture2D layerBG = new Texture2D(1, 1);
        layerBG.SetPixel(0, 0, buttonsBG);
        layerBG.Apply();
        GUI.DrawTexture(projectNameRect, layerBG);

        GUILayout.BeginArea(projectNameRect);
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(PIASession.Instance.ProjectName, skin.GetStyle("projectname"));
                GUILayout.FlexibleSpace();


            }
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();

        }
        GUILayout.EndArea();
        

    }
    private void DrawGridInfo(Rect parent)
    {
        PIAImageData imageData = PIASession.Instance.ImageData;

        //GRID INFO RECT
        float gridInfoRectHeight = 40;
        Rect gridInfoRect = new Rect(0, parent.height - gridInfoRectHeight, parent.width, gridInfoRectHeight);
        Texture2D gridInfoBG = new Texture2D(1, 1);

        //DRAWING STUFF
        gridInfoBG.SetPixel(0, 0, buttonsBG);
        gridInfoBG.Apply();
        GUI.DrawTexture(gridInfoRect, gridInfoBG);

        GUILayout.BeginArea(gridInfoRect);
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                {
                    GUILayout.Label("[" + imageData.Width + "x" + imageData.Height + "]", skin.GetStyle("editorbutton2"));
                    GUILayout.Label(mouseCellCoordinate.ToString(), skin.GetStyle("editorbutton2"));
                }
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();


            }
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();

        }
        GUILayout.EndArea();


    }

    private Rect DrawPreview(Rect parent) {
        Vector2 offset = new Vector2(20, 20);

        // PREVIEW RECT
        Vector2 previewLocalPosition = new Vector2(offset.x, offset.y);
        Vector2 previewDimension = new Vector2(parent.width - offset.x * 2, parent.width - offset.x * 2);
        Rect previewRect = new Rect(previewLocalPosition, previewDimension);

        // SLIDER RECT
        Vector2 speedSliderLocalPosition = new Vector2(previewRect.x, previewRect.yMax);
        Vector2 speedSliderDimension = new Vector2(previewRect.width, 20);
        Rect speedSliderRect = new Rect(speedSliderLocalPosition, speedSliderDimension);

        // BG RECT
        Texture2D previewBG = new Texture2D(1, 1);
        previewBG.SetPixel(0, 0, buttonsBG);
        previewBG.Apply();
        Rect previewBGRect = new Rect(previewRect.x - offset.x / 2, previewRect.y - offset.y / 2, previewRect.width + offset.x,
            previewRect.height + speedSliderRect.height + offset.y / 2);

        // EXTEND WINDOW RECT
        Vector2 extendWindowRectOffset = new Vector2(5, 5);
        float extendWindowRectWidth = 24;
        float extendWindowRectHeight = 24;
        Rect extendWindowRect = new Rect(previewRect.xMax - extendWindowRectWidth - extendWindowRectOffset.x, 
            previewRect.yMax - extendWindowRectHeight - extendWindowRectOffset.y, extendWindowRectWidth, extendWindowRectHeight);

        // DRAWING STUFF
        GUI.DrawTexture(previewBGRect, previewBG);
        EditorGUI.DrawTextureTransparent(previewRect,PIAAnimator.Instance.GetFrameOrFirst().GetFrameTexture());
        PIAAnimator.Instance.Speed = (int)GUI.HorizontalSlider(speedSliderRect, PIAAnimator.Instance.Speed, 1, 24);
        if (PIAInputArea.IsMouseInsideRect(previewBGRect))
        {
            if (GUI.Button(extendWindowRect, GUIContent.none,skin.GetStyle("extendpreview")))
                PIAExtendedPreviewWindow.ShowWindow();
        }
        return previewBGRect;
    }
    private void DrawLayers(Rect verticalParent) {
        Vector2 offset = new Vector2(10, 20);

        float layerRectPositionX = verticalParent.x + offset.x;
        float layerRectPositionY = verticalParent.yMax + offset.y;
        float layerRectWidth = verticalParent.width-offset.x*2;
        float layerRectHeight = 40;
        float spaceBetweenLayers = 10;

        Texture2D layerBG = new Texture2D(1, 1);
        layerBG.SetPixel(0, 0, buttonsBG);
        layerBG.Apply();

        for (int i = 0; i < PIASession.Instance.ImageData.Layers.Count; i++)
        {
            var item = PIASession.Instance.ImageData.Layers[i];
            Rect layerRect = new Rect(layerRectPositionX, layerRectPositionY, layerRectWidth, layerRectHeight);

            GUI.DrawTexture(layerRect, layerBG);
            GUILayout.BeginArea(layerRect);
            {
                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal();
                {

                    GUILayout.BeginVertical();
                    {
                        GUILayout.FlexibleSpace();

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(15);
                            GUILayout.Label(item.Name, skin.GetStyle("editorbutton2"));
                            GUILayout.FlexibleSpace();

                        }
                        GUILayout.EndHorizontal();
                        GUILayout.FlexibleSpace();

                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical();
                    {
                        GUILayout.FlexibleSpace();

                        GUILayout.BeginHorizontal();
                        {
                            item.Hidden = GUILayout.Toggle(item.Hidden, GUIContent.none, skin.GetStyle("layereye"), GUILayout.MaxWidth(30), GUILayout.MaxHeight(30));
                            GUILayout.Space(5);
                            if (i != 0)
                                if (GUILayout.Button(GUIContent.none, skin.GetStyle("deletelayer"), GUILayout.MaxWidth(30), GUILayout.MaxHeight(30))) {
                                    PIASession.Instance.ImageData.RemoveLayer(i);
                                }

                            GUILayout.Space(5);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.FlexibleSpace();


                    }
                    GUILayout.EndVertical();

                }
                GUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();


            }
            GUILayout.EndArea();
            if (GUI.Button(layerRect, GUIContent.none, skin.GetStyle("bglayerbutton")))
            {
                PIASession.Instance.ImageData.CurrentLayer = i;
            }
            layerRectPositionY += layerRectHeight + spaceBetweenLayers;

            if (i == PIASession.Instance.ImageData.CurrentLayer)
            {
                GUI.Label(layerRect, GUIContent.none, skin.GetStyle("selectedlayeroverlay"));
            }
            PIASession.Instance.ImageData.Layers[i] = item;
        }


        Rect addLayerRect = new Rect(layerRectPositionX, layerRectPositionY, layerRectWidth, layerRectHeight);

        GUI.DrawTexture(addLayerRect, layerBG);
        GUILayout.BeginArea(addLayerRect);
        {
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            {

                GUILayout.BeginVertical();
                {
                    GUILayout.FlexibleSpace();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(15);
                        GUILayout.Label("", skin.GetStyle("addlayerlabelicon"), GUILayout.MaxWidth(30), GUILayout.MaxHeight(30));
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.FlexibleSpace();


                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                {
                    GUILayout.FlexibleSpace();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(15);
                        GUILayout.Label("Add layer", skin.GetStyle("editorbutton2"));
                        GUILayout.FlexibleSpace();

                    }
                    GUILayout.EndHorizontal();
                    GUILayout.FlexibleSpace();

                }
                GUILayout.EndVertical();

                

            }
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            


        }
        GUILayout.EndArea();
        if (GUI.Button(addLayerRect, "", skin.GetStyle("bglayerbutton")))
        {
            PIASession.Instance.ImageData.AddLayer();
        }

    }
    private void DrawFrames(Rect parent)
    {
        PIAImageData imageData = PIASession.Instance.ImageData;
        Vector2 offset = new Vector2(20, 20);
        Vector2 bgSize = new Vector2(15, 15);

        float frameRectPositionX = offset.x;
        float frameRectPositionY = offset.y;
        float frameRectWidth = 100;
        float frameRectHeight = frameRectWidth;
        float spaceBetweenFrames = 25;

        float frameNumberRectWidth = 22;
        float frameNumberRectHeight = 22;

        float deleteFrameRectWidth = 32;
        float deleteFrameRectHeight = 32;
        Vector2 deleteFrameRectOffset = new Vector2(2, 2);

        float duplicateFrameRectWidth = 32;
        float duplicateFrameRectHeight = 32;
        Vector2 duplicateFrameRectOffset = new Vector2(2, 2);

        float addFrameIconRectWidth = 40;
        float addFrameIconRectHeight = 40;

        Texture2D frameBG = new Texture2D(1, 1);
        frameBG.SetPixel(0, 0, buttonsBG);
        frameBG.Apply();

        Rect viewRect = new Rect(0, 0, parent.width, (frameRectHeight+spaceBetweenFrames) * (imageData.Frames.Count+1)+offset.y);
        Rect sliderRect = new Rect(0, 0, parent.width, parent.height-offset.y);

        GUIStyle nativeVerticalScrollbarThumb = GUI.skin.verticalScrollbarThumb;
        GUI.skin.verticalScrollbarThumb.normal.background = PIATextureDatabase.Instance.GetTexture("empty");
        GUIStyle nativeVerticalScrollbarDownButton = GUI.skin.verticalScrollbarDownButton;
        GUI.skin.verticalScrollbarDownButton.normal.background = PIATextureDatabase.Instance.GetTexture("empty");
        GUIStyle nativeVerticalScrollbarUpButton = GUI.skin.verticalScrollbarUpButton;
        GUI.skin.verticalScrollbarUpButton.normal.background = PIATextureDatabase.Instance.GetTexture("empty");


        framesSlider = GUI.BeginScrollView(sliderRect, framesSlider,viewRect,false,false,skin.GetStyle("horizontalscrollbar"),skin.GetStyle("verticalscrollbar"));
        {
            for (int i = 0; i < imageData.Frames.Count; i++)
            {
                var item = imageData.Frames[i];
                Rect frameRect = new Rect(frameRectPositionX, frameRectPositionY, frameRectWidth, frameRectHeight);
                Rect frameBGRect = new Rect(frameRect.x - bgSize.x / 2, frameRect.y - bgSize.y / 2, frameRect.width + bgSize.x,
                    frameRect.height + bgSize.y);
                Rect frameNumberBGRect = new Rect(frameBGRect.xMax, frameBGRect.center.y - frameNumberRectHeight / 2, frameNumberRectWidth, frameNumberRectHeight);
                Rect deleteFrameRect = new Rect(frameRect.xMax - deleteFrameRectWidth - deleteFrameRectOffset.x,
                    frameRect.y + deleteFrameRectOffset.y, deleteFrameRectWidth, deleteFrameRectHeight);
                Rect duplicateFrameRect = new Rect(frameRect.xMax - duplicateFrameRectWidth - duplicateFrameRectOffset.x,
                    frameRect.yMax - duplicateFrameRectOffset.y - duplicateFrameRectHeight, duplicateFrameRectWidth, duplicateFrameRectHeight);

                GUI.DrawTexture(frameNumberBGRect, frameBG);
                GUI.Label(frameNumberBGRect, i.ToString(), skin.GetStyle("editorbutton2"));
                GUI.DrawTexture(frameBGRect, frameBG);
                EditorGUI.DrawTextureTransparent(frameRect, imageData.Frames[i].GetFrameTexture());
                if (PIAInputArea.IsMouseInsideRect(frameBGRect))
                {
                    if (imageData.Frames.Count > 1)
                    {

                        if (GUI.Button(deleteFrameRect, GUIContent.none, skin.GetStyle("deleteframe")))
                        {
                            PIASession.Instance.ImageData.RemoveFrame(i);
                        }

                    }

                    if (GUI.Button(duplicateFrameRect, GUIContent.none, skin.GetStyle("copyframe")))
                    {
                        PIAFrame newFrame = PIASession.Instance.ImageData.AddFrame();
                        newFrame.CopyFrom(item);
                    }
                }


                if (GUI.Button(frameBGRect, GUIContent.none, skin.GetStyle("bglayerbutton")))
                {
                    PIASession.Instance.ImageData.CurrentFrameIndex = i;
                }

                frameRectPositionY += frameRectHeight + spaceBetweenFrames;
                if (i == PIASession.Instance.ImageData.CurrentFrameIndex)
                {
                    GUI.Label(frameBGRect, GUIContent.none, skin.GetStyle("selectedframeoverlay"));
                }
            }



            Rect addFrameRect = new Rect(frameRectPositionX, frameRectPositionY, frameRectWidth, frameRectHeight);
            Rect addFrameBGRect = new Rect(addFrameRect.x - bgSize.x / 2, addFrameRect.y - bgSize.y / 2, addFrameRect.width + bgSize.x,
                    addFrameRect.height + bgSize.y);
            Rect addFrameBGLabelIcon = new Rect(addFrameRect.center.x - addFrameIconRectWidth / 2, addFrameRect.center.y - addFrameIconRectHeight / 2, addFrameIconRectWidth, addFrameIconRectHeight);



            GUI.DrawTexture(addFrameBGRect, frameBG);
            GUI.Label(addFrameBGLabelIcon, GUIContent.none, skin.GetStyle("addframe"));


            if (GUI.Button(addFrameRect, GUIContent.none, skin.GetStyle("bglayerbutton")))
            {
                PIASession.Instance.ImageData.AddFrame();
            }
            frameRectPositionY += frameRectHeight + spaceBetweenFrames;
        }
        GUI.EndScrollView();


        GUI.skin.verticalScrollbarThumb = nativeVerticalScrollbarThumb;
        GUI.skin.verticalScrollbarDownButton = nativeVerticalScrollbarDownButton;
        GUI.skin.verticalScrollbarUpButton = nativeVerticalScrollbarUpButton;


    }
    private void DrawSessionBar(Rect parent) {
        float buttonWidth = 46;
        float buttonHeight = 46;
        float spaceBetweenRects = 20;

        Vector2 offset = new Vector2(5, 180);
        
        Vector2 bgOffset = new Vector2(8, 8);
        Texture2D bg = new Texture2D(1, 1);
        bg.SetPixel(0, 0, buttonsBG);
        bg.Apply();
        
        Rect firstRect = new Rect(parent.width - buttonWidth - offset.x, offset.y, buttonWidth, buttonHeight * 3);
        Rect firstRectBG = new Rect(firstRect.x - bgOffset.x / 2, firstRect.y - bgOffset.y / 2, firstRect.width + bgOffset.x, firstRect.height + bgOffset.y);

        GUI.DrawTexture(firstRectBG, bg);
        GUILayout.BeginArea(firstRect);
        {
            GUILayout.BeginVertical();
            {
                if (GUILayout.Button(GUIContent.none, skin.GetStyle("newsession"), GUILayout.MaxWidth(buttonWidth), GUILayout.MaxHeight(buttonHeight))) {
                    PIANewImageWindow.ShowWindow();
                }
                if (GUILayout.Button(GUIContent.none, skin.GetStyle("openasset"), GUILayout.MaxWidth(buttonWidth), GUILayout.MaxHeight(buttonHeight))) {
                    PIASession.Instance.LoadAsset();
                }
                if (GUILayout.Button(GUIContent.none, skin.GetStyle("savesession"), GUILayout.MaxWidth(buttonWidth), GUILayout.MaxHeight(buttonHeight))) {
                    PIASession.Instance.SaveAsset();
                }

            }
            GUILayout.EndVertical();

        }
        GUILayout.EndArea();

        Rect secondRect = new Rect(firstRect.x,firstRect.yMax+ spaceBetweenRects,firstRect.width, buttonHeight * 2);
        Rect secondRectBG = new Rect(secondRect.x - bgOffset.x / 2, secondRect.y - bgOffset.y / 2, secondRect.width + bgOffset.x, secondRect.height + bgOffset.y);

        GUI.DrawTexture(secondRectBG, bg);
        GUILayout.BeginArea(secondRect);
        {
            GUILayout.BeginVertical();
            {
                if (GUILayout.Button(GUIContent.none, skin.GetStyle("importtexture"), GUILayout.MaxWidth(buttonWidth), GUILayout.MaxHeight(buttonHeight))) {
                    PIASession.Instance.LoadImageFromFile();
                }
                if (GUILayout.Button(GUIContent.none, skin.GetStyle("exporttexture"), GUILayout.MaxWidth(buttonWidth), GUILayout.MaxHeight(buttonHeight))) {
                    PIAExportSettingsWindow.ShowWindow();
                }

            }
            GUILayout.EndVertical();

        }
        GUILayout.EndArea();

    }


    private void DrawGrid(Rect rect)
    {
       
        if (grid.CellWidth <= 10 || grid.CellHeight <= 10)
            return;

        //for (float offsetX = 0; offsetX <= rect.width+1; offsetX += grid.CellWidth)
        //{
        //    EditorGUI.DrawRect(new Rect(new Vector2(rect.x + offsetX, rect.y), new Vector2(1, rect.height)), Color.black);
        //}
        //for (float offsetY = 0; offsetY <= rect.height+1; offsetY += grid.CellHeight)
        //{
        //    EditorGUI.DrawRect(new Rect(new Vector2(rect.x , rect.y + offsetY), new Vector2(rect.width, 1)), Color.black);

        //}
        Handles.BeginGUI();
        {
            Handles.color = Color.black;

            for (float offsetX = 0; offsetX <= rect.width; offsetX += grid.CellWidth)
            {
                Handles.DrawLine(new Vector2(rect.x + offsetX, rect.y), new Vector2(rect.x + offsetX, rect.y + rect.height));
            }
            for (float offsetY = 0; offsetY <= rect.height; offsetY += grid.CellHeight)
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

                drawer.ToolType = (PIAToolType)GUILayout.SelectionGrid((int)drawer.ToolType,
                    icons, iconsPerLine, skin.GetStyle("editorbutton"),
                    GUILayout.MaxWidth(iconsWidth * iconsPerLine), GUILayout.MaxHeight(iconsWidth * icons.Length / 2));
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginArea(new Rect(0, 170, leftSide.GetRect().width, 200));
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

        Rect framesRect = new Rect(rightSide.GetRect().x + offset.x, animationPreviewRect.yMax + offset.y, rightSide.GetRect().width, rightSide.GetRect().height - animationPreviewRect.height);

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



   
}
