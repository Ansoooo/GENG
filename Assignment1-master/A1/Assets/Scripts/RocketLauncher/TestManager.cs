using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        PoolManager.instance.CreatePool(prefab, 3);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            PoolManager.instance.ReuseObject(prefab, Vector3.zero, Quaternion.identity);
        }
    }
}
