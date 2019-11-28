using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadPlayer : MonoBehaviour
{
    public float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 0)
        {
            timer = 0;
        }
        else
        {
            timer -= Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            SaveAndLoad.Save("SaveData/save.gsave", transform.position.x.ToString() + "," + transform.position.y.ToString() + "," + transform.position.z.ToString());
        }

        if (Input.GetKeyDown(KeyCode.X) && timer <= 0)
        {
            timer = 5.0f;
            string[] content = new string[0];
            SaveAndLoad.Load("SaveData/save.gsave", ref content);
            transform.position = new Vector3(float.Parse(content[0]), float.Parse(content[1]), float.Parse(content[2]));
        }
    }
}
