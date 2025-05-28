using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void OnTriggerEnter  (Collider other)
    {
        if (other.gameObject.CompareTag("Player 01"))
        {
            GameObject canvas =  GameObject.FindGameObjectWithTag("DialogTag");
            if (canvas == null)
            {
                return;
            }
            
            Transform transform = other.gameObject.transform;
            GameObject canvas2 = GameObject.FindGameObjectWithTag("DialogTag");

            if (canvas2 == null)
            {
                return;
            }
            canvas.SetActive(true);
        }
    }
}
