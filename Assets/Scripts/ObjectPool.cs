using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    public List<NetworkObject> bulletPrefabs;
    public int poolSize = 10;

    private Dictionary<string, Queue<NetworkObject>> poolDictionary;

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
        InitObjectPools();
    }

    private void InitObjectPools()
    {
        poolDictionary = new Dictionary<string, Queue<NetworkObject>>();

        foreach (NetworkObject prefab in bulletPrefabs)
        {
            string tag = prefab.tag;
            Queue<NetworkObject> objectPool = new Queue<NetworkObject>();

            for (int i = 0; i < poolSize; i++)
            {
                NetworkObject netObj = Instantiate(prefab, transform);
                netObj.gameObject.SetActive(false);
                objectPool.Enqueue(netObj);
            }

            poolDictionary.Add(tag, objectPool);
        }
    }

    public NetworkObject Fire(string tag, Vector3 position, Quaternion rotation, NetworkRunner runner, PlayerRef owner)
    {
        if (poolDictionary.ContainsKey(tag))
        {
            Queue<NetworkObject> pool = poolDictionary[tag];

            if (pool.Count > 0)
            {
                NetworkObject obj = pool.Dequeue();
                if (obj != null)
                {
                    // Đảm bảo rằng chỉ server mới spawn đối tượng mạng
                    if (runner.IsServer)
                    {
                        runner.Spawn(obj, position, rotation, owner);
                        obj.gameObject.SetActive(true);
                        return obj;
                    }
                }
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
