using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PhotonInGameManager : MonoBehaviourPunCallbacks
{
    public static PhotonInGameManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.IsOpen = false;

        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Alerts a = new Alerts();
        StartCoroutine(a.NewAlert(new string[3] { "The host left the room you were playing on" , "El anfitrión ha abandonado la sala en la que estabas jugando", "L'amfitrió ha abandonat la sala en qué estaves jugant"}, new string[3] { "DISCONNECTED" , "DESCONECTADO", "DESCONECTAT"}));

        SceneManager.LoadScene("Lobby");
    }
}
