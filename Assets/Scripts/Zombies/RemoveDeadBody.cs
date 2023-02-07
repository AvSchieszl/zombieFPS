using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveDeadBody : MonoBehaviour
{
    [SerializeField] float sinkDelay = 5f;
    [SerializeField] float destroyDelay = 10f;
    [SerializeField] float sinkDrag = 40f;
    float destroyHeight;
    public void Start()
    {
        Invoke(nameof(StartSink), sinkDelay);
    }

    public void StartSink()
    {
        Collider[] colliders = this.transform.GetComponentsInChildren<Collider>();
        Rigidbody[] rigidbodies = this.transform.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.drag = sinkDrag;
        }
        foreach (Collider col in colliders)
        {
            Destroy(col);
        }
        Invoke(nameof(SinkIntoGround), destroyDelay);
    }
    void SinkIntoGround()
    {
        Destroy(this.gameObject);
    }
}
