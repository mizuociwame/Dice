using UnityEngine;
using UnityEngine.EventSystems;

public class HelpButton : MonoBehaviour
{
    public GameObject helpCanvas;

    private void Initialize()
    {
        gameObject.GetComponent<BaseButton>().onClickCallback = (PointerEventData eventData) =>
        {
            helpCanvas.SetActive(!helpCanvas.activeSelf);
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
