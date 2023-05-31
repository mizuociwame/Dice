using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int coin = 1000;
    public Vector3 playerPosition = new Vector3(0f, 3f, -1.4f);
    public Vector3 throwTarget = new Vector3(0f, 4f, 0f);
    public Vector3 tableCenter = new Vector3(0f, 3f, 0f);
    public float rotateAngle = 120f;
    public bool isFold = false;

    public int selectedDice = -1;

    bool isAI = false;
    GameObject[] playersDice;
    int selectedDiceCache = -1;
    int payed = 0;

    public void ResetTurn()
    {
        selectedDice = selectedDiceCache;
        isFold = false;
        payed = 0;
    }

    public void SetDice(GameObject[] playersDice)
    {
        this.playersDice = playersDice;
    }

    public void SetIsAI(bool isAI)
    {
        this.isAI = isAI;
        if ( !isAI)
        {
            gameObject.GetComponent<PlayerAI>().enabled = false;
        }
    }

    public void SetPosition(int numRotation)
    {
        gameObject.transform.position = playerPosition;
        gameObject.transform.RotateAround(tableCenter, new Vector3(0f, 1f, 0f),  rotateAngle*numRotation);
    }

    public void DestroyPlayersDice()
    {
        if (playersDice != null)
        {
            foreach (GameObject dice in playersDice) 
            {
                Destroy(dice);
            }
        }
    }

    public void SetSelectedDice(int selectedDice)
    {
        this.selectedDice = selectedDice;
        selectedDiceCache = selectedDice;
    }

    public GameObject[] GetSelectDice(bool allFetch)
    {
        GameObject[] selectDice = new GameObject[playersDice.Length];
        for (int i = 0; i < selectDice.Length; i++)
        {
            if (allFetch || i != selectedDice)
            {
                GameObject copyDice = playersDice[i].GetComponent<DiceBase>().GetDiceSelectView();
                DiceSelectView diceView = copyDice.GetComponent<DiceSelectView>();
                diceView.InitSelectPositionMove(i);
                diceView.SetSelectView();
                selectDice[i] = copyDice;
            }
        }
        return selectDice;
    }

    public int GetDiceFace(int index, int face)
    {
        return playersDice[index].GetComponent<DiceBase>().GetFace(face);
    }

    public int GetSelectedDiceFace(int face)
    {
        return GetDiceFace(selectedDice, face);
    }

    public int GetNonSelectedDiceFace(int index, int face)
    {
        if (index >= selectedDice)
        {
            return GetDiceFace(index + 1, face);
        }
        else
        {
            return GetDiceFace(index, face);
        }
    }

    public ThrowDice ThrowSelectedDice()
    {
        if (selectedDice == -1 && selectedDiceCache == -1)
        {
            selectedDice = gameObject.GetComponent<PlayerAI>().RandomSelectDice(playersDice.Length);
        }
        else if (selectedDice == -1 && selectedDiceCache > -1)
        {
            selectedDice = selectedDiceCache;
        }
        GameObject thrownDiceObject = playersDice[selectedDice].GetComponent<DiceBase>().GetThrowDice();
        ThrowDice thrownDice = thrownDiceObject.GetComponent<ThrowDice>();
        thrownDice.SetThrowDirection(gameObject.transform.position, throwTarget - gameObject.transform.position);
        thrownDice.isThrown = true;
        return thrownDice;
    }

    public ThrowDice[] ThrowNonSelectedDice()
    {
        ThrowDice[] thrownDices = new ThrowDice[playersDice.Length -1];
        int j = 0;
        for (int i = 0; i < playersDice.Length; i++)
        {
            if (i != selectedDice)
            {
                GameObject thrownDiceObject = playersDice[i].GetComponent<DiceBase>().GetThrowDice();
                ThrowDice thrownDice = thrownDiceObject.GetComponent<ThrowDice>();
                thrownDice.SetThrowDirection(gameObject.transform.position, throwTarget - gameObject.transform.position);
                thrownDice.transform.RotateAround(throwTarget, new Vector3(0, 1f, 0), 10*i);
                thrownDice.isThrown = true;
                thrownDices[j] = thrownDice;
                j++;
            }
        }
        return thrownDices;
    }

    public int PayCoin(int amount)
    {
        int pay = amount - payed;
        if (pay > 0)
        {
            coin -= pay;
            payed = amount;
        }
        return pay;
    }

    public void GainCoin(int amount)
    {
        coin += amount;
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
