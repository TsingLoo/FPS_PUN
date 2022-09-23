using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] PhotonView playerPV;
    [SerializeField] TMPro.TMP_Text text;

    private void Start()
    {
        if (playerPV.IsMine)
        {
            gameObject.SetActive(false);
        }
        text.text = playerPV.Owner.NickName;
    }
}
