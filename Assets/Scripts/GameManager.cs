using Photon.Pun;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public struct Face
{
    public int suit;
    public int number;

    public Face(int suit, int number)
    {
        this.suit = suit;
        this.number = number;
    }

    public int FaceToRank(int numSuit)
    {
        if (this.number < 0)
        {
            return -52*5;
        }
        else
        {
            return this.number * numSuit + (numSuit - this.suit);
        }
    }
}

public struct Hand
{
    public bool isStraight;
    public bool isFlash;
    public int[] pairs;
    public bool isRoyal;

    public int handClass;
    public int handRank;

    public int owner; 

    public Hand(bool isStraight, bool isFlash, int[] pairs, bool isRoyal, int handClass, int handRank, int owner)
    {
        this.isStraight = isStraight;
        this.isFlash = isFlash;
        this.pairs = pairs;
        this.isRoyal = isRoyal;

        this.handClass = handClass;
        this.handRank = handRank;

        this.owner = owner;
    }

    public void SetStraight(int topNum, int secondNum, int numbers, int numJoker) 
    {
        this.isStraight = true;
        this.handRank = topNum;
        if (topNum <= numbers + 1 &&
            topNum >= numbers + 1 - numJoker &&
            secondNum > 9)
        { this.isRoyal = true; }
    }

    public void SetFlash(int topNum)
    {
        this.isFlash = true;
        this.handRank = topNum;
    }

    public void HandClassDetermine()
    {
        if (pairs[0] == 5) { this.handClass = 10; }
        else if (this.isStraight && this.isFlash && this.isRoyal) { this.handClass = 9; }
        else if (this.isStraight && this.isFlash) { this.handClass = 8; }
        else if (pairs[0] == 4) { this.handClass = 7; }
        else if (pairs[0] + pairs[1] == 5) { this.handClass = 6; }
        else if (this.isFlash) { this.handClass = 5; }
        else if (this.isStraight) { this.handClass = 4; }
        else if (pairs[0] == 3) { this.handClass = 3; }
        else if (pairs[0] == 2 && pairs[1] == 2) { this.handClass = 2; }
        else if (pairs[0] == 2) { this.handClass = 1; }
        else { this.handClass = 0; }
    }

    public string HandName()
    {
        if (this.handClass == 10) { return "Five Card"; }
        else if (this.handClass == 9) { return "Royal Straight Flash"; }
        else if (this.handClass == 8) { return "Straight Flash"; }
        else if (this.handClass == 7) { return "Four Card"; }
        else if (this.handClass == 6) { return "Full House"; }
        else if (this.handClass == 5) { return "Flash"; }
        else if (this.handClass == 4) { return "Straight"; }
        else if (this.handClass == 3) { return "Three Card"; }
        else if (this.handClass == 2) { return "Two Pair"; }
        else if (this.handClass == 1) { return "One Pair"; }
        else { return "No Hand"; }
    }
}

public class GameManager : MonoBehaviour
{
    public GameObject prefabDice;
    public GameObject prefabPlayer;
    public CoinView coinUI;
    public InformationController informationController;
    [SerializeField] GameObject nameField;
    [SerializeField] ButtonController buttonController;
    [SerializeField] ButtonListener buttonListener;

    public int numPlayers = 3;
    public int dicesPerPlayer = 3;
    public int numbers = 13;
    public int numSuits = 4;
    public int numResultDices = 5;
    public float timeToThrow = 0.5f;
    public float timeToViewResult = 1f;
    public int rate;

    public int betCoin;
    [SerializeField] int inPhaseTurn = 5;

    Player myPlayer;
    int gamePhase = -1;
    int[] allDiceFaces;
    GameObject[] players;
    ThrowDice[,] thrownDice;
    int operationPlayerID = 0;
    Face[,] result;
    int[,] resultFaceIndex;
    string[,] materials;
    int preBetCoin;
    int carry;
    int winner;
    int elapsedTurn = 0;

