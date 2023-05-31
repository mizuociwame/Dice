using JetBrains.Annotations;
using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.XR;
using static UnityEngine.UI.GridLayoutGroup;

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
    public int numPlayers = 3;
    public int dicesPerPlayer = 3;
    public int numbers = 13;
    public int numSuits = 4;
    public int numResultDices = 5;
    public GameObject prefabDice;
    public GameObject prefabPlayer;
    public float timeToThrow = 0.5f;
    public float timeToViewResult = 1f;
    public GameObject diceUI;
    public GameObject coinUI;
    public int rate;
    public int betCoin;

    int gamePhase = 0;
    int[] allDiceFaces;
    GameObject[] players;
    ThrowDice[,] thrownDice;
    Face[,] result;
    int[,] resultFaceIndex;
    int preTurnBetCoin;
    int carry;
    int callCount;
    int foldCount;
    int winner;
    // Start is called before the first frame update

    int CountMovingDices()
    {
        int count = 0;
        foreach (ThrowDice dice in thrownDice)
        {
            if (dice == null || (dice.onTable && !dice.CheckMoving()))
            {
                count++;
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
            GameObject[] diceObjects = new GameObject[dicesPerPlayer];
            for (int dice = 0; dice < dicesPerPlayer; dice++)
            {
                diceObjects[dice] = Instantiate(prefabDice);
                DiceBase diceBase = diceObjects[dice].GetComponent<DiceBase>();
                int faceArrayStartPosition = diceBase.numDiceFaces * (player * dicesPerPlayer + dice);
                diceBase.Initialize(allDiceFaces[faceArrayStartPosition..(faceArrayStartPosition + diceBase.numDiceFaces)]);
            }
            players[player].GetComponent<Player>().SetDice(diceObjects);
        }
    }

    void SetThrowDice()
    {
        if (gamePhase == 1)
        {
            thrownDice = new ThrowDice[numPlayers, 1];
            for (int player = 0; player < numPlayers; player++)
            {
                thrownDice[player, 0] = players[player].GetComponent<Player>().ThrowSelectedDice();
            }
        }
        else if (gamePhase == 3) 
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

    void RefreshBoard()
    {
        CoinView coinUIComponent = coinUI.GetComponent<CoinView>();
        Player[] playersComponent = new Player[players.Length];
        for (int i = 0; i < players.Length; i++) 
        { 
            playersComponent[i] = players[i].GetComponent<Player>(); 
        }
        coinUIComponent.SetPlayers(playersComponent);
        coinUIComponent.SetGameManager(gameObject.GetComponent<GameManager>());

        DestroyPlayerDice();
        CreateDices();
        carry = 0;
        ToPhaseFirstBet();
    }

    void ResetTurn()
    {
        DiceView diceUIComponent = diceUI.GetComponent<DiceView>();
        diceUIComponent.DestroyDiceSelection();
        diceUIComponent.ResetDiceFaceFrame();

        DestroyThrownDice();

        result = new Face[numPlayers, numResultDices];
        resultFaceIndex = new int[numPlayers, numResultDices];
        for (int i = 0; i < numPlayers; i++)
        {
            for (int j = 0; j < numResultDices; j++)
            {
                resultFaceIndex[i, j] = -1;
            }
        }

        foreach (GameObject player in players)
        {
            player.GetComponent<Player>().ResetTurn();
        }

        betCoin = rate;
        preTurnBetCoin = 0;
        winner = -1;
    }

    void ToPhaseFirstBet()
    {
        ResetTurn();
        Player player = players[0].GetComponent<Player>();
        diceUI.GetComponent<DiceView>().SetSelectDices(player.GetSelectDice(true), player.selectedDice);
        gamePhase = 0;
    }

    void KeyReceiveFirstBet()
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyUp(KeyCode.R))
        {
            diceUI.GetComponent<DiceView>().FlipDice();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            players[0].GetComponent<Player>().SetSelectedDice(0);
            diceUI.GetComponent<DiceView>().SelectedDiceToHIghlightLayer(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            players[0].GetComponent<Player>().SetSelectedDice(1);
            diceUI.GetComponent<DiceView>().SelectedDiceToHIghlightLayer(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            players[0].GetComponent<Player>().SetSelectedDice(2);
            diceUI.GetComponent<DiceView>().SelectedDiceToHIghlightLayer(2);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(ToPhaseFirstThrow());
        }
        // test code
        else if (Input.GetKeyDown(KeyCode.T))
        {
            Hand hand = CheckHand(new Face[] 
            { 
                new Face(0, 14),
                new Face(-1, -1),
                new Face(4, 13),
                new Face(-1, -1),
                new Face(2, 13)
            });
        }
    }

    IEnumerator ToPhaseFirstThrow()
    {
        gamePhase = -1;
        diceUI.GetComponent<DiceView>().DestroyDiceSelection();
        foreach (GameObject player in players) 
        {
            player.GetComponent<Player>().PayCoin(betCoin);
            carry += betCoin;
        }
        preTurnBetCoin = betCoin;
        yield return new WaitForSeconds(timeToThrow);
        gamePhase = 1;
        SetThrowDice();
    }

    void CheckThrownDice()
    {
        if (CountMovingDices() == thrownDice.Length)
        {
            DiceView ui = diceUI.GetComponent<DiceView>();
            string[] materials = new string[numPlayers];
            if (gamePhase == 1)
            {
                for (int player = 0; player < numPlayers; player++)
                {
                    Player playerObj = players[player].GetComponent<Player>();
                    ThrowDice dice = thrownDice[player, 0];
                    materials[player] = dice.GetComponent<DiceBase>().diceFaceMaterial;
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
                ui.PlayerSetResultDiceFace(0, resultFaceIndex, materials[0]);
                StartCoroutine(ToPhaseSecondBet());
            }
            else if (gamePhase == 3)
            {
                for (int player = 0; player < numPlayers; player++)
                {
                    Player playerObj = players[player].GetComponent<Player>();
                    if (playerObj.isFold) 
                    {
                        for (int i = 0; i < dicesPerPlayer; i++) { resultFaceIndex[player, i] = -1; }
                    }
                    else
                    {
                        for (int i = 0; i < (dicesPerPlayer - 1); i++)
                        {
                            ThrowDice dice = thrownDice[player, i];
                            materials[player] = dice.GetComponent<DiceBase>().diceFaceMaterial;
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
                ui.SetResultDiceFace(resultFaceIndex, materials);
                StartCoroutine(ToPhaseEnd());
            }
        }
    }

    IEnumerator ToPhaseSecondBet()
    {
        gamePhase = -1;
        callCount = 0;
        foldCount = 0;
        yield return new WaitForSeconds(timeToViewResult);
        gamePhase = 2;
        DestroyThrownDice();
        Player player = players[0].GetComponent<Player>();
        diceUI.GetComponent<DiceView>().SetSelectDices(player.GetSelectDice(false), player.selectedDice);
    }

    void Raise(GameObject player)
    {
        int pay = player.GetComponent<Player>().PayCoin(betCoin);
        carry += pay;
        preTurnBetCoin = betCoin;
    }

    void AIPlayerRaise(GameObject player)
    {
        if (player.GetComponent<PlayerAI>().RandomRaise() == 0) 
        { 
            callCount++;
        }
        else
        {
            betCoin += rate;
            callCount = 0;
        }
        Raise(player);
        if (callCount + foldCount == numPlayers) { StartCoroutine(ToPhaseSecondThrow()); }
    }

    void KeyReceiveSecondBet()
    {
        if ( Input.GetKeyDown(KeyCode.R) || Input.GetKeyUp(KeyCode.R) )
        {
            diceUI.GetComponent<DiceView>().FlipDice();
        }
        else if ( Input.GetKeyDown(KeyCode.Space) )
        {
            Raise(players[0]);
            if (betCoin == preTurnBetCoin) 
            { 
                callCount++; 
                if ( callCount + foldCount == numPlayers ) { StartCoroutine(ToPhaseSecondThrow()); }
            }
            else 
            { 
                callCount = 0;
            }
            AIPlayerRaise(players[1]);
            AIPlayerRaise(players[2]);
        }
        else if ( Input.GetKeyDown(KeyCode.F) )
        {
            players[0].GetComponent<Player>().isFold = true;
            foldCount++;
            StartCoroutine(ToPhaseSecondThrow());
        }
        else if ( Input.GetKeyDown(KeyCode.UpArrow) )
        {
            betCoin += rate;
        }
        else if ( Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (betCoin >= preTurnBetCoin + rate) { betCoin -= rate; }
        }
    }
    IEnumerator ToPhaseSecondThrow()
    {
        gamePhase = -1;
        diceUI.GetComponent<DiceView>().DestroyDiceSelection();
        yield return new WaitForSeconds(timeToThrow);
        gamePhase = 3;
        SetThrowDice();
    }

    IEnumerator ToPhaseEnd()
    {
        gamePhase = 4;
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
        if (!(handOrder[0].handClass == handOrder[1].handClass && handOrder[0].handRank == handOrder[1].handRank)) { winner = handOrder[0].owner; }
        if (winner >= 0) 
        { 
            players[winner].GetComponent<Player>().coin += carry;
            carry = 0;
        }
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

    void GatherPlayers(int numAIPlayers)
    {
        players = new GameObject[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {
            players[i] = Instantiate(prefabPlayer);
            players[i].GetComponent<Player>().SetPosition(i);
            if (i < (numPlayers - numAIPlayers))
            {
                players[i].GetComponent<Player>().SetIsAI(false);
            }
        }
    }

    void Start()
    {
        GatherPlayers(2);
        RefreshBoard();
    }

    // Update is called once per frame
    void Update()
    {
        if (gamePhase == 0)
        {
            KeyReceiveFirstBet();
        }
        else if (gamePhase == 1)
        {
            CheckThrownDice();
        }
        else if (gamePhase == 2)
        {
            KeyReceiveSecondBet();
        }
        else if (gamePhase == 3)
        {
            CheckThrownDice();
        }
        else if (gamePhase == 4)
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                ToPhaseFirstBet();
            }
        }
    }

    void FixedUpdate()
    {

    }
}
