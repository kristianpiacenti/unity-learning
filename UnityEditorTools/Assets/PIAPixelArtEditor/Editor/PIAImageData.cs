using UnityEngine;
using UnityEditor;
[CreateAssetMenu(fileName ="NewImageData",menuName ="Create ImageData",order =0)]
public class PIAImageData : ScriptableObject {
    #region Fields

    [SerializeField]
    private PIACanvas _canvas;

    #endregion

    #region Properties

    public PIACanvas Canvas { get { return _canvas; } set { _canvas = value; } }


    #endregion

    #region Methods

    public void Init()
    {
        _canvas = new PIACanvas();
        _canvas.Init();
    }

    #endregion









}
