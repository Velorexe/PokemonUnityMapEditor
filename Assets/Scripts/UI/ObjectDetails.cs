using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectDetails : MonoBehaviour
{
    public GameobjectListItem SelectedObject;

    public Toggle IsWalkable;
    public Toggle IsWater;

    public Toggle WarpPad;
    public GameObject DirectionDropdown;

    public void Update()
    {
        if (WarpPad.isOn && !DirectionDropdown.activeSelf)
            DirectionDropdown.SetActive(true);
        else if(!WarpPad.isOn && DirectionDropdown.activeSelf)
            DirectionDropdown.SetActive(false);
    }

    public void Apply()
    {

    }

    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
}
