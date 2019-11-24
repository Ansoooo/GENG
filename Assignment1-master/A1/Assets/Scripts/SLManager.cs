using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SLManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            SaveAndLoad.Save("SaveData/save.gsave", transform.position.x.ToString() + "," + transform.position.y.ToString() + "," + transform.position.z.ToString());
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            string[] content = new string[0];
            SaveAndLoad.Load("SaveData/save.gsave", ref content);
            transform.position = new Vector3(float.Parse(content[0]), float.Parse(content[1]), float.Parse(content[2]));
        }
    }
}
