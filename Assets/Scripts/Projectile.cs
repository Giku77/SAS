using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float lifeTime = 5f;
    public LayerMask hitMask;
    Rigidbody rb;
    Transform owner;
    public int damage = 10;

    void Awake() => rb = GetComponent<Rigidbody>();

    private void OnEnable()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void LaunchTo(Vector3 start, Vector3 target, float flightTime, Transform owner = null)
    {
        this.owner = owner;
        transform.position = start;

        Vector3 g = Physics.gravity;              
        Vector3 to = target - start;
        Vector3 toXZ = Vector3.ProjectOnPlane(to, Vector3.up);

        Vector3 vXZ = toXZ / flightTime;                         
        float vY = (to.y - 0.5f * g.y * flightTime * flightTime) / flightTime;  
        Vector3 v0 = vXZ + Vector3.up * vY;

        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.linearVelocity = v0;

        transform.rotation = Quaternion.LookRotation(v0);

        CancelInvoke();
        Invoke(nameof(Despawn), lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Projectile hit {other.name}");
        if (owner && other.transform.root == owner.root) return;
        Despawn();
        if (((1 << other.gameObject.layer) & hitMask) != 0)
        {
            Debug.Log($"Projectile hit {other.name}");

            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.OnDamage(damage);
            }
        }
    }

    void Despawn() => gameObject.SetActive(false);
    //void Despawn() => Destroy(gameObject);
}
 