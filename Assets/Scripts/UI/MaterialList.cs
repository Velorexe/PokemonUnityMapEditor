using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class MaterialList : MonoBehaviour
{
    public GameObject gameobjectItem;
    public Transform contentPanel;
    public List<Texture> terrainTiles;

    void Start()
    {
        terrainTiles = Resources.LoadAll("TerrainTiles/Textures").Cast<Texture>().ToList();
        PopulateList();
    }

    public void PopulateList()
    {
        foreach (Texture terrainMaterial in terrainTiles)
        {
            GameObject newTileItem = gameobjectItem;
            MaterialListItem listItemScript = newTileItem.GetComponent<MaterialListItem>();

            listItemScript.ObjectThumbnail.sprite = Sprite.Create((Texture2D)terrainMaterial, new Rect(0.0f, 0.0f, terrainMaterial.width, terrainMaterial.height), new Vector2(), 100.0f);
            listItemScript.ObjectName.text = terrainMaterial.name;

            newTileItem = Instantiate(newTileItem) as GameObject;
            newTileItem.transform.SetParent(contentPanel);
            newTileItem.SetActive(true);
        }
    }
}
