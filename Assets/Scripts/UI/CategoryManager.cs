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
    private Vector2 contentOriginalSize;

    public void Start()
    {
        CategoryPanel.SetActive(false);
        contentOriginalSize = Content.GetComponent<RectTransform>().sizeDelta;
    }

    public void AddCategory(Dropdown dropDown)
    {
        dropDownMenu = dropDown;
        CategoryPanel.SetActive(true);

        RectTransform rt = Content.GetComponent<RectTransform>();
        foreach (GameObject listObject in ObjectList.Items)
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

            rt.sizeDelta = new Vector2(rt.rect.width, rt.rect.height + newTileItem.GetComponent<RectTransform>().rect.height);
        }
    }

    public void EditCategory(Dropdown dropDown)
    {
        dropDownMenu = dropDown;
        CategoryPanel.SetActive(true);

        RectTransform rt = Content.GetComponent<RectTransform>();
        foreach (GameObject listObject in ObjectList.Items)
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

            if (listItem.Categories.Contains(dropDown.options[dropDown.value].text))
                listItemScript.Select();

            rt.sizeDelta = new Vector2(rt.rect.width, rt.rect.height + newTileItem.GetComponent<RectTransform>().rect.height);
        }
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

        selectedItems = new List<CategoryObjectListItem>();

        Content.GetComponent<RectTransform>().sizeDelta = contentOriginalSize;

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
