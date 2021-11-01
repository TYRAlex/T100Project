using UnityEngine;
using UnityEngine.EventSystems;



public class PointerClickListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{

    public delegate void VoidDelegate(GameObject go);
    public VoidDelegate onClick;

    public VoidDelegate clickDown;

    public VoidDelegate clickUp;
   

    static public PointerClickListener Get(GameObject go)
    {
        PointerClickListener listener = go.GetComponent<PointerClickListener>();
        if (listener == null) listener = go.AddComponent<PointerClickListener>();
        return listener;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
        {
            onClick(gameObject);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        clickDown?.Invoke(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        clickUp?.Invoke(gameObject);
    }
}
