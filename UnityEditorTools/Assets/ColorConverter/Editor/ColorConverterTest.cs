using UnityEditor;
using UnityEngine;
using Piacenti.EditorTools;
using System.Collections.Generic;
namespace Piacenti.ColorConverter {
    public class ColorConverterTest : EditorWindow {

        #region Fields

        WindowSection header;
        WindowSection body;
        WindowSection footer;

        Vector2 bodyScrollPosition;
        List<WindowSection> sections;
        Color inputColor;
        string r;
        string g;
        string b;
        string a;
        GUISkin skin;
        #endregion

        #region Methods
        [MenuItem("EditorTools/ColorConverterTest")]
        public static void InitWindow()
        {
            EditorWindow window = GetWindow<ColorConverterTest>();
            window.titleContent = new GUIContent("Color Converter");
            window.minSize = new Vector2(275, 200);
            window.ShowPopup();


        }

        private void OnEnable()
        {
            sections = new List<WindowSection>();
            header = new WindowSection(new Rect(0, 0, position.width, 50), new Color(0.3804f, 0.3804f, 0.3804f, 1.0000f));
            footer = new WindowSection(new Rect(0, position.height - 25, position.width, 25), new Color(0.3804f, 0.3804f, 0.3804f, 1.0000f));
            body = new WindowSection(new Rect(0, header.GetRect().height, position.width, position.height - footer.GetRect().height), Color.clear);

            sections.Add(header);
            sections.Add(footer);
            sections.Add(body);

            skin = Resources.Load<GUISkin>("Style/ColorConverterSkin");

            inputColor = Color.white;
            r = inputColor.r.ToString();
            g = inputColor.g.ToString();
            b = inputColor.b.ToString();
            a = inputColor.a.ToString();

        }
        private void OnGUI()
        {
            DrawSections();
            DrawHeader();
            DrawBody();
            DrawFooter();
        }
        private void DrawSections()
        {
            header.SetRect(0, 0, position.width, 50);
            footer.SetRect(0, position.height - 25, position.width, 25);
            body.SetRect(0, header.GetRect().height, position.width, position.height - footer.GetRect().height);
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
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("RGB-HEX to Float Value",skin.GetStyle("header"));

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();

            }
            GUILayout.EndArea();
        }
        private void DrawFooter()
        {
            GUILayout.BeginArea(footer.GetRect());
            {
                EditorGUILayout.LabelField("by Kristian Piacenti",skin.GetStyle("footer"));
            }
            GUILayout.EndArea();
        }
        private void DrawBody()
        {
            GUILayout.BeginArea(body.GetRect());
            {
                bodyScrollPosition = EditorGUILayout.BeginScrollView(bodyScrollPosition);
                {
                    EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
                    {
                        EditorGUILayout.LabelField("RGB - HEX", skin.GetStyle("bodytitle"));
                        GUILayout.Space(10);
                        inputColor = EditorGUILayout.ColorField(inputColor);
                        r = inputColor.r.ToString("0.0000");
                        g = inputColor.g.ToString("0.0000");
                        b = inputColor.b.ToString("0.0000");
                        a = inputColor.a.ToString("0.0000");


                        GUILayout.Space(10);
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("R", EditorStyles.boldLabel, GUILayout.MaxWidth(12));
                            EditorGUILayout.TextField(r, GUILayout.MinWidth(20));
                            EditorGUILayout.LabelField("G", EditorStyles.boldLabel, GUILayout.MaxWidth(12));
                            EditorGUILayout.TextField(g, GUILayout.MinWidth(20));
                            EditorGUILayout.LabelField("B", EditorStyles.boldLabel, GUILayout.MaxWidth(12));
                            EditorGUILayout.TextField(b, GUILayout.MinWidth(20));
                            EditorGUILayout.LabelField("A", EditorStyles.boldLabel, GUILayout.MaxWidth(12));
                            EditorGUILayout.TextField(a, GUILayout.MinWidth(20));
                        }
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(5);
                        EditorGUILayout.TextField("new Color(" + r + "f, " + g + "f, " + b + "f, " + a + "f" + ")");

                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndArea();
        }
        #endregion






    }


}
