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
        boss.SetActive(false);

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
        bossIntroCam.Priority = 100;
        player.GetComponent<NavMeshAgent>().isStopped = true;
        yield return new WaitForSeconds(1.5f);
        boss.SetActive(true);
        yield return new WaitForSeconds(2f);
        var impulse = boss.GetComponent<CinemachineImpulseSource>();
        if (impulse) impulse.GenerateImpulse();

        yield return new WaitForSeconds(introDuration);
        player.GetComponent<NavMeshAgent>().isStopped = false;  

        playerCam.Priority = 100;
        bossIntroCam.Priority = 5;
    }
}
