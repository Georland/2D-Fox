using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleController : Enemies
{
    private Rigidbody2D rb;
    public Transform high, low;
    public Transform player;
    // public Animator anim;

    float highest, lowest;  // �ܹ����е�����ߵ� �� ��͵�ĸ߶�
    public float speed;     // �ⲿ�趨�����ٶ�
    float towardsUp = 1;    // �ڲ��趨��ʼ���з�������or��
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
        // ���趨����
        float direction = player.position.x < this.transform.position.x ? 1 : -1;
        this.transform.localScale = new Vector3(direction, 1, 1);

        // ���趨���з�ʽ        
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
