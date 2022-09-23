using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    GameObject controller;

    int kills;
    int deads;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        //如果此PhotonView的所有者是本地玩家
        if (PV.IsMine)
        { 
            CreateController();
        }
    }

    void CreateController() 
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        Debug.Log("[Player]Instantiated Player Controller: " + PhotonNetwork.NickName);
        controller = PhotonNetwork.Instantiate(System.IO.Path.Combine("PhotonPrefabs",nameof(PlayerController)),spawnpoint.position, spawnpoint.rotation, 0, new object[] {PV.ViewID});
    }

    public void Die() 
    {
        Debug.Log("[Combat][Player]I " + "(" + PV.Owner.NickName + ") Died");
        PhotonNetwork.Destroy(controller);
        CreateController();

        deads++;

        Hashtable hash = new Hashtable(0);
        hash.Add("deads", deads);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }



    public void GetKill() 
    {
        PV.RPC(nameof(RPC_GetKill), PV.Owner);
    }

    [PunRPC]
    void RPC_GetKill() 
    {
        Debug.Log("[RPC][Combat]I " + "(" + PV.Owner.NickName + ") Get a kill");
        kills++;

        Hashtable hash = new Hashtable(0);
        hash.Add("kills",kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }
}
