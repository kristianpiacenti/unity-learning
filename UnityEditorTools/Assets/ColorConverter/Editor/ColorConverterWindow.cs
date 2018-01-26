using UnityEditor;
using UnityEngine;
using Piacenti.EditorTools;
using System.Collections.Generic;
namespace Piacenti.ColorConverter {
    public class ColorConverterWindow : EditorWindow {

        GUISkin skin;
        WindowSection headerSection;
        WindowSection firstTypeSection;
        WindowSection secondTypeSection;
        WindowSection footSection;
        ColorType firstType = ColorType.RGB_HEX;
        ColorType secondType = ColorType.FloatValue;

        Color rgbColor = Color.white;

        List<WindowSection> sections;
        [MenuItem("EditorTools/ColorConverter")]
        public static void InitWindow() {
            EditorWindow window = GetWindow<ColorConverterWindow>();
            window.minSize = new Vector2(510, 140);
            window.maxSize = new Vector2(510, 140);
            window.titleContent=new GUIContent("Color Converter");
            window.ShowPopup();
            

        }

        private void OnEnable()
        {
            skin = Resources.Load<GUISkin>("Style/ColorConverter");
            sections = new List<WindowSection>();
            headerSection = new WindowSection(new Rect(0, 0, position.width, 30), new Color(0.4000f, 0.5961f, 0.5961f, 1.00f));
            firstTypeSection = new WindowSection(new Rect(5, 30, position.width / 2, 75),Color.clear);
            secondTypeSection = new WindowSection(new Rect(260, 30, position.width / 2, 75), Color.clear);

            footSection = new WindowSection(new Rect(0, 110, position.width, 30), new Color(0.4000f, 0.5961f, 0.5961f, 1.00f));

            sections.Add(headerSection);
            sections.Add(firstTypeSection);
            sections.Add(secondTypeSection);
            sections.Add(footSection);

        }
        private void OnGUI()
        {
            DrawSections();
            DrawHeader();
            DrawTypeSection(firstTypeSection, ref firstType);
            DrawTypeSection(secondTypeSection, ref secondType);
            DrawFoot();
            
            
        }
        private void DrawHeader() {
            GUILayout.BeginArea(headerSection.GetRect());
            {
                EditorGUILayout.LabelField("COLOR CONVERTER", skin.GetStyle("header"));
            }
            GUILayout.EndArea();
        }
        private void DrawTypeSection(WindowSection section,ref ColorType colorType) {
            GUILayout.BeginArea(section.GetRect());
            {
                EditorGUILayout.BeginVertical();
                {
                    GUILayout.Space(15);
                    EditorGUILayout.BeginHorizontal();
                    {

                        colorType = (ColorType)EditorGUILayout.EnumPopup(colorType, GUILayout.Width(section.GetRect().width),GUILayout.MaxWidth(240));
                        
                       // secondType = (ColorType)EditorGUILayout.EnumPopup(secondType, GUILayout.Width(section.GetRect().width));
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        switch (colorType) {
                            case ColorType.FloatValue:

                                break;
                            case ColorType.RGB_HEX:
                                rgbColor = EditorGUILayout.ColorField(rgbColor, GUILayout.Width(section.GetRect().width),GUILayout.MaxWidth(240));
                                break;
                            default:
                                break;
                        }
                        switch (colorType)
                        {
                            case ColorType.FloatValue:
                                EditorGUILayout.BeginVertical();
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.LabelField("R",EditorStyles.boldLabel, GUILayout.Width(15));
                                        EditorGUILayout.TextField(rgbColor.r.ToString(), GUILayout.Width(30));
                                        GUILayout.Space(10);
                                        EditorGUILayout.LabelField("G", EditorStyles.boldLabel, GUILayout.Width(15));
                                        EditorGUILayout.TextField(rgbColor.g.ToString(), GUILayout.Width(30));
                                        GUILayout.Space(10);
                                        EditorGUILayout.LabelField("B", EditorStyles.boldLabel, GUILayout.Width(15));
                                        EditorGUILayout.TextField(rgbColor.b.ToString(), GUILayout.Width(30));
                                        GUILayout.Space(10);
                                        EditorGUILayout.LabelField("A",EditorStyles.boldLabel, GUILayout.Width(15));
                                        EditorGUILayout.TextField(rgbColor.a.ToString(), GUILayout.Width(30));

                                    }
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.TextField("new Color(" + rgbColor.r.ToString("0.0000")+ "f, " + 
                                            rgbColor.g.ToString("0.0000") + "f, " + rgbColor.b.ToString("0.0000") + "f, " + 
                                            rgbColor.a.ToString("0.00") + "f)",GUILayout.MaxWidth(240));
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                                EditorGUILayout.EndVertical();
                                break;
                            case ColorType.RGB_HEX:
                                break;
                            default:
                                break;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                

            }
            GUILayout.EndArea();
        }

        private void DrawFoot() {
            GUILayout.BeginArea(footSection.GetRect());
            {
                EditorGUILayout.LabelField("by Kristian Piacenti", skin.GetStyle("footer"));
            }
            GUILayout.EndArea();
        }
        
        private void DrawSections()
        {
            headerSection.SetRect(position.width, 30);
            firstTypeSection.SetRect(5,30,position.width/2, 75);
            secondTypeSection.SetRect(260, 30,position.width/2, 75);

            footSection.SetRect(position.width, 30);

            foreach (var item in sections)
            {
                GUI.DrawTexture(item.GetRect(), item.GetTexture());
            }
        }
        private enum ColorType {
            RGB_HEX,
            FloatValue
        }

        

    }


}
