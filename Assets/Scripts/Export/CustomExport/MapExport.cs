using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MapExport
{
    public GameObject[] ExportObjects;
    public GameObject CollisionMap;

    public MapExport(GameObject[] exportObjects, GameObject collisionObject)
    {
        ExportObjects = exportObjects;
        CollisionMap = collisionObject;
    }

    public void Export()
    {
        File.WriteAllText(Application.dataPath + @"/Exports/Map.pku", JsonUtility.ToJson(new MapHolder(ExportObjects, CollisionMap.GetComponent<MeshFilter>().sharedMesh), true));
    }

    public void Import()
    {

    }

    [Serializable]
    public class MapHolder
    {
        public MapObject[] Objects;
        public SerializeMesh CollisionMap;

        public MapHolder(GameObject[] exportObjects, Mesh collisionMap)
        {
            List<MapObject> objects = new List<MapObject>();
            foreach(GameObject gameObject in exportObjects)
                objects.Add(new MapObject(@"Assets/Resources/TerrainTiles/Models/" + gameObject.name.Split(new string[] { "ID-{" }, StringSplitOptions.None)[1].Split(new string[] { " !-! " }, StringSplitOptions.None)[0], AssetDatabase.GetAssetPath(gameObject.GetComponent<MeshRenderer>().material.mainTexture), gameObject.transform.position, gameObject.transform.eulerAngles));

            Objects = objects.ToArray();
            CollisionMap = new SerializeMesh(collisionMap);
        }
    }

    [Serializable]
    public class SerializeMesh
    {
        public SeriPose[] Vertices;
        public SeriVector2[] UV;
        public int[] Triangles;
        public SeriPose[] Normals;

        public SerializeMesh(Mesh mesh)
        {
            Vertices = new SeriPose[mesh.vertices.Length];
            for (int i = 0; i < mesh.vertices.Length; i++)
                Vertices[i] = new SeriPose(mesh.vertices[i]);
            UV = new SeriVector2[mesh.uv.Length];
            for (int i = 0; i < mesh.uv.Length; i++)
                UV[i] = new SeriVector2(mesh.uv[i]);
            Triangles = mesh.triangles;
            Normals = new SeriPose[mesh.normals.Length];
            for (int i = 0; i < mesh.normals.Length; i++)
                Normals[i] = new SeriPose(mesh.normals[i]);
        }

        public Vector3[] GetVertices()
        {
            Vector3[] vertices = new Vector3[Vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = new Vector3(Vertices[i].X, Vertices[i].Y, Vertices[i].Z);
            return vertices;
        }

        public Vector2[] GetUV()
        {
            Vector2[] uv = new Vector2[UV.Length];
            for (int i = 0; i < uv.Length; i++)
                uv[i] = new Vector2(UV[i].X, UV[i].Y);
            return uv;
        }

        public Vector3[] GetNormals()
        {
            Vector3[] normals = new Vector3[Normals.Length];
            for (int i = 0; i < normals.Length; i++)
                normals[i] = new Vector3(Normals[i].X, Normals[i].Y, Normals[i].Z);
            return normals;
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
    public class SeriVector2
    {
        public float X;
        public float Y;

        public SeriVector2(Vector2 position)
        {
            X = position.x;
            Y = position.y;
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
