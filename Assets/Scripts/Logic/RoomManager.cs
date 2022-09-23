using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.SceneManagement;
using System.IO;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public override void OnEnable() 
    {
        base.OnEnable(); 
        //Debug.Log("[PUN] OnEnable is called");
        //接受场景加载完成的事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void  OnDisable()
    {
        base.OnDisable(); 
        //Debug.Log("[PUN] OnDisable is called");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) 
    {
        //game Scene
        if (scene.buildIndex == 1)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", nameof(PlayerManager)), Vector3.zero, Quaternion.identity);
        }
    }
}
