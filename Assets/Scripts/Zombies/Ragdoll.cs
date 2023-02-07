using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private GameObject enemyRig;
    private Animator enemyAnimator;
    private Collider[] ragdollColliders;
    private Rigidbody[] limbsRigidbodies;
    void Start()
    {
        enemyRig = this.gameObject;
        enemyAnimator = this.GetComponent<Animator>();
        GetRagdollParts();
        RagdollModeOff();
    }

    void GetRagdollParts()
    {
        ragdollColliders = enemyRig.GetComponentsInChildren<Collider>();
        limbsRigidbodies = enemyRig.GetComponentsInChildren<Rigidbody>();
    }
    public void RagdollModeOn()
    {
        enemyAnimator.enabled = false;
        foreach (Rigidbody rb in limbsRigidbodies)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        foreach (Collider col in ragdollColliders)
        {
            col.gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        }
    }

    public void RagdollModeOff()
    {
        enemyAnimator.enabled = true;
        foreach (Collider col in ragdollColliders)
        {
            col.enabled = true;
        }
        foreach (Rigidbody rb in limbsRigidbodies)
        {
            rb.isKinematic = true;
        }
    }
}