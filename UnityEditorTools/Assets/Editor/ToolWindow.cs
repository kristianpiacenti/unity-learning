using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Piacenti.EditorTools;
using System;
using System.Reflection;
using System.Linq;
public class ToolWindow : EditorWindow
  {
    private List<WindowSection> sections;
    private WindowSection buttonsSection;
    private WindowSection iconSection;
    private WindowSection bodySection;
    private Vector2 scrollPosition;

    [MenuItem("EditorTools/ToolWindow")]
    private static void InitWindow() {
        EditorWindow window = GetWindow<ToolWindow>();
        window.titleContent = new GUIContent("ToolWindow");
        window.minSize=new Vector2(600, 300);
        window.Show();
    }


    private void OnEnable()
    {
        sections = new List<WindowSection>();
        iconSection = new WindowSection(new Rect(0, 0, 200, 50), new Color(1, 1, 1 ));
        buttonsSection = new WindowSection(new Rect(0, 50, 200, position.height), new Color(0.1568f,0.2078f,0.5764f));
        bodySection = new WindowSection(new Rect(200, 0, position.width, position.height), new Color(0.1882f,0.2470f,0.6235f));
        sections.Add(buttonsSection);
        sections.Add(bodySection);
        sections.Add(iconSection);
    }
    private void OnGUI()
    {
        DrawSections();
        DrawIconSection();
        DrawButtonsSection();
        DrawBodySection();

    }
    private void DrawBodySection() {
        GUILayout.BeginArea(bodySection.GetRect());
        {
            GUILayout.Toolbar(0, new string[6] { "String 0", "String 1", "String 0", "String 1" , "String 0", "String 1" },GUILayout.Width(bodySection.GetRect().width-buttonsSection.GetRect().width), GUILayout.Height(50));
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar,GUILayout.ExpandWidth(true));
            {
                GUILayout.Button("Hello", EditorStyles.toolbarButton,GUILayout.Width(50));
            }
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    private void DrawIconSection()
    {
        GUILayout.BeginArea(iconSection.GetRect());
        {
            EditorGUILayout.LabelField("Editor Window", GUILayout.Width(iconSection.GetRect().width),GUILayout.Height(iconSection.GetRect().height));
        }
        GUILayout.EndArea();
    }
    private void DrawButtonsSection() {
        GUILayout.BeginArea(buttonsSection.GetRect());
        {
            EditorGUILayout.BeginVertical();
            {
                //GUILayout.Button("Button");
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,GUILayout.Height(buttonsSection.GetRect().height));

                for (int i = 0; i < 5; i++)
                {
                    GUILayout.Space(5);
                    if (GUILayout.Button("Button" + i, GUILayout.Height(70), GUILayout.Width(buttonsSection.GetRect().width - 10))) {
                        string colorPresetPath = "Assets/Editor/test.colors";
                        UnityEngine.Object presetObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(colorPresetPath);

                        Type colorPreview = Type.GetType("UnityEditor.ColorPicker, UnityEditor");
                        MethodInfo[] info = colorPreview.GetMethods();
                        foreach (var item in info)
                        {
                            //Debug.Log(item.Name);
                            if (item.Name == "Count")
                                Debug.Log(item.Invoke(presetObject,null));
                        }
                        EditorWindow colorPickerWindow = EditorWindow.GetWindow(colorPreview);
                        colorPickerWindow.Show();

                    }
                }
                EditorGUILayout.EndScrollView();

            }
            EditorGUILayout.EndVertical();
        }
        GUILayout.EndArea();
    }
    private void DrawSections() {
        buttonsSection.SetRect(200, position.height);
        bodySection.SetRect(position.width, position.height);
        foreach (var item in sections)
        {
            GUI.DrawTexture(item.GetRect(), item.GetTexture());
        }
    }

}

    

