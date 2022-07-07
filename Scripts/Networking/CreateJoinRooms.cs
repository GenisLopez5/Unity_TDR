using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class CreateJoinRooms : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [Header("Create")]
    public Toggle isPrivate;
    public TMP_InputField NameInput;
    public Slider PlayersSlider;
    public TextMeshProUGUI MaxPlayersText;

    [Header("Join")]
    public TMP_InputField RoomNameInput;
    public Button JoinButton;
    public TextMeshProUGUI buttonText;

    [Header("RoomListing")]
    List<RoomInfo> AviableRooms = new List<RoomInfo>();
    public GameObject RoomListed;
    public Transform content;
    List<GameObject> instantedLists = new List<GameObject>();
    bool firstTime = true;

    private void Start()
    {
        CheckJoinLenght();
        Cursor.lockState = CursorLockMode.None;
        SceneManager.UnloadSceneAsync("LoginOrRegister");
    }

    private void Awake()
    {
        UpdatePlayerText();
    }

    public void Create()
    {
        if (NameInput.text.Length < 3 && NameInput.text.Length > 20)
        {
            Alerts a = new Alerts();
            StartCoroutine(a.NewAlert(new string[3] { "The name lenght must be between 3 and 20" , "El nombre tiene que contener entre 3 y 20 carácteres", "El nom ha de contenir entre 3 i 20 caràcters"}, new string[3] { "ALERT!", "ALERTA!", "ALERTA!"}));
        }

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)Mathf.RoundToInt(PlayersSlider.value);
        options.IsVisible = !isPrivate.isOn;

        PhotonNetwork.CreateRoom(NameInput.text, options);
    }

    public void Join()
    {
        PhotonNetwork.JoinRoom(RoomNameInput.text);
    }

    public void UpdatePlayerText()
    {
        switch (ConfigurationUIManager.current.language)
        {
            case 0:
                MaxPlayersText.text = "Max. Players: " + Mathf.RoundToInt(PlayersSlider.value);
                break;
            case 1:
                MaxPlayersText.text = "Max. Jugadores: " + Mathf.RoundToInt(PlayersSlider.value);
                break;
            case 2:
                MaxPlayersText.text = "Max. Jugadors: " + Mathf.RoundToInt(PlayersSlider.value);
                break;
        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Map1");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Alerts a = new Alerts();
        StartCoroutine(a.NewAlert(new string[3] { message , message , message }, new string[3] { "ALERT!", "ALERTA!", "ALERTA!" }));
    }

    public void CheckJoinLenght()
    {
        if (RoomNameInput.text.Length < 3)
        {
            JoinButton.interactable = false;
            buttonText.alpha = 0;
        }
        else
        {
            JoinButton.interactable = true;
            buttonText.alpha = 1;
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room list updated");

        AviableRooms.Clear();

        foreach (RoomInfo room in roomList)
        {
            if (room.IsVisible && room.IsOpen && !room.RemovedFromList) AviableRooms.Add(room);
        }

        if (firstTime) RefreshRoomList();
        firstTime = false;
    }

    public async void RefreshRoomList()
    {
        foreach (var roomObj in instantedLists)
        {
            Destroy(roomObj);
        }

        instantedLists.Clear();

        for (int i = 0; i < AviableRooms.Count; i++)
        {
            instantedLists.Add(Instantiate(RoomListed, content));
            instantedLists[i].AddComponent<ListedRoom>();
            instantedLists[i].GetComponent<ListedRoom>().SetVariables(AviableRooms[i].Name, AviableRooms[i].PlayerCount, AviableRooms[i].MaxPlayers);

            await Task.Delay(2000 / AviableRooms.Count);
        }
    }
}