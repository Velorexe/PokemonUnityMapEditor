using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ColliderExport : MonoBehaviour
{
    public Transform CollisionTransform;
    public Transform WaterTransform;
    public Transform WalkableTransform;

    public Material CollisionMaterial;
    public Material WaterMaterial;
    public Material WalkableMaterial;

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
            }

            CollisionTransform.GetComponent<MeshFilter>().mesh = new Mesh();
            CollisionTransform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true, true);

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
            }

            WaterTransform.GetComponent<MeshFilter>().mesh = new Mesh();
            WaterTransform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true, true);

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
            }

            WalkableTransform.GetComponent<MeshFilter>().mesh = new Mesh();
            WalkableTransform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true, true);

            WalkableTransform.GetComponent<MeshRenderer>().sharedMaterial = WalkableMaterial;

            foreach (Transform childObject in WalkableTransform)
            {
                if (childObject != WalkableTransform)
                    Destroy(childObject.gameObject);
            }
        }
        #endregion
    }
}
