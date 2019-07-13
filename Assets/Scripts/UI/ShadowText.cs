using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShadowText : MonoBehaviour
{
    public Text text;
    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Text>().text = text.text;
    }
}
