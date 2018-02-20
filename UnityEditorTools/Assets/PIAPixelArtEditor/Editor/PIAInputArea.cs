using UnityEngine;
using System;
public class PIAInputArea {

    #region Static

    public static Vector2 MousePosition { get { return e.mousePosition; } }
    public static bool IsMouseInsideRect(Rect rect) {
        return rect.Contains(MousePosition);
    }

    #endregion

    #region Fields

    private static Event e;
    public event Action<Event> OnGUIUpdate = delegate { };
    public event Action<Event> OnUpdate = delegate { };


    #endregion

    #region Properties



    #endregion

    #region Methods

    public void GUIUpdate(Rect area)
    {
        e = Event.current;
        if (!area.Contains(e.mousePosition))
            return;
        OnGUIUpdate(e);

    }
    public void Update(Rect area, PIAEditorWindow window)
    {
        window.Repaint();
        if (!area.Contains(e.mousePosition))
            return;
        OnUpdate(e);
    }

    #endregion





}
