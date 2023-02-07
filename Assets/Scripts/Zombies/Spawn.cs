using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawn : MonoBehaviour
{
    [SerializeField] GameObject zombiePrefab;
    [SerializeField] int numberOfZombies;
    [SerializeField] float spawnRadius;
    [SerializeField] bool spawnOnStart;

    // Start is called before the first frame update
    void Start()
    {
        if (spawnOnStart)
            SpawnEnemies();
    }
    void SpawnEnemies()
    {
        for (int i = 0; i < numberOfZombies; i++)
        {
            Vector3 randomPoint = this.transform.position + Random.insideUnitSphere * spawnRadius;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas)) //prevent spawn above ground
            {
                Instantiate(zombiePrefab, hit.position, Quaternion.identity);
            }
            else
                i--;
        }
        Destroy(this.gameObject, 1f);
    }
    void OnTriggerEnter(Collider collider)
    {
        if (!spawnOnStart && collider.gameObject.tag == "Player")
            SpawnEnemies();
    }

    void OnDrawGizmosSelected()

    {
        // Zombie spawn radius
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, spawnRadius);
    }
}
