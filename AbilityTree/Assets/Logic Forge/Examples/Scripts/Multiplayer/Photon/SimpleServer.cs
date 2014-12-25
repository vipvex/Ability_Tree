using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;

public class SimpleServer : MonoBehaviour {

    public string gameType = "LogicObject Engine Example";
    public string gameName = "New Game";


    public int maxPlayers = 4;


    public string gameLevel;


    private void StartServer()      
    {
        PhotonNetwork.CreateRoom("", true, true, 10);
    }

    void OnJoinedRoom()
    {
       Application.LoadLevel(gameLevel);
    }

    void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Debug.Log("Player " + player + " | " + player.name);
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 200, 300, 400));
        GUILayout.BeginVertical();
        
        if (!Network.isClient && !Network.isServer)
        {
            
            if (GUILayout.Button("Start Server"))
                StartServer();

            GUILayout.BeginHorizontal();
            foreach (RoomInfo room in PhotonNetwork.GetRoomList())
            {
                GUILayout.Label("Name: " + room.name + "   " + room.playerCount + "/" + room.maxPlayers);
                if (GUILayout.Button("Join"))
                {
                    PhotonNetwork.JoinRoom(room.name);
                    PhotonNetwork.LoadLevel(gameLevel);
                }
            }
            GUILayout.EndHorizontal();

        
        }
        
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

}
