using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    AudioSource getItem;
    Animator anim;
    bool destroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        getItem = this.GetComponent<AudioSource>();
        anim = this.GetComponent<Animator>();
    }


    public void ItemDestroy()
    {
        anim.SetTrigger("destroy");
        getItem.Play();
        destroyed = true;
        Debug.Log("item called destroyed");
    }

    public bool Destroyed()
    {
        return destroyed;
    }

    void death()
    {
        Destroy(this.gameObject);
        Debug.Log("Item destroyed");
    }
}
