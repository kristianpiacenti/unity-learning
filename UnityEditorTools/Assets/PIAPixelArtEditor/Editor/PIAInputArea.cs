using UnityEngine;
using System;
public class PIAInputArea {
    public static Vector2 MousePosition { get { return e.mousePosition; } }

    public event Action<Event> OnGUIUpdate = delegate { };
    public event Action<Event> OnUpdate = delegate { };

    private static Event e;
    
    public void GUIUpdate(Rect area) {
        e = Event.current;
        if (!area.Contains(e.mousePosition))
            return;
        OnGUIUpdate(e);
        
    }
    public void Update(Rect area,PIAEditorWindow window) { 
        window.Repaint();
        if (!area.Contains(e.mousePosition))
            return;
        OnUpdate(e);
    }
}
