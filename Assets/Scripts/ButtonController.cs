using TMPro;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] GameObject confirmButton;
    [SerializeField] GameObject rejectButton;
    [SerializeField] GameObject plusButton;
    [SerializeField] GameObject minusButton;
    [SerializeField] GameObject helpButton;

    public void ResetButtonLayout()
    {
        foreach (Transform child in transform)
        {
            if (child != null) { child.gameObject.SetActive(false); }
        }
        helpButton.SetActive(true);
    }

    public void SetConfirmButton(string text)
    {
        confirmButton.SetActive(true);
        confirmButton.GetComponent<TextMeshProUGUI>().text = text;
    }
    public void SetRejectButton(string text)
    {
        rejectButton.SetActive(true);
        rejectButton.GetComponent<TextMeshProUGUI>().text = text;
    }
    public void SetPlusButton(string text)
    {
        plusButton.SetActive(true);
        plusButton.GetComponent<TextMeshProUGUI>().text = text;
    }
    public void SetMinusButton(string text)
    {
        minusButton.SetActive(true);
        minusButton.GetComponent<TextMeshProUGUI>().text = text;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
