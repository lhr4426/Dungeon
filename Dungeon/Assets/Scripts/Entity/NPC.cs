using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Wandering,
    Attacking
}


public class NPC : MonoBehaviour, IDamagable
{
    [Header("Stats")]
    public int health;
    public float walkSpeed;
    public float runSpeed;
    public ItemData[] dropOnDeath; // 죽었을 때 드랍할 아이템들


    [Header("AI")]
    private NavMeshAgent agent;
    public float detectDistance; // 목표 지점까지의 거리
    private AIState aiState;

    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime; // 최소 대기 시간
    public float maxWanderWaitTime;

    [Header("Combat")]
    public int damage;
    public float attackRate;
    private float lastAttackTime;
    public float attackDistance;

    private float playerDistance;
    public float fieldOfView = 120f; // 공격 가능한 시야각

    private Animator animator;
    private SkinnedMeshRenderer[] meshRenders;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        meshRenders = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    void Start()
    {
        SetState(AIState.Wandering);
    }


    void Update()
    {
        // 플레이어와 NPC간의 거리 계산
        playerDistance = Vector3.Distance(PlayerManager.Player.transform.position, transform.position);

        // State가 멈춤 상태가 아니면 Moving 애니메이션 재생
        animator.SetBool("Moving", aiState != AIState.Idle);

        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Wandering:
                PassiveUpdate();
                break;
            case AIState.Attacking:
                AttackingUpdate();
                break;
        }
    }

    public void SetState(AIState state)
    {
        aiState = state;

        switch (aiState)
        {
            case AIState.Idle:
                agent.speed = walkSpeed;
                agent.isStopped = true; // agent 멈추기
                break;
            case AIState.Wandering:
                agent.speed = walkSpeed;
                agent.isStopped = false;
                break;
            case AIState.Attacking:
                agent.speed = runSpeed;
                agent.isStopped = false;

                break;
        }

        animator.speed = agent.speed / walkSpeed;
    }

    void PassiveUpdate()
    {
        /// 나의 예상
        /*
        minWanderDistance와 maxWanderDistance 사이의 랜덤한 위치를 생성
        그 위치로 NavMeshAgent 이동시킴
        만약에 도착했으면 minWanderWaitTime <-> maxWanderWaitTime 사이의 랜덤한 시간 동안 대기
        그러고 다시 랜덤 위치 만들어서 이동?
        */
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle); // 대기 상태로 전환.
            // 어느정도 맞는듯?
            // 현재 상태가 방황함 + NavMeshAgent가 목표 지점까지 남은 거리가 거의 없을때
            // 랜덤 시간 뒤에 새 위치 찾는 함수 호출이니까.. 비슷한듯
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
        }

        if (playerDistance < detectDistance)
        {
            SetState(AIState.Attacking);
            AttackingUpdate();
        }
    }

    void WanderToNewLocation()
    {
        if (aiState != AIState.Idle) return;
        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation());
    }

    Vector3 GetWanderLocation()
    {
        NavMeshHit hit;

        // Vector3 NavMesh.SamplePosition(Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int areaMask) 
        // Vector3 sourcePosition : 일정 영역 지정
        // out NavMeshHit hit : 최단 경로에 관련된 값 반환
        // float maxDistance : 최대 거리
        // int areaMask : 탐색할 영역의 마스크

        // Random.onUnitSphere : 반지름이 1인 구
        // NPC의 현재 위치 + 반지름이 minWanderDistance와 maxWanderDistance인 구 사이의 랜덤한 값만큼 이동한 위치 


        // 원본
        /*
        NavMesh.SamplePosition(
            transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), 
            out hit, 
            maxWanderDistance, 
            NavMesh.AllAreas);

        int i = 0;

        // detectDistance 보다는 먼 거리로 이동하고자 함
        while(Vector3.Distance(transform.position, hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(
            transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)),
            out hit,
            maxWanderDistance,
            NavMesh.AllAreas);
            i++;
            if (i == 30) break; 
        }
        */


        // Do While 문으로 변경한 버전
        int i = 0;

        do
        {
            NavMesh.SamplePosition(
            transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)),
            out hit,
            maxWanderDistance,
            NavMesh.AllAreas);
            i++;
        } while (Vector3.Distance(transform.position, hit.position) > detectDistance || i <= 30);

        return hit.position;
    }

    void AttackingUpdate()
    {
        /// 나의 예상
        /*
        만약에 플레이어와의 거리가 attackDistance보다 멀면 플레이어 쪽으로 이동
        아니라면 플레이어를 공격 시도 (시야각 내로 들어왔다는 전제 하에)
        */


        // 플레이어와의 거리가 공격 범위 내에 있고, 시야각 내부에 있을 때
        if (playerDistance < attackDistance && IsPlayerInFieldOfView())
        {
            agent.isStopped = true;
            if (Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;
                PlayerManager.Player.controller.GetComponent<IDamagable>().TakeDamage(damage);
                animator.speed = 1f;
                animator.SetTrigger("Attack");
            }
        }
        else // 아닐 때
        {
            // 그럼에도 플레이어가 공격 범위 내에 있을 때
            if (playerDistance < detectDistance)
            {
                // 플레이어에게 가는 새로운 길을 또 만들어서 가려는 시도를 함
                agent.isStopped = false;
                NavMeshPath path = new NavMeshPath();
                // 플레이어한테 갈 수 있으면 감
                // NavMeshAgent.CalculatePath(Vector3 targetPosition, NavMeshPath path) : targetPosition으로 이동 가능한지 불가능한지 반환함
                if (agent.CalculatePath(PlayerManager.Player.transform.position, path))
                {
                    // 여기 path 안에는 다양한 정보가 있음.
                    // 애초에 길을 못찾았는지, 아니면 장애물 때문에 갈 수 없는지 등등 자세하게 써있으니까
                    // 나중에 찾아보기
                    agent.SetDestination(PlayerManager.Player.transform.position);
                }
                else // 갈 수 없으면 추적을 멈추고 다시 Wandering 상태로 바꿈
                {
                    agent.SetDestination(transform.position);
                    agent.isStopped = false;
                    SetState(AIState.Wandering);
                }
            }
            else // 플레이어가 공격 범위 내에 있지 않을 때 추적을 멈추고 Wandering 상태로 바꿈
            {
                agent.SetDestination(transform.position);
                agent.isStopped = true;
                SetState(AIState.Wandering);
            }
        }
    }

    bool IsPlayerInFieldOfView()
    {
        // 시야각 내로 들어왔는지 여부

        // 1. 몬스터가 플레이어를 바라보는 방향의 벡터를 만듬 : 플레이어 위치 - 몬스터 위치
        Vector3 directionToPlayer = PlayerManager.Player.transform.position - transform.position;
        // 2. 몬스터 위치와 몬스터->플레이어 방향 벡터 간의 각도를 구함
        float angle = Vector3.Angle(transform.position, directionToPlayer);
        // 3. 그 각도가 몬스터의 시야각보다 작으면 true, 아니면 false


        // 의문 : 근데 fieldOfView가 항상 양수로만 나오나?
        return angle < fieldOfView * 0.5f;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // 죽어야됨
            Die();
        }

        // 데미지 효과 
        StartCoroutine(DamageFlash());
    }

    void Die()
    {
        for (int i = 0; i < dropOnDeath.Length; i++)
        {
            Instantiate(dropOnDeath[i].dropPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    IEnumerator DamageFlash()
    {
        for (int i = 0; i < meshRenders.Length; i++)
        {
            meshRenders[i].material.color = new Color(1.0f, 0.6f, 0.6f);
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < meshRenders.Length; i++)
        {
            meshRenders[i].material.color = Color.white;
        }
    }


}
