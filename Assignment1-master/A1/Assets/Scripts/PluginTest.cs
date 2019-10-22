using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
public class PluginTest : MonoBehaviour // <- name of script
{
    public GameObject Prefab;
    public GameObject Prefab2;
    public GameObject[] instObjects;
    public int objectIndex;
    public int maxSpawnedObjects;
    public Vector3 position = new Vector3(0f, 0f, 0f);

    const string DLL_NAME = "A1"; // <- name of plugin
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            //Debug.Log(SimpleFunction());
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast (ray, out hit))
            {
                maxSpawnedObjects = objectIndex;
                Vector3 position = new Vector3(hit.point.x - hit.point.x / 8, hit.point.y - hit.point.y / 8, hit.point.z - hit.point.z / 8);
                instObjects[objectIndex] = Instantiate(Prefab, position, Quaternion.identity);
                savePosi(position.x, position.y, position.z, objectIndex);
                saveType(1, objectIndex);
                objectIndex += 1;
                maxSpawnedObjects += 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                maxSpawnedObjects = objectIndex;
                Vector3 position = new Vector3(hit.point.x - hit.point.x / 8, hit.point.y - hit.point.y / 8, hit.point.z - hit.point.z / 8);
                instObjects[objectIndex] = Instantiate(Prefab2, position, Quaternion.identity);
                savePosi(position.x, position.y, position.z, objectIndex);
                saveType(2, objectIndex);
                objectIndex += 1;
                maxSpawnedObjects += 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (objectIndex > 0 && maxSpawnedObjects - objectIndex != 20)
            {
                objectIndex -= 1;
                Destroy(instObjects[objectIndex]);
            }
        }
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
                objectIndex += 1;
            }
        }

    }
}