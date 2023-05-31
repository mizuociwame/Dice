using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinView : MonoBehaviour
{
    public TextMeshProUGUI player0CoinText;
    public TextMeshProUGUI player1CoinText;
    public TextMeshProUGUI player2CoinText;
    public TextMeshProUGUI betCoinText;

    Player player0;
    Player player1;
    Player player2;
    GameManager gameManager;

    public void SetPlayers(Player[] players)
    {
        player0 = players[0];
        player1 = players[1];
        player2 = players[2];
    }

    public void RemovePlayers()
    {
        player0 = null;
        player1 = null;
        player2 = null;
    }

    public void SetGameManager(GameManager gm) { gameManager = gm; }

    public void RemoveGameManager() { gameManager = null; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player0 != null) { player0CoinText.text = player0.coin.ToString(); }
        if (player1 != null) { player1CoinText.text = player1.coin.ToString(); }
        if (player2 != null) { player2CoinText.text = player2.coin.ToString(); }
        if (gameManager != null) { betCoinText.text = gameManager.betCoin.ToString(); }
    }
}
