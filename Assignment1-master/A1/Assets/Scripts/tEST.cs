using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tEST : MonoBehaviour
{
    public Rigidbody x;
    public float spd = 500.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Rigidbody xx = Instantiate(x, transform.position, Quaternion.identity)as Rigidbody;
            xx.AddForce(Vector3.right * spd);
        }
    }
}
