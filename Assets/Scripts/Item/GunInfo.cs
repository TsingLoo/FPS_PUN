using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New Gun", order = 1)]
public class GunInfo : ItemInfo
{
    public float damage;
    public float fireRate;

    public int clipSize;

    public Vector3 normalLocationPosition;
    public Vector3 aimingLocalPosition;

    //Hipfire Recoil
    [Header("垂直后坐力最大高度 负为向上弹跳")]
    public float recoilX;

    [Header("水平后坐力弹跳范围")]
    public float recoilY;

    [Header("前后弹跳")]
    public float recoilZ;

    [Header("弹跳力度，越大手感越硬")]
    public float snappiness;

    [Header("回正速度")]
    public float returnSpeed;
}
