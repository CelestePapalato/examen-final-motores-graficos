using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    static public SpawnManager instance;
    [SerializeField]
    GameObject[] objects;
    [SerializeField]
    string[] names;

    Dictionary<string, GameObject> spawnableObjects = new Dictionary<string, GameObject>();

    void Start()
    {
        instance = this;
        createDictionary();
    }

    void createDictionary()
    {
        int index = 0;
        foreach(string name in names)
        {
            spawnableObjects.Add(name, objects[index]);
            index++;
        }
    }

    public GameObject spawnObject(string key, Vector3 position)
    {
        if (spawnableObjects.ContainsKey(key))
        {
            GameObject obj = spawnableObjects[key];
            return Instantiate(obj, position, Quaternion.identity);
        }
        return null;
    }

    public GameObject spawnObjectAndParent(string key, Transform parent)
    {
        if (spawnableObjects.ContainsKey(key))
        {
            GameObject obj = spawnableObjects[key];
            return Instantiate(obj, parent);
        }
        return null;
    }

    public GameObject spawnAccordingToTransform(string key, Transform caller, Vector3 positionOffset)
    {
        if (spawnableObjects.ContainsKey(key))
        {
            GameObject obj = spawnableObjects[key];
            return Instantiate(obj, caller.position + positionOffset, caller.rotation);
        }
        return null;
    }
}
