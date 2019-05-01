using UnityEngine;
using UnityEditor;

public class PKUnityMapEditor : EditorWindow {

    [MenuItem("Window/Pokemon Unity Map Editor")]
    public static void ShowWindow()
    {
        GetWindow<PKUnityMapEditor>("Pokemon Unity Map Editor");
    }

    void OnGUI()
    {
        GUILayout.Label("Put your map here:");
        string MapInput = EditorGUILayout.TextArea("Name", GUILayout.Height(position.height - 30));
    }

}
