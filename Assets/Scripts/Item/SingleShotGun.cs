using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SingleShotGun : Gun
{
 

    [SerializeField] Camera cam;
    PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void Use()
    {
        Debug.Log("[Weapon]Using " + itemInfo.name + " the type is  " + typeof(SingleShotGun)  );
        Shoot();
    }

    void Shoot()
    {
        //从摄像机的近剪裁面的中点向着远剪裁面的中点绘制一条射线
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("[Weapon]Ray cast hit " + hit.collider.gameObject.name);
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            PV.RPC(nameof(RPC_Shoot),RpcTarget.All, hit.point,hit.normal);
        } 
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition,Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        Debug.Log("[RPC][Combat]" + hitPosition);
        if (colliders.Length != 0)
        {
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.005f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj,10f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
    }

    public override void Reload()
    {
        //throw new System.NotImplementedException();
    }

    public override void DetermineAim()
    {
        
        //throw new System.NotImplementedException();
    }
}
