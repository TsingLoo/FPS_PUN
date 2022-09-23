using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    public RoomInfo info;

    public void SetUp(RoomInfo roomInfo)
    {
        text.text = roomInfo.Name;
        info = roomInfo;
    }

    public void OnClick() 
    {
        Launcher.Instance.JoinRoom(info);
    }
}
