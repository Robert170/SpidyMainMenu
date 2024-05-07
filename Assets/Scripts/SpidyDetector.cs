using UnityEngine;

//-------------------------------------------------------------------------------------------------
//Requires a collider component to overlap with Spidy
[RequireComponent(typeof(Collider))]
public class SpidyDetector : MonoBehaviour
{

    public bool spidyIsNearby;                          //Boolean that detects if Spidy touches collider

    //-------------------------------------------------------------------------------------------------
    //Collider that sets to true spidyIsNearby by checking the tag of the character being nearby
    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Spidy"))
        {
            spidyIsNearby = true;
        }
    }
    //Collider that sets to true spidyIsNearby by checking the tag of the character being nearby
    //-------------------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------------------
    //Collider that sets to false spidyIsNearby by checking the tag of the character being nearby
    public void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Spidy"))
        {
            spidyIsNearby = false;
        }
    }
    //Collider that sets to false spidyIsNearby by checking the tag of the character being nearby
    //-------------------------------------------------------------------------------------------------
}
//Requires a collider component to overlap with Spidy
//-------------------------------------------------------------------------------------------------
