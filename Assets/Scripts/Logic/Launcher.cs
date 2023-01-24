using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;
using System.Linq;
using System;
using Photon.Pun;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNamePUN;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameBtn;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        MenuManager.Instance.OpenMenu("Loading");
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        Debug.Log("[Network]Trying to create the room");
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void StartGame()
    {
        GC.Collect();
        PhotonNetwork.LoadLevel(1);
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #region Override Functions
    public override void OnConnectedToMaster()
    {
        Debug.Log("[Network]Connected to Master");
        PhotonNetwork.JoinLobby();
        //同步场景
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("[Network]Joined Lobby");
        MenuManager.Instance.OpenMenu("Title");
     
        //生成诸如Player 2423的四位随机名称
        //PhotonNetwork.NickName = "Player " + UnityEngine.Random.Range(0, 1000).ToString("0000");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("Room");
        roomNamePUN.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform trans in playerListContent)
        {
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        //使得按钮只对创建客户端可见
        startGameBtn.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameBtn.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        Debug.Log("[Network]Room Creation Failed: " + message);
        MenuManager.Instance.OpenMenu("Error");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("Title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //PhotonNetwork.countOfRooms != PhotonNetwork.GetRoomList();
        Debug.Log("[Network]Room List is Updated: " + roomList.Count() + " in total");
         
        //先清理已有的房间列表
        foreach (Transform item in roomListContent)
        {
            Destroy(item.gameObject);
        }


        for (int i = 0; i < roomList.Count(); i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("[Network]" + newPlayer.NickName + " connected, in room: " + PhotonNetwork.NickName);
        Instantiate(playerListItemPrefab,playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
    #endregion
}
