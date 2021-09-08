using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogController : Enemies
{
    // public 变量
    public Transform player;
    // public Animator anim;
    Rigidbody2D rb;

    // 基础数据
    float jumpTimeGap = 3f;     // 判定多少秒执行一次跳跃
    float jumpTime;
    public float jumpHeight;    // 跳跃纵向高度
    public float jumpSpeed;     // 跳跃横向速度
    float jumpThreshold = 0.2f; // 判定跳起并转变动画的阈值
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
        // 调整面朝的方向，朝向player
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
