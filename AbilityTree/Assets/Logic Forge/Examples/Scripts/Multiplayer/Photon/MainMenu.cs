using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;

public class MainMenu : MonoBehaviour
{


    public string newRoomName;
    public string newRoomType;
    public string newRoomMap;
    public string newRoomPlayers;

    
    //private Photon 


    void Start()
    {


    }

    void Update()
    {
        //PhotonNetwork.playerName = playerName.text;
    }

    public void Refresh()
    {
        foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
        {
            GUILayout.Label("Name: " + roomInfo.name + "   " + roomInfo.playerCount + "/" + roomInfo.maxPlayers);
        }

    }

    public void CreateNewGame()
    {

        if (PhotonNetwork.connectionStateDetailed == PeerState.JoinedLobby)
        {

            //ExitGames.Client.Photon.Hashtable hashRoomProp = new ExitGames.Client.Photon.Hashtable();
            //hashRoomProp.Add("type", newRoomType.text.ToString());
            //hashRoomProp.Add("map", newRoomMap.text.ToString());
           // string[] stringRoomProp = new string[] { "type", "map" };

            PhotonNetwork.CreateRoom("", true, true, 10);

        }

    }

    void OnJoinedRoom()
    {
        //PhotonNetwork.LoadLevel(newRoomMap.text.ToString());
    }
}