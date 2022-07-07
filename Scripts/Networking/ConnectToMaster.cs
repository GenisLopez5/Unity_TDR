using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class ConnectToMaster : MonoBehaviourPunCallbacks
{
    public string version;
    public string region;
    public TextMeshProUGUI connectionText;

    void Start()
    {
        PhotonNetwork.GameVersion = version;
        
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        SceneManager.UnloadSceneAsync("Master");
    }

    private void Update()
    {
        connectionText.SetText("Connection: " + PhotonNetwork.NetworkClientState);
    }
}
