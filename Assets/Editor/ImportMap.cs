using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class ImportMap : EditorWindow
{
    [MenuItem("Window/PKU Map Editor/Importer")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ImportMap));
        EditorGUILayout.Space();
    }

    public void DropAreaGui()
    {
        Event evt = Event.current;
        Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(drop_area, "Place Your PKU Files Here");

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop_area.Contains(evt.mousePosition))
                    return;
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if(evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach(Object dragged_object in DragAndDrop.objectReferences)
                    {
                        string objectName = AssetDatabase.GetAssetPath(dragged_object);
                        if(Path.GetExtension(objectName) == ".pku")
                        {
                            Transform Parent = Instantiate(new GameObject(dragged_object.name)).transform;
                            MapExport.MapHolder mapHolder = (MapExport.MapHolder)JsonUtility.FromJson(File.ReadAllText(AssetDatabase.GetAssetPath(dragged_object)), typeof(MapExport.MapHolder));

                            List<GameObject> objects = Resources.LoadAll<GameObject>("TerrainTiles/Models").ToList();
                            List<Texture> materials = Resources.LoadAll<Texture>("TerrainTiles/Textures").ToList();

                            foreach(MapExport.MapObject mapObject in mapHolder.Objects)
                            {
                                GameObject newObject = Instantiate(objects.Find(x => x.name == Path.GetFileNameWithoutExtension(mapObject.ModelPath)), Parent);
                                Material objectMaterial = newObject.GetComponent<Renderer>().sharedMaterial;

                                objectMaterial = new Material(Shader.Find("Standard"));
                                objectMaterial.mainTexture = materials.Find(x => x.name == Path.GetFileNameWithoutExtension(mapObject.TexturePath));

                                objectMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                                objectMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                                objectMaterial.SetInt("_ZWrite", 0);
                                objectMaterial.DisableKeyword("_ALPHATEST_ON");
                                objectMaterial.EnableKeyword("_ALPHABLEND_ON");
                                objectMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                                objectMaterial.renderQueue = 3000;
                                objectMaterial.SetFloat("_Glossiness", 0.0f);

                                newObject.GetComponent<Renderer>().sharedMaterial = objectMaterial;

                                newObject.transform.position = new Vector3(mapObject.Position.X, mapObject.Position.Y, mapObject.Position.Z);
                                newObject.transform.eulerAngles = new Vector3(mapObject.Rotation.X, mapObject.Rotation.Y, mapObject.Rotation.Z);
                            }
                        }
                    }
                }
                break;
        }
    }

    private void OnGUI()
    {
        DropAreaGui();

    }
}
