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
    public ItemData[] dropOnDeath; // �׾��� �� ����� �����۵�


    [Header("AI")]
    private NavMeshAgent agent;
    public float detectDistance; // ��ǥ ���������� �Ÿ�
    private AIState aiState;

    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime; // �ּ� ��� �ð�
    public float maxWanderWaitTime;

    [Header("Combat")]
    public int damage;
    public float attackRate;
    private float lastAttackTime;
    public float attackDistance;

    private float playerDistance;
    public float fieldOfView = 120f; // ���� ������ �þ߰�

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
        // �÷��̾�� NPC���� �Ÿ� ���
        playerDistance = Vector3.Distance(PlayerManager.Player.transform.position, transform.position);

        // State�� ���� ���°� �ƴϸ� Moving �ִϸ��̼� ���
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
                agent.isStopped = true; // agent ���߱�
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
        /// ���� ����
        /*
        minWanderDistance�� maxWanderDistance ������ ������ ��ġ�� ����
        �� ��ġ�� NavMeshAgent �̵���Ŵ
        ���࿡ ���������� minWanderWaitTime <-> maxWanderWaitTime ������ ������ �ð� ���� ���
        �׷��� �ٽ� ���� ��ġ ���� �̵�?
        */
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle); // ��� ���·� ��ȯ.
            // ������� �´µ�?
            // ���� ���°� ��Ȳ�� + NavMeshAgent�� ��ǥ �������� ���� �Ÿ��� ���� ������
            // ���� �ð� �ڿ� �� ��ġ ã�� �Լ� ȣ���̴ϱ�.. ����ѵ�
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
        // Vector3 sourcePosition : ���� ���� ����
        // out NavMeshHit hit : �ִ� ��ο� ���õ� �� ��ȯ
        // float maxDistance : �ִ� �Ÿ�
        // int areaMask : Ž���� ������ ����ũ

        // Random.onUnitSphere : �������� 1�� ��
        // NPC�� ���� ��ġ + �������� minWanderDistance�� maxWanderDistance�� �� ������ ������ ����ŭ �̵��� ��ġ 


        // ����
        /*
        NavMesh.SamplePosition(
            transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), 
            out hit, 
            maxWanderDistance, 
            NavMesh.AllAreas);

        int i = 0;

        // detectDistance ���ٴ� �� �Ÿ��� �̵��ϰ��� ��
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


        // Do While ������ ������ ����
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
        /// ���� ����
        /*
        ���࿡ �÷��̾���� �Ÿ��� attackDistance���� �ָ� �÷��̾� ������ �̵�
        �ƴ϶�� �÷��̾ ���� �õ� (�þ߰� ���� ���Դٴ� ���� �Ͽ�)
        */


        // �÷��̾���� �Ÿ��� ���� ���� ���� �ְ�, �þ߰� ���ο� ���� ��
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
        else // �ƴ� ��
        {
            // �׷����� �÷��̾ ���� ���� ���� ���� ��
            if (playerDistance < detectDistance)
            {
                // �÷��̾�� ���� ���ο� ���� �� ���� ������ �õ��� ��
                agent.isStopped = false;
                NavMeshPath path = new NavMeshPath();
                // �÷��̾����� �� �� ������ ��
                // NavMeshAgent.CalculatePath(Vector3 targetPosition, NavMeshPath path) : targetPosition���� �̵� �������� �Ұ������� ��ȯ��
                if (agent.CalculatePath(PlayerManager.Player.transform.position, path))
                {
                    // ���� path �ȿ��� �پ��� ������ ����.
                    // ���ʿ� ���� ��ã�Ҵ���, �ƴϸ� ��ֹ� ������ �� �� ������ ��� �ڼ��ϰ� �������ϱ�
                    // ���߿� ã�ƺ���
                    agent.SetDestination(PlayerManager.Player.transform.position);
                }
                else // �� �� ������ ������ ���߰� �ٽ� Wandering ���·� �ٲ�
                {
                    agent.SetDestination(transform.position);
                    agent.isStopped = false;
                    SetState(AIState.Wandering);
                }
            }
            else // �÷��̾ ���� ���� ���� ���� ���� �� ������ ���߰� Wandering ���·� �ٲ�
            {
                agent.SetDestination(transform.position);
                agent.isStopped = true;
                SetState(AIState.Wandering);
            }
        }
    }

    bool IsPlayerInFieldOfView()
    {
        // �þ߰� ���� ���Դ��� ����

        // 1. ���Ͱ� �÷��̾ �ٶ󺸴� ������ ���͸� ���� : �÷��̾� ��ġ - ���� ��ġ
        Vector3 directionToPlayer = PlayerManager.Player.transform.position - transform.position;
        // 2. ���� ��ġ�� ����->�÷��̾� ���� ���� ���� ������ ����
        float angle = Vector3.Angle(transform.position, directionToPlayer);
        // 3. �� ������ ������ �þ߰����� ������ true, �ƴϸ� false


        // �ǹ� : �ٵ� fieldOfView�� �׻� ����θ� ������?
        return angle < fieldOfView * 0.5f;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // �׾�ߵ�
            Die();
        }

        // ������ ȿ�� 
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
