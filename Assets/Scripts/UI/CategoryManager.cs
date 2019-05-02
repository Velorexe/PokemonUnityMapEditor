using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class CategoryManager : MonoBehaviour
{
    public GameObject CategoryPanel;
    public GameObjectList ObjectList;

    public GameObject Content;
    public GameObject ListItem;

    public InputField CategoryName;

    private List<string> categoryList = new List<string>();
    private List<CategoryObjectListItem> selectedItems = new List<CategoryObjectListItem>();

    private Dropdown dropDownMenu;

    public void Start()
    {
        CategoryPanel.SetActive(false);
    }

    public void AddCategory(Dropdown dropDown)
    {
        dropDownMenu = dropDown;
        CategoryPanel.SetActive(true);

        foreach(GameObject listObject in ObjectList.items)
        {
            GameObject newTileItem = ListItem;
            CategoryObjectListItem listItemScript = newTileItem.GetComponent<CategoryObjectListItem>();
            GameobjectListItem listItem = listObject.GetComponent<GameobjectListItem>();

            listItemScript.ObjectThumbnail.sprite = listItem.ObjectThumbnail.sprite;
            listItemScript.ObjectName.text = listItem.ObjectName.text;
            listItemScript.Object = listItem.Object;
            listItemScript.Item = listObject.GetComponent<GameobjectListItem>();

            newTileItem = Instantiate(newTileItem) as GameObject;
            newTileItem.transform.SetParent(Content.transform);
            newTileItem.SetActive(true);
        }
    }

    public void EditCategory(Dropdown dropDown)
    {
        dropDownMenu = dropDown;
        CategoryPanel.SetActive(true);
    }

    public void FinishCategory()
    {
        dropDownMenu.options.Add(new Dropdown.OptionData(CategoryName.text));
        foreach(CategoryObjectListItem item in selectedItems)
            item.Item.Categories.Add(CategoryName.text);
        ResetCategoryPanel();
    }

    public void CancelCategory()
    {
        ResetCategoryPanel();
    }

    private void ResetCategoryPanel()
    {
        CategoryPanel.SetActive(false);

        dropDownMenu = null;
        CategoryName.text = string.Empty;

        foreach (Transform child in Content.transform)
            Destroy(child.gameObject);
    }

    public void SelectObject(CategoryObjectListItem listItem)
    {
        selectedItems.Add(listItem);
    }

    public void DeselectObject(CategoryObjectListItem listItem)
    {
        selectedItems.Remove(listItem);
    }
}
