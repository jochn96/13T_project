using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolData
{
    public string poolName;
    public GameObject prefab;
    public int poolSize;
    public Transform parent;
}

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [SerializeField] private List<PoolData> poolDataList = new List<PoolData>();
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ��� ������Ʈ Ǯ�� �ʱ�ȭ�մϴ�.
    /// poolDataList�� ������ �� Ǯ�� ���� �̸� ������Ʈ���� �����Ͽ� Queue�� �����մϴ�.
    /// </summary>
    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (PoolData poolData in poolDataList)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < poolData.poolSize; i++)
            {
                GameObject obj = Instantiate(poolData.prefab);
                obj.SetActive(false);

                if (poolData.parent != null)
                    obj.transform.SetParent(poolData.parent);
                else
                    obj.transform.SetParent(transform);

                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(poolData.poolName, objectPool);
        }
    }

    /// <summary>
    /// ������ Ǯ���� ������Ʈ�� ������ Ȱ��ȭ�ϰ� ��ġ�� �����մϴ�.
    /// </summary>
    /// <param name="poolName">����� Ǯ�� �̸�</param>
    /// <param name="position">������ ��ġ</param>
    /// <param name="rotation">������ ȸ����</param>
    /// <returns>Ȱ��ȭ�� GameObject, Ǯ�� ����ְų� �������� ������ null</returns>
    public GameObject SpawnFromPool(string poolName, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(poolName))
        {
            Debug.LogWarning($"Pool with name {poolName} doesn't exist.");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[poolName];

        if (pool.Count == 0)
        {
            Debug.LogWarning($"Pool {poolName} is empty!");
            return null;
        }

        GameObject objectToSpawn = pool.Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        return objectToSpawn;
    }

    /// <summary>
    /// ����� ���� ������Ʈ�� ��Ȱ��ȭ�ϰ� Ǯ�� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="poolName">��ȯ�� Ǯ�� �̸�</param>
    /// <param name="objectToReturn">Ǯ�� ��ȯ�� GameObject</param>
    public void ReturnToPool(string poolName, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(poolName))
        {
            Debug.LogWarning($"Pool with name {poolName} doesn't exist.");
            return;
        }

        objectToReturn.SetActive(false);
        poolDictionary[poolName].Enqueue(objectToReturn);
    }

    /// <summary>
    /// ������ Ǯ�� ��� Ȱ��ȭ�� ������Ʈ�� ã�Ƽ� Ǯ�� ��ȯ�մϴ�.
    /// �ַ� ���� ���ᳪ �� ��ȯ �� ���������� ���˴ϴ�.
    /// </summary>
    /// <param name="poolName">��� ������Ʈ�� ��ȯ�� Ǯ�� �̸�</param>
    public void ReturnAllToPool(string poolName)
    {
        if (!poolDictionary.ContainsKey(poolName))
        {
            Debug.LogWarning($"Pool with name {poolName} doesn't exist.");
            return;
        }

        // ���� Ȱ��ȭ�� ��� ������Ʈ�� ã�Ƽ� Ǯ�� ��ȯ
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        PoolData poolData = poolDataList.Find(data => data.poolName == poolName);

        if (poolData != null)
        {
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.Contains(poolData.prefab.name) && obj.activeInHierarchy)
                {
                    ReturnToPool(poolName, obj);
                }
            }
        }
    }

    /// <summary>
    /// ������ Ǯ���� ���� Ȱ��ȭ�Ǿ� ��� ���� ������Ʈ�� ������ ��ȯ�մϴ�.
    /// </summary>
    /// <param name="poolName">Ȯ���� Ǯ�� �̸�</param>
    /// <returns>Ȱ��ȭ�� ������Ʈ ����</returns>
    public int GetActiveCount(string poolName)
    {
        if (!poolDictionary.ContainsKey(poolName))
            return 0;

        PoolData poolData = poolDataList.Find(data => data.poolName == poolName);
        if (poolData == null) return 0;

        return poolData.poolSize - poolDictionary[poolName].Count;
    }

    /// <summary>
    /// ������ Ǯ�� ��ü ũ��(�ִ� ������Ʈ ����)�� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="poolName">Ȯ���� Ǯ�� �̸�</param>
    /// <returns>Ǯ�� ��ü ũ��, Ǯ�� �������� ������ 0</returns>
    public int GetPoolSize(string poolName)
    {
        PoolData poolData = poolDataList.Find(data => data.poolName == poolName);
        return poolData?.poolSize ?? 0;
    }

    /// <summary>
    /// ������ Ǯ���� ��� ������(��Ȱ��ȭ��) ������Ʈ�� ������ ��ȯ�մϴ�.
    /// </summary>
    /// <param name="poolName">Ȯ���� Ǯ�� �̸�</param>
    /// <returns>��� ������ ������Ʈ ����</returns>
    public int GetAvailableCount(string poolName)
    {
        if (!poolDictionary.ContainsKey(poolName))
            return 0;

        return poolDictionary[poolName].Count;
    }

    /// <summary>
    /// ������ Ǯ�� �����ϴ��� Ȯ���մϴ�.
    /// </summary>
    /// <param name="poolName">Ȯ���� Ǯ�� �̸�</param>
    /// <returns>Ǯ ���� ����</returns>
    public bool PoolExists(string poolName)
    {
        return poolDictionary.ContainsKey(poolName);
    }

    /// <summary>
    /// ��� Ǯ�� �̸� ����� ��ȯ�մϴ�.
    /// </summary>
    /// <returns>��ϵ� ��� Ǯ �̸��� �迭</returns>
    public string[] GetAllPoolNames()
    {
        string[] poolNames = new string[poolDictionary.Keys.Count];
        poolDictionary.Keys.CopyTo(poolNames, 0);
        return poolNames;
    }
}