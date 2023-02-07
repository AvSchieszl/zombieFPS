using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DismemberLimb : MonoBehaviour
{
    [SerializeField] private GameObject severedLimb;
    [SerializeField] private GameObject bloodFX;
    [SerializeField] private float seperationForce = 1000f;
    public void SevereLimb()
    {
        this.transform.localScale = Vector3.zero; //make hit limb disappear
        RemoveComponents();
        SpawnBloodFX();
        GameObject limb = Instantiate(severedLimb, this.transform.position, this.transform.rotation);
        Vector3 x = new Vector3(0, 1, 1);
        limb.GetComponentInChildren<Rigidbody>().AddForce((Camera.main.transform.up + Camera.main.transform.forward) * seperationForce);
    }
    private void SpawnBloodFX()
    {
        GameObject blood = Instantiate(bloodFX, this.transform.position, transform.rotation);
        blood.transform.parent = this.transform;

        Destroy(blood, 5f);
    }
    private void RemoveComponents()
    {
        Destroy(this.GetComponent<CharacterJoint>());
        foreach (var comp in this.GetComponents<Component>())
        {
            if (!(comp is Transform))
                Destroy(comp);
        }
        foreach (var comp in this.GetComponentsInChildren<CharacterJoint>())
        {
            Destroy(comp);
        }
        foreach (var comp in this.GetComponentsInChildren<Component>())
        {
            if (!(comp is Transform))
                Destroy(comp);
        }
    }
}
