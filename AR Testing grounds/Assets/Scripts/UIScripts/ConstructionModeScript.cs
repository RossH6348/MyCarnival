using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

//Name: Ross Hutchins
//ID: HUT18001284

//I have followed this tutorial to help figure out and solve my requirements of needing to track surfaces
//so the player can build their attractions and props upon them: https://www.youtube.com/watch?v=Ml2UakwRxjk&t=1430s
//I would also like to thank to the commenter: David Grinbaum, for pointing out that ARFoundation have changed where Raycast function is accessed.


public class ConstructionModeScript : MonoBehaviour
{

    //Reference to the arSession gameobject, for tracking.
    [SerializeField] private Transform arSession;

    //Reference to main menu, incase we need to go back.
    [SerializeField] private GameObject MainMenu;

    //The two Placement and Destruction HUDs.
    [SerializeField] private GameObject PlacementHUD;
    [SerializeField] private GameObject DestructionHUD;

    //Using the AR Camera, and the new class: ARRaycastManager for tracking raycast.
    [SerializeField] private Camera camera;
    [SerializeField] private ARRaycastManager arCast;

    //Three variables to keep track of what the user have selected, the preview of the preab in the world, and target prop to destroy.
    private GameObject selectedBuilding = null;
    private GameObject previewPrefab = null;
    private GameObject victimBuilding = null;

    //Going to cache the Attraction layermask into this, for later filtering.
    private LayerMask attractionMask;

    public void Start()
    {
        attractionMask = LayerMask.GetMask("Attraction");
    }

    public void OnEnable()
    {
        //Clear all objects within the ar session, to start a new scratch funfair.
        for (int i = arSession.childCount - 1; i > -1; i--)
            Destroy(arSession.GetChild(i).gameObject);
    }

    public void Update()
    {

        //Test to see if the player is looking at an existing attraction and if so, prompt them the option to destroy?
        victimBuilding = null;

        //Fire a raycast from centre of screen.
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 999.0f, attractionMask))
        {
            //Get the gameobject of the collider.
            victimBuilding = hit.collider.gameObject;

            //We may have hit a collider of an attraction, but it might not be the root parent.
            //Keep looping until either we don't have a parent, or the parent isn't in Attraction layer.
            while (true)
            {
                if (victimBuilding.transform.parent == null)
                    break;
                else if (LayerMask.LayerToName(victimBuilding.transform.parent.gameObject.layer).Equals("Attraction"))
                    victimBuilding = victimBuilding.transform.parent.gameObject;
                else
                    break;
            }
        }

        //Change which HUD to display, based on whether the player is looking at an existing attraction or not.
        PlacementHUD.SetActive(victimBuilding == null);
        DestructionHUD.SetActive(victimBuilding != null);

        //Check if the player isn't looking at an existing attraction and have selected a building.
        if(victimBuilding == null && selectedBuilding != null)
        {
            //Check if they got a preview prefab?
            if(previewPrefab == null)
            {
                //No, so create our preview prefab for the player! As well so the construction mode can display it.
                previewPrefab = Instantiate(selectedBuilding.transform.GetChild(0).gameObject, arSession);
                previewPrefab.transform.localPosition = Vector3.zero;
                previewPrefab.transform.localScale = new Vector3(0.6667f, 0.6667f, 0.6667f);
            }

            //Time to raycast any potential surfaces, if so display our model.
            List<ARRaycastHit> results = new List<ARRaycastHit>();

            //Cache the result and change the active state of the preview based on if it founds a valid surface.
            bool surfaceFound = arCast.Raycast(ray, results);
            previewPrefab.SetActive(surfaceFound);
            if (surfaceFound)
            {
                //Extract the first pose it found.
                Pose pose = results[0].pose;

                //Rotate the model based on the direction the player is looking, without any upside down nonsense.
                Vector3 cameraForward = camera.transform.forward;
                cameraForward = new Vector3(cameraForward.x, 0.0f, cameraForward.z).normalized;
                pose.rotation = Quaternion.LookRotation(cameraForward);

                //Move the preview to the surface facing the player.
                previewPrefab.transform.SetPositionAndRotation(pose.position, pose.rotation);
            }

        }
        else if (previewPrefab != null)
        {
            //Remove the preview prefab if the player have one while looking at an existing attraciton.
            Destroy(previewPrefab);
            previewPrefab = null;
        }

    }

    //Functions for handling all the controls.

    //This sends the player back to MainMenu.
    public void onReturnClick()
    {
        MainMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    //When player selects a building, change it size in the UI while keeping a record of which button it was.
    public void onSelectBuilding(GameObject building)
    {
        if (selectedBuilding != null)
            selectedBuilding.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        if (previewPrefab != null)
            Destroy(previewPrefab);

        selectedBuilding = building;
        building.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    //Get the child of the button selected, which contains the prefab and place it within arSession gameobject.
    public void onPlaceClick()
    {
        if(previewPrefab != null && previewPrefab.activeSelf)
        {
            Transform realPlacement = previewPrefab.transform.GetChild(0);
            realPlacement.SetParent(arSession);
            realPlacement.gameObject.SetActive(true);
            Destroy(previewPrefab);
        }
    }

    //Simply deletes the victim attraction the player is looking at.
    public void onDestroyClick()
    {
        if (victimBuilding != null)
            Destroy(victimBuilding);
    }

    //This ends construction mode, and transitions to playmode.
    public void onPlayClick()
    {
        //The player decided to begin playing in their funfair, close down construction mode and enable the camera control script.
        CameraScript controls = camera.GetComponent<CameraScript>();
        if (controls != null && !controls.isActiveAndEnabled)
        {
            if (previewPrefab != null)
            {
                Destroy(previewPrefab);
                previewPrefab = null;
            }
            controls.enabled = true;
            gameObject.SetActive(false);
        }
    }
}
