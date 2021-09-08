using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleController : Enemies
{
    private Rigidbody2D rb;
    public Transform high, low;
    public Transform player;
    // public Animator anim;

    float highest, lowest;  // 能够飞行到的最高点 和 最低点的高度
    public float speed;     // 外部设定飞行速度
    float towardsUp = 1;    // 内部设定初始飞行方向是上or下
    // bool dead = false;
    // Vector3 stayPos;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rb = this.GetComponent<Rigidbody2D>();
        highest = high.position.y;
        lowest = low.position.y;
        Destroy(high.gameObject);
        Destroy(low.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            Movement();
        }
        else
        {
            Stay();
        }
    }

    void Movement()
    {
        // MovingHorizontally();
        MovingVertically();
    }
    void MovingVertically()
    {
        // 先设定朝向
        float direction = player.position.x < this.transform.position.x ? 1 : -1;
        this.transform.localScale = new Vector3(direction, 1, 1);

        // 再设定飞行方式        
        if (towardsUp > 0)
        {
            towardsUp = this.transform.position.y > highest ? -1 : 1;
        }
        else
        {
            towardsUp = this.transform.position.y < lowest ? 1 : -1;
        }
        rb.velocity = new Vector2(0, towardsUp * speed);
    }


}
