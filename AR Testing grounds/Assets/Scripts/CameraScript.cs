using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Name: Ross Hutchins
//ID: HUT18001284

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update

    private Camera camera;
    [SerializeField] Transform mountPoint; //empty GameObject to parent equipped item to.

    private GameObject equip = null; //What item the player have equipped?

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

        //Loop through all inputs unity returns (fingers touching the screen.)
        for(int i = 0; i < Input.touchCount; i++) {

            //First check if the player have an item equipped or not.
            if (mountPoint.childCount < 1)
            {
                //They don't have an item equipped, get the touch object from the current index.
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began)
                {
                    //Okay this touch phase happened to be began (just touched) fire a raycast from finger into the screen.
                    Ray ray = camera.ScreenPointToRay(touch.position);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 2.0f)) //Test raycast to see if they hit something, if so return the result.
                    {

                        //Attempt to get the interactable component from this object hitted by the raycast.
                        Interactable interact = hit.collider.gameObject.GetComponent<Interactable>();
                        if (interact != null)
                        {
                            //This object appears to have the interactable component, this means it is an item!
                            //Equip the item.
                            equip = hit.collider.gameObject;

                            //Pass this event along onto the interactable component through onPress.
                            interact.onPress(touch.position, i);
                        }
                    }
                }
            }
            else
            {

                //They got an item equipped, get our touch data.
                Touch touch = Input.GetTouch(i);

                //Just to be sure, check for if item got an interactable component.
                Interactable interact = equip.GetComponent<Interactable>();
                if (interact != null)
                {
                    //Yes they do, go ahead and pass along this event data depending on if they moved their finger, or released their finger.
                    if (touch.phase == TouchPhase.Ended)
                        interact.onRelease(touch.position,i); //They released their finger.
                    else if (touch.phase == TouchPhase.Moved)
                        interact.onDrag(touch.deltaPosition,i); //They moved their finger.
                }
            }
        }
    }
}
