using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

//The entirety of this script is to allow overriding these three functions in different scripts as an inheritance.
//That way the camera can just look for the "Interactable" class type, even though it may be an entirely different script/component, such as BallScript.
public class Interactable : MonoBehaviour
{

    //This is called when the item receives a press from a new finger.
    public virtual void onPress(Vector3 position,int index)
    {

    }

    //This is called when the item receives a drag from an existing finger.
    public virtual void onDrag(Vector2 dragVector, int index)
    {

    }

    //This is called when the item receives a release from a finger lifting off the screen.
    public virtual void onRelease(Vector3 position, int index)
    {

    }

}
