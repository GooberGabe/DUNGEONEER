using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pathfinder : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 target;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameManager.instance.GetGrid().endModule.dragonRef.transform.position;
    }

    void Update()
    {
        if (agent.isOnNavMesh) { agent.SetDestination(target); }
        if (agent.hasPath && agent.path.status == NavMeshPathStatus.PathComplete)
        {
            FoundPath();
        }
    }

    void FoundPath()
    {
        GameManager.instance.isValidPath = true;
        Destroy(gameObject);
    }
}
