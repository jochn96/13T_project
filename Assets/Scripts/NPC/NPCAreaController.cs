using UnityEngine;

public class NPCAreaController : MonoBehaviour
{
    public string ChatText = "";
    public GameObject canvasGameobject; 
    
    private void OnTriggerEnter ( Collider other )
    {
        if (other.gameObject.CompareTag("Player")) // 플레이어와 충돌
        {
            canvasGameobject.SetActive(true);
        }
        
        
    }
}
