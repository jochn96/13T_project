using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Resource : MonoBehaviour
{
    static Resource resource;
    public ItemData itemToGive;
    public int quiiantityPerHit = 1;
    public int capacy;
    

    private void Start()
    {
        resource = GetComponent<Resource>();
    }
    public void Gather(Vector3 hitPoint, Vector3 hitNomal)
    {
        for (int i = 0; i < quiiantityPerHit; i++)
        {
            capacy -= 1;
            Instantiate(itemToGive.dropPrefab, hitPoint + Vector3.up, Quaternion.LookRotation(hitNomal, Vector3.up));
            if (capacy <= 0)
            {
                Destroy(gameObject);
                break;
            }
        }
    }
}
