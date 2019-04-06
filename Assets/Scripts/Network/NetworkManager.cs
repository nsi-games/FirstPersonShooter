using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public Camera standbyCamera;
    public int version = 1; 

    void Start()
    {
        Connect();
    }

    void Connect()
    {
        PhotonNetwork.ConnectUsingSettings(version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");
        PhotonNetwork.JoinRandomRoom();
    }
    
    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("OnPhotonRandomJoinFailed");
        PhotonNetwork.CreateRoom(null);
    }

    void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        SpawnMyPlayer();
    }

    void SpawnMyPlayer()
    {
        PhotonNetwork.Instantiate("PlayerController", Vector3.zero, Quaternion.identity, 0);
        standbyCamera.enabled = false;
    }
}
