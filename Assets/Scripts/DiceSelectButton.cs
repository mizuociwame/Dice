using UnityEngine;
using UnityEngine.EventSystems;


public class DiceSelectButton : MonoBehaviour
{
    public Player owner;

    private void Initialize()
    {
        gameObject.GetComponent<BaseButton>().onClickCallback = (PointerEventData eventData) =>
        {
            if (eventData.button == PointerEventData.InputButton.Left && owner != null)
            {
                int diceID = gameObject.GetComponent<DiceBase>().diceID;
                owner.SetSelectedDice(diceID);
                owner.diceUI.SelectedDiceToHIghlightLayer(diceID);
            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                gameObject.GetComponent<DiceSelectView>().FlipDice();
            }
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
