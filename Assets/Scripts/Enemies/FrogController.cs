using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogController : Enemies
{
    // public ����
    public Transform player;
    // public Animator anim;
    Rigidbody2D rb;

    // ��������
    float jumpTimeGap = 3f;     // �ж�������ִ��һ����Ծ
    float jumpTime;
    public float jumpHeight;    // ��Ծ����߶�
    public float jumpSpeed;     // ��Ծ�����ٶ�
    float jumpThreshold = 0.2f; // �ж�����ת�䶯������ֵ
    //bool dead = false;
    //Vector3 stayPos;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rb = this.GetComponent<Rigidbody2D>();
        jumpTime = jumpTimeGap;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dead)
        {
            SwitchAnim();
            Jump();
        } 
        else
        {
            Stay();
        }
    }

    void Jump()
    {
        float forward = -this.transform.localScale.x;
        jumpTime -= Time.fixedDeltaTime;
        if (jumpTime < 0)
        {
            rb.velocity = new Vector2(jumpSpeed * forward, jumpHeight);
            jumpTime = jumpTimeGap;
        }
    }

    void SwitchAnim()
    {
        // �����泯�ķ��򣬳���player
        float x = player.transform.position.x < this.transform.position.x ? 1 : -1;
        this.transform.localScale = new Vector3(x, 1, 1);

        float Vy = rb.velocity.y;
        if (Vy > jumpThreshold)
        {
            anim.SetBool("jumping", true);
            anim.SetBool("falling", false);
        }
        else if (Vy < -jumpThreshold)
        {
            anim.SetBool("jumping", false);
            anim.SetBool("falling", true);
        }
        else
        {
            anim.SetBool("jumping", false);
            anim.SetBool("falling", false);
        }
    }

    //public void GettingHit()
    //{
    //    anim.SetTrigger("dead");
    //    dead = true;
    //    stayPos = this.transform.position;
    //}

    //void Stay()
    //{
    //    this.transform.position = stayPos;
    //}

    //void Death()
    //{
    //    Destroy(this.gameObject);
    //}

}
