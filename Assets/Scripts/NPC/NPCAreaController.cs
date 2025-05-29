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
        
       /* else 
        {
            canvasGameobject.SetActive(false);
        }*/ 
       // NPC 거리 떨어졌을 때 대화창 사라지기 미구현
    }
}
