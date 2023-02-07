using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    //Refactor: Put in Health class
    [Header("Health Pickup")]
    [SerializeField] private Image healthCircle;
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int MediKitHP = 20;
    [SerializeField] private AudioSource[] hurtAudio;
    [SerializeField] private AudioSource healthPickUpAudio;
    [SerializeField] private GameObject protagonistPrefab;

    void Start()
    {
        health = maxHealth;
        healthCircle.fillAmount = health / 100f;
    }

    //Refactor to Health class:
    public void TakeDamage(int damage)
    {
        health = (int)Mathf.Clamp(health - damage, 0, maxHealth);
        healthCircle.fillAmount = health / 100f;
        Invoke(nameof(PlayHurtAudio), 0.3f);
        if (health == 0)
        {
            PlayerDeath();
        }
    }

    private void PlayerDeath()
    {
        GameState.gameOver = true;
        Vector3 position = new Vector3(this.transform.position.x,
                                            Terrain.activeTerrain.SampleHeight(this.transform.position),
                                            this.transform.position.z);
        GameObject protagonist = Instantiate(protagonistPrefab, position, this.transform.rotation);
        protagonist.GetComponent<Animator>().SetTrigger("Death");
        Destroy(this.gameObject);

    }
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Health" && health < maxHealth)
        {
            health = Mathf.Clamp(health + MediKitHP, 0, maxHealth);
            healthCircle.fillAmount = health / 100f;
            healthPickUpAudio.Play();
            Destroy(col.gameObject);
        }
    }
    private void PlayHurtAudio()
    {
        AudioSource audioSource = new AudioSource();
        int i = Random.Range(1, hurtAudio.Length);

        audioSource = hurtAudio[i];
        audioSource.Play();
        hurtAudio[i] = hurtAudio[0];
        hurtAudio[0] = audioSource;
    }
}