    bool stepPhase = false;
    // Start is called before the first frame update

    int CountMovingDices()
    {
        int count = 0;
        if (thrownDice != null)
        {
            foreach (ThrowDice dice in thrownDice)
            {
                if (dice == null || (dice.onTable && !dice.CheckMoving()))
                {
                    count++;
                }
            }
        }
          return count;
    }

    void CreateDices()
    {
        int[] facesArray = Enumerable.Range(0, numbers*numSuits+2).ToArray();
        allDiceFaces = facesArray.OrderBy(i => Guid.NewGuid()).ToArray();

        for (int player = 0;  player < numPlayers; player++)
        {
            Player playerObj = players[player].GetComponent<Player>();
            GameObject[] diceObjects = new GameObject[dicesPerPlayer];
            for (int dice = 0; dice < dicesPerPlayer; dice++)
            {
                diceObjects[dice] = Instantiate(prefabDice);
                DiceBase diceBase = diceObjects[dice].GetComponent<DiceBase>();
                int faceArrayStartPosition = diceBase.numDiceFaces * (player * dicesPerPlayer + dice);
                diceBase.SetDiceFaceMaterial(playerObj.faceMaterial);
                diceBase.Initialize(allDiceFaces[faceArrayStartPosition..(faceArrayStartPosition + diceBase.numDiceFaces)]);
            }
            players[player].GetComponent<Player>().SetDice(diceObjects);
        }
    }

    void SetThrowDice(int phase)
    {
        if (phase == 0)
        {
            thrownDice = new ThrowDice[numPlayers, 1];
            for (int player = 0; player < numPlayers; player++)
            {
                thrownDice[player, 0] = players[player].GetComponent<Player>().ThrowSelectedDice();
            }
        }
        else if (phase == 2) 
        {
            thrownDice = new ThrowDice[numPlayers, dicesPerPlayer - 1];
            for (int player = 0; player < numPlayers; player++)
            {
                Player playerObj = players[player].GetComponent<Player>();
                if (playerObj.isFold)
                {
                    for (int dice = 0; dice < dicesPerPlayer - 1; dice++)
                    {
                        thrownDice[player, dice] = null;
                    }
                }
                else
                {
                    ThrowDice[] dices = playerObj.ThrowNonSelectedDice();
                    for (int dice = 0; dice < dicesPerPlayer - 1; dice++)
                    {
                        thrownDice[player, dice] = dices[dice];
                    }
                }
            }
        }
    }

    void DestroyThrownDice()
    {
        if (thrownDice != null)
        {
            foreach (ThrowDice dice in thrownDice)
            {
                if (dice != null)
                {
                    Destroy(dice.gameObject);
                }
            }
        }
    }

