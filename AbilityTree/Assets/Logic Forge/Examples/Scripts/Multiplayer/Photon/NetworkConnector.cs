using UnityEngine;
using System.Collections;

/// <summary>
/// A script that makes sure that we are always connected to the server
/// </summary>
public class NetworkConnector : MonoBehaviour
{
    void Start()
    {
        //DontDestroyOnLoad(gameObject);
        PhotonNetwork.ConnectToBestCloudServer("1.0");
    }

    void FixedUpdate()
    {

        //if (PhotonNetwork.connectionState == ConnectionState.Disconnected)
           //  PhotonNetwork.ConnectToBestCloudServer("1.0");

    }

    void OnGUI()
    {
        GUILayout.Box(PhotonNetwork.connectionState.ToString());
    }
}