using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    public Transform Cam;
    public float moveRate;
    private float startPoint;

    // Start is called before the first frame update
    void Start()
    {
        startPoint = this.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector2(startPoint + (Cam.position.x - startPoint) * moveRate,
                                                this.transform.position.y);
    }
}
