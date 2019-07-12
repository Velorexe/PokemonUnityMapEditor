using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using System.Xml;
using System.IO;
using System;

using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

using UnityEditor.Formats.Fbx.Exporter;

using ProBuilder2.MeshOperations;
using ProBuilder2.Common;
using ProBuilder2;

public class EditorCamera : MonoBehaviour
{
    //Visible to Unity
    public LayerMask BuildMask;
    public GameObject CurrentObject;

    public Material StandardMaterial;
    public Shader StandardShader;

    public GameObject Parent;

    public Text X;
    public Text Y;
    public Text Z;

    //Invisible to Unity
    private Material ObjectTexture;

    private GameObject ghostObject;

    public ButtonMenuItem CurrentItem;
    private EditStyle editStyle;

    private DragTypes dragType = DragTypes.FREE;
    private bool isDragging;
    private bool dragPositionDone;
    private Vector3 dragPosition;
    private List<GameObject> dragGhostObjects = new List<GameObject>();

    private float yOffset = 0.000f;

    private int currentLayer;

    private void Start()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(mouseRay, out RaycastHit gridPosition, int.MaxValue, BuildMask);

        ghostObject = Instantiate(CurrentObject, FixToGrid(gridPosition.point, /*ghostObject.GetComponent<Renderer>().bounds.size.y / 2*/ 0.001f), new Quaternion(), Parent.transform);
        ghostObject.GetComponent<Renderer>().materials = new Material[] { new Material(ObjectTexture) };
        ghostObject.GetComponent<Renderer>().materials[0] = GhostifyMaterial(ghostObject.GetComponent<Renderer>().materials[0], 2);

        Color buttonHighlight = CurrentItem.ButtonImage.color;
        buttonHighlight.a = 0;

