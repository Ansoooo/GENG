using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public GameObject prefab;
    public bool direction = true;
    public bool ShootButton = false;
    public void HIDControlPunchD()
    {
        ShootButton = true;
    }
    public void HIDControlPunchU()
    {
        ShootButton = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        PoolManager.instance.CreatePool(prefab, 3);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) || ShootButton == true)
        {        
            PoolManager.instance.ReuseObject(prefab, new Vector3(GameObject.Find("Player").transform.position.x, GameObject.Find("Player").transform.position.y, 0), Quaternion.identity);

            TutorialText accessTutorialText = GameObject.Find("TutorialText").GetComponent<TutorialText>();
            if (accessTutorialText.stageNum == 2.2f)
            { accessTutorialText.updateText(accessTutorialText.stageNum, 2.2f); }
        }
    }
}
