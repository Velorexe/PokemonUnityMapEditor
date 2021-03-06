﻿using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class GameObjectList : MonoBehaviour
{
    public GameObject gameobjectItem;
    public Transform contentPanel;
    public List<GameObject> terrainTiles;

    public List<GameObject> items { get; private set; } = new List<GameObject>();

    void Start()
    {
        terrainTiles = Resources.LoadAll<GameObject>("TerrainTiles/Models").ToList();
        PopulateList();
    }

    public void PopulateList()
    {
        foreach(GameObject terrainTile in terrainTiles)
        {
            GameObject newTileItem = gameobjectItem;
            GameobjectListItem listItemScript = newTileItem.GetComponent<GameobjectListItem>();


            Texture2D objectThumbnail = RuntimePreviewGenerator.GenerateModelPreview(terrainTile.transform);
            if(objectThumbnail != null)
                listItemScript.ObjectThumbnail.sprite = Sprite.Create(objectThumbnail, new Rect(0.0f, 0.0f, objectThumbnail.width, objectThumbnail.height), new Vector2(), 100.0f);
            listItemScript.ObjectName.text = terrainTile.name;
            listItemScript.Object = terrainTile;

            newTileItem = Instantiate(newTileItem) as GameObject;
            newTileItem.transform.SetParent(contentPanel);
            newTileItem.SetActive(true);

            items.Add(newTileItem);
        }
    }

    public void UpdateList(Dropdown dropMenu)
    {
        foreach (Transform child in contentPanel.transform)
            child.gameObject.SetActive(false);

        if (dropMenu.value != 0)
        {
            foreach (GameObject item in items)
            {
                if (item.GetComponent<GameobjectListItem>().Categories.Contains(dropMenu.options[dropMenu.value].text))
                    item.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject item in items)
                item.SetActive(true);
        }
    }
}
