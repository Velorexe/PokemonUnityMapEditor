using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;

public class ColliderExport : MonoBehaviour
{
    public Transform CollisionTransform;
    public Transform WaterTransform;
    public Transform WalkableTransform;

    public Material CollisionMaterial;
    public Material WaterMaterial;
    public Material WalkableMaterial;

    private List<Vector3> CollisionBlocks = new List<Vector3>();
    private List<Vector3> WaterBlocks = new List<Vector3>();
    private List<Vector3> WalkableBlocks = new List<Vector3>();

    public void CombineColliderMeshes()
    {
        #region Collision
        if (CollisionTransform.childCount > 0)
        {
            MeshFilter[] childMeshFilters = CollisionTransform.GetComponentsInChildren<MeshFilter>();
            if (CollisionTransform.GetComponent<MeshFilter>().sharedMesh != null)
                childMeshFilters[0] = CollisionTransform.GetComponent<MeshFilter>();
            else
                childMeshFilters = childMeshFilters.Skip(1).ToArray();
            CombineInstance[] combine = new CombineInstance[childMeshFilters.Length];

            for (int i = 0; i < childMeshFilters.Length; i++)
            {
                combine[i].mesh = childMeshFilters[i].sharedMesh;
                combine[i].transform = childMeshFilters[i].transform.localToWorldMatrix;
                CollisionBlocks.Add(childMeshFilters[i].gameObject.transform.position);
            }

            CollisionTransform.GetComponent<MeshFilter>().mesh = new Mesh();
            CollisionTransform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);

            CollisionTransform.GetComponent<MeshRenderer>().sharedMaterial = CollisionMaterial;

            foreach (Transform childObject in CollisionTransform)
            {
                if (childObject != CollisionTransform)
                    Destroy(childObject.gameObject);
            }
        }
        #endregion

        #region Water
        if (WaterTransform.childCount > 0)
        {
            MeshFilter[] childMeshFilters = WaterTransform.GetComponentsInChildren<MeshFilter>();
            if (WaterTransform.GetComponent<MeshFilter>().sharedMesh != null)
                childMeshFilters[0] = WaterTransform.GetComponent<MeshFilter>();
            else
                childMeshFilters = childMeshFilters.Skip(1).ToArray();
            CombineInstance[] combine = new CombineInstance[childMeshFilters.Length];

            for (int i = 0; i < childMeshFilters.Length; i++)
            {
                combine[i].mesh = childMeshFilters[i].sharedMesh;
                combine[i].transform = childMeshFilters[i].transform.localToWorldMatrix;
                WaterBlocks.Add(childMeshFilters[i].gameObject.transform.position);
            }

            WaterTransform.GetComponent<MeshFilter>().mesh = new Mesh();
            WaterTransform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);

            WaterTransform.GetComponent<MeshRenderer>().sharedMaterial = WaterMaterial;

