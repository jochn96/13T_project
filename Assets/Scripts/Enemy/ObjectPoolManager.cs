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
    /// 모든 오브젝트 풀을 초기화합니다.
    /// poolDataList에 설정된 각 풀에 대해 미리 오브젝트들을 생성하여 Queue에 저장합니다.
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
    /// 지정된 풀에서 오브젝트를 가져와 활성화하고 위치를 설정합니다.
    /// </summary>
    /// <param name="poolName">사용할 풀의 이름</param>
    /// <param name="position">스폰될 위치</param>
    /// <param name="rotation">스폰될 회전값</param>
    /// <returns>활성화된 GameObject, 풀이 비어있거나 존재하지 않으면 null</returns>
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
    /// 사용이 끝난 오브젝트를 비활성화하고 풀로 반환합니다.
    /// </summary>
    /// <param name="poolName">반환할 풀의 이름</param>
    /// <param name="objectToReturn">풀로 반환할 GameObject</param>
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
    /// 지정된 풀의 모든 활성화된 오브젝트를 찾아서 풀로 반환합니다.
    /// 주로 게임 종료나 씬 전환 시 정리용으로 사용됩니다.
    /// </summary>
    /// <param name="poolName">모든 오브젝트를 반환할 풀의 이름</param>
    public void ReturnAllToPool(string poolName)
    {
        if (!poolDictionary.ContainsKey(poolName))
        {
            Debug.LogWarning($"Pool with name {poolName} doesn't exist.");
            return;
        }

        // 현재 활성화된 모든 오브젝트를 찾아서 풀로 반환
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
    /// 지정된 풀에서 현재 활성화되어 사용 중인 오브젝트의 개수를 반환합니다.
    /// </summary>
    /// <param name="poolName">확인할 풀의 이름</param>
    /// <returns>활성화된 오브젝트 개수</returns>
    public int GetActiveCount(string poolName)
    {
        if (!poolDictionary.ContainsKey(poolName))
            return 0;

        PoolData poolData = poolDataList.Find(data => data.poolName == poolName);
        if (poolData == null) return 0;

        return poolData.poolSize - poolDictionary[poolName].Count;
    }

    /// <summary>
    /// 지정된 풀의 전체 크기(최대 오브젝트 개수)를 반환합니다.
    /// </summary>
    /// <param name="poolName">확인할 풀의 이름</param>
    /// <returns>풀의 전체 크기, 풀이 존재하지 않으면 0</returns>
    public int GetPoolSize(string poolName)
    {
        PoolData poolData = poolDataList.Find(data => data.poolName == poolName);
        return poolData?.poolSize ?? 0;
    }

    /// <summary>
    /// 지정된 풀에서 사용 가능한(비활성화된) 오브젝트의 개수를 반환합니다.
    /// </summary>
    /// <param name="poolName">확인할 풀의 이름</param>
    /// <returns>사용 가능한 오브젝트 개수</returns>
    public int GetAvailableCount(string poolName)
    {
        if (!poolDictionary.ContainsKey(poolName))
            return 0;

        return poolDictionary[poolName].Count;
    }

    /// <summary>
    /// 지정된 풀이 존재하는지 확인합니다.
    /// </summary>
    /// <param name="poolName">확인할 풀의 이름</param>
    /// <returns>풀 존재 여부</returns>
    public bool PoolExists(string poolName)
    {
        return poolDictionary.ContainsKey(poolName);
    }

    /// <summary>
    /// 모든 풀의 이름 목록을 반환합니다.
    /// </summary>
    /// <returns>등록된 모든 풀 이름의 배열</returns>
    public string[] GetAllPoolNames()
    {
        string[] poolNames = new string[poolDictionary.Keys.Count];
        poolDictionary.Keys.CopyTo(poolNames, 0);
        return poolNames;
    }
}