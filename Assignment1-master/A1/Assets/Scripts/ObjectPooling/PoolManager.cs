using UnityEngine;
using System.Collections;
using System.Collections.Generic;

    public class ObjectInstance
    {

        GameObject gameObject;
        Transform transform;

        bool hasPoolObjectComponent;
        PoolObject poolObjectScript;

        public ObjectInstance(GameObject objectInstance)
        //Creates an instance of the game object to be pooled
        {
            gameObject = objectInstance;
            transform = gameObject.transform;
            gameObject.SetActive(false);

            if (gameObject.GetComponent<PoolObject>())
            {
                hasPoolObjectComponent = true;
                poolObjectScript = gameObject.GetComponent<PoolObject>();
            }
        }

        public void Reuse(Vector3 position, Quaternion rotation)
        //Allows objects belonging to the pool to be reused
        {
            gameObject.SetActive(true);
            transform.position = position;
            transform.rotation = rotation;

            if (hasPoolObjectComponent)
            {
                poolObjectScript.OnObjectReuse();
            }
        }

        public void SetParent(Transform parent)
        {
            transform.parent = parent;
        }
    }

//The pool manager controls the functions of the pool.
//It specifies how many objects need to be pooled and what to do with them upon emptying of the pool.
public class PoolManager : MonoBehaviour
{

    Dictionary<int, Queue<ObjectInstance>> poolDictionary = new Dictionary<int, Queue<ObjectInstance>>();

    static PoolManager _instance;

    public static PoolManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PoolManager>();
            }
            return _instance;
        }
    }

    public void CreatePool(GameObject prefab, int poolSize)
        //Creates an instance of the pool
    {
        int poolRef = prefab.GetInstanceID();

        if (!poolDictionary.ContainsKey(poolRef))
        {
            poolDictionary.Add(poolRef, new Queue<ObjectInstance>());

            GameObject poolContainer = new GameObject(prefab.name + " pool");
            poolContainer.transform.parent = transform;

            for (int i = 0; i < poolSize; i++)
            {
                ObjectInstance newObject = new ObjectInstance(Instantiate(prefab) as GameObject);
                poolDictionary[poolRef].Enqueue(newObject);
                newObject.SetParent(poolContainer.transform);
            }
        }
    }

    public void ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolRef = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolRef))
        {
            ObjectInstance objectToReuse = poolDictionary[poolRef].Dequeue();
            poolDictionary[poolRef].Enqueue(objectToReuse);

            objectToReuse.Reuse(position, rotation);
        }
    }

}
