
using UnityEngine;

public class Resource : MonoBehaviour
{
    //public ItemData itemToGive;  //주는 아이템
    //public Resource resource;
    
    public ResourceType type; // 돌인지, 나무인지 구분
    public int amountPerHit = 1; //타격수
    public int capacity;  //채취할수 있는 갯수
    
    public void Gather(Vector3 hitPoint, Vector3 hitNomal)
    {
        ResourceManager.Instance.AddResource(type, amountPerHit);

        for (int i = 0; i < amountPerHit; i++)
        {
            
           capacity -= 1;
        
          if(capacity <= 0)
                 {
                Destroy(gameObject);  //게임오브젝트는 파괴
                 }
            
        }
    }
}

