using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
namespace Piacenti.EditorTools
{
    public class ToolWindow : EditorWindow
    {

        private List<WindowSection> sections;

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
            sections.Add(new WindowSection(new Rect(0, 0,100, position.width),Color.grey));
            sections.Add(new WindowSection(new Rect(100, 0, position.width, Screen.width), Color.green));
        }
        private void OnGUI()
        {
            DrawSections();    
        }
        private void DrawSections() {
            foreach (WindowSection section in sections)
            {
                GUI.DrawTexture(section.GetRect(), section.GetTexture());
            }
        }

    }
    public class WindowSection {

        private Rect _rect;
        private Texture2D _texture2D;

        public WindowSection(Rect rect, Color color) {
            _rect = rect;

            _texture2D = new Texture2D(1, 1);
            _texture2D.SetPixel(0, 0, color);
            _texture2D.Apply();
        }
        public WindowSection(Rect rect, Texture2D texture)
        {
            _rect = rect;
            _texture2D = texture;
            
        }

        public Rect GetRect() {
            return _rect;
        }
        public Texture2D GetTexture() {
            return _texture2D;
        }
        public Color GetTextureColor(int x,int y) {
            return _texture2D.GetPixel(x, y);
        }

    }
    
}
