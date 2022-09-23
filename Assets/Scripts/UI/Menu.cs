using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
    [HideInInspector] public bool isOpen = false;

    public void Open()
    {
        Debug.Log("[UIManager]" + menuName + " Menu opened");
        gameObject.SetActive(true);
        isOpen = true;
    }

    public void Close() 
    {
        Debug.Log("[UIManager]" + menuName + " Menu closed");
        gameObject.SetActive(false);
        isOpen = false;
    }
}
