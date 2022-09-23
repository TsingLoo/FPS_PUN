using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item
{
    [SerializeField] protected GameObject cameraHolder;

    public abstract override void Use();
    public abstract void Reload();

    public abstract void DetermineAim();

    public GameObject bulletImpactPrefab;
}
