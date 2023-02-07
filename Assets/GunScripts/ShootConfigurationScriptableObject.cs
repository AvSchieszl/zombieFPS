using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Configuration", order = 2)]

public class ShootConfigurationScriptableObject : ScriptableObject
{
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private Vector3 spread = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] float fireRate = 0.25f;
}