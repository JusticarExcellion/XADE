using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectableCowboy : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RadialMenu Menu;
    public int ButtonIndex;

    public void
    OnPointerEnter( PointerEventData eventData )
    {
        Menu.SetLabelText( ButtonIndex );
    }

    public void
    OnPointerExit( PointerEventData eventData )
    {
        Menu.ClearLabelText(  );
    }
}
