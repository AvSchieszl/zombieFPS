using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconRotation : MonoBehaviour
{
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameState.gameOver)
        {
            Vector3 eulerRotation = new Vector3(90f, player.transform.eulerAngles.y - 90f, 0f);
            transform.rotation = Quaternion.Euler(eulerRotation);
        }
    }
}
