using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Damageable))]
public class EnemyAI : NetworkBehaviour
{
    [SerializeField] private Transform targetDestination;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float searchRadius;

    [SerializeField] private Transform shotPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private bool rangeAttacker;
    [SerializeField] private GameObject projectile;
    public override void OnNetworkSpawn()
    {
        agent = GetComponent<NavMeshAgent>();
        Damageable damageable = GetComponent<Damageable>();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, searchRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, attackRadius);
    }
    // Update is called once per frame
    void Update()
    {
        if (targetDestination == null)
        {
            SearchForTarget();
        }
        else
        {
            SetDestinationServerRpc(targetDestination.position);
            if (Vector3.Distance(transform.position, targetDestination.position) <= attackRadius)
            {
                InvokeRepeating("EnemyAttackServerRpc", timeBetweenAttacks, timeBetweenAttacks);
            }
        }
    }
    [ServerRpc]
    private void EnemyAttackServerRpc()
    {
        if(rangeAttacker) 
        {
            Rigidbody rb = Instantiate(projectile, shotPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            projectile.GetComponent<NetworkObject>().Spawn(true);
            //rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            //rb.AddForce(transform.up * 8f, ForceMode.Impulse);
        }
    }
    private void SearchForTarget()
    {
        float closestDistance = Mathf.Infinity;
        Collider[] potentialTargetColliders = Physics.OverlapSphere(transform.position, searchRadius);
        foreach (Collider potentialTarget in potentialTargetColliders)
        {
           bool isPlayer = potentialTarget.gameObject.TryGetComponent<PlayerController>(out PlayerController player);
            if(player != null)
            {
                float targetDistance = Vector3.Distance(transform.position, player.transform.position);
                if (targetDistance < closestDistance)
                {
                    closestDistance = targetDistance;
                    targetDestination = player.transform;
                    //SetDestinationServerRpc(targetDestination.position);
                }
            }
        }
    }
    public void OnDeath()
    {
        Destroy(gameObject);
    }

    public void LogShooter(ulong shooterID)
    {
        if (targetDestination == null)
        {
           targetDestination = NetworkManager.Singleton.ConnectedClients[shooterID].PlayerObject.transform;
        }
    }

    [ServerRpc]
    private void SetDestinationServerRpc(Vector3 destination)
    {
        agent.SetDestination(destination);
    }
}
