using UnityEditor;
using UnityEngine;
using Piacenti.EditorTools;
using System.Collections.Generic;
using System.IO;
public class PIAEditorWindow : EditorWindow{
    #region Fields

    private static PIAEditorWindow window;
    WindowSection header;
    WindowSection leftSide;
    WindowSection body;
    WindowSection rightSide;
    List<WindowSection> sections;

    PIASession session;
    PIAInputArea bodyInputArea;
    PIAInputArea imageInputArea;

    GUISkin skin;
    Texture2D penTexture;
    float scaleMultiplier = 0.5f;
    Rect imageRect;
    Vector2 pixelCoordinate;
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
        //INIT SESSION
        session = new PIASession();

        //INIT INPUT AREAS 
        bodyInputArea = new PIAInputArea();
        imageInputArea = new PIAInputArea();
        bodyInputArea.OnScrollWheel += IncreaseScaleMultiplier;
        bodyInputArea.OnGUIUpdate+= DrawPixelCoordinates;
        //INIT WINDOW SECTIONS
        InitializeSections();

        //INIT GRAPHICS
        skin = Resources.Load<GUISkin>("Skins/PIAPixelArtEditorSkin");
        penTexture = (Texture2D)Resources.Load("pen");
    }

    private void OnGUI()
    {
        DrawLayouts();
        bodyInputArea.GUIUpdate(body.GetRect());

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
    private void DrawHeader()
    {
        GUILayout.BeginArea(header.GetRect());
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                GUILayout.Label(session.ProjectName, skin.GetStyle("header"));
                GUILayout.FlexibleSpace();

            }
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();

        }
        GUILayout.EndArea();
    }
    private void DrawLeftSide()
    {
        GUILayout.BeginArea(new Rect(leftSide.GetRect().x + 10, leftSide.GetRect().y + 10, leftSide.GetRect().width - 20, leftSide.GetRect().height - 20));
        {


            GUILayout.SelectionGrid(0, new string[] { "Paint", "Erase", "Box", "Line", "Color", "Select" }, 2, skin.GetStyle("editorbutton"), GUILayout.MaxWidth(55 * 2), GUILayout.MaxHeight(55 * 3));

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
        GUILayout.BeginArea(new Rect(rightSide.GetRect().x + 60, rightSide.GetRect().height / 3, rightSide.GetRect().width / 2, 55 * 5));
        {
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
                session.LoadNewAsset();
            }
            if (GUILayout.Button("Load", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(55)))
            {
                session.LoadAsset();

            }
            if (GUILayout.Button("Save", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(55)))
            {
                session.SaveAsset();
            }
            if (GUILayout.Button("Import", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(55)))
            {
                session.LoadImageFromFile();
            }
            if (GUILayout.Button("Export", skin.GetStyle("editorbutton2"), GUILayout.MaxWidth(55), GUILayout.MaxHeight(55)))
            {
                session.ExportImage(session.ImageData.Canvas.GetFinalImage());

            }

        }
        GUILayout.EndArea();
    }

    private void DrawBody()
    {
        float scale = body.GetRect().width * scaleMultiplier;
        imageRect = new Rect(body.GetRect().width / 2 - scale / 2, body.GetRect().center.y - scale / 2, scale, scale);
        Texture2D tex = session.ImageData.Canvas.GetFinalImage();
        
        GUILayout.BeginArea(body.GetRect());
        {
            if (tex != null)
            {
                EditorGUI.DrawTextureTransparent(imageRect, tex);
                DrawGrid(imageRect, new Vector2(tex.width, tex.height));
  
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

    private void DrawPixelCoordinates(Event e)
    {
        Texture tex = session.ImageData.Canvas.GetFinalImage();
        pixelCoordinate = PIACanvas.WorldPositionToGridPixel(PIAInputArea.MousePosition, imageRect,
                    body.GetRect().position, tex);
       

        GUILayout.BeginArea(rightSide.GetRect());
        {
            EditorGUILayout.LabelField(pixelCoordinate.ToString(), skin.GetStyle("editorbutton2"));
            window.Repaint();
        }
        GUILayout.EndArea();
    }
    public void IncreaseScaleMultiplier(Event e)
    {
        float deltaY = e.delta.y * (-1) * Time.deltaTime;
        scaleMultiplier += 0.15f * deltaY;
        window.Repaint();
    }
    #endregion



}
