using UnityEngine;


public class MenuManager : SingletonForMonobehaviour<MenuManager>
{
    [SerializeField] Menu[] menus;

    private void Awake()
    {
        foreach(Menu menu in menus)
        {
            if (!menu.isOpen)
            {
                CloseMenu(menu);
            }
        }
    }

    public void OpenMenu(string menuName) 
    {
        for (int i = 0; i < menus.Length; i++) 
        {
            if (menus[i].menuName == menuName)
            {
                menus[i].Open();
            }
            else if (menus[i].isOpen) 
            {
                CloseMenu(menus[i]);
            }
        }
    }

    public void OpenMenu(Menu menu) 
    {
        //ֻ��ʾһ���˵�ҳ��
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].isOpen)
            {
                CloseMenu(menus[i]);
            }
        }
        menu.Open();
    }

    public void CloseMenu(Menu menu) 
    {
        menu.Close();
    }

}
