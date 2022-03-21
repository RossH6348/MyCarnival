using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Name: Ross Hutchins
//ID: HUT18001284

public class BallScript : Interactable
{

    //currentDrag is to store the drag vector received from onDrag
    private Vector2 currentDrag = Vector2.zero;

    //This is to record its position it to respawn back to upon start up.
    private Vector3 spawnPoint = Vector3.zero;

    private Rigidbody rigid;
    private Collider collision;


    //This is to check if the sphere is equipped or not.
    [SerializeField] private Transform mountPoint;
    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = gameObject.transform.position;
        rigid = GetComponent<Rigidbody>();
        collision = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if it fallen off the map too far while not equipped, if so respawn it.
        if (transform.parent != mountPoint && transform.position.y <= -5.0f)
            ResetObject();
    }

    //This respawns the sphere back to its start with zero velocity.
    public void ResetObject()
    {
        transform.position = spawnPoint;
        rigid.velocity = rigid.angularVelocity = Vector3.zero;
    }

    //Inheriting from Interactable Script, I am overriding these three functions to handle how the ball will react to said inputs.
    public override void onPress(Vector3 position, int index)
    {
        if (index == 0)
        {
            //If the first finger press, equip it to the player and disable gravity & collision.
            rigid.velocity = rigid.angularVelocity = Vector3.zero;
            rigid.useGravity = collision.enabled = false;
            transform.SetParent(mountPoint);
            transform.localPosition = new Vector3(0.0f, -0.125f, 0.3f);
        }
        base.onPress(position, index);
    }

    public override void onDrag(Vector2 dragVector, int index)
    {
        if(index == 0)
        {
            //If the drag function is called by the same finger used to pick the ball up, store it for later use.
            currentDrag = dragVector;
        }
        base.onDrag(dragVector, index);
    }

    public override void onRelease(Vector3 position, int index)
    {
        if (index == 0)
        {
            //They finally released the finger holding the ball.
            rigid.useGravity = collision.enabled = true; //Re-enable gravity and collision on it.
            rigid.velocity = (mountPoint.forward + mountPoint.up).normalized * currentDrag.magnitude * 0.0125f; //Apply velocity based on recorded drag vector.
            transform.SetParent(null);
        }
        base.onRelease(position, index);
    }

}
