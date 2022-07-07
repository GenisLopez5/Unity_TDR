using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon;
using Photon.Pun;

public class ListedRoom : MonoBehaviour
{
    public string roomName;
    public int players, maxPlayers;

    public TextMeshProUGUI nameText, playersText;

    public void SetVariables(string roomName, int playersInRoom, int MaxPlayersInRoom)
    {
        this.roomName = roomName;
        players = playersInRoom;
        maxPlayers = MaxPlayersInRoom;

        nameText.text = roomName;
        playersText.text = players + " / " + maxPlayers;
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
