using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using DG.Tweening;

public class ScoreboardManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreboardItemPrefab;
    [SerializeField] CanvasGroup canvasGroup;

    Dictionary<Player,ScoreboardItem> scoreboardItems = new Dictionary<Player, ScoreboardItem> ();

    private void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddScoreboardItem(player);
        }
    }

    private void AddScoreboardItem(Player player)
    {
        ScoreboardItem item = Instantiate(scoreboardItemPrefab, container).GetComponent<ScoreboardItem>();
        scoreboardItems[player] = item;
        item.Initialize(player);
    }

    private void RemoveScoreboardItem(Player player)
    {
        Destroy(scoreboardItems[player].gameObject);
        scoreboardItems.Remove(player);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        RemoveScoreboardItem(newPlayer);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Tween tween = DOTween.To(()=>canvasGroup.alpha,x=>canvasGroup.alpha=x,1f,0.2f);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            Tween tween = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0f, 0.15f);
        }
    }
}


