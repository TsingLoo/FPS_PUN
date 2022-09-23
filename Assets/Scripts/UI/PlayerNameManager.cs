using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEditor;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInput;

    private void Start()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            usernameInput.text = PlayerPrefs.GetString("username");
            PhotonNetwork.NickName = usernameInput.text;
        }
        else 
        {
            usernameInput.text = "Player " + Random.Range(0,1000).ToString("0000");
            OnUsernameInputValueChanged();
        }
    }

    public void OnUsernameInputValueChanged() 
    {
        PhotonNetwork.NickName = usernameInput.text;
        PlayerPrefs.SetString("username",usernameInput.text);
    }
}
