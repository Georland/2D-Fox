using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // ��дui ��ش�������Ŀ�
using UnityEngine.SceneManagement; // ��д����ת����������Ҫ�Ŀ�

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;      // ����������������ԣ������ٶȣ������
    private Animator ani;        // ������ɫ�Ķ���
    public Collider2D coll;     // ������ɫ����ײ
    public Collider2D crouchDisColl;  // �¶�ʱ��Ҫdisable ��box collider
    public Transform ceiling;   // �¶�ʱͷ�����жϷ�Χ�������ͼ�����ͷ�����������crouch
    public LayerMask ground;    // �����浼�������ײ�ı��棬�˴����������ó������㣬���Ѹò�ȫ���������
    public LayerMask ladder;    // �����浼�������ײ�ı��棬�˴���¥�ݵ���
    public AudioSource jumpAudio; // ��Ծ��Ч
    public AudioSource hurtAudio; // ������Ч    
    public AudioSource deadAudio; // ������Ч

    bool isAlive;               // �Ƿ����main camera ��������ְ󶨣�������֮��ֹͣ��������
    public float speed;         // ˮƽ������ٶ�
    public float jumpHeight;    // ��ֱ�������Ծ�߶�

    readonly float movingThreshold = 0.1f;  // ˮƽ�ٶ��ڶ���ʱ���ж�Ϊ�ƶ�
    readonly float jumpingThreshold = 1f;   // ��ֱ�ٶ��ڶ���ʱ���ж�Ϊ��Ծ

    // onLadder �߼��������
    bool onLadder = false;      // ����ɫ�Ƿ���������
    public float climbSpeed;    // ��ɫ��¥���ϵ��ٶ�
    readonly float climbDecreaseRate = 3;   // ��ɫ��¥���Ϻ� �����ٶ�˥������
    readonly float crouchDecreaseRate = 3;  // ��ɫ�ڶ���ʱ �����ٶ�˥������

    // jump �߼��������
    bool jumpUpdate = false;    // ����Ƿ�������jump
    bool isJumping = false;     // ������ɫ�Ƿ�����jump
    //bool switched = false;      // �����Ƿ񾭹���״̬ת��
    //bool floating = false;      // �����Ƿ�ӱ�Ե����
    readonly int maxJumpTime = 2;        // �����Ծ����
    int jumpTime;               // ��ǰʣ�µĿ���Ծ��������ʱ�������ã�    

    // �¶� �߼��������
    bool isCrouching = false;   // ����ɫ�Ƿ����¶�
    readonly float ceilingRadius = 0.2f;  // ��ɫ����ʱ���Ϸ��ж��Ƿ��������ָʾԲ�İ뾶��С

    // ���� �߼��������
    bool isHurt;        // �ж��Ƿ�����
    readonly float hurtHorizontal = -5f; // ����ʱˮƽ�������ص���ĸ߶�
    readonly float hurtVertical = 5f;    // ����ʱ��ֱ�������ص���ĸ߶�    
    readonly float hurtDelayTime = 0.8f;   // ����֮��ָ��ľ���ʱ��

    int score = 0;              // ��ǰ�÷֣�ͨ���õ�item ��ȡ
    public Text scoreNumbers;   // ��¼��ǰ�ĵ÷֣�����ӳ��UI��

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        ani = this.GetComponent<Animator>();

        // ������ʼ��
        isAlive = true;
        jumpTime = maxJumpTime;
        isHurt = false;
    }

    /// <summary>
    /// Update �Ǹ��ݵ���������ִ����ִ�е�֡�ʵģ���ͬ���Ե�update ���ô�����һ�������ܸߵ�֡�ʸ㣬update ���öࡣ��֮�����������١�
    /// FixedUpdate �Ǹ���һ�µ�ʱ������0.02s ����һ�Σ����е��õķ��̡����ԣ�����ʹ�õ�Rigidbody �����ܵģ���Ҫ�ƶ��ģ�ǣ�浽�������Ķ�����������Ҫ���� Fixedupdate ����ִ�С�
    /// ʵ������ǣ�Update �� FixedUpdate ���ᱻ���ã���������������������� ��Ҫ�ǲ����ظ��ġ� �������жϰ���/������Ҫ��ǰ׼���Ĺ��ܷŵ�Update �������ʵ���ƶ�������������FixedUpdate��
    /// </summary>

    // Update is called once per frame
    void Update()
    {
        // ��update �����ж� GetButtonDown ������Ϊgetbuttondown ��Ҫ��update ���� FixedUpdate ������á�������������Ҫ��FixedUpdate ����ͳһ��Ծ֡��
        // ��������ʹ��bool ��ע����һ�����������ˣ�ʵ�ʵĹ���ִ�У�����Jump() ) �ͷ���FixedUpdate ����ִ��
        // �����߼��жϼ����� isJumping ����ע��ǰ�Ƿ��ڿ��� ��״̬����������space ʱ��ʼ��Ϊtrue�������ʱ��Ϊfalse����Ȼ�˴��������ж��е����⣬������������ʵ�����ڿ���ֻ�ܵ�����Ծ�Ĺ���
        //if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        if (Input.GetButtonDown("Jump"))
        {
            jumpUpdate = true;
        }
        
    }

    private void FixedUpdate()
    {
        // ����Ĳ���Ϊ �� ��ȫ������������ƣ�����Բ������з�Ӧ�Ķ����ϼ�
        if (ani.GetBool("hurt")) // �����˵�����£���ִ������֮���ǿ��λ�ƣ��ٽ���һ��ʱ�������������ʹ������λ�ƺͶ�����˳������
        {
            return;
        }

        Move();  // move Ӧ�÷ŵ�fixedUpdate ���棬��Ϊ��Ҫȷ���ȶ�֡������move

        // �����ж�
        Crouch();

        //if (Input.GetButtonDown("Jump") && !isJumping)
        if (jumpUpdate) // ��ס����֡�������� �Ƿ��� Jump ���жϣ��ڹ̶�֡������Ӧ���������Ұ� �жϵĲ���ֵ ���ã��Ա���һ�ε��á�
        {
            Debug.Log("jumpUpdate is true");
            Jump();
            jumpUpdate = false;  // �������Ҫ����jumpUpdate ��ԭ���ǣ������ʹ�õ�һbool ��ȷ���Ƿ�����Ծ�������Jump() �����ڰ���space���ÿһ֡��������Ծ�����Ա�����������bool
                                 // һ����ʾ������jump����һ����ʾjumping ��״̬
        }

        // ����Ĳ���Ϊ ����Ϊ��ҵ�һЩ��Ϊ����������֮��ĸ���ʱ��״̬ or û�а�����Ծ��ȴ��������ʱ�ĸ���״̬�������еĶ���ת���ĺϼ�
        // ��ʱ��Ҫ���е��жϣ��ж��Ƿ��ڿ��У�Ҳ���Ǵ� run/idle ת����jump/fall ״̬���жϡ�
        // ��������û����������Ĳ����߼��ģ�ȫ�������ڲ���֮��ͨ���ı���Ӧ�ı���������Щ�������ǿ���״̬�ı���
        // ���Ե�����һ��������д��animation �ı仯���ڳ����д���Ǻܷ����߼���
        // 
        SwitchAnim();

    }

    void Move()
    {
        float movement = Input.GetAxis("Horizontal"); // getAxis �ܹ�ȡ��-1 �� 1 ֮�������float �������Ի�ý���Ĺ��̣�����getAxis �������ڽ����ֱ����룬��þ�ȷ������
        float facedirection = Input.GetAxisRaw("Horizontal"); // getAxisRaw ֻ�ܽ��� -1�� 0�� 1 ���������������÷���pc����Ҫ��ȷ���Ƶ�Ҫ����Ҫ��ȷ���Ƶ�2d��Ϸһ����Raw

        if (Mathf.Abs(movement) > movingThreshold)
        {
            ani.SetBool("run", true);
            rb.velocity = new Vector2(movement * speed * Time.fixedDeltaTime, rb.velocity.y); // 2d �� velocity ֻ����������x �� y������������new Vector2
        }
        else
        {
            ani.SetBool("run", false);
        }

        if (facedirection != 0)
        {
            this.transform.localScale = new Vector3(facedirection, 1, 1); // ��ʹ��2d ����Ŀ��transform ��Ķ�����Ȼ������������ġ�����������new Vector3
        }


    }

    // ��ɫ��Ծ
    void Jump()
    {
        Debug.Log("current jump time is " + jumpTime);
        if (jumpTime > 0)
        {
            // ����������ϣ��򴥷���Ծʱȡ��onLadder ʹ���������߼�������
            //onLadder = false;
            if (onLadder)
            {
                LadderMovement(false);
            }
            //Debug.Log("offLadder");

            // ��ʼ��Ծ
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight * Time.fixedDeltaTime);
            jumpTime--;
            jumpAudio.Play();

        }
    }

    // ��Ծ�����仯����
    void JumpMovement()
    {
        float Vy = rb.velocity.y;
        if (Vy > jumpingThreshold)
        {
            ani.SetBool("jumping", true);
            ani.SetBool("falling", false);
        }
        else if (Vy < -jumpingThreshold)
        {
            //if (ani.GetBool("jumping"))
            //{
            //    switched = true;
            //}


            //if (switched) // �ӱ�Ե���� ������Ծ���� ʱ��Ӧ�ü���һ��jump ʹ�ô���
            //{
            //    jumpTime--; // �˷��̱����޵��ã��޷������������ʱ��ֻ��һ�Ρ�����ԭ���ǣ��޷��� �жϳ���״̬�Ĵ����� ����һ����ִ�еĴ��롣jumitime-- ֻ�ܱ�ִ��һ�Σ�������״̬�ж�ȴ������Ч��
            //}
            ani.SetBool("jumping", false);
            ani.SetBool("falling", true);
            isJumping = true;
        }

    }


    void LadderMovement(bool value)
    {
        onLadder = value;
        if (onLadder)
        {
            Debug.Log("On ladder");
            rb.gravityScale = 0;
            ani.SetBool("climb", true);
            float Ydirection = Input.GetAxisRaw("Vertical");
            float faceDirection = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(faceDirection * speed * Time.fixedDeltaTime / climbDecreaseRate, Ydirection * climbSpeed * Time.fixedDeltaTime);
            //LadderMovement();
        }
        else
        {
            Debug.Log("Off ladder");
            rb.gravityScale = 1;
            ani.SetBool("climb", false);
        }
    }

    void Crouch()
    {
        if (!Physics2D.OverlapCircle(ceiling.position, ceilingRadius, ground)) // ���ͷ��û�ж�������ִ��crouch �߼�
        {            
            Collider2D cd = this.GetComponent<BoxCollider2D>();
            if (Input.GetButton("Crouch") && !isJumping) // ��û��������°�����crouch
            {
                isCrouching = true;
                crouchDisColl.enabled = false;
                ani.SetBool("crouch", true);
                rb.velocity = new Vector2(rb.velocity.x / crouchDecreaseRate, rb.velocity.y);
            }
            else //if (Input.GetButtonUp("Crouch"))
            {
                isCrouching = false;
                crouchDisColl.enabled = true;
                ani.SetBool("crouch", false);
            }

        }
        else if (isCrouching)// ���ͷ���Ѿ��ж����ˣ���crouch ״̬Ϊ�棬��ʱ����
        {
            rb.velocity = new Vector2(rb.velocity.x / crouchDecreaseRate, rb.velocity.y);
        }
    }
    

    // �����仯���
    void SwitchAnim()
    {
        // �������δ��룬������ͨ��bool ����״̬�ķǳ��õķ������������ڱ���Ĵ��벻��������������
        // ��SwitchAnim �и���״̬���˴��������߼��ж� ���ǻ��ڶ�����״̬���жϣ���û���κΰ������жϡ�
        // ��ǿ������λ�Ƶ��߼��У�����֪����isHurt = true��Ȼ������ڴ˹����У�x�����ٶȽ���һ����ֵ�������״̬
        //if (isHurt && Mathf.Abs(rb.velocity.x) < hurtThreshold)
        //{
        //    isHurt = false;
        //}


        // �����¥���ϣ��� ��ס�˷������ or �Ѿ���Ladder��
        // �̶�λ�ã�����y���ƶ����ڰ�����ʱ��������
        if (coll.IsTouchingLayers(ladder) && (onLadder || Input.GetAxisRaw("Vertical") != 0))
        {
            //onLadder = true;
            LadderMovement(true);
            return; // ���return ��Ϊ��ȡ�������ж�jump �������ж���Ҳ������else �ν������jump ���������
        }
        // ���ͨ���������if ��˵����û�н�����¥��״̬��һ�����Ȱ� gravity ���ԭ��
        if (!coll.IsTouchingLayers(ladder) || onLadder && coll.IsTouchingLayers(ground))
        {
            LadderMovement(false);
        }

        // ����ڿ��У��ͽ���jump ���Ƶ��ж�����ֱ�ӷ���
        //if (!CollisionMovement())
        if (!coll.IsTouchingLayers(ground))
        {
            JumpMovement();
            return; // ���return ��Ϊ���ν������ ��غ��߼� ��д�ģ����������������ص��жϣ�����Ͳ���дreturn
        }

        // �������أ����ж�
        if (ani.GetBool("falling"))
        {
            jumpTime = maxJumpTime; // ���������������������������Ϊ���û��֮ǰ����������������jumpTime��ǰ�����touchingLayer ���жϻ�������֮ǰ��ճ�ڵ��ϵ�ʱ������jumpTime
            isJumping = false;
        }

        // ���ڵ���֮������
        ani.SetBool("falling", false);

        //if (isJumping && ani.GetBool("falling")) // ͬʱ���� isJumping Ϊtrue������������ falling Ϊtrue�������䣩��else if ��֤�Ѿ���ؽӴ�����
        //{
        //    Debug.Log("isJumping is false");
        //    isJumping = false;
        //    jumpTime = maxJumpTime;
        //    //ani.SetBool("falling", false);
        //}
        //else // �Ӵ����������״̬����
        //{
        //    Debug.Log("didn't enter the if");
        //    //ani.SetBool("jumping", false);
        //}
        //ani.SetBool("falling", false);
    }

    // ������ײ���
    //bool CollisionMovement()
    //{
    //    // ��������ػ��� jump ֮������֮ǰ ��Ȼ��Ч�����Ի���� jumping ��һ�α���true����Ϊ���������ֱ���Ϊfalse �ˡ����¿����ڿ������ڶ��Ρ�
    //    // ������������ʱû�취���������Ϊ��
    //    // 1. Ϊ��ͳһ����+��ÿ��space �����붼��Ч������Ҫ�� update ��jumpָ�Ȼ���� bool JumpUpdate ���� FixedUpdate ������Ծ����
    //    // 2. ��ʱû�취ͨ���ı�collider ����״ ��ȷ���������ĵ�һ֡���뿪�˵��棬��ײ��������
    //    // 3. �޷�ͨ����ʱ�жϵķ��� �ȵ����������֮�����޸� isJumping��
    //    // ���������е�ˮƽ�޷���������⡣������������������¾ͱ���ܶ�������
    //    if (coll.IsTouchingLayers(ground))
    //    {
    //        jumpTime = maxJumpTime;
    //        // isJumping = false;            
    //        // ani.SetBool("falling", false);
    //        return true;
    //    }
    //    return false;
    //}
     
    // �ռ�item / ��������
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �ռ�item
        if (collision.CompareTag("Items"))
        {
            Items item = collision.gameObject.GetComponent<Items>();
            if (!item.Destroyed()) // Ϊ�˱���item ����֮��player ײ�� item ��Ȼ�ܽ���ӷ��߼�
            {
                score++;
                scoreNumbers.text = ": " + score.ToString();
                //Destroy(collision.gameObject);            
                item.ItemDestroy();
            }
        }


    }

    // ��ײ����
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemies")) // ײ��enemy
        {
            if (!ani.GetBool("jumping") && ani.GetBool("falling")) // ����״̬
            {
                // ���ݲ�ͬenemy �����͵�������gettinghit ������Ӧ����abstract class + ��̬ ����
                //if (collision.gameObject.GetComponent<FrogController>()) 
                //{
                //    FrogController frog = collision.gameObject.GetComponent<FrogController>();
                //    frog.GettingHit();
                //}
                //else if (collision.gameObject.GetComponent<EagleController>())
                //{
                //    EagleController eagle = collision.gameObject.GetComponent<EagleController>();
                //    eagle.GettingHit();
                //}

                Enemies enemy = collision.gameObject.GetComponent<Enemies>();
                if (!enemy.isDead())
                {
                    enemy.GettingHit();
                    jumpTime++;
                    jumpUpdate = true;
                }
            }
            else
            {
                isHurt = true;
                Invoke("hurtDelay", hurtDelayTime);
                float direction = this.transform.position.x < collision.gameObject.transform.position.x ? 1 : -1;
                ani.SetBool("hurt", true);
                rb.velocity = new Vector2(direction * hurtHorizontal, hurtVertical);
                hurtAudio.Play();
            }
        }

        // ׹������
        if (collision.gameObject.CompareTag("deadline"))
        {
            isAlive = false;  // ��main camera ֹͣ��������
            deadAudio.Play(); // ����������Ч
            this.GetComponent<BoxCollider2D>().enabled = false; // �������δ���trigger ���²�����������
            Invoke("Restart", 2.5f); // ��ʱ����
        }

    }

    void hurtDelay()
    {
        //isHurt = false;
        ani.SetBool("hurt", false);
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool PlayerIsAlive()
    {
        return isAlive;
    }
}
