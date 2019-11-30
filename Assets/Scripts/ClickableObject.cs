using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickableObject : MonoBehaviour, IPointerClickHandler
{
    public System.Action Clicked = delegate{};
    public System.Action RightClicked = delegate { };
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Clicked();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            RightClicked();
        }
    }
}