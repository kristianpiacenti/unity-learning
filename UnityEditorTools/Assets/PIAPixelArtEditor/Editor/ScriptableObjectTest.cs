using UnityEngine;
using UnityEditor;
[CreateAssetMenu(fileName = "test",menuName = "Create Test", order=1)]
public class ScriptableObjectTest : ScriptableObject {

    [SerializeField]
    private AnotherClass myAnotherClass;
    private void OnEnable()
    {
       // myAnotherClass = new AnotherClass();
        Debug.Log("init scriptableobjecttest");
    }
    public AnotherClass MyAnotherClass{ get { return myAnotherClass; } set { myAnotherClass = value; } }
}
[System.Serializable]
public class AnotherClass{

    [SerializeField]
    private int anotherNumber = 55;
    public int AnotherNumber { get { return anotherNumber; } set { anotherNumber = value; } }
}
public class ScriptableObjectTestWindow : EditorWindow {

    ScriptableObjectTest obj;

    [MenuItem("EditorTools/ScriptableObjectTestWindow")]
    public static void ShowWindow() {
        ScriptableObjectTestWindow window = GetWindow<ScriptableObjectTestWindow>();
        window.minSize = new Vector2(800, 600);
    }

    private void OnGUI()
    {
        obj = EditorGUILayout.ObjectField("Obj", obj, typeof(ScriptableObjectTest),false) as ScriptableObjectTest;
        if (GUILayout.Button("ChangeAnotherNumberTo4")) {
            obj.MyAnotherClass.AnotherNumber = 65 ;
            Debug.Log(obj.MyAnotherClass.AnotherNumber);
            EditorUtility.SetDirty(obj);
        }
    }
}