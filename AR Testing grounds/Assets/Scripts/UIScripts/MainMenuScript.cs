using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Name: Ross Hutchins
//ID: HUT18001284

public class MainMenuScript : MonoBehaviour
{

    //This list holds all the submenus in Main Menu, this makes transitioning between them easier to pull off.
    [SerializeField] private List<GameObject> Menus = new List<GameObject>();

    //This boolean is to prevent activating buttons over and over during transitions to Construction Mode.
    private bool enableButtons = true;
    private GameObject currentMenu;

    //Reference to the ConstructionMode canvas.
    [SerializeField] private GameObject ConstructionMode;

    // Start is called before the first frame.
    void Start()
    {
        currentMenu = Menus[0];
    }

    private void OnEnable()
    {
        //If this canvas is re-activated, it will reset all the submenus and currentMenu.
        currentMenu = Menus[0];
        for (int i = Menus.Count - 1; i > -1; i--)
        {
            Menus[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            Vector3 moveTo = Vector3.zero;
            if (Menus[i] != currentMenu)
                moveTo.x = -340.0f;
            Menus[i].transform.localPosition = moveTo;
        }
        enableButtons = true;
    }

    // Update is called once per frame
    void Update()
    {

        //If buttons are enabled, handle transitions between other sub menus.
        if (enableButtons)
        {
            for (int i = Menus.Count - 1; i > -1; i--)
            {
                //Zero will cause the menu selected to move onscreen.
                Vector3 moveTo = Vector3.zero;
                //If the menu is not selected, it will move offscreen.
                if (Menus[i] != currentMenu)
                    moveTo.x = -340.0f;

                //Move the menu over time for a smooth transistion.
                Menus[i].transform.localPosition = Vector3.MoveTowards(Menus[i].transform.localPosition, moveTo, 20.0f);
            }
        }
        else
        {
            //Progressively make all the menus get smaller and smaller.
            for (int i = Menus.Count - 1; i > -1; i--)
                Menus[i].transform.localScale = Vector3.MoveTowards(Menus[i].transform.localScale, Vector3.zero, 10.0f * Time.deltaTime);

            //Once the menus are at scale 0 or lower, deactivate the Main Menu and activate Construction Mode.
            if(Menus[0].transform.localScale.magnitude <= Vector3.zero.magnitude)
            {
                gameObject.SetActive(false);
                ConstructionMode.SetActive(true);
            }
        }
    }

    //Clicking on play will disable buttons, which causes the transition to construction mode to play.
    public void onPlayClick()
    {
        if (enableButtons)
            enableButtons = false;
    }

    //Select help menu.
    public void onHelpClick()
    {
        if (enableButtons)
            currentMenu = Menus[1];
    }
    
    //Quit the game.
    public void onQuitClick()
    {
        if (enableButtons)
            Application.Quit();
    }

    //Return back to main menu.
    public void onBackClick()
    {
        if (enableButtons)
            currentMenu = Menus[0];
    }
}
