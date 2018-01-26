using UnityEngine;
using System;
public class PIAInputArea {
    public static Vector2 MousePosition { get { return e.mousePosition; } }

    public event Action<Event> OnScrollWheel = delegate { };
    public event Action<Event> OnMouseDown = delegate { };
    public event Action<Event> OnMouseUp = delegate { };
    public event Action<Event> OnGUIUpdate = delegate { };
    public event Action<Event> OnUpdate = delegate { };

    private static Event e;
    
    public void GUIUpdate(Rect area) {
        e = Event.current;
        if (!area.Contains(e.mousePosition))
            return;
        OnGUIUpdate(e);
        switch (e.type)
        {

            case EventType.MouseDown:
                OnMouseDown(e);
                break;
            case EventType.MouseUp:
                OnMouseUp(e);
                break;
            case EventType.ScrollWheel:
                //Debug.Log(e.delta);
                OnScrollWheel(e);
                break;
                
        }
    }
    public void Update(Rect area,PIAEditorWindow window) { 
        window.Repaint();
        if (!area.Contains(e.mousePosition))
            return;
        OnUpdate(e);
    }
}
