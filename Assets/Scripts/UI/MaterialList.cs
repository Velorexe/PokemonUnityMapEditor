using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MaterialList : MonoBehaviour
{
    public GameObject gameobjectItem;
    public Transform contentPanel;
    public List<Object> terrainTiles;

    void Start()
    {
        terrainTiles = Resources.LoadAll("TerrainTiles/Textures").ToList();
        PopulateList();
    }

    public void PopulateList()
    {
        RectTransform rt = contentPanel.GetComponent<RectTransform>();
        foreach (Object terrainMaterial in terrainTiles)
        {
            GameObject newTileItem = gameobjectItem;
            MaterialListItem listItemScript = newTileItem.GetComponent<MaterialListItem>();

            if (terrainMaterial.GetType() == typeof(Texture2D) || terrainMaterial.GetType() == typeof(Texture))
                listItemScript.ObjectThumbnail.sprite = Sprite.Create((Texture2D)terrainMaterial, new Rect(0.0f, 0.0f, ((Texture)terrainMaterial).width, ((Texture)terrainMaterial).height), new Vector2(), 100.0f);
            else if (terrainMaterial.GetType() == typeof(Sprite))
                listItemScript.ObjectThumbnail.sprite = (Sprite)terrainMaterial;
            listItemScript.ObjectName.text = terrainMaterial.name;

            newTileItem = Instantiate(newTileItem) as GameObject;
            newTileItem.transform.SetParent(contentPanel);
            newTileItem.SetActive(true);

            rt.sizeDelta = new Vector2(rt.rect.width, rt.rect.height + newTileItem.GetComponent<RectTransform>().rect.height);
        }
    }
}
