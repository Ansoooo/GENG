using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Factory method from https://www.youtube.com/watch?v=n3YAuCsIWqA 

public interface typeSpawner
{
    void process(Transform a, Transform b);
}
public class lemming : typeSpawner
{
    public lemming()
    {
    }

    public void process(Transform a, Transform b)
    {
        a.position = Vector3.MoveTowards(a.position, b.position, 20 * Time.deltaTime);
    }

}
public class jumper : typeSpawner
{
    public jumper()
    {

    }

    public void process(Transform a, Transform b)
    {
        a.Translate(0f, 30 * Time.deltaTime, 0f);
    }
}
public class spawnerFactory
{
    public typeSpawner GetType(string type)
    {
        switch (type)
        {
            case ("lemming"):
                return new lemming();
            case ("jumper"):
                return new jumper();
            default:
                return new lemming();
        }
    }
}

public class sphereMovement : MonoBehaviour
{
    public GameObject player;

    public List<typeSpawner> spawner = new List<typeSpawner>();
    spawnerFactory sFactory = new spawnerFactory();

    public int typeSwitcher = 1;

    private void Awake()
    {
        spawner.Add(sFactory.GetType("lemming"));
        spawner.Add(sFactory.GetType("jumper"));
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            typeSwitcher = 1;
        }
        else if(Input.GetKey(KeyCode.Alpha2))
        {
            typeSwitcher = 2;
        }
        else if(Input.GetKey(KeyCode.Alpha3))
        {
            typeSwitcher = 3;
        }


        if (typeSwitcher == 1)
        {
            spawner[0].process(transform, player.transform);
        }
        else if (typeSwitcher == 2)
        {
            spawner[1].process(transform, player.transform);
        }
    }
}

