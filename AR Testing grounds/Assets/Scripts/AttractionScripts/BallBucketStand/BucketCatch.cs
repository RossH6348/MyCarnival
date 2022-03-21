using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Name: Ross Hutchins
//ID: HUT18001284

public class BucketCatch : MonoBehaviour
{

    //Keep track of score, and a reference to the text to display the store.
    private int score = 0;
    [SerializeField] private Text scoreboard;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {

        //An object have entered the trigger box.
        GameObject obj = other.gameObject;

        //Does this object have an interactable script attached, is it name is sphere? and has it stop moving.
        if(obj.GetComponent<Interactable>() != null && obj.name == "Sphere" && obj.GetComponent<Rigidbody>().velocity.magnitude <= 0.1f)
        {

            //Yes, go ahead and add a point to the scoreboard.
            score = score + 1;
            scoreboard.text = score.ToString();

            //Get the BallScript and respawn it.
            BallScript respawn = other.gameObject.GetComponent<BallScript>();
            respawn.ResetObject();
        }
    }
}
