using UnityEngine;

public class RabbitDespawn : MonoBehaviour
{
    [SerializeField] private float despawnDelay = 2f;

    private bool grounded;

    private void OnCollisionEnter(Collision collision)
    {
        if (grounded)
            return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
            Invoke(nameof(DespawnSelf), despawnDelay);
        }
    }

    private void DespawnSelf()
    {
        GameManager.Instance?.OnRabbitDestroyed();
        Destroy(gameObject);
    }
}
