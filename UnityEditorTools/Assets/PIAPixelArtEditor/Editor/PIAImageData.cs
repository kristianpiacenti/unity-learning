using UnityEngine;
using UnityEditor;
[CreateAssetMenu(fileName ="NewImageData",menuName ="Create ImageData",order =0)]
public class PIAImageData : ScriptableObject {


    
    public PIACanvas Canvas { get; set; }

    public void Init() {
        Canvas = new PIACanvas();
    }

}
