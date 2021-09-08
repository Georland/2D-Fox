using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSprite : MonoBehaviour
{

    private Transform player; // 获取玩家位置，private 是因为，本身ShadowSprite 将被做成预制体，但预制体变成实体参与进游戏的时候，
                              // 需要手动拖拽player 到这个位置才能赋值。所以这里用另外的，稳定的方式给player 赋值，也就不需要public 了

    private SpriteRenderer thisSprite; // 获取当前自己的Sprite Renderer 组件，以拿取当前自己显示的图像
    private SpriteRenderer playerSprite; // 获取之前player 的Sprite Renderer 组件，以拿取上一帧player 的图像

    private Color color; // 调整图像的透明度及颜色等参数

    [Header("时间控制参数")] // 这个的作用是，在外面unity 的显示栏里，能够看到以 “时间控制参数” 为名字的专门名类，是自己制作的unity 变量隔间
    public float activeTime; // 幻影持续显示时间
    public float activeStart; // 开始显示时间点

    [Header("不透明度控制")]
    private float alpha; // 可自动改变的 内置alpha，即内部最终使用的不透明度
    public float alphaSet; // 对象池的每个对象初始获得的alpha 值，这个值从外部的unity 端获得，要赋给上面的alpha
    public float alphaMultiplier; // 0-1 之间的一个倍数值，alpha 值将乘以此值，以达到透明度倍数缩小的效果，创建的幻影效果会相对柔和

    // 不使用Start 给单个的ShadowSprite 赋值，是因为单个shadowsprite 会被反复使用，每一次使用都会赋值
    // 所以抓住他启用的时候给他赋值是最正确的赋值点。这个点就在 OnEnable 函数中。且在对象池中，本身所有的对象都是处于disable 状态，只有用的时候才会变成enable
    // 所以这里就在OnEnable 里给所有的变量赋初值
    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // GameObject 调用，属于是类直接调用其静态方法了
        thisSprite = this.GetComponent<SpriteRenderer>();
        playerSprite = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet; // 把外部设置量加进来

        // 保存这一帧的 player 各项数据
        thisSprite.sprite = playerSprite.sprite; // sprite 存放图片，这里的赋值，是把现在的 player sprite 图片存进该 shadow sprite 里，以在下一帧进行显示
        this.transform.position = player.transform.position; // 保存player 这一帧的位置
        this.transform.localScale = player.transform.localScale; // 保存player 这一帧的 朝向
        this.transform.rotation = player.transform.rotation; // 保存player 这一帧的 旋转角度（一般不会变化，此处为防止万一）

        activeStart = Time.time; // 幻影开始显示的时间，就是当前时间
    }

    // update 在每一帧被调用
    void Update()
    {
        // 更新每一帧，其alpha 值就会相应地乘上倍率缩小
        alpha *= alphaMultiplier;
        // 把更新的alpha 写入color 的rgba 里
        color = new Color(1, 1, 1, alpha); // new Color(1, 1, 1, alpha) 为原色，仅变透明的图像颜色，如果需要偏蓝，可以将蓝色拉多一些，红色和绿色变少一些，例如new Color(0.5f, 0.5f, 1, alpha);
        //color = new Color(0.5f, 0.5f, 1, alpha); 
        thisSprite.color = color; // 替换当前color

        // 检测是否已经超时
        if (Time.time >= activeStart + activeTime)
        {
            // 如果超过，则将对象放回对象池 
        }
    }
}
