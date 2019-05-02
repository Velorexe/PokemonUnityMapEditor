using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryObjectListItem : MonoBehaviour
{
    public CategoryManager CategoryManager;

    public Button ObjectButton;
    public Image ObjectThumbnail;
    public Text ObjectName;
    public Text ObjectCatagorie;

    public GameobjectListItem Item;

    public Color SelectedColor;
    private Color deselectedColor;

    public List<string> Categories;

    public GameObject Object;

    private bool isSelected;

    private void Start()
    {
        deselectedColor = ObjectButton.image.color;
    }

    public void Select()
    {
        if (!isSelected)
        {
            CategoryManager.SelectObject(this);
            isSelected = true;
        }
        else
        {
            CategoryManager.DeselectObject(this);
            isSelected = false;
        }

        UpdateSelect();
    }

    private void UpdateSelect()
    {
        if (isSelected)
            ObjectButton.image.color = SelectedColor;
        else
            ObjectButton.image.color = deselectedColor;
    }
}
