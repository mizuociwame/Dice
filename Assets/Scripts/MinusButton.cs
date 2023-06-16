using UnityEngine;
using UnityEngine.EventSystems;

public class MinusButton : MonoBehaviour
{
    public GameObject buttonListener;

    private void Initialize()
    {
        gameObject.GetComponent<BaseButton>().onClickCallback = (PointerEventData eventData) =>
        {
            buttonListener.GetComponent<ButtonListener>().SetMinus();
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
