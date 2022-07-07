using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color NormalColor, HighlightedColor;
    public Vector3 highScale = new Vector3(1, 1, 1);
    Vector3 normScale;

    Image thisImage;

    [SerializeField]
    UnityEvent OnLeftClick;
    [SerializeField]
    UnityEvent OnRightClick;
    [SerializeField]
    UnityEvent OnShiftLeftClick;
    [SerializeField]
    UnityEvent OnMiddleClick;

    public bool isInside;

    private void Start()
    {
        if (OnLeftClick == null) OnLeftClick = new UnityEvent();
        if (OnShiftLeftClick == null) OnShiftLeftClick = new UnityEvent();
        if (OnRightClick == null) OnRightClick = new UnityEvent();
        if (OnMiddleClick == null) OnMiddleClick = new UnityEvent();
        thisImage = GetComponent<Image>();

        normScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isInside = true;

        TransitionToColor(HighlightedColor);
        TransitionToScale(highScale);

        if (Input.GetMouseButton(1)) OnRightClick?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isInside = false;

        TransitionToColor(NormalColor);
        TransitionToScale(normScale);
    }

    private void Update()
    {
        if (isInside)
        {
            if (Input.GetMouseButtonDown(0)) OnLeftClick?.Invoke();
            if (Input.GetMouseButtonDown(1)) OnRightClick?.Invoke();
            if (Input.GetMouseButtonDown(2)) OnMiddleClick?.Invoke();
            if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift)) OnShiftLeftClick?.Invoke();
        }
    }

    void TransitionToColor(Color Col)
    {
        thisImage.color = Col;
    }

    void TransitionToScale(Vector3 newScale)
    {
        transform.localScale = newScale;
    }
}
