using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject : PoolObject
{
    TrailRenderer trail;
    float trailTime;

    private void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        trailTime = trail.time;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * 25);
    }

    public override void OnObjectReuse()
    {
        trail.time = -1;
        Invoke("ResetTrail", .1f);
    }

    void ResetTrail()
    {
        trail.time = trailTime;
    }

}

