using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    [SerializeField]
    GameObject bullet;
    Spidy spidyRef;
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Spidy"))
        {
            spidyRef = collision.gameObject.GetComponent<Spidy>();
            if (!spidyRef.isKnockedOut)
            {
                Destroy(bullet);
            }
        }
    }
}
