using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMenuManager : MonoBehaviour {

    public static bool showCursor = false;
    public static bool inMenu = false;


	public static bool showTooltip = false;
	public static string tooltipText = "";

	public UILabel tooltipLabel;





    public List<Menu> menus = new List<Menu>();

    public MenuType menuType;
    public enum MenuType { Single, Multiple}


    public CursorDisplay cursorDisplay;
    public enum CursorDisplay { AlwaysShow, ShowInWindow }



	// Use this for initialization
	void Start () 
    {
        if (cursorDisplay == CursorDisplay.ShowInWindow)
            ToggleCursor();
        else
        {
            Screen.showCursor = true;
            Screen.lockCursor = false;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {

		if (showTooltip)
		{
			tooltipLabel.gameObject.SetActive(true);
			tooltipLabel.text = tooltipText;
            tooltipLabel.transform.localPosition = new Vector3(Input.mousePosition.x - Screen.width / 2 + 20, Input.mousePosition.y - Screen.height / 2, 0f);
		}else{
			tooltipLabel.gameObject.SetActive(false);
		}

        // Check if the player is pressing any of the menu buttons
        foreach (Menu menu in menus)
        {
            if (Input.GetButtonUp(menu.toggleKey))
            {
                showTooltip = false;

                // Hide other windows if only one is allowed
                if (menuType == MenuType.Single)
                {
                    foreach (Menu disableMenu in menus)
                    {
                        if (disableMenu == menu)
                            continue;
                        disableMenu.window.SetActive(false);
                        disableMenu.open = false;
                    }
                }

                menu.open = !menu.open;
                menu.window.SetActive(menu.open);

            }
        }

        if (cursorDisplay == CursorDisplay.ShowInWindow)
            ToggleCursor();
        else
        {
            Screen.showCursor = true;
            Screen.lockCursor = false;
        }

        inMenu = false;
        foreach (Menu menu in menus)
            if (menu.open) inMenu = true;

	}

    public void ToggleCursor()
    {
        showCursor = true;
        Screen.showCursor = true;
        Screen.lockCursor = false;
        tooltipLabel.gameObject.SetActive(showCursor);


        foreach (Menu menuCheck in menus)
            if (menuCheck.open)
                return;

        showCursor = false;
        Screen.showCursor = false;
        Screen.lockCursor = true;
    }

    public static bool GetShowCursor()
    {
        return showCursor;
    }

}

[System.Serializable]
public class Menu
{
    
    public bool open = false;
    public string toggleKey = "";
    
    public GameObject window;

    public Menu(string toggleKey, GameObject window)
    {
        this.toggleKey = toggleKey;
        this.window = window;
    }

}
