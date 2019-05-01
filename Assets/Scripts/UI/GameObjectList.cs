using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameObjectList : MonoBehaviour
{
    public GameObject gameobjectItem;
    public Transform contentPanel;
    public List<GameObject> terrainTiles;

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

            Texture2D objectThumbnail = AssetPreview.GetAssetPreview(terrainTile);
            if(objectThumbnail != null)
                listItemScript.ObjectThumbnail.sprite = Sprite.Create(objectThumbnail, new Rect(0.0f, 0.0f, objectThumbnail.width, objectThumbnail.height), new Vector2(), 100.0f);
            listItemScript.ObjectName.text = terrainTile.name;
            listItemScript.Object = terrainTile;

            newTileItem = Instantiate(newTileItem) as GameObject;
            newTileItem.transform.SetParent(contentPanel);
            newTileItem.SetActive(true);
        }
    }
}
