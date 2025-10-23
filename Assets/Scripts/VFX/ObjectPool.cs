using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public interface IPoolable
{
    // Ǯ���� ���� �� ���� �ʱ�ȭ
    void OnPoppedFromPool();
    // Ǯ�� ��ȯ�� �� ���� ����
    void OnPushedToPool();
}
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int prewarm = 0;
    [SerializeField] private bool dontDestroyOnLoad = true;

    private readonly Queue<GameObject> q = new();
    private static readonly Dictionary<GameObject, ObjectPool> pools = new();

    public static ObjectPool GetOrCreate(GameObject prefab)
    {
        if (!pools.TryGetValue(prefab, out var pool) || pool == null)
        {
            var go = new GameObject($"Pool_{prefab.name}");
            pool = go.AddComponent<ObjectPool>();
            pool.prefab = prefab;
            // ������ ��ü�� ��Ȱ��ȭ�صθ� Instantiate�� ��Ȱ�� �ν��Ͻ�
            pool.prefab.SetActive(false);
            pools[prefab] = pool;

            if (pool.dontDestroyOnLoad) DontDestroyOnLoad(go);
            if (pool.prewarm > 0) pool.Prewarm(pool.prewarm);
        }
        return pool;
    }

    private void Prewarm(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(prefab, transform);
            obj.name = $"{prefab.name}_Pooled_{i}";
            q.Enqueue(obj); // ��Ȱ�� ���·� ť�� ����
        }
    }

    public GameObject Pop(Vector3 pos, Quaternion rot)
    {
        GameObject obj = q.Count > 0 ? q.Dequeue() : Instantiate(prefab, transform);

        // 1) ��ġ/ȸ�� ���� (Ȱ��ȭ ����)
        obj.transform.SetPositionAndRotation(pos, rot);

        // 2) ���� �ܻ� ����(����)
        if (obj.TryGetComponent<Rigidbody>(out var rb))
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector3.zero;
#else
            rb.velocity = Vector3.zero;
#endif
            rb.angularVelocity = Vector3.zero;
        }

        // Ȱ��ȭ
        if (!obj.activeSelf) obj.SetActive(true);

        // Ǯ �� (Ȱ��ȭ�� ���¸� ����ϴ� �ʱ�ȭ�� ���⼭ ����)
        if (obj.TryGetComponent<IPoolable>(out var poolable))
            poolable.OnPoppedFromPool();

        return obj;
    }

    public void Push(GameObject obj)
    {
        if (!obj || !obj.scene.IsValid()) return;

        // 1) Ǯ ��
        if (obj.TryGetComponent<IPoolable>(out var poolable))
            poolable.OnPushedToPool();

        // 2) ���� ����(����)
        if (obj.TryGetComponent<Rigidbody>(out var rb))
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector3.zero;
#else
            rb.velocity = Vector3.zero;
#endif
            rb.angularVelocity = Vector3.zero;
        }

        // 3) ��Ȱ�� + �θ� �̵� + ť�� ����
        if (obj.activeSelf) obj.SetActive(false);
        obj.transform.SetParent(transform, false);
        q.Enqueue(obj);
    }
}