using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabFold : MonoBehaviour
{
    public RectTransform FoldPanel;
    public int FoldingSpeed;

    private bool isFolding = false;
    private bool isUnfolding = false;

    private Vector2 originalScale;

    private void Start()
    {
        originalScale = FoldPanel.sizeDelta;
    }
    private void Update()
    {
        if (isFolding && FoldPanel.sizeDelta.y > GetComponent<RectTransform>().sizeDelta.y)
        {
            Vector2 newSize = FoldPanel.sizeDelta;
            newSize.y -= Time.deltaTime * FoldingSpeed;
            FoldPanel.sizeDelta = newSize;
        }
        else if(isUnfolding && FoldPanel.sizeDelta.y < originalScale.y)
        {
            Vector2 newSize = FoldPanel.sizeDelta;
            newSize.y += Time.deltaTime * FoldingSpeed;
            FoldPanel.sizeDelta = newSize;
        }
    }

    public void Fold()
    {
        if (isUnfolding || (!isUnfolding && !isFolding))
        {
            isFolding = true;
            isUnfolding = false;
        }
        else
        {
            isUnfolding = true;
            isFolding = false;
        }
    }
}
