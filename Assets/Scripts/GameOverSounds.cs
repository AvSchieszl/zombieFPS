using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverSounds : MonoBehaviour
{
    [SerializeField] AudioSource hitOne;
    [SerializeField] AudioSource hitTwo;
    [SerializeField] AudioSource death;
    public void firstHit()
    {
        hitOne.Play();
    }
    public void secondHit()
    {
        hitTwo.Play();
    }
    public void PlayerDeath()
    {
        death.Play();
    }
}
