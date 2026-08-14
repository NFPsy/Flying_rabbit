using UnityEngine;

public class Marble : MonoBehaviour
{
    [SerializeField] private float destroyForceThreshold = 3.5f;

    private void Start()
    {
        GameManager.Instance?.RegisterMarble();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude >= destroyForceThreshold)
        {
            GameManager.Instance?.OnMarbleDestroyed();
            Destroy(gameObject);
        }
    }
}
