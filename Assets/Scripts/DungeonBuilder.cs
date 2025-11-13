using UnityEngine;
using System.Collections.Generic;
using Unity.AI.Navigation;

public class DungeonBuilder : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;

    void Start()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    public void BakeNavMesh()
    {
        //navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
        navMeshSurface.BuildNavMesh();
    }
}