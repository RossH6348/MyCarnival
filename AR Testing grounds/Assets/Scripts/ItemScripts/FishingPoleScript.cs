using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Name: Ross Hutchins
//ID: HUT18001284

public class FishingPoleScript : Interactable
{

    [SerializeField] private Transform mountPoint;

    //The two transforms which happened to be the rigged bones.
    [SerializeField] private Transform stringBase;
    [SerializeField] private Transform hook;

    //Where to attach the duck to if caught.
    public Transform hookMount;

    //How far have they reeled, and far can they reel.
    private float lengthReeled = 0.0f;
    private float maxLengthReeled = 3.0f;

    //To record its starting position and rotation upon start.
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private Rigidbody rigid;
    private Collider collision;

    //Scoreboard nonsense.
    private int totalDucks = 0;
    [SerializeField] private Text scoreboard;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        collision = GetComponent<Collider>();

        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

        //Check if they fallen off the map and not attached to player.
        if(transform.parent != mountPoint && transform.position.y <= -5.0f)
        {

            //Okay happened to be the case, reset its position, rotation and velocities.
            rigid.velocity = rigid.angularVelocity = Vector3.zero;
            transform.position = spawnPosition;
            transform.rotation = spawnRotation;

            lengthReeled = 0.0f;
            stringBase.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
            hook.localPosition = Vector3.zero;

            //If there happened to be a duck attached to it, destroy it and don't count score.
            if (hookMount.childCount > 0)
                Destroy(hookMount.GetChild(0).gameObject);

        }
        else if(transform.parent == mountPoint)
        {

            //It is attached to the player, make the hook of the fishing pole always face downward, as if it was affected by gravity.
            stringBase.rotation = Quaternion.FromToRotation(stringBase.up, -Vector3.up) * stringBase.rotation;

            //Move the hook down or up based on how much the user reeled.
            hook.localPosition = new Vector3(0.0f, 0.01f * lengthReeled, 0.0f);

            //Does the fishing pole have a duck attached to it?
            if (hookMount.childCount < 1)
            {
                //No it doesn't, attempt to find one by checking all ducks (gameObjects with Ducks as its tag)
                GameObject[] ducks = GameObject.FindGameObjectsWithTag("Ducks");
                foreach(GameObject duck in ducks){
                    if((duck.transform.position - hookMount.position).magnitude <= 0.1f)
                    {
                        //It is close enough to the hook, attach it and stop here.
                        duck.transform.SetParent(hookMount);
                        duck.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                        duck.transform.localRotation = Quaternion.identity;
                        break;
                    }
                }
            }
            else if(lengthReeled <= 0.1f)
            {
                //The player have reeled it in close enough to catch the duck and increase score.
                totalDucks++;
                scoreboard.text = totalDucks.ToString();
                Destroy(hookMount.GetChild(0).gameObject);
            }
        }

    }

    //Inheriting the interactable script and overriding the functions to do the unique controls.
    public override void onPress(Vector3 position, int index)
    {
        if (index == 0)
        {
            //Checking if the player have touched it, if so equip it.
            rigid.velocity = rigid.angularVelocity = Vector3.zero;
            rigid.useGravity = collision.enabled = false;
            transform.SetParent(mountPoint);
            transform.localPosition = new Vector3(0.07f, 0.0f, 0.125f);
            transform.localRotation = Quaternion.Euler(-60.0f, 0.0f, 180.0f);
        }
        base.onPress(position, index);
    }

    public override void onDrag(Vector2 dragVector, int index)
    {
        if(index == 1) //Are they attempting to reel with another finger?
        {
            //Produce dot product to figure out which way the player is dragging.
            //(Up the phone for reeling in, down the phone to unreel.)
            float dot = Vector2.Dot(dragVector.normalized, Vector2.up);

            //Start adding length or removing length from the string.
            lengthReeled = Mathf.Min(Mathf.Max(0.0f, lengthReeled + dragVector.magnitude * Time.deltaTime * -0.02f * dot), maxLengthReeled);
        }
        base.onDrag(dragVector, index);
    }

    public override void onRelease(Vector3 position, int index)
    {
        if(index == 0)
        {
            //Simply let go of it.
            rigid.useGravity = collision.enabled = true;
            transform.SetParent(null);
        }
        base.onRelease(position, index);
    }

}
