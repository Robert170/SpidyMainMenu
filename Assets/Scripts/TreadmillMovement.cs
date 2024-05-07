using UnityEngine;
using UnityEngine.AI;

public class TreadmillMovement : MonoBehaviour
{
    public float push = 500.0f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NavMeshAgent>(out NavMeshAgent Agent))
        {
            if (!Agent.CompareTag("Spidy"))
            {
                Vector3 relativePosition = other.transform.position - transform.position;
                if (gameObject.tag == "Treadmill")
                {
                    if (Vector3.Dot(relativePosition, transform.right) > 0)
                    {
                        Agent.velocity = -transform.right * push;
                    }
                }
                if (gameObject.tag == "Treadmill2")
                {
                    if (Vector3.Dot(relativePosition, transform.right) < 0)
                    {
                        Agent.velocity = transform.right * push;
                    }
                }
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out NavMeshAgent Agent))
        {
            if (!Agent.CompareTag("Spidy"))
            {
                if (gameObject.tag == "Treadmill")
                {
                    other.GetComponent<Actor>().isOverTreadmill = true;
                }
                if (gameObject.tag == "Treadmill2")
                {
                    other.GetComponent<Actor>().isOverTreadmill2 = true;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out NavMeshAgent Agent))
        {
            if (!Agent.CompareTag("Spidy"))
            {
                other.GetComponent<Actor>().isOverTreadmill = false;
                other.GetComponent<Actor>().isOverTreadmill2 = false;
                other.GetComponent<Actor>().treadmillSpeed = 0;
            }
        }
    }
}
