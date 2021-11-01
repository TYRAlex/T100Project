using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MonoPointEvent : MonoBehaviour,IPointerExitHandler,IPointerEnterHandler
{
    public bool IsEnter = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsEnter = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsEnter = false;
    }
}
