using System.Collections.Generic;
using System.Linq;
using System;

using UnityEngine;

public class EditorCamera : MonoBehaviour
{
    //Visible to Unity
    public GameObject CurrentObject;
    public string TextureName;
    public Texture ObjectTexture;
    public EditorCameraBehaviour CameraBehaviour;

    public LayerMask BuildMask;

    //Invisible to Unity
    private GameObject ghostObject;

    private bool isDragging;
    private Vector3 dragPosition;
    private List<GameObject> dragGhostObjects = new List<GameObject>();

    private void Start()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(mouseRay, out RaycastHit gridPosition, int.MaxValue, BuildMask);

        ghostObject = Instantiate(CurrentObject, FixToGrid(gridPosition.point, CurrentObject.GetComponent<Renderer>().bounds.size.y / 2), new Quaternion());
        ghostObject.GetComponent<Renderer>().materials[0] = GhostifyMaterial(ghostObject.GetComponent<Renderer>().materials[0], 2);
        ghostObject.GetComponent<Renderer>().materials[0].mainTexture = ObjectTexture;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging && dragGhostObjects.Count > 0)
            {
                foreach(GameObject dragObject in dragGhostObjects)
                {
                    GameObject newObject = Instantiate(CurrentObject, dragObject.transform.position, new Quaternion());
                    newObject.GetComponent<Renderer>().materials[0].mainTexture = ObjectTexture;
                    newObject.layer = LayerMask.NameToLayer("EditObject");
                    newObject.tag = "EditObject";

                    Destroy(dragObject);
                }
                dragGhostObjects = new List<GameObject>();
            }
            else
            {
                GameObject newObject = Instantiate(CurrentObject, ghostObject.transform.position, new Quaternion());
                newObject.GetComponent<Renderer>().materials[0].mainTexture = ObjectTexture;
                newObject.layer = LayerMask.NameToLayer("EditObject");
                newObject.tag = "EditObject";
            }

            isDragging = false;
        }
        else if (Input.GetMouseButtonDown(0) || isDragging)
        {
            isDragging = true;
            if (!isDragging)
            {
                dragPosition = ghostObject.transform.position;
            }

            //Replace with options for dragging in Switch Case
            //For now we'll keep it on every dragged position
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(mouseRay, out RaycastHit gridPosition, int.MaxValue, BuildMask);

            Vector3 ghostNewPosition = FixToGrid(gridPosition.point, ghostObject.GetComponent<Renderer>().bounds.size.y / 2);
            if (ghostNewPosition != dragPosition)
            {
                if (IsOverlapping(ghostNewPosition) == false)
                {
                    GameObject dragGhost = CurrentObject;

                    dragGhost = Instantiate(dragGhost, ghostNewPosition, new Quaternion());
                    dragGhost.GetComponent<Renderer>().materials[0].mainTexture = ObjectTexture;
                    dragGhost.GetComponent<Renderer>().materials[0] = GhostifyMaterial(dragGhost.GetComponent<Renderer>().materials[0], 3);

                    dragGhostObjects.Add(dragGhost);

                    ghostObject.transform.position = ghostNewPosition;
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(mouseRay, out RaycastHit gridPosition, int.MaxValue, BuildMask);

            if (gridPosition.transform.tag == "EditObject")
                Destroy(gridPosition.transform.gameObject);
        }
        else
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(mouseRay, out RaycastHit gridPosition, int.MaxValue, BuildMask);

            Vector3 ghostNewPosition = FixToGrid(gridPosition.point, ghostObject.GetComponent<Renderer>().bounds.size.y / 2);
            if (ghostNewPosition != ghostObject.transform.position)
                ghostObject.transform.position = FixToGrid(gridPosition.point, ghostObject.GetComponent<Renderer>().bounds.size.y / 2);
        }
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

    private Vector3 FixToGrid(Vector3 raycastHit, float yOffset)
    {
        if (raycastHit.x <= 0)
            raycastHit.x = (float)(Math.Truncate(raycastHit.x) - 0.5);
        else
            raycastHit.x = (float)(Math.Truncate(raycastHit.x) + 0.5);

        if(Math.Round(raycastHit.y, 0) == 0)
            raycastHit.y = 0 + yOffset;
        else if (raycastHit.y < 0)
            raycastHit.y = (float)(Math.Truncate(raycastHit.y) - yOffset);
        else if (raycastHit.y > 0)
            raycastHit.y = (float)(Math.Truncate(raycastHit.y) + yOffset);

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

        ghostObject = Instantiate(CurrentObject, FixToGrid(gridPosition.point, CurrentObject.GetComponent<Renderer>().bounds.size.y / 2), new Quaternion());
        ghostObject.GetComponent<Renderer>().materials[0] = GhostifyMaterial(ghostObject.GetComponent<Renderer>().materials[0], 2);
    }

    public void SetMaterial(MaterialListItem materialContainer)
    {
        ObjectTexture = materialContainer.ObjectThumbnail.mainTexture;
        TextureName = materialContainer.ObjectName.text;
        ghostObject.GetComponent<Renderer>().materials[0].mainTexture = ObjectTexture;
    }
}
