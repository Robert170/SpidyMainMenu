using UnityEngine;

public class DestroyOnComplete : MonoBehaviour
{
    //-------------------------------------------------------------------------------------------------
    //Code that will be executed when the animation of the associated sprite is over
    public void DidComplete()
    {
        Destroy(gameObject);
    }
    //Code that will be executed when the animation of the associated sprite is over
    //-------------------------------------------------------------------------------------------------
}
