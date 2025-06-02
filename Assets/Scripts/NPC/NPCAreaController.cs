using UnityEngine;

public class NPCAreaController : MonoBehaviour
{
    public string ChatText = "";
    public GameObject canvasGameobject;

    private void Start()
    {
        canvasGameobject.SetActive(false);
    }

    private void OnTriggerEnter ( Collider other )
    {
        if (other.gameObject.CompareTag("Player")) // 플레이어와 충돌
        {
            canvasGameobject.SetActive(true);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canvasGameobject.SetActive(false); // 플레이어와 거리가 멀어질 시 대화창 삭제
        }
    }
}
