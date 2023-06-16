using TMPro;
using UnityEngine;

public class InformationController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI buttom;
    [SerializeField] TextMeshProUGUI top;
    [SerializeField] TextMeshProUGUI yourName;
    [SerializeField] TextMeshProUGUI opponentName1;
    [SerializeField] TextMeshProUGUI opponentName2;

    public void SetPlayerName(GameObject[] players)
    {
        yourName.gameObject.SetActive(true);
        yourName.text = players[0].GetComponent<Player>().playerName;
        opponentName1.gameObject.SetActive(true);
        opponentName1.text = players[1].GetComponent<Player>().playerName;
        opponentName2.gameObject.SetActive(true);
        opponentName2.text = players[2].GetComponent<Player>().playerName;
    }

    public void ResetInformation()
    {
        buttom.text = string.Empty;
        buttom.gameObject.SetActive(false);
        top.text = string.Empty;
        top.gameObject.SetActive(false);
    }

    public void SetButtom(string text)
    {
        buttom.gameObject.SetActive(true);
        buttom.text = text;
    }

    public void SetTop(string text)
    {
        top.gameObject.SetActive(true);
        top.text = text;
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
