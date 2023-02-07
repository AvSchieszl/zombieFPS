using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
public class GunScriptableObject : ScriptableObject
{
    [SerializeField] private GunType type;
    [SerializeField] private string gunName;
    [SerializeField] private GameObject modelPrefab;
    [SerializeField] private Vector3 spawnPoint;
    [SerializeField] private Vector3 spawnRotation;

    [SerializeField] private ShootConfigurationScriptableObject shootConfig;
    [SerializeField] private TrailConfigurationScriptableObject trailConfig;

    private MonoBehaviour activeMonoBehaviour;
    private GameObject Model;
    private float lastShootTime;
    private ParticleSystem ShootSystem;
    private ObjectPool<TrailRenderer> Trailpool; //https://youtu.be/zyzqA_CPz2E

    public void Spawn(Transform parent, MonoBehaviour activeMonoBehaviour)
    {

    }
}
