using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttachText : MonoBehaviour
{
    public GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (parent != null)
        {
            gameObject.transform.position = Camera.main.WorldToScreenPoint(parent.transform.position);
            if (parent.tag.Equals("Enemy"))
            {
                gameObject.GetComponent<UnityEngine.UI.Text>().text = "Health: " + parent.GetComponent<enemyBehaviour>().GetHealth();
            }
        }
    }
}
