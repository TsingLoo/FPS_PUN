using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

//����C#���õ�Hashtable��Photon�ṩ��Hashtable
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] UnityEngine.UI.Image healthbarImage;
    [SerializeField] GameObject ui;

    [SerializeField] TMP_Text currentAmmo;
    [SerializeField] TMP_Text reversedAmmo;

   
    [SerializeField] GameObject cameraHolder;

    [SerializeField] float mouseSensitivity;
    [SerializeField] float weaponSwayAmount;
    [SerializeField] float sprintSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float smoothTime;

    [SerializeField] Item[] items;

    [SerializeField] Recoil recoil;

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

        SwitchGun();

        DetermineAim();

        if (Input.GetMouseButton(0))
        {
            items[itemIndex].Use();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ((Gun)items[itemIndex]).Reload();
        }

        if (transform.position.y < -10)
        {
            Debug.Log("[Player]Die because of fall");
            Die();
        }
    }

    void Look() 
    {
        Vector2 mouseAxis = new Vector2(Input.GetAxisRaw("Mouse X"),Input.GetAxisRaw("Mouse Y")) ;

        //����������ת�ӽ�
        transform.Rotate(Vector3.up * mouseAxis.x * mouseSensitivity);

        if (!((Gun)items[itemIndex]).gameObject.TryGetComponent<GunController>(out GunController gunController)) return;

        ((Gun)items[itemIndex]).gameObject.GetComponent<GunController>()
            .GunPrefab.transform.localPosition += (Vector3)mouseAxis*weaponSwayAmount/1000;

        //����������ת�ӽ�
        verticalLookRotation += mouseAxis.y * mouseSensitivity;
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

    void SwitchGun() 
    {
        //Switch weapon by keyboard 
        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
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
                EquipItem(items.Length - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }
    }

    void DetermineAim() 
    {
        ((Gun)items[itemIndex]).DetermineAim();
    }
    void EquipItem(int _index)
    {
        //��֤�����ظ��ͳ�����
        if (_index == previousItemIndex)
            return;

        itemIndex = _index;
        recoil.SetRecoil((GunInfo)items[itemIndex].itemInfo);
        items[itemIndex].itemGameObject.SetActive(true);

        //��ǰ��װ������������Ҫ����һ���ر���һ������
        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;

        //Ensure that we only our syncing data from local player
        //������װ��һ������ʱ�����Ȳ鿴�Ƿ���localPlayer�����ǣ���������л���������index����ȥ
        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    //���߼����һЩ���α�Ե��Ч����������
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

    //�˴���Hashtable��ֵ��Object����Ҫ�Լ�castΪ��������
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        //�����Ѿ��л��������������ٴ�ͬ�� && �ڿͻ��˿��������ı��Ӧ��ҵ�index���������������
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
            //�ҵ�������Ϣ��
            PlayerManager.Find(info.Sender).GetKill();
        }
    }

    public void RefreshAmmoUI(int curr, int reversed)
    {
        currentAmmo.text = curr.ToString();
        reversedAmmo.text = reversed.ToString();
    
    }

    void Die() 
    {
        playerManager.Die();
    }
}
