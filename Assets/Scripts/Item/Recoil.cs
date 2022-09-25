using UnityEditor;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    //Rotations
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    //Hipfire Recoil
    private float recoilX;
    private float recoilY;
    private float recoilZ;

    //Settings
    private float snappiness;
    private float returnSpeed;

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        //Slerp is good for rotation or direction
        currentRotation = Vector3.Slerp(currentRotation,targetRotation,snappiness*Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);

    }

    public void RecoilFire() 
    {
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }

    public void SetRecoil(GunInfo info) 
    {
        recoilX = info.recoilX;
        recoilY = info.recoilY;
        recoilZ = info.recoilZ;

        snappiness = info.snappiness;
        returnSpeed = info.snappiness;

        Debug.Log("[Weapon] Recoil is set as " + recoilX + " , " + recoilY + " , " + recoilZ + " , " + snappiness + " , " + returnSpeed);
    }

}
