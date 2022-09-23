using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

//区分C#内置的Hashtable与Photon提供的Hashtable
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] UnityEngine.UI.Image healthbarImage;
    [SerializeField] GameObject ui;

    [SerializeField] GameObject cameraHolder;

    [SerializeField] float mouseSensitivity;
    [SerializeField] float sprintSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float smoothTime;

    [SerializeField] Item[] items;

    int itemIndex;
    int previousItemIndex = -1;

    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    Rigidbody rb;

    PhotonView PV;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    PlayerManager playerManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            EquipItem(0);
        }
        else 
        {
            //Destroy other's camera in scene
            Destroy(GetComponentInChildren<Camera>().gameObject);
            //Destroy other's rigidbody so that there is no drag
            Destroy(rb);
            Destroy(ui);
        }
    }

    private void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        Look();

        Move();

        Jump();

        //Switch weapon by keyboard 
        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i+1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }
         

        //Switch weapon by scrollwheel 
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }

        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (itemIndex <= 0)
            {
                EquipItem(items.Length -1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            items[itemIndex].Use();
        }

        if (transform.position.y < -10)
        {
            Debug.Log("[Player]Die because of fall");
            Die();
        }
    }

    void Look() 
    {
        //控制左右旋转视角
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        //控制上下旋转视角
        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    private void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void Jump() 
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            Debug.Log("[Player]Jump is called");
            rb.AddForce(transform.up * jumpForce);
        }
    }

    void EquipItem(int _index)
    {
        //保证不会重复掏出武器
        if (_index == previousItemIndex)
            return;

        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true);

        //当前已装备了武器，需要将上一个关闭上一个武器
        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;

        //Ensure that we only our syncing data from local player
        //当我们装备一个武器时，首先查看是否是localPlayer，若是，将把这个切换到的武器index发出去
        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    //射线检测在一些几何边缘等效果并不理想
    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);

    }

    //此处的Hashtable的值是Object，需要自己cast为所需类型
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        //本地已经切换了武器，无需再次同步 && 在客户端看，仅仅改变对应玩家的index，而不是所有玩家
        if (changedProps.ContainsKey("itemIndex")&&!PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);

        }
        //base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    /// <summary>
    /// This function will run on the shooter's computer, call from victim's object 
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
        Debug.Log("[Combat]" + "I(" + PhotonNetwork.NickName + ") Try to hurt " + PV.Owner.NickName + " at damage " + damage);

    }


    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info) 
    {
        Debug.Log("[RPC][Combat]" + PV.Owner.NickName +  " Take damage " + damage);
        currentHealth -= damage;

        healthbarImage.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Die();
            //找到发送信息者
            PlayerManager.Find(info.Sender).GetKill();
        }
    }

    void Die() 
    {
        playerManager.Die();
    }
}
