using UnityEngine;
using System.Collections;

public class GameManager : Photon.MonoBehaviour {


    public GameObject playerPrefab;
    public Transform[] spawnPositions;


    public int redTeamScore = 0;
    public int blueTeamScore = 0;


    public GameObject localPlayer;


    void OnGUI()
    {

        GUILayout.BeginArea(new Rect(Screen.width / 2 - 150, 0, 300, 400));
        GUILayout.BeginHorizontal();

        GUI.color = Color.red;
        GUILayout.Box("Red: " + redTeamScore);

        GUI.color = Color.blue;
        GUILayout.Box("Blue: " + blueTeamScore);

        GUI.color = Color.white;
        GUILayout.EndHorizontal();
        GUILayout.EndArea();


        if (localPlayer)
            return;

        GUILayout.BeginArea(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100, 300, 400));
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Join Red team"))
        {
            photonView.RPC("AddPlayer", PhotonTargets.AllBuffered, PhotonNetwork.player, "Red", PhotonNetwork.AllocateViewID(), (int)Random.Range(0, spawnPositions.Length));
        }

        if (GUILayout.Button("Join Blue team"))
        {
            photonView.RPC("AddPlayer", PhotonTargets.AllBuffered, PhotonNetwork.player, "Blue", PhotonNetwork.AllocateViewID(), (int)Random.Range(0, spawnPositions.Length));
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

    }

    [RPC]
    public void AddPlayer(PhotonPlayer player, string teamID, int photonID, int spawnIndex)
    {
        // Spawn player and assign photon ID
        GameObject newPlayer = (GameObject)Instantiate(playerPrefab, spawnPositions[spawnIndex].position, Quaternion.identity);

        newPlayer.GetComponent<TeamID>().ID = teamID;


        PhotonView[] photonViews = newPlayer.GetComponentsInChildren<PhotonView>();
        foreach (PhotonView photonView in photonViews)
            photonView.viewID = photonID;

        if (player == PhotonNetwork.player)
            localPlayer = newPlayer;

    }

}
