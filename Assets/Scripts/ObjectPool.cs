using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    public Transform parent;
    public List<NetworkObject> bulletPrefabs;
    public int poolSize = 10;

    private Dictionary<string, Queue<NetworkObject>> poolDictionary;
    private NetworkRunner runner;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Thực hiện các hành động khác trong Start nếu cần
    }

    public void SetNetworkRunner(NetworkRunner networkRunner)
    {
        runner = networkRunner;
        InitObjectPools(); // Khởi tạo pool khi runner được gán
    }

    private void InitObjectPools()
    {
        if (runner == null)
        {
            Debug.LogError("NetworkRunner chưa được gán.");
            return;
        }

        poolDictionary = new Dictionary<string, Queue<NetworkObject>>();

        foreach (NetworkObject prefab in bulletPrefabs)
        {
            string tag = prefab.tag;
            Queue<NetworkObject> objectPool = new Queue<NetworkObject>();

            for (int i = 0; i < poolSize; i++)
            {
                NetworkObject netObj = runner.Spawn(prefab, Vector3.zero, Quaternion.identity, runner.LocalPlayer);
                netObj.transform.parent = parent;
                netObj.gameObject.SetActive(false);
                objectPool.Enqueue(netObj);
            }

            poolDictionary.Add(tag, objectPool);
        }
    }

    public NetworkObject Fire(string tag, Vector3 position, Quaternion rotation, NetworkRunner runner, PlayerRef owner)
    {
        if (runner == null || poolDictionary == null || !poolDictionary.ContainsKey(tag))
        {
            return null;
        }

        Queue<NetworkObject> pool = poolDictionary[tag];

        if (pool.Count > 0)
        {
            NetworkObject obj = pool.Dequeue();
            if (obj != null)
            {
                if (runner.IsServer)
                {
                    obj.transform.position = position;
                    obj.transform.rotation = rotation;
                    obj.gameObject.SetActive(true);
                    return obj;
                }
                // Return the object to the pool nếu không phải server
                pool.Enqueue(obj);
            }
        }
        return null;
    }

    public void ReturnPooledObject(string tag, NetworkObject obj)
    {
        if (poolDictionary.ContainsKey(tag))
        {
            obj.gameObject.SetActive(false);
            poolDictionary[tag].Enqueue(obj);
        }
    }
}
