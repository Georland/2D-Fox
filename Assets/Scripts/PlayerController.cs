using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 书写ui 相关代码所需的库
using UnityEngine.SceneManagement; // 书写场景转换代码所需要的库

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;      // 操作物体的物理属性，包括速度，朝向等
    private Animator ani;        // 操作角色的动画
    public Collider2D coll;     // 操作角色的碰撞
    public Collider2D crouchDisColl;  // 下蹲时需要disable 的box collider
    public Transform ceiling;   // 下蹲时头顶的判断范围，如果有图层进入头顶则不允许继续crouch
    public LayerMask ground;    // 从外面导入可能碰撞的表面，此处将地面设置成其他层，并把该层全部导入进来
    public LayerMask ladder;    // 从外面导入可能碰撞的表面，此处将楼梯导入
    //public AudioSource jumpAudio; // 跳跃音效
    //public AudioSource hurtAudio; // 受伤音效    
    //public AudioSource deadAudio; // 死亡音效

    private float faceDirection;    // 伪面部朝向，为了方便各个函数使用方向，所以放在全局变量中
    private float towards;          // 真面部朝向，利用localScale 确定朝向，即使速度为0 也能有值
    bool isAlive;               // 是否存活，与main camera 上面的音乐绑定，在死亡之后停止背景音乐
    public float speed;         // 水平方向的速度
    public float jumpHeight;    // 竖直方向的跳跃高度

    readonly float movingThreshold = 0.1f;  // 水平速度在多大的时候被判定为移动
    readonly float jumpingThreshold = 1f;   // 竖直速度在多大的时候被判定为跳跃

    // onLadder 逻辑所需变量
    bool onLadder = false;      // 监测角色是否在梯子上
    public float climbSpeed;    // 角色在楼梯上的速度
    readonly float climbDecreaseRate = 3;   // 角色在楼梯上后 横向速度衰减倍率
    readonly float crouchDecreaseRate = 3;  // 角色在蹲下时 横向速度衰减倍率

    // jump 逻辑所需变量
    bool jumpUpdate = false;    // 监测是否输入了jump
    bool isJumping = false;     // 表征角色是否正在jump
    //bool switched = false;      // 表征是否经过了状态转换
    //bool floating = false;      // 表征是否从边缘滑落
    readonly int maxJumpTime = 2;        // 最大跳跃次数
    int jumpTime;               // 当前剩下的可跳跃次数（暂时不起作用）    

    // 下蹲 逻辑所需变量
    bool isCrouching = false;   // 监测角色是否在下蹲
    readonly float ceilingRadius = 0.2f;  // 角色蹲下时，上方判定是否有物体的指示圆的半径大小

    // Dash 逻辑所需变量
    [Header("Dash参数")]
    public float dashTime;      // dash 的持续时长
    private float dashTimeLeft; // dash 剩余的时间，自己运算出来
    public float dashSpeed;     // dash 时候的速度
    private Vector2 velocityBeforeDash = new Vector2(0, 0); // dash 之前的速度，dash 之后需要还原回这个速度

    // Dash cd逻辑所需变量
    private float lastDash = -10f;     // 上一次dash 的时间点，此处有初值，是为了让游戏开始的时候一定能dash。因为float 有默认值0，所以按照dash 的条件判断，0 + cd > 刚开始的操作时间必然不成立。所以初始值改成-10
    public float dashCoolDown;  // dash 的cd，可设置
    private bool isDashing;      // 冲刺状态的判断

    // 受伤 逻辑所需变量
    bool isHurt;        // 判定是否受伤
    readonly float hurtHorizontal = -5f; // 受伤时水平方向往回弹起的高度
    readonly float hurtVertical = 5f;    // 受伤时竖直方向往回弹起的高度    
    readonly float hurtDelayTime = 0.8f;   // 受伤之后恢复的具体时间

    int score = 0;              // 当前得分，通过得到item 获取
    public Text scoreNumbers;   // 记录当前的得分，并反映到UI上

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        ani = this.GetComponent<Animator>();

        // 变量初始化        
        isAlive = true;
        jumpTime = maxJumpTime;
        isHurt = false;
    }

    /// <summary>
    /// Update 是根据电脑性能来执行能执行的帧率的，不同电脑的update 调用次数不一样，性能高的帧率搞，update 调用多。反之差电脑则调用少。
    /// FixedUpdate 是根据一致的时间间隔（0.02s 调用一次）进行调用的方程。所以，所有使用到Rigidbody 的性能的，需要移动的，牵涉到物理层面的动作，都必须要放在 Fixedupdate 里面执行。
    /// 实际情况是，Update 和 FixedUpdate 都会被调用，所以这两个函数里的内容 需要是不能重复的。 把所有判断按键/部分需要提前准备的功能放到Update 里，把所有实际移动的物理动作放在FixedUpdate里
    /// </summary>

    // Update is called once per frame
    void Update()
    {
        // 在update 里面判断 GetButtonDown ，是因为getbuttondown 需要在update 而非 FixedUpdate 里面调用。但是我们又需要在FixedUpdate 里面统一跳跃帧，
        // 所以这里使用bool 标注跳这一动作被按下了，实际的功能执行（调用Jump() ) 就放在FixedUpdate 里面执行
        // 后面逻辑判断加上了 isJumping ，标注当前是否在空中 的状态。他从输入space 时开始置为true，在落地时置为false。虽然此处的贴地判断有点问题，但是整体上是实现了在空中只能单次跳跃的功能
        //if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        if (Input.GetButtonDown("Jump"))
        {
            jumpUpdate = true;
        }

        // 冲锋逻辑
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > lastDash + dashCoolDown)
        {
            Debug.Log("Ready to dash");
            // 可以执行dash
            ReadyToDash();
        }

    }

    private void FixedUpdate()
    {
        // 下面的部分为 ： 完全由玩家主动控制，程序对操作进行反应的动作合集
        faceDirection = Input.GetAxisRaw("Horizontal");
        towards = transform.localScale.x;

        if (ani.GetBool("hurt")) // 在受伤的情况下，先执行受伤之后的强制位移，再禁用一段时间的其他动作，使得受伤位移和动画能顺利播完
        {
            return;
        }

        if (!isCrouching && !onLadder) // 下蹲 or 在楼梯上时不能dash
        {
            Dash();     // dash 判定+执行
        }
        else
        {
            isDashing = false; // 如果有上述任意条件，把isDashing 关掉，让下面的isDashing 判断能够跳过，就可以执行Move 等函数了
        }

        if (isDashing)
        {
            return; // 冲刺时 不进行move，jump, crouch 判断
        }

        Move();     // move 应该放到fixedUpdate 里面，因为需要确保稳定帧数调用move

        Crouch();   // 蹲下 判定+执行

        //if (Input.GetButtonDown("Jump") && !isJumping)
        if (jumpUpdate) // 接住不定帧传过来的 是否按下 Jump 的判断，在固定帧调用相应动作，并且把 判断的布尔值 重置，以便下一次调用。
        {
            Debug.Log("jumpUpdate is true");
            Jump();
            jumpUpdate = false;  // 这里必须要设置jumpUpdate 的原因是，如果仅使用单一bool 来确定是否在跳跃，这里的Jump() 将会在按下space后的每一帧都进行跳跃。所以必须设置两个bool
                                 // 一个表示按下了jump，另一个显示jumping 的状态
        }

        // 下面的部分为 ：因为玩家的一些行为（例如跳起之后的浮空时的状态 or 没有按下跳跃键却跳下悬崖时的浮空状态）而进行的动作转换的合集
        // 随时需要进行的判断，判断是否在空中，也就是从 run/idle 转换到jump/fall 状态的判断。
        // 这里面是没有玩家主动的操作逻辑的，全部都是在操作之后，通过改变相应的变量，且这些变量都是控制状态的变量
        // 所以单独用一个函数来写明animation 的变化，在程序编写上是很符合逻辑的
        // 
        SwitchAnim();

    }

    void Move()
    {
        float movement = Input.GetAxis("Horizontal"); // getAxis 能够取得-1 到 1 之间的任意float 数，可以获得渐变的过程，所以getAxis 可以用于接收手柄输入，获得精确的数据
        // faceDirection = Input.GetAxisRaw("Horizontal"); // getAxisRaw 只能接收 -1， 0， 1 三个数，但是正好符合pc端需要精确控制的要求，需要精确控制的2d游戏一般用Raw

        if (Mathf.Abs(movement) > movingThreshold)
        {
            ani.SetBool("run", true);
            rb.velocity = new Vector2(movement * speed * Time.fixedDeltaTime, rb.velocity.y); // 2d 的 velocity 只有两个参数x 和 y，所以这里是new Vector2
        }
        else
        {
            ani.SetBool("run", false);
        }

        if (faceDirection != 0)
        {
            this.transform.localScale = new Vector3(faceDirection, 1, 1); // 即使是2d 的项目，transform 里的东西仍然是有三个坐标的。所以这里是new Vector3
        }


    }

    // 角色跳跃
    void Jump()
    {
        Debug.Log("current jump time is " + jumpTime);
        if (jumpTime > 0)
        {
            // 如果在梯子上，则触发跳跃时取消onLadder 使得爬梯子逻辑不成立
            //onLadder = false;
            if (onLadder)
            {
                LadderMovement(false);
            }
            //Debug.Log("offLadder");

            // 开始跳跃
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight * Time.fixedDeltaTime);
            jumpTime--;
            //jumpAudio.Play();
            SoundManager.instance.JumpAudio(); // soundManager 为单例，所以直接用单例做。
                                               // 单例的好处是，各个地方都能方便地像调用写在自己文档里的变量一样调用该方法，而不用特意写很多东西去取这个变量，再调用
                                               // 缺点是自己是静态static 的，所以会一直存放在内存里，占用内存空间
        }
    }

    // 跳跃动作变化动画
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


            //if (switched) // 从边缘滑落 而非跳跃下落 时，应该减掉一次jump 使用次数
            //{
            //    jumpTime--; // 此方程被无限调用，无法做到在下落的时候只减一次。根本原因是，无法在 判断持续状态的代码里 控制一次性执行的代码。jumitime-- 只能被执行一次，但是其状态判断却长期有效。
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
        if (!Physics2D.OverlapCircle(ceiling.position, ceilingRadius, ground)) // 如果头顶没有东西，才执行crouch 逻辑
        {
            Collider2D cd = this.GetComponent<BoxCollider2D>();
            if (Input.GetButton("Crouch") && !isJumping) // 在没跳的情况下按下了crouch
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
        else if (isCrouching)// 如果头顶已经有东西了，且crouch 状态为真，此时减速
        {
            rb.velocity = new Vector2(rb.velocity.x / crouchDecreaseRate, rb.velocity.y);
        }
    }

    void ReadyToDash()
    {
        // 顺发按键动作，在Update 里面调用。允许dash，设置dash 的各项变量值
        isDashing = true;
        dashTimeLeft = dashTime; // 倒计时开始
        lastDash = Time.time; // 将此次dash 的时间点传给 lastDash
        velocityBeforeDash = new Vector2(towards * speed * Time.fixedDeltaTime, 0); // 将此次dash 前的velocity 保存下来，以便后面恢复使用
    }

    void Dash()
    {
        // 接过Update 里面的判断，在FixedUpdate 里面执行的 dash 行为
        if (isDashing)
        {
            if (dashTimeLeft > 0) // 还有时间剩余，可以继续冲锋
            {
                rb.velocity = new Vector2(dashSpeed * towards * Time.fixedDeltaTime, rb.velocity.y); // 冲锋速度设置
                dashTimeLeft -= Time.fixedDeltaTime;
                ShadowPool.instance.GetFromPool();
            }
            else
            {
                Debug.Log("stored velocity is " + velocityBeforeDash.x + " " + velocityBeforeDash.y);
                rb.velocity = velocityBeforeDash;
                isDashing = false; // isDashing 为false 之后，fixedUpdate 里面自动开启下面的move 判断，所以速度会恢复到正常状态
            }
        }
    }


    // 动作变化监测
    void SwitchAnim()
    {
        // 这里的这段代码，本来是通过bool 控制状态的非常好的范例，但是由于本身的代码不合理所以舍弃。
        // 在SwitchAnim 中更改状态，此处的所有逻辑判断 都是基于对现有状态的判断，并没有任何按键的判断。
        // 在强制受伤位移的逻辑中，先已知他是isHurt = true，然后如果在此过程中，x方向速度降到一定数值，则更改状态
        //if (isHurt && Mathf.Abs(rb.velocity.x) < hurtThreshold)
        //{
        //    isHurt = false;
        //}


        // 如果在楼梯上，且 按住了方向键上 or 已经在Ladder上
        // 固定位置，进行y轴移动。在按下跳时允许跳出
        if (coll.IsTouchingLayers(ladder) && (onLadder || Input.GetAxisRaw("Vertical") != 0))
        {
            //onLadder = true;
            LadderMovement(true);
            return; // 这个return 是为了取消掉空中对jump 动作的判定，也可以用else 衔接下面的jump 内容来替代
        }
        // 如果通过了上面的if ，说明并没有进入爬楼梯状态，一定优先把 gravity 变回原样
        if (!coll.IsTouchingLayers(ladder) || onLadder && coll.IsTouchingLayers(ground))
        {
            LadderMovement(false);
        }

        // 如果在空中，就进行jump 姿势的判定，并直接返回
        //if (!CollisionMovement())
        if (!coll.IsTouchingLayers(ground))
        {
            JumpMovement();
            return; // 这个return 是为了衔接下面的 落地后逻辑 而写的，如果在下面多加上落地的判断，这里就不用写return
        }

        // 如果刚落地，则判断
        if (ani.GetBool("falling"))
        {
            jumpTime = maxJumpTime; // 必须满足落下来这个条件，是因为如果没有之前的下落条件就重置jumpTime，前面这个touchingLayer 的判断会在起跳之前还粘在地上的时候重置jumpTime
            isJumping = false;
        }

        // 落在地上之后，设置
        ani.SetBool("falling", false);

        //if (isJumping && ani.GetBool("falling")) // 同时满足 isJumping 为true（已起跳）， falling 为true（已下落），else if 保证已经落地接触地面
        //{
        //    Debug.Log("isJumping is false");
        //    isJumping = false;
        //    jumpTime = maxJumpTime;
        //    //ani.SetBool("falling", false);
        //}
        //else // 接触地面后，所有状态归零
        //{
        //    Debug.Log("didn't enter the if");
        //    //ani.SetBool("jumping", false);
        //}
        //ani.SetBool("falling", false);
    }

    // 地面碰撞检测
    //bool CollisionMovement()
    //{
    //    // 这里的贴地会在 jump 之后，跳起之前 仍然有效，所以会出现 jumping 第一次被置true后，因为贴地马上又被置为false 了。导致可以在空中跳第二次。
    //    // 现在这个情况暂时没办法解决，是因为：
    //    // 1. 为了统一速率+让每个space 的输入都有效，必须要用 update 接jump指令，然后用 bool JumpUpdate 传给 FixedUpdate 进行跳跃操作
    //    // 2. 暂时没办法通过改变collider 的形状 来确保跳起来的第一帧就离开了地面，碰撞在所难免
    //    // 3. 无法通过延时判断的方法 等到人物飞起来之后再修改 isJumping。
    //    // 即，以现有的水平无法解决此问题。于是人物在这种情况下就变成能二段跳了
    //    if (coll.IsTouchingLayers(ground))
    //    {
    //        jumpTime = maxJumpTime;
    //        // isJumping = false;            
    //        // ani.SetBool("falling", false);
    //        return true;
    //    }
    //    return false;
    //}

    // 收集item / 掉落重置
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 收集item
        if (collision.CompareTag("Items"))
        {
            Items item = collision.gameObject.GetComponent<Items>();
            if (!item.Destroyed()) // 为了避免item 销毁之后，player 撞击 item 仍然能进入加分逻辑
            {
                score++;
                scoreNumbers.text = ": " + score.ToString();
                //Destroy(collision.gameObject);            
                item.ItemDestroy();
            }
        }


    }

    // 碰撞敌人
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemies")) // 撞到enemy
        {
            if (!ani.GetBool("jumping") && ani.GetBool("falling")) // 下落状态
            {
                // 根据不同enemy 的类型调用他的gettinghit 方法，应该用abstract class + 多态 来做
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
                //hurtAudio.Play();
                SoundManager.instance.HurtAudio();
            }
        }

        // 坠崖重置
        if (collision.gameObject.CompareTag("deadline"))
        {
            isAlive = false;  // 让main camera 停止背景音乐
            //deadAudio.Play(); // 播放死亡音效
            SoundManager.instance.DeadAudio();
            this.GetComponent<BoxCollider2D>().enabled = false; // 避免两次触发trigger 导致播放两次声音
            Invoke("Restart", 2.5f); // 延时复活
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
