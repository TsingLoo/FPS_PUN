using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class GunController : Gun
{
    [SerializeField] PlayerController playerController;

    public GameObject GunPrefab;

    [Header("Gun Settings")]
    public float fireRate = 0.1f;
    public int reservedAmmoCapacity = 270;
   
    GunInfo gunInfo;

    //Variables 
    bool _canShoot;
    int _currentAmmoInClip;
    int _ammoInReserve;
    int clipSize;

    //Muzzle Flash
    public Image muzzleFlashImage;
    public Sprite[] flashes;

    //Aimming
    //public Vector3 normalLocationPosition;
    //public Vector3 aimingLocalPosition;

    //Weapon Recoil

    [SerializeField] Recoil recoil;
    public bool randomizeRecold;
    public Vector2 randomRecoilConstraints;


    public Vector2 recoilPattern;

    public float aimSmoothing = 10f;

    public override void Use() 
    {
        if (_canShoot && _currentAmmoInClip > 0)
        {
            _canShoot = false;
            _currentAmmoInClip--;
            recoil.RecoilFire();
            playerController.RefreshAmmoUI(_currentAmmoInClip,_ammoInReserve);
            StartCoroutine(ShootGun());
        }
    }

    public override void Reload()
    {
        if (_currentAmmoInClip < clipSize && _ammoInReserve > 0)
        {
            int amountNeeded = clipSize - _currentAmmoInClip;
            if (amountNeeded >= _ammoInReserve)
            {
                _currentAmmoInClip += _ammoInReserve;
                _ammoInReserve -= amountNeeded;
            }
            else 
            {
                _currentAmmoInClip = clipSize;
                _ammoInReserve -= amountNeeded;
            }
        }

        //throw new System.NotImplementedException();
    }

    public override void DetermineAim()
    {
        if (gunInfo == null) return;
        Vector3 target = gunInfo.normalLocationPosition;
      
        if(Input.GetMouseButton(1))
        target = gunInfo.aimingLocalPosition;

        Vector3 desiredPosition = Vector3.Lerp(GunPrefab.transform.localPosition,target,Time.deltaTime*aimSmoothing);

        GunPrefab.transform.localPosition = desiredPosition;
    }

    IEnumerator ShootGun() 
    {
        //DeterminRecoil();
        StartCoroutine(MuzzleFlash());
        yield return new WaitForSeconds(fireRate);
        _canShoot = true;
    }

    IEnumerator MuzzleFlash() 
    {
        muzzleFlashImage.sprite = flashes[Random.Range(0, flashes.Length)];
        muzzleFlashImage.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        muzzleFlashImage.sprite = null;
        muzzleFlashImage.color = new Color(0,0,0,0);
    }

    void DeterminRecoil() 
    {
        transform.localPosition -= Vector3.forward * 0.08f;

        if (randomizeRecold)
        {
            float xRecoil = Random.Range(-randomRecoilConstraints.x,randomRecoilConstraints.y);
            float yRecoil = Random.Range(-randomRecoilConstraints.y,randomRecoilConstraints.y);

            Vector2 recoil = new Vector2(xRecoil,yRecoil);
        }

        //cameraHolder.transform.localEulerAngles = Vector3.left * recoilPattern.y;
    }


    // Start is called before the first frame update
    void Start()
    {

        gunInfo = (GunInfo)itemInfo;

        clipSize = gunInfo.clipSize;
        _currentAmmoInClip = clipSize;
        _ammoInReserve = reservedAmmoCapacity;
        _canShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