    void DestroyPlayerDice()
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<Player>().DestroyPlayersDice();
        }
    }

    void RefreshDiceView()
    {
        myPlayer.diceUI.DestroyDiceSelection();
        myPlayer.diceUI.ResetDiceFaceFrame();
    }

    void ResetResult()
    {
        result = new Face[numPlayers, numResultDices];
        resultFaceIndex = new int[numPlayers, numResultDices];
        for (int i = 0; i < numPlayers; i++)
        {
            for (int j = 0; j < numResultDices; j++)
            {
                resultFaceIndex[i, j] = -1;
            }
        }
    }

    void SetMaterials()
    {
        materials = new string[numPlayers, numResultDices];
        for (int i = 0; i < numPlayers; i++) 
        {
            for (int j = 0; j < numPlayers; j++)
            {
                materials[i, j] = players[j].GetComponent<Player>().faceMaterial;
            }
            materials[i, numPlayers] = players[i].GetComponent<Player>().faceMaterial;
            materials[i, numPlayers+1] = players[i].GetComponent<Player>().faceMaterial;
        }
    }

    void RefreshBoard()
    {
        Player[] playersComponents = new Player[players.Length];
        for (int i = 0; i < players.Length; i++) 
        { 
            playersComponents[i] = players[i].GetComponent<Player>(); 
        }
        SetMaterials();
        coinUI.SetPlayers(playersComponents);
        coinUI.SetGameManager(gameObject.GetComponent<GameManager>());

        ResetPhase();
    }

    void ResetPhase()
    {
        DestroyPlayerDice();
        CreateDices();
        carry = 0;
        elapsedTurn = 0;
        ResetTurn();
    }

    void ResetTurn()
    {
        RefreshDiceView();
        DestroyThrownDice();
        ResetResult();

        foreach (GameObject player in players)
        {
            player.GetComponent<Player>().ResetTurn();
        }

        gamePhase = 0;
        stepPhase = false;
        betCoin = rate;
        preBetCoin = 0;
        winner = -1;
    }

    void ToPhaseFirstBet()
    {
        myPlayer.SetSelectDiceView(true);
        foreach (GameObject playerObj in players)
        {
            Player player = playerObj.GetComponent<Player>();
            if (player.isAI) 
            { 
                player.isCall = true;
                player.SetSelectedDice(player.GetComponent<PlayerAI>().RandomSelectDice(dicesPerPlayer));
            }
        }

        informationController.ResetInformation();
        informationController.SetTop("choose the dice to share.");

        buttonController.ResetButtonLayout();
        buttonController.SetConfirmButton("OK");
    }

    void CheckCall()
    {
        int readyCount = 0;
        foreach (GameObject playerObj in players)
        {
            Player player = playerObj.GetComponent<Player>();
            if (player.isCall || player.isFold) { readyCount++; }
        }

        if (readyCount == numPlayers) { stepPhase = true; }
        else { stepPhase = false; }
    }

    void KeyReceiveFirstBet()
    {
        if (buttonListener.GetConfirm())
        {
            myPlayer.isCall = !myPlayer.isCall;
        }

        buttonListener.ResetButtons();
        // test code
        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     Hand hand = CheckHand(new Face[] 
        //     { 
        //         new Face(0, 14),
        //         new Face(-1, -1),
        //         new Face(4, 13),
        //         new Face(-1, -1),
        //         new Face(2, 13)
        //     });
        // }
    }

    IEnumerator ToPhaseFirstThrow(int phase)
    {
        myPlayer.diceUI.DestroyDiceSelection();
        informationController.ResetInformation();
        buttonController.ResetButtonLayout();

        thrownDice = null;
        foreach (GameObject player in players) 
        {
            player.GetComponent<Player>().PayCoin(betCoin);
            carry += betCoin;
        }
        preBetCoin = betCoin;
        yield return new WaitForSeconds(timeToThrow);
        SetThrowDice(phase);
    }

    void CheckThrownDice()
    {
        if (thrownDice != null && CountMovingDices() == thrownDice.Length)
        {
            if (gamePhase == 1)
            {
                for (int player = 0; player < numPlayers; player++)
                {
                    Player playerObj = players[player].GetComponent<Player>();
                    ThrowDice dice = thrownDice[player, 0];
                    int faceIndex = dice.CheckFace();
                    int face = playerObj.GetSelectedDiceFace(faceIndex);
                    for (int i = 0;  i < numPlayers; i++)
                    {
                        resultFaceIndex[i, player] = face;
                        if (face >= numbers * numSuits)
                        {
                            result[i, player] = new Face(-1, -1);
                        }
                        else
                        {
                            int suit = (int)(face / numbers);
                            int num = face % numbers + 1;
                            if (num == 1) { num = numbers + 1; }
                            result[i, player] = new Face(suit, num);
                        }
                    }
                }
                myPlayer.SetMyResultDice(resultFaceIndex, materials);
            }
            else if (gamePhase == 3)
            {
                for (int player = 0; player < numPlayers; player++)
                {
                    Player playerObj = players[player].GetComponent<Player>();
                    if (playerObj.isFold) 
                    {
                        for (int i = 0; i < numResultDices; i++) { resultFaceIndex[player, i] = -1; }
                    }
                    else
                    {
                        for (int i = 0; i < (dicesPerPlayer - 1); i++)
                        {
                            ThrowDice dice = thrownDice[player, i];
                            int faceIndex = dice.CheckFace();
                            int face = playerObj.GetNonSelectedDiceFace(i, faceIndex);
                            resultFaceIndex[player, numPlayers + i] = face;
                            if (face >= numbers * numSuits)
                            {
                                result[player, numPlayers + i] = new Face(-1, -1);
                            }
                            else
                            {
                                int suit = (int)(face / numbers);
                                int num = face % numbers + 1;
                                if (num == 1) { num = numbers + 1; }
                                result[player, numPlayers + i] = new Face(suit, num);
                            }
                        }
                    }
                }
                myPlayer.SetAllResultDice(resultFaceIndex, materials);
            }
            stepPhase = true;
        }
    }

    IEnumerator ToPhaseSecondBet()
    {
        foreach (GameObject player in players) { player.GetComponent<Player>().ResetBetStatus(); }
        yield return new WaitForSeconds(timeToViewResult);
        DestroyThrownDice();
        myPlayer.SetSelectDiceView(false);

        informationController.ResetInformation();
        informationController.SetTop("How much to bet?");

        buttonController.ResetButtonLayout();
        buttonController.SetConfirmButton("Call");
        buttonController.SetRejectButton("Fold");
        buttonController.SetPlusButton("+");
        buttonController.SetMinusButton("-");
    }

    void Squaring(Player player)
    {
        int pay = player.PayCoin(betCoin);
        if (pay > 0)
        {
            carry += pay;
            preBetCoin = betCoin;
            foreach (GameObject playerObj in players)
            {
                playerObj.GetComponent<Player>().isCall = false;
            }
        }
        else { player.isCall = true; }
    }

    void KeyReceiveSecondBet()
    {
        if (!myPlayer.isFold && operationPlayerID == myPlayer.playerID)
        {
            if (buttonListener.GetConfirm())
            {
                Squaring(myPlayer);
                operationPlayerID = (operationPlayerID + 1) % numPlayers;
                buttonController.SetConfirmButton("Call");
            }
            if (buttonListener.GetReject())
            {
                myPlayer.isFold = true;
                betCoin = preBetCoin;
                operationPlayerID = (operationPlayerID + 1) % numPlayers;
            }
            if (buttonListener.GetPlus())
            {
                betCoin += rate;
                buttonController.SetConfirmButton("Raise");
            }
            if (buttonListener.GetMinus())
            {
                if (betCoin > preBetCoin) { betCoin -= rate; }
                if (betCoin == preBetCoin) { buttonController.SetConfirmButton("Call"); }
            }
        }
        else if (players[operationPlayerID].GetComponent<Player>().isAI)
        {
            betCoin += rate * players[operationPlayerID].GetComponent<PlayerAI>().RandomRaise();
            Squaring(players[operationPlayerID].GetComponent<Player>());
            operationPlayerID = (operationPlayerID + 1) % numPlayers;
        }

        buttonListener.ResetButtons();
    }
    IEnumerator ToPhaseSecondThrow(int phase)
    {
        myPlayer.diceUI.DestroyDiceSelection();
        informationController.ResetInformation();
        buttonController.ResetButtonLayout();

        thrownDice = null;
        yield return new WaitForSeconds(timeToThrow);
        SetThrowDice(phase);
    }

    IEnumerator ToPhaseEnd()
    {
        yield return new WaitForSeconds(timeToViewResult);
        DestroyThrownDice();

        Hand[] resultHands = new Hand[numPlayers];
        for (int i = 0; i < numPlayers; i++) 
        {
            Player player = players[i].GetComponent<Player>();
            Face[] playerResult = new Face[numResultDices];
            if (player.isFold) { resultHands[i] = new Hand(false, false, new int[] { 0, 0 }, false, -1, 0, i); }
            else
            {
                for (int j = 0; j < numResultDices; j++) { playerResult[j] = result[i, j]; }
                resultHands[i] = CheckHand(playerResult);
                resultHands[i].owner = i;
            }
        }

        Hand[] handOrder = resultHands.OrderByDescending(x => x.handClass).ThenByDescending(x => x.handRank).ToArray();
        if (!(handOrder[0].handClass == handOrder[1].handClass && handOrder[0].handRank == handOrder[1].handRank))
        { 
            winner = handOrder[0].owner; 
        }
        if (winner >= 0) 
        {
            Player winnerPlayer = players[winner].GetComponent<Player>();
            winnerPlayer.coin += carry;

            informationController.ResetInformation();
            informationController.SetButtom($"{winnerPlayer.playerName} get {carry} coins.");
            carry = 0;
        }
        else
        {
            informationController.ResetInformation();
            informationController.SetButtom($"{carry} coins carried over.");
        }
        buttonController.ResetButtonLayout();
        buttonController.SetConfirmButton("To NEXT");
        Debug.Log($"player {winner} is win.");
    }

    Hand CheckHand(Face[] result)
    {
        Face[] sortedResult = result.OrderByDescending(x => x.number).ToArray();
        int jokerIndex = sortedResult.Length;
        for (int i = 0; i < sortedResult.Length; i++)
        {
            if (sortedResult[i].number == -1)
            {
                jokerIndex = i;
                break;
            }
        }
        int numJoker = sortedResult.Length - jokerIndex;

        Hand hand = new Hand(false, false, new int[(int)sortedResult.Length/2], false, 0, 0, 0);

        int jokerStock = numJoker;
        for (int i = 1; i < jokerIndex; i++) 
        {
            if (sortedResult[i].number == sortedResult[i - 1].number - 1) 
            {
                if (i == jokerIndex - 1) 
                {
                    hand.SetStraight(sortedResult[0].number, sortedResult[1].number, numbers, numJoker);
                }
            }
            else if (jokerStock > 0 && 
                     (sortedResult[i].number < sortedResult[i - 1].number - 1) &&
                     (sortedResult[i].number >= sortedResult[i - 1].number - (1 + jokerStock)))
            {
                if (i == jokerIndex - 1)
                {
                    hand.SetStraight(sortedResult[0].number, sortedResult[1].number, numbers, numJoker);
                }
                else { jokerStock -= (sortedResult[i - 1].number - sortedResult[i].number - 1); }
            }
            else if (i == 1 && sortedResult[i - 1].number == numbers + 1 && sortedResult[i].number < 6 && sortedResult[i].number >= 3) { continue; }
            else { break; }
        }

        for (int i = 1; i < jokerIndex; i++) 
        {
            if (sortedResult[i].suit == sortedResult[i - 1].suit)
            {
                if (i == jokerIndex - 1) 
                {
                    hand.SetFlash(sortedResult[0].number);
                }
            }
            else { break; }
        }

        int pairIndex = 0;
        int pairCount = 1;
        for (int i = 1; i < jokerIndex; i++)
        {
            if (sortedResult[i].number == sortedResult[i - 1].number) { pairCount++; }
            else if (pairCount > 1)
            {
                hand.pairs[pairIndex] = pairCount;
                hand.handRank = hand.handRank * (numbers + 1) + sortedResult[i - 1].number;
                pairIndex++;
                pairCount = 1;
            }
        }
        if (pairCount > 1) 
        { 
            hand.pairs[pairIndex] = pairCount;
            pairIndex++;
            if (pairCount > 2) { hand.handRank = hand.handRank + sortedResult[jokerIndex - 1].number * (numbers + 1); }
            else { hand.handRank = hand.handRank * (numbers + 1) + sortedResult[jokerIndex - 1].number; }
        }
        if (numJoker > 0)
        {
            if (pairIndex == 0) 
            { 
                hand.pairs[0] = 1 + numJoker;
                hand.handRank = sortedResult[0].number;
            }
            else { hand.pairs[0] += numJoker; }
            hand.handRank -= (numbers + 1) * (numbers + 1) + numbers;
        }

        if (!hand.isStraight && !hand.isFlash) 
        {
            for (int i = hand.pairs[0] + hand.pairs[1]; i < sortedResult.Length; i++) 
            { hand.handRank = hand.handRank * (numbers + 1) + sortedResult[i].number; }
        }

        hand.HandClassDetermine();
        Debug.Log($"hand is {hand.HandName()}, rank is {hand.handRank}.");
        return hand;
    }

    public void AddPlayer(GameObject playerObj)
    {
        Player player = playerObj.GetComponent<Player>();
        if (player.playerID == 0)
        {
            players = new GameObject[numPlayers];
            players[0] = playerObj;
        }
        else { players[player.playerID] = playerObj; }

        if (playerObj.GetPhotonView().IsMine) { myPlayer = player; }
    }

    void GatherAIPlayers(int currentPlayers)
    {
        for (int i = currentPlayers; i < numPlayers; i++)
        {
            GameObject AIPlayer = Instantiate(prefabPlayer);
            AIPlayer.GetComponent<Player>().SetPosition(i);
            AIPlayer.GetComponent<Player>().SetIsAI(true);
            players[i] = AIPlayer;
        }
    }

    void StepPhase()
    {
        if (stepPhase)
        {
            int cacheGamePhase = gamePhase;
            gamePhase = 99;

            switch (cacheGamePhase)
            {
                case -1:
                    {
                        StartGame(); break;
                    }
                case 0:
                    {
                        StartCoroutine(ToPhaseFirstThrow(cacheGamePhase)); break;
                    }
                case 1:
                    {
                        StartCoroutine(ToPhaseSecondBet()); break;
                    }
                case 2:
                    {
                        StartCoroutine(ToPhaseSecondThrow(cacheGamePhase)); break;
                    }
                case 3:
                    {
                        StartCoroutine(ToPhaseEnd()); break;
                    }
            }

            gamePhase = cacheGamePhase + 1;
            stepPhase = false;
        }
    }

    void StartGame()
    {
        GatherAIPlayers(PhotonNetwork.CurrentRoom.PlayerCount);
        RefreshBoard();
        coinUI.gameObject.SetActive(true);
        string yourName = nameField.GetComponent<TMP_InputField>().text;
        if (yourName.Length > 0 ) { players[0].GetComponent<Player>().playerName = yourName; }
        nameField.SetActive(false);
        informationController.SetPlayerName(players);
        ToPhaseFirstBet();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!stepPhase)
        {
            switch (gamePhase)
            {
                case -1:
                {
                    if (buttonListener.GetConfirm())
                    {
                        stepPhase = true;
                        buttonListener.ResetButtons();
                    }
                    break;
                }
                case 0:
                {
                    KeyReceiveFirstBet();
                    CheckCall(); 
                    break;
                }
                case 1:
                {
                    CheckThrownDice(); break;
                }
                case 2:
                {
                    KeyReceiveSecondBet();
                    CheckCall();
                    break;
                }
                case 3:
                {
                    CheckThrownDice(); break;
                }
                case 4:
                {
                    if (buttonListener.GetConfirm())
                    {
                        if (elapsedTurn > inPhaseTurn)
                        {
                            ResetPhase();
                        }
                        else
                        {
                            elapsedTurn++;
                            ResetTurn();
                        }
                        ToPhaseFirstBet();
                        buttonListener.ResetButtons();
                    }
                    break;
                }
            }
        }
    }

    void FixedUpdate()
    {
        StepPhase();
    }
}
