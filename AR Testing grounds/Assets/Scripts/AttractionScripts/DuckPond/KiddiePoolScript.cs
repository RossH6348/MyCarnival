using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

public class KiddiePoolScript : MonoBehaviour
{

    //How many ducks can exist in the pond.
    [SerializeField] private int maxDuckCount = 10;

    //How large is the pond in world coordinates?
    [SerializeField] private float poolRadius = 1.0f;

    //Prefab template for a duck, and the water plane to parent to.
    [SerializeField] private Transform Water;
    [SerializeField] private GameObject duckTemplate;

    // Start is called before the first frame update
    void Start()
    {
        //Begin a new coroutine loop that executes every 3 seconds.
        StartCoroutine(SpawnDucks());
    }

    //A coroutine loop function.
    IEnumerator SpawnDucks()
    {
        yield return new WaitForSeconds(3);

        if(Water.childCount < maxDuckCount)
        {
            //Spawn in a new duck if there is less than the max allowed.
            GameObject duck = Instantiate(duckTemplate, Water);

            //Pick a random point in a plane circle to spawn the duck.
            float angle = Mathf.Deg2Rad * Random.Range(-180.0f, 180.0f);

            duck.transform.localPosition = new Vector3(0.0f, 0.2f, 0.0f) + (new Vector3(Mathf.Sin(angle), 0.0f, Mathf.Cos(angle)) * Random.Range(-poolRadius, poolRadius));

            duck.SetActive(true);
        }

        //Begin the coroutine loop again.
        StartCoroutine(SpawnDucks());
    }
}
