using UnityEngine;
using UnityEngine.EventSystems;

public class BaseButton : 
    MonoBehaviour,
    IPointerClickHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    public System.Action<PointerEventData> onClickCallback;

    public void OnPointerClick(PointerEventData eventData)
    {
        onClickCallback?.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData) { }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
