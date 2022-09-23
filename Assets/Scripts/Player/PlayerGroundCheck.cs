using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other == playerController.gameObject)
        {
            return;
        }
        playerController.SetGroundedState(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == playerController.gameObject)
        {
            return;
        }

        playerController.SetGroundedState(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other == playerController.gameObject)
        {
            return;
        }

        playerController.SetGroundedState(true);
    }
}
