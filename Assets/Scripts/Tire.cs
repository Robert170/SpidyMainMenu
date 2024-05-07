using System.Collections;
using UnityEngine;

public class Tire : MonoBehaviour
{
    private Spidy spidy;
    public Animator baseAnim;

    private void Start()
    {
        StartCoroutine(FindSpidy());

    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector2 direction = collision.GetContact(0).normal;
        if (direction.y == -1)
        {
            spidy.tireForce = 2;
            baseAnim.SetBool("OnTop", true);

        }
    }

    private void OnCollisionExit(Collision collision)
    {
            spidy.tireForce = 1;
            baseAnim.SetBool("OnTop", false);

    }

    private IEnumerator FindSpidy()
    {
        yield return null;
        spidy = FindObjectOfType<Spidy>();

    }
}
