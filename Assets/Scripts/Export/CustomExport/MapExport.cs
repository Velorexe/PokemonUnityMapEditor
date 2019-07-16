using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MapExport
{
    public GameObject[] ExportObjects;

    public MapExport(GameObject[] exportObjects)
    {
        ExportObjects = exportObjects;
    }

    public void Export()
    {
        File.WriteAllText(Application.dataPath + @"/Exports/Map.pku", JsonUtility.ToJson(new MapHolder(ExportObjects), true));
    }

    public void Import()
    {

    }

    [Serializable]
    public class MapHolder
    {
        public MapObject[] Objects;

        public MapHolder(GameObject[] exportObjects)
        {
            List<MapObject> objects = new List<MapObject>();
            foreach(GameObject gameObject in exportObjects)
                objects.Add(new MapObject(@"Assets/Resources/TerrainTiles/Models/" + gameObject.name.Split(new string[] { "ID-{" }, StringSplitOptions.None)[1].Split(new string[] { " !-! " }, StringSplitOptions.None)[0], AssetDatabase.GetAssetPath(gameObject.GetComponent<MeshRenderer>().material.mainTexture), gameObject.transform.position, gameObject.transform.eulerAngles));
            Objects = objects.ToArray();
        }
    }

    [Serializable]
    public class MapObject
    {
        public SeriPose Position;
        public SeriPose Rotation;

        public string ModelPath;
        public string TexturePath;

        public MapObject(string modelPath, string texturePath, Vector3 position, Vector3 rotation)
        {
            ModelPath = modelPath;
            TexturePath = texturePath;

            Position = new SeriPose(position);
            Rotation = new SeriPose(rotation);
        }
    }

    [Serializable]
    public class SeriPose
    {
        public float X;
        public float Y;
        public float Z;

        public SeriPose(Vector3 position)
        {
            X = position.x;
            Y = position.y;
            Z = position.z;
        }

        public SeriPose(Quaternion rotation)
        {
            X = rotation.x;
            Y = rotation.y;
            Z = rotation.z;
        }
    }
}
