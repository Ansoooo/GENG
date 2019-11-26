using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject : PoolObject
{
    TrailRenderer trail;
    float trailTime;
    public bool direction = true;

    private void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        trailTime = trail.time;
        Physics.IgnoreCollision(gameObject.GetComponentInChildren<Collider>(), GameObject.Find("Player").GetComponent<Collider>(), true);
    }

    // Update is called once per frame
    void Update()
    {
        if (direction)
            transform.Translate(Vector3.left * Time.deltaTime * 50);
        else
            transform.Translate(Vector3.right * Time.deltaTime * 50);
    }

    public override void OnObjectReuse()
    {
        direction = GameObject.Find("TestRocket").GetComponent<TestManager>().direction;
        trail.time = -1;
        Invoke("ResetTrail", .1f);
    }

    void ResetTrail()
    {
        trail.time = trailTime;
    }

}

