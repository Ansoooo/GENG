using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
public class objectMemory : MonoBehaviour // <- name of script
{
    private static playerControl _objectMemInstance;
    public static playerControl singletonGetInstance()
    {
        if (_objectMemInstance == null) //If no instance, make one.
        {
            _objectMemInstance = new playerControl();
        }
        return _objectMemInstance;
    }

    private int changeObjectTo = 1;
    private GameObject switcherPrefab;
    
    public GameObject Prefab;
    public GameObject Prefab2;
    public GameObject Prefab3;
    public GameObject[] instObjects;

    private int objectIndex;
    private int maxSpawnedObjects;
    public Vector3 position = new Vector3(0f, 0f, 0f);

    const string DLL_NAME = "objectMemory"; // <- name of plugin
    [DllImport(DLL_NAME)]
    private static extern int SimpleFunction();
    [DllImport(DLL_NAME)]
    private static extern void savePosi(float _x, float _y, float _z, int index);
    [DllImport(DLL_NAME)]
    private static extern void saveType(int _type, int index);
    [DllImport(DLL_NAME)]
    private static extern float getPosiX(int index);
    [DllImport(DLL_NAME)]
    private static extern float getPosiY(int index);
    [DllImport(DLL_NAME)]
    private static extern float getPosiZ(int index);
    [DllImport(DLL_NAME)]
    private static extern int getType(int index);
    
    //SPAWN OBJECTS
    void spawnObject()
    {
        //Switch to the user UI selected object.
        if (changeObjectTo == 1)
        {
            switcherPrefab = Prefab;
        }
        else if (changeObjectTo == 2)
        {
            switcherPrefab = Prefab2;
        }
        else if (changeObjectTo == 3)
        {
            switcherPrefab = Prefab3;
        }

        //Raycast method from https://answers.unity.com/questions/1397510/converting-mouse-position-to-worldpoint-in-3d.html
        //On right click cast ray from camera until hit, instantiate object at hit location. Save position and add to index for undo/redo functionality.
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast (ray, out hit))
            {
                maxSpawnedObjects = objectIndex;
                Vector3 position = new Vector3(hit.point.x - hit.point.x / 8, hit.point.y - hit.point.y / 8, hit.point.z - hit.point.z / 8);
                instObjects[objectIndex] = Instantiate(switcherPrefab, position, Quaternion.identity);
                savePosi(position.x, position.y, position.z, objectIndex);
                saveType(changeObjectTo, objectIndex);
                objectIndex += 1;
                maxSpawnedObjects += 1;
            }
        }
    }
    void undoRedo()
    {
        //Check that we have enough objects to undo, then delete most recent object.
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (objectIndex > 0 && maxSpawnedObjects - objectIndex != 20)
            {
                objectIndex -= 1;
                Destroy(instObjects[objectIndex]);
            }
        }
        //Check that we have spawned enough recent objects to redo, then instantiate current index object.
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objectIndex < maxSpawnedObjects)
            {
                Vector3 position = new Vector3(getPosiX(objectIndex), getPosiY(objectIndex), getPosiZ(objectIndex));

                if (getType(objectIndex) == 1)
                {
                    instObjects[objectIndex] = Instantiate(Prefab, position, Quaternion.identity);
                }
                else if (getType(objectIndex) == 2)
                {
                    instObjects[objectIndex] = Instantiate(Prefab2, position, Quaternion.identity);
                }
                else if (getType(objectIndex) == 3)
                {
                    instObjects[objectIndex] = Instantiate(Prefab3, position, Quaternion.identity);
                }
                objectIndex += 1;
            }
        }
    }

    //INITIALIZE ELEMENTS
    private void Start()
    {
        switcherPrefab = Prefab;
    }

    //UPDATE
    void Update()
    {
        spawnObject();
        undoRedo();
    }

    //UI FUNCTIONS
    public void switchObject(int index)
    {
        changeObjectTo = index;
    }
}