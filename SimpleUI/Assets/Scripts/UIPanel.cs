using UnityEngine;

public class UIPanel : UIElement{
    [SerializeField]
    private UIPanel[] parents;
    [SerializeField]
    private UIPanel[] children;
    [ContextMenu("EnablePanel")]
    public override void Enable()
    {
        base.Enable();
    }
    [ContextMenu("DisablePanel")]
    public override void Disable()
    {
        foreach (var item in children)
        {
            item.Disable();
        }
        foreach (var item in parents)
        {
            item.Enable();
        }
        
        gameObject.SetActive(false);
    }

}
