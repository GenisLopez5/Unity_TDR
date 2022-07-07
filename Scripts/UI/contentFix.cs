using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class contentFix : MonoBehaviour
{
    RectTransform rect;
    float x, y;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        x = rect.sizeDelta.x;
        y = rect.sizeDelta.y;
    }

    public void setHeight()
    {
        if (transform.childCount > 9)
        {
            rect.sizeDelta = new Vector2(x, y + 70 * (transform.childCount - 9));
        }
        else
        {
            rect.sizeDelta = new Vector2(x, y);
        }
    }
}
