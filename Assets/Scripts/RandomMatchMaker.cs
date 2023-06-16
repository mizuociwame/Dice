using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RandomMatchMaker : MonoBehaviourPunCallbacks
{
    public GameObject PhotonObject;

    public override void OnJoinedRoom()
    {
        GameManager gm = gameObject.GetComponent<GameManager>();

        GameObject playerObj = PhotonNetwork.Instantiate(
            gm.prefabPlayer.name,
            Vector3.zero,
            Quaternion.identity,
            0
        );
        int playerID = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        Player player = playerObj.GetComponent<Player>();
        player.playerID = playerID;
        player.SetPosition(playerID);
        gm.AddPlayer(playerObj);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOption = new RoomOptions();
        roomOption.MaxPlayers = gameObject.GetComponent<GameManager>().numPlayers;
        PhotonNetwork.CreateRoom(null, roomOption);
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
