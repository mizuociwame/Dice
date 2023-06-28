using UnityEngine;

public class Player : MonoBehaviour
{
    public DiceView diceUI;

    public int playerID;
    public string playerName = "player";
    public bool isAI = false;
    public int coin = 1000;
    public Vector3 playerPosition = new Vector3(0f, 3f, -1.4f);
    public Vector3 throwTarget = new Vector3(0f, 4f, 0f);
    public Vector3 tableCenter = new Vector3(0f, 3f, 0f);
    public float rotateAngle = 120f;
    public string faceMaterial = "Illust_ya";

    public bool isCall = false;
    public bool isFold = false;
    public int selectedDice = -1;

    GameObject[] playersDice;
    int selectedDiceCache = -1;
    int payed = 0;

    void CheckMine()
    {
        if (playerID == 0)
        {
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void ResetTurn()
    {
        selectedDice = selectedDiceCache;
        isCall = false;
        isFold = false;
        payed = 0;
        if (isAI) { gameObject.GetComponent<PlayerAI>().SetTendency(); }
    }

    public void SetDice(GameObject[] playersDice)
    {
        this.playersDice = playersDice;
        for (int i = 0; i < this.playersDice.Length; i++)
        {
            this.playersDice[i].GetComponent<DiceBase>().diceID = i;
        }
    }

    public void SetIsAI(bool isAI)
    {
        this.isAI = isAI;
        if (isAI)
        {
            playerName = gameObject.GetComponent<PlayerAI>().RandomName();
        }
        else
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

    public void SetSelectDiceView(bool allFetch)
    {
        DiceSelectView[] selectDice = new DiceSelectView[playersDice.Length];
        for (int i = 0; i < selectDice.Length; i++)
        {
            if (allFetch || i != selectedDice)
            {
                GameObject copyDice = playersDice[i].GetComponent<DiceBase>().GetDiceSelectView(
                    allFetch ? gameObject.GetComponent<Player>() : null);
                DiceSelectView diceView = copyDice.GetComponent<DiceSelectView>();
                diceView.InitSelectPositionMove(i);
                diceView.SetSelectView();
                selectDice[i] = diceView;
            }
        }
        diceUI.SetSelectDices(selectDice, selectedDice);
    }

    void SetResultDice(int playerID, int position, int[,] resultFaceIndex, string[,] materials)
    {
        int[] faceIndex = new int[resultFaceIndex.GetLength(1)];
        string[] mats = new string[materials.GetLength(1)];
        for (int i = 0; i < faceIndex.Length; i++)
        {
            faceIndex[i] = resultFaceIndex[playerID, i];
            mats[i] = materials[playerID, i];
        }
        diceUI.SetResultDiceFace(position, faceIndex, mats);
    }

    public void SetMyResultDice(int[,] resultFaceIndex, string[,] materials) 
    {
        diceUI.ResetDiceFaceFrame();
        SetResultDice(playerID, 0, resultFaceIndex, materials);
    }

    public void SetAllResultDice(int[,] resultFaceIndex, string[,] materials)
    {
        diceUI.ResetDiceFaceFrame();
        for (int player = 0; player < resultFaceIndex.GetLength(0); player++)
        {
            SetResultDice(player, player, resultFaceIndex, materials);
        }
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

    public void ResetBetStatus()
    {
        isCall = false; isFold = false;
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

    private void OnEnable()
    {
        CheckMine();
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckMine();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {

    }
}