        CurrentItem.ButtonImage.color = buttonHighlight;
        editStyle = CurrentItem.EditStyle;
    }

    private void Update()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(mouseRay, out RaycastHit gridPosition, int.MaxValue, BuildMask);

        #region MouseInput
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            switch (editStyle)
            {
                case EditStyle.OBJECT:
                    if (isDragging && dragGhostObjects.Count > 0)
                    {
                        foreach (GameObject dragObject in dragGhostObjects)
                        {
                            GameObject newObject = Instantiate(CurrentObject, dragObject.transform.position, ghostObject.transform.rotation, Parent.transform);
                            newObject.name += "ID-{" + CurrentObject.name + " !-! " + ObjectTexture.mainTexture.name + "}";
                            newObject.GetComponent<Renderer>().materials = new Material[] { new Material(ObjectTexture) };
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
                        newObject.name += "ID-{" + CurrentObject.name + " !-! " + ObjectTexture.mainTexture.name + "}";
                        newObject.GetComponent<Renderer>().materials = new Material[] { new Material(ObjectTexture) };
                        newObject.layer = LayerMask.NameToLayer("EditObject");
                        newObject.tag = "EditObject";

                        if (newObject.GetComponent<Collider>() == null)
                            newObject.AddComponent<MeshCollider>();
                    }
                    isDragging = false;
                    dragPositionDone = false;
                    break;
                case EditStyle.PAINT:
                    if (gridPosition.transform.gameObject.tag == "EditObject" && !isDragging)
                    {
                        PaintObject(gridPosition.transform.gameObject, ObjectTexture);
                    }
                    isDragging = false;
                    dragPositionDone = false;
                    break;
                default:
                    break;
            }
        }
        else if ((Input.GetMouseButtonDown(0) || isDragging) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (!isDragging)
                dragPosition = ghostObject.transform.position;
            isDragging = true;

            Vector3 ghostNewPosition = FixToGrid(gridPosition.point, /*ghostObject.GetComponent<Renderer>().bounds.size.y / 2*/ 0f);
            if (editStyle == EditStyle.OBJECT)
            {
                switch (dragType)
                {
                    case DragTypes.FREE:
                        if (ghostNewPosition != dragPosition)
                        {
                            if (IsOverlapping(ghostNewPosition) == false)
                            {
                                GameObject dragGhost = CurrentObject;
                                dragGhost = Instantiate(dragGhost, ghostNewPosition, ghostObject.transform.rotation, Parent.transform);
                                dragGhost.GetComponent<Renderer>().materials = new Material[] { new Material(ObjectTexture) };
                                dragGhost.GetComponent<Renderer>().materials[0] = GhostifyMaterial(dragGhost.GetComponent<Renderer>().materials[0], 3);

                                dragGhostObjects.Add(dragGhost);
                                ghostObject.transform.position = ghostNewPosition;
                            }
                        }
                        else if (!dragPositionDone)
                        {
                            GameObject dragGhost = CurrentObject;

                            dragGhost = Instantiate(dragGhost, ghostNewPosition, ghostObject.transform.rotation, Parent.transform);
                            dragGhost.GetComponent<Renderer>().materials = new Material[] { new Material(ObjectTexture) };
                            dragGhost.GetComponent<Renderer>().materials[0] = GhostifyMaterial(dragGhost.GetComponent<Renderer>().materials[0], 3);

                            dragGhostObjects.Add(dragGhost);
                            ghostObject.transform.position = ghostNewPosition;
                            dragPositionDone = true;
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

                                    dragGhost = Instantiate(dragGhost, squareDragPosition, ghostObject.transform.rotation, Parent.transform);
                                    dragGhost.GetComponent<Renderer>().materials = new Material[] { new Material(ObjectTexture) };
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
            }
            else if (editStyle == EditStyle.PAINT)
            {
                if (gridPosition.transform.gameObject.tag == "EditObject" && ((ghostNewPosition != dragPosition && !IsOverlapping(gridPosition.point)) || !dragPositionDone))
                {
                    if (ghostNewPosition == dragPosition)
                        dragPositionDone = true;
                    PaintObject(gridPosition.transform.gameObject, ObjectTexture);
                }
            }
        }
        else if ((Input.GetMouseButtonDown(1)) && !EventSystem.current.IsPointerOverGameObject())
        {
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

        #region CoordinateUpdate;
        X.text = "X: " + Math.Round(ghostObject.transform.position.x, 2);
        Y.text = "Y: " + Math.Round(ghostObject.transform.position.y, 2);
        Z.text = "Z: " + Math.Round(ghostObject.transform.position.z, 2);
        #endregion
    }

    private void PaintObject(GameObject targetObject, Material targetTexture)
    {
        targetObject.GetComponent<Renderer>().materials = new Material[] { new Material(ObjectTexture) };
    }

    public void SetDragType(Dropdown dropdownMenu)
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
        foreach (GameObject ghostObject in dragGhostObjects)
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

        if (Math.Round(raycastHit.y, 0) == 0)
            raycastHit.y = 0 + yBound + yOffset;
        else if (raycastHit.y < 0)
            raycastHit.y = (float)(Math.Truncate(raycastHit.y) - yBound) + yOffset;
        else if (raycastHit.y > 0)
            raycastHit.y = (float)(Math.Truncate(raycastHit.y) + yBound) + yOffset;

        if (raycastHit.z <= 0)
            raycastHit.z = (float)(Math.Truncate(raycastHit.z) - 0.5);
        else
            raycastHit.z = (float)(Math.Truncate(raycastHit.z) + 0.5);

        if (raycastHit.y == 0)
            raycastHit.y += 0.0001f;

        return raycastHit;
    }

    public void SetCurrentObject(GameobjectListItem objectContainer)
    {
        CurrentObject = objectContainer.Object;
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(mouseRay, out RaycastHit gridPosition, int.MaxValue, BuildMask);

        Destroy(ghostObject);
        ghostObject = Instantiate(CurrentObject, FixToGrid(gridPosition.point, CurrentObject.GetComponent<Renderer>().bounds.size.y / 2), new Quaternion());

        ghostObject.GetComponent<Renderer>().materials = new Material[] { new Material(ObjectTexture) };
        ghostObject.GetComponent<Renderer>().materials[0] = GhostifyMaterial(ghostObject.GetComponent<Renderer>().materials[0], 2);
    }

    public void SetMaterial(MaterialListItem materialContainer)
    {
        ObjectTexture = materialContainer.ObjectMaterial;
        ghostObject.GetComponent<Renderer>().materials = new Material[] { materialContainer.ObjectMaterial };
    }

    public void SetEditType(ButtonMenuItem buttonItem)
    {
        editStyle = buttonItem.EditStyle;
        Color buttonHighlight = buttonItem.ButtonImage.color;
        CurrentItem.ButtonImage.color = buttonHighlight;

        buttonHighlight.a = 0;
        buttonItem.ButtonImage.color = buttonHighlight;

        editStyle = buttonItem.EditStyle;
        CurrentItem = buttonItem;
        UpdateEditStyle();
    }

    private void UpdateEditStyle()
    {
        switch (editStyle)
        {
            case EditStyle.INSPECT:
                ghostObject.SetActive(false);
                break;
            case EditStyle.PAINT:
                ghostObject.SetActive(false);
                break;
            case EditStyle.OBJECT:
                ghostObject.SetActive(true);
                break;
        }
    }

    private class MeshTile
    {
        public Vector3 position;
        public Quaternion rotation;

        public Transform transform;

        public Mesh mesh;
        public Material[] materials;
    }

    private class MeshTileGroup
    {
        public string ID;
        public List<MeshTile> tiles = new List<MeshTile>();
    }

    public void Save()
    {
        //Hard coded value, only avaiable in Editor
        bool useFBX = true;
        new MapExport(GameObject.FindGameObjectsWithTag("EditObject")).Export();
    }


    private enum DragTypes
    {
        FREE,
        SQUARE
    }

    public enum EditStyle
    {
        OBJECT,
        PAINT,
        INSPECT
    }
}
