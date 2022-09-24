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
    [Header("��ֱ���������߶� ��Ϊ���ϵ���")]
    public float recoilX;

    [Header("ˮƽ������������Χ")]
    public float recoilY;

    [Header("ǰ����")]
    public float recoilZ;

    [Header("�������ȣ�Խ���ָ�ԽӲ")]
    public float snappiness;

    [Header("�����ٶ�")]
    public float returnSpeed;
}