            foreach (Transform childObject in WaterTransform)
            {
                if (childObject != WaterTransform)
                    Destroy(childObject.gameObject);
            }
        }
        #endregion

        #region Walkable
        if (WalkableTransform.childCount > 0)
        {
            MeshFilter[] childMeshFilters = WalkableTransform.GetComponentsInChildren<MeshFilter>();
            if (WalkableTransform.GetComponent<MeshFilter>().sharedMesh != null)
                childMeshFilters[0] = WalkableTransform.GetComponent<MeshFilter>();
            else
                childMeshFilters = childMeshFilters.Skip(1).ToArray();
            CombineInstance[] combine = new CombineInstance[childMeshFilters.Length];

            for (int i = 0; i < childMeshFilters.Length; i++)
            {
                combine[i].mesh = childMeshFilters[i].sharedMesh;
                combine[i].transform = childMeshFilters[i].transform.localToWorldMatrix;
                WalkableBlocks.Add(childMeshFilters[i].gameObject.transform.position);
            }

            WalkableTransform.GetComponent<MeshFilter>().mesh = new Mesh();
            WalkableTransform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);

            WalkableTransform.GetComponent<MeshRenderer>().sharedMaterial = WalkableMaterial;

            foreach (Transform childObject in WalkableTransform)
            {
                if (childObject != WalkableTransform)
                    Destroy(childObject.gameObject);
            }
        }
        #endregion
    }

    public void ExportColliderMap()
    {

    }

    public void CreateCustomMesh()
    {
        Tuple<List<Vector3>, List<Vector2>> verticesAndUv = CreateVertices(WalkableBlocks);
        List<Vector3> vertices = verticesAndUv.Item1;
        List<Vector2> uv = verticesAndUv.Item2;

        Mesh newMesh = new Mesh() { vertices = vertices.ToArray(), uv = uv.ToArray() };
        CollisionTransform.GetComponent<MeshFilter>().mesh = newMesh;
    }

    private Tuple<List<Vector3>, List<Vector2>> CreateVertices(List<Vector3> originalBlocks)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();

        Vector3[,] vectorArray = new Vector3[Convert.ToInt32(Math.Round(originalBlocks.Max(x => x.x), 0)), Convert.ToInt32(Math.Round(originalBlocks.Max(x => x.z), 0))];
        for (int i = 0; i < originalBlocks.Count; i++)
            vectorArray[FloatToInt(originalBlocks[i].x), FloatToInt(originalBlocks[i].z)] = originalBlocks[i];

        for (int x = 0; x < vectorArray.GetUpperBound(0); x++)
        {
            for (int z = 0; z < vectorArray.GetUpperBound(1); z++)
            {
                if (vectorArray[x, z] != null && vectorArray[x - 1, z] != null && vectorArray[x + 1, z] != null && vectorArray[x, z - 1] != null && vectorArray[x, z + 1] != null)
                {
                    if (vectorArray[x - 1, z] == null)
                    {
                        vertices.Add(new Vector3(vectorArray[x, z].x - 0.5f, vectorArray[x, z].y, vectorArray[x, z].z - 0.5f));
                        uv.Add(new Vector2(vectorArray[x, z].x - 0.5f, vectorArray[x, z].z - 0.5f));
                        vertices.Add(new Vector3(vectorArray[x, z].x - 0.5f, vectorArray[x, z].y, vectorArray[x, z].z + 0.5f));
                        uv.Add(new Vector2(vectorArray[x, z].x - 0.5f, vectorArray[x, z].z + 0.5f));
                    }
                    if (vectorArray[x + 1, z] == null)
                    {
                        vertices.Add(new Vector3(vectorArray[x, z].x + 0.5f, vectorArray[x, z].y, vectorArray[x, z].z - 0.5f));
                        uv.Add(new Vector2(vectorArray[x, z].x + 0.5f, vectorArray[x, z].z - 0.5f));
                        vertices.Add(new Vector3(vectorArray[x, z].x + 0.5f, vectorArray[x, z].y, vectorArray[x, z].z + 0.5f));
                        uv.Add(new Vector2(vectorArray[x, z].x + 0.5f, vectorArray[x, z].z + 0.5f));
                    }
                    if (vectorArray[x, z - 1] == null)
                    {
                        vertices.Add(new Vector3(vectorArray[x, z].x - 0.5f, vectorArray[x, z].y, vectorArray[x, z].z - 0.5f));
                        uv.Add(new Vector2(vectorArray[x, z].x - 0.5f, vectorArray[x, z].z - 0.5f));
                        vertices.Add(new Vector3(vectorArray[x, z].x + 0.5f, vectorArray[x, z].y, vectorArray[x, z].z - 0.5f));
                        uv.Add(new Vector2(vectorArray[x, z].x + 0.5f, vectorArray[x, z].z - 0.5f));
                    }
                    if (vectorArray[x, z - 1] == null)
                    {
                        vertices.Add(new Vector3(vectorArray[x, z].x - 0.5f, vectorArray[x, z].y, vectorArray[x, z].z + 0.5f));
                        uv.Add(new Vector2(vectorArray[x, z].x - 0.5f, vectorArray[x, z].z + 0.5f));
                        vertices.Add(new Vector3(vectorArray[x, z].x + 0.5f, vectorArray[x, z].y, vectorArray[x, z].z + 0.5f));
                        uv.Add(new Vector2(vectorArray[x, z].x + 0.5f, vectorArray[x, z].z + 0.5f));
                    }
                }
            }
        }

        vertices = vertices.Distinct().ToList();
        uv = uv.Distinct().ToList();

        return new Tuple<List<Vector3>, List<Vector2>>(vertices, uv);
    }

    private int FloatToInt(float floatValue)
    {
        return Convert.ToInt32(Math.Round(floatValue));
    }
}
