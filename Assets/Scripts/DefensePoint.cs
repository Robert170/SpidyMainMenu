using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensePoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider hitCollider)
    {
        if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Detector"))
        {

        }
    }

}
