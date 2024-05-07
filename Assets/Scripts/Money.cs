using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    public float timeLimit;                 //Time before money is destroyed without Spidy touching it
    public int value;                       //Amount this object has in money value for Spidy

    //-------------------------------------------------------------------------------------------------
    //Update used to count when object is destroyed before Spidy gets the money
    void Update()
    {
        timeLimit -= Time.deltaTime;
        if (timeLimit <= 0)
        {
            Destroy(gameObject);
        }
    }
    //Update used to count when object is destroyed before Spidy gets the money
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Increase money for spidy when Spidy touches it
    void OnCollisionEnter(Collision collision)
    {
       if (collision.collider.tag == "Spidy")
        {
            StaticVar.money += value;
            Destroy(gameObject);
        }
    }
    //Increase money for spidy when Spidy touches it
    //-------------------------------------------------------------------------------------------------
}
