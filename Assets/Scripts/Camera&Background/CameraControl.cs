using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject player;
    //public Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerController playerC = player.GetComponent<PlayerController>();
        if (!playerC.PlayerIsAlive())
        {
            GetComponent<AudioSource>().enabled = false;
        }
        // this.transform.position = new Vector3(playerTransform.position.x, 0, -10f);
        //float x = player.transform.position.x;
    }
}
