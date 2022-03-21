using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

public class DuckScript : MonoBehaviour
{

    //Where is the pool centre point.
    [SerializeField] private Transform poolCentre;

    //How large is the pool in world coordinate?
    [SerializeField] private float poolRadius = 1.0f;

    private Rigidbody rigid;

    //Record positions of the last point it travelled to, and what's next location.
    private Vector3 lastPos = Vector3.zero;
    private Vector3 nextPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = nextPos = transform.position;
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent.name.Equals("Water")) //Only do its AI, if it is parented to a transform named water.
        {
            if ((transform.position - nextPos).magnitude <= 0.05f) //Have the duck reached it position yet?
            {

                //Yes it has, record the nextPos as lastPos, and generate a new random point in the circle as its next location.
                lastPos = nextPos;
                float angle = Mathf.Deg2Rad * Random.Range(-180.0f, 180.0f);
                nextPos = poolCentre.position + new Vector3(0.0f, 0.2f, 0.0f) + (new Vector3(Mathf.Sin(angle), 0.0f, Mathf.Cos(angle)) * Random.Range(-poolRadius, poolRadius));
            }
            else
            {

                //Must find out how long they are on their travel from lastPos to nextPos, using dot product.
                float dot = Vector3.Dot(transform.position - lastPos, nextPos - lastPos);

                //Make them accelerate to midpoint, then after that slow down upon arrival.
                float speed = dot;
                if (dot > 0.5f)
                    speed = (1.0f - speed);

                //In case if dot product is zero (which it always is at the start and end positions.), make it so they still have a minimum of 5% speed.
                transform.position = Vector3.MoveTowards(transform.position, nextPos, Mathf.Max(speed * 0.5f, 0.05f) * Time.deltaTime);
                transform.rotation = Quaternion.LookRotation((nextPos - lastPos).normalized);
            }
        }
    }
}
