using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pathfinder : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 target;
    public StartModule origin;

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
        else if (agent.path.status == NavMeshPathStatus.PathPartial || agent.path.status == NavMeshPathStatus.PathInvalid)
        {
            PathError();
        }
    }

    void PathError()
    {
        origin.OnPathError();
    }

    void FoundPath()
    {
        origin.OnPathConnect();
        Destroy(gameObject);
    }
}
