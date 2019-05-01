using System.Collections.Generic;
using System.Linq;
using System;

using UnityEngine;
using UnityEngine.EventSystems;

public class EditorCamera : MonoBehaviour
{
    //Visible to Unity
    public LayerMask BuildMask;

    //Invisible to Unity
    public GameObject CurrentObject;
    private string TextureName;
    private Texture ObjectTexture;

    private GameObject ghostObject;

    private bool isDeleting;

    private DragTypes dragType = DragTypes.FREE;
    private bool isDragging;
    private Vector3 dragPosition;
    private List<GameObject> dragGhostObjects = new List<GameObject>();

    private float yOffset = 0.000f;

    private int currentLayer;

    private void Start()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(mouseRay, out RaycastHit gridPosition, int.MaxValue, BuildMask);

        ghostObject = Instantiate(CurrentObject, FixToGrid(gridPosition.point, /*ghostObject.GetComponent<Renderer>().bounds.size.y / 2*/ 0.001f), new Quaternion());
        ghostObject.GetComponent<Renderer>().materials = SetTextureAndRenderMode(ghostObject.GetComponent<Renderer>().materials, ObjectTexture);
        ghostObject.GetComponent<Renderer>().materials[0] = GhostifyMaterial(ghostObject.GetComponent<Renderer>().materials[0], 2);
    }

    private void Update()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(mouseRay, out RaycastHit gridPosition, int.MaxValue, BuildMask);

        #region MouseInput
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (isDragging && dragGhostObjects.Count > 0)
            {
                foreach (GameObject dragObject in dragGhostObjects)
                {
                    GameObject newObject = Instantiate(CurrentObject, dragObject.transform.position, ghostObject.transform.rotation);
                    newObject.GetComponent<Renderer>().materials = SetTextureAndRenderMode(newObject.GetComponent<Renderer>().materials, ObjectTexture);
                    newObject.layer = LayerMask.NameToLayer("EditObject");
                    newObject.tag = "EditObject";

                    if (newObject.GetComponent<Collider>() == null)
                        newObject.AddComponent<MeshCollider>();

                    Destroy(dragObject);
                }
                dragGhostObjects = new List<GameObject>();
            }
            else
            {
                GameObject newObject = Instantiate(CurrentObject, ghostObject.transform.position, ghostObject.transform.rotation);
                newObject.GetComponent<Renderer>().materials = SetTextureAndRenderMode(newObject.GetComponent<Renderer>().materials, ObjectTexture);
                newObject.layer = LayerMask.NameToLayer("EditObject");
                newObject.tag = "EditObject";

                if (newObject.GetComponent<Collider>() == null)
                    newObject.AddComponent<MeshCollider>();
            }
            isDragging = false;
        }
        else if ((Input.GetMouseButtonDown(0) || isDragging) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (!isDragging)
            {
                dragPosition = ghostObject.transform.position;
            }
            isDragging = true;

            Vector3 ghostNewPosition = FixToGrid(gridPosition.point, /*ghostObject.GetComponent<Renderer>().bounds.size.y / 2*/ 0.001f);
            switch (dragType)
            {
                case DragTypes.FREE:
                    if (ghostNewPosition != dragPosition)
                    {
                        if (IsOverlapping(ghostNewPosition) == false)
                        {
                            GameObject dragGhost = CurrentObject;

                            dragGhost = Instantiate(dragGhost, ghostNewPosition, ghostObject.transform.rotation);
                            dragGhost.GetComponent<Renderer>().materials[0].SetTexture("_MainTex", ObjectTexture);
                            dragGhost.GetComponent<Renderer>().materials[0] = GhostifyMaterial(dragGhost.GetComponent<Renderer>().materials[0], 3);

                            dragGhostObjects.Add(dragGhost);
                            ghostObject.transform.position = ghostNewPosition;
                        }
                    }
                    break;
                case DragTypes.SQUARE:
                    if (ghostNewPosition != dragPosition)
                    {
                        foreach (GameObject oldGhost in dragGhostObjects)
                            Destroy(oldGhost);
                        dragGhostObjects = new List<GameObject>();
                        int differenceX = 1;
                        int differenceZ = 1;

                        if (dragPosition.x > ghostNewPosition.x)
                            differenceX = -1;

                        if (dragPosition.z > ghostNewPosition.z)
                            differenceZ = -1;

                        for (int i = 0; i < Math.Abs(dragPosition.x - ghostNewPosition.x) + 1; i++)
                        {
                            for (int j = 0; j < Math.Abs(dragPosition.z - ghostNewPosition.z) + 1; j++)
                            {
                                Vector3 squareDragPosition = new Vector3(dragPosition.x + (differenceX * i), ghostNewPosition.y, dragPosition.z + (differenceZ * j));
                                GameObject dragGhost = CurrentObject;

                                dragGhost = Instantiate(dragGhost, squareDragPosition, ghostObject.transform.rotation);
                                dragGhost.GetComponent<Renderer>().materials[0].SetTexture("_MainTex", ObjectTexture);
                                dragGhost.GetComponent<Renderer>().materials[0] = GhostifyMaterial(dragGhost.GetComponent<Renderer>().materials[0], 3);

                                dragGhostObjects.Add(dragGhost);
                                ghostObject.transform.position = ghostNewPosition;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            //Vector3 ghostNewPosition = FixToGrid(gridPosition.point, /*ghostObject.GetComponent<Renderer>().bounds.size.y / 2*/ 0.001f);
            //if (ghostNewPosition != dragPosition)
            //{
            //    if (IsOverlapping(ghostNewPosition) == false)
            //    {
            //        GameObject dragGhost = CurrentObject;

            //        dragGhost = Instantiate(dragGhost, ghostNewPosition, ghostObject.transform.rotation);
            //        dragGhost.GetComponent<Renderer>().materials[0].SetTexture("_MainTex", ObjectTexture);
            //        dragGhost.GetComponent<Renderer>().materials[0] = GhostifyMaterial(dragGhost.GetComponent<Renderer>().materials[0], 3);

            //        dragGhostObjects.Add(dragGhost);
            //        ghostObject.transform.position = ghostNewPosition;
            //    }
            //}
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isDeleting = false;
        }
        else if ((Input.GetMouseButtonDown(1) || isDeleting) && !EventSystem.current.IsPointerOverGameObject())
        {
            isDeleting = true;
            if (gridPosition.transform.tag == "EditObject")
                Destroy(gridPosition.transform.gameObject);
            Vector3 ghostNewPosition = FixToGrid(gridPosition.point, /*ghostObject.GetComponent<Renderer>().bounds.size.y / 2*/ 0.001f);
            if (ghostNewPosition != ghostObject.transform.position)
                ghostObject.transform.position = FixToGrid(gridPosition.point, /*ghostObject.GetComponent<Renderer>().bounds.size.y / 2*/ 0.001f);
        }
        else
        {
            Vector3 ghostNewPosition = FixToGrid(gridPosition.point, /*ghostObject.GetComponent<Renderer>().bounds.size.y / 2*/ 0.001f);
            if (ghostNewPosition != ghostObject.transform.position)
                ghostObject.transform.position = FixToGrid(gridPosition.point, /*ghostObject.GetComponent<Renderer>().bounds.size.y / 2*/ 0.001f);
        }
        #endregion

        #region KeyboardInput
        if (Input.GetKeyDown(KeyCode.Q))
            ghostObject.transform.Rotate(new Vector3(0, -90, 0));
        else if (Input.GetKeyDown(KeyCode.E))
            ghostObject.transform.Rotate(new Vector3(0, 90, 0));
        else if (Input.GetKeyUp(KeyCode.W))
            yOffset++;
        else if (Input.GetKeyDown(KeyCode.S))
            yOffset--;
        #endregion
    }

    public void SetDragType(UnityEngine.UI.Dropdown dropdownMenu)
    {
        switch (dropdownMenu.options[dropdownMenu.value].text)
        {
            case "Free Drag":
                dragType = DragTypes.FREE;
                break;
            case "Square Drag":
                dragType = DragTypes.SQUARE;
                break;
            default:
                dragType = DragTypes.FREE;
                break;
        }
    }

    private Material[] SetTextureAndRenderMode(Material[] materials, Texture newTexture)
    {
        Material returnMaterial = materials[0];
        returnMaterial.EnableKeyword("_NORMALMAP");
        returnMaterial.SetTexture("_MainTex", newTexture);
        returnMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        returnMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        returnMaterial.SetInt("_ZWrite", 0);
        returnMaterial.DisableKeyword("_ALPHATEST_ON");
        returnMaterial.EnableKeyword("_ALPHABLEND_ON");
        returnMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        returnMaterial.renderQueue = 3000;
        return new Material[] { returnMaterial };
    }

    private bool IsOverlapping(Vector3 dragGhostPosition)
    {
        foreach(GameObject ghostObject in dragGhostObjects)
        {
            if (dragGhostPosition == ghostObject.transform.position)
                return true;
        }

        return false;
    }

    private Material GhostifyMaterial(Material originalMaterial, int alphaDivide)
    {
        originalMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        originalMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        originalMaterial.SetInt("_ZWrite", 0);
        originalMaterial.DisableKeyword("_ALPHATEST_ON");
        originalMaterial.DisableKeyword("_ALPHABLEND_ON");
        originalMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        originalMaterial.renderQueue = 3000;

        Color ghostColor = originalMaterial.color;

        ghostColor.a /= alphaDivide;
        originalMaterial.color = ghostColor;

        return originalMaterial;
    }

    private Vector3 FixToGrid(Vector3 raycastHit, float yBound)
    {
        if (raycastHit.x <= 0)
            raycastHit.x = (float)(Math.Truncate(raycastHit.x) - 0.5);
        else
            raycastHit.x = (float)(Math.Truncate(raycastHit.x) + 0.5);

        if(Math.Round(raycastHit.y, 0) == 0)
            raycastHit.y = 0 + yBound + yOffset;
        else if (raycastHit.y < 0)
            raycastHit.y = (float)(Math.Truncate(raycastHit.y) - yBound) + yOffset;
        else if (raycastHit.y > 0)
            raycastHit.y = (float)(Math.Truncate(raycastHit.y) + yBound) + yOffset;

        if (raycastHit.z <= 0)
            raycastHit.z = (float)(Math.Truncate(raycastHit.z) - 0.5);
        else
            raycastHit.z = (float)(Math.Truncate(raycastHit.z) + 0.5);

        return raycastHit;
    }

    public void SetCurrentObject(GameobjectListItem objectContainer)
    {
        CurrentObject = objectContainer.Object;
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(mouseRay, out RaycastHit gridPosition, int.MaxValue, BuildMask);

        Destroy(ghostObject);
        ghostObject = Instantiate(CurrentObject, FixToGrid(gridPosition.point, CurrentObject.GetComponent<Renderer>().bounds.size.y / 2), new Quaternion());
        ghostObject.GetComponent<Renderer>().materials[0].mainTexture = ObjectTexture;
        ghostObject.GetComponent<Renderer>().materials[0] = GhostifyMaterial(ghostObject.GetComponent<Renderer>().materials[0], 2);
    }

    public void SetMaterial(MaterialListItem materialContainer)
    {
        ObjectTexture = materialContainer.ObjectThumbnail.mainTexture;
        TextureName = materialContainer.ObjectName.text;
        ghostObject.GetComponent<Renderer>().materials[0].mainTexture = ObjectTexture;
    }

    private enum DragTypes
    {
        FREE,
        SQUARE
    }

    private enum EditStyle
    {
        OBJECT,
        PAINT,
        INSPECT
    }
}
