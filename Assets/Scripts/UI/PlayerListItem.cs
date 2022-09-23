using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon;
using TMPro;
using Photon.Pun;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text text;
    Player player;

    public void SetUp(Player _player)
    {
        player = _player;
        text.text = _player.NickName;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("[Network]" + otherPlayer.NickName + " disconnected, in room: " + PhotonNetwork.NickName);
        if (player == otherPlayer) 
        {
            Destroy(gameObject); 
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
