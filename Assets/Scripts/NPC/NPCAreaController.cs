using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NPCAreaController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("DialogTag");

            if (canvas == null)
            {
                return;
            }

            Transform transform = canvas.transform;
            GameObject panel = GameObject.FindGameObjectWithTag("DialogTag");
            if (panel == null)
            {
                return;
            }
            panel.SetActive(true); // tag answer input
        }
    }
}
