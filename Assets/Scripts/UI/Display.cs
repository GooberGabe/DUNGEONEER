using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display : MonoBehaviour
{
    public Entity subject;
    public Vector3 offset;

    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
    protected virtual void Update()
    {
        transform.position = subject.transform.position + offset;
        transform.eulerAngles = new Vector3(-Camera.main.transform.eulerAngles.x, -45, 0);
    }
}
