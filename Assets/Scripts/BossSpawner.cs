using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class BossSpawner : MonoBehaviour
{
    [Header("Refs")]
    public GameObject boss;
    public CinemachineCamera playerCam;   
    public CinemachineCamera bossIntroCam; 
    public AudioClip bossBGM; 

    public float triggerDist = 20f;
    public float introDuration = 3f;     // 보스 비추는 시간

    GameObject player;
    bool played;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");

        playerCam.Priority = 10;
        bossIntroCam.Priority = 5;
    }

    void Update()
    {
        if (played || player == null) return;

        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist <= triggerDist)
        {
            StartCoroutine(PlayBossIntro());
            played = true;
            AudioManager.I.PlayBGM(bossBGM, 2f);
        }
    }

    public void Stomp() {}

    System.Collections.IEnumerator PlayBossIntro()
    {
        player.GetComponent<Player>().isGodMode = true;
        bossIntroCam.Priority = 100;
        player.GetComponent<NavMeshAgent>().isStopped = true;
        yield return new WaitForSeconds(2f);
        var vfxManager = FindFirstObjectByType<VfxManager>();
        if (boss) boss.SetActive(true);
        yield return new WaitForSeconds(2f);
        if (boss)
        {
            var impulse = boss.GetComponent<CinemachineImpulseSource>();
            if (impulse) impulse.GenerateImpulse();
        }
        vfxManager.PlayBossVfx();

        yield return new WaitForSeconds(introDuration);
        player.GetComponent<NavMeshAgent>().isStopped = false;
        player.GetComponent<Player>().isGodMode = false;

        playerCam.Priority = 100;
        bossIntroCam.Priority = 5;
    }
}
