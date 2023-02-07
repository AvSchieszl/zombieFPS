using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSound : MonoBehaviour
{
    [SerializeField] private AudioSource shot;
    public void Fire()
    {
        shot.Play();
    }
    public void CanShoot()
    {
        GameState.canShoot = true;
    }
}
