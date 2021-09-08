using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    protected Animator anim;

    // 爆炸动画和死亡逻辑的变量设置
    protected Vector3 stayPos;
    protected bool dead = false;

    // 爆炸音频变量设置
    protected AudioSource deathAudio;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        anim = this.GetComponent<Animator>();
        deathAudio = this.GetComponent<AudioSource>();
    }

    public void GettingHit()
    {
        anim.SetTrigger("dead");
        dead = true;
        stayPos = this.transform.position;
        deathAudio.Play();
    }

    public bool isDead()
    {
        return dead;
    }

    protected void Death()
    {
        Destroy(this.gameObject);
    }

    protected void Stay()
    {
        this.transform.position = stayPos;
    }
}
