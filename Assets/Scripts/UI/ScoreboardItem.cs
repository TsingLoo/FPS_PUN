using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ScoreboardItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text usernameText;
    [SerializeField] TMP_Text killsText;
    [SerializeField] TMP_Text deathsText;

    Player player;

    public void Initialize(Player player) 
    {
        usernameText.text = player.NickName;
        this.player = player;
        UpdateStatus();
    }

    void UpdateStatus() 
    {
        if (player.CustomProperties.TryGetValue("kills", out object kills))
        {
            Debug.Log("[UIManager][Combat]The kill is now " + kills.ToString());
            killsText.text = kills.ToString();
        }
        if (player.CustomProperties.TryGetValue("deads", out object deads))
        {
            Debug.Log("[UIManager][Combat]The death is now " + deads.ToString());
            deathsText.text = deads.ToString();
        }
    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == player)
        {
            if (changedProps.ContainsKey("kills") || changedProps.ContainsKey("deads"))
            { 
                UpdateStatus();
            }
        }
    }
}
