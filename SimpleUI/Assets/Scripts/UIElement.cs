using UnityEngine;

public abstract class UIElement : MonoBehaviour {
    public virtual void Enable() {
        gameObject.SetActive(true);
    }
    public abstract void Disable();
}
