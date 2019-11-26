using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fmodSoundController : MonoBehaviour
{

    // Start is called before the first frame update
    private void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/punch");
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/RocketLaunch");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Jump");
        }
    }
}
