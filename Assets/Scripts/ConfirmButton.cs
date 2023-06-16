using UnityEngine;
using UnityEngine.EventSystems;

public class ConfirmButton : MonoBehaviour
{
    public GameObject buttonListener;

    private void Initialize()
    {
        gameObject.GetComponent<BaseButton>().onClickCallback = (PointerEventData eventData) =>
        {
            buttonListener.GetComponent<ButtonListener>().SetConfirm();
        };
    }

    private void OnEnable()
    {
        Initialize();
    }
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
