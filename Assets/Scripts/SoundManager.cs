using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  注意，该样例并不是装载音频的好设计，因为声音是很容易同时出现的，单例反而会让声音无法实现同时出现。
 *  该例子实际上是意在展示 单例设计模式 的优点，其优点为：
 *  各个地方都能方便地像调用写在自己文档里的变量一样调用该方法，而不用特意写查找方法，花费额外的资源在find 方法上，再调用 
 *  缺点是自己是静态static 的，所以会一直存放在内存里，占用内存空间。且占用的是小的栈内存，而非大的堆内存，这就导致单例占用的内存空间比例很大（这一点决定了单例无论在什么情况下都不能滥用）
 *  这里用在音源上的缺点还有，仅有单一发声的音源，导致无法同时播放不同声音，无法播放需要循环播放的声音，无法调整单个音频的参数，例如音量等等，只能对所有音频进行统一的参数调节
 *  单一发声音源的解决办法： 在这里多写几个audioSource，附加不同的clip 即可，然后bgm 就按照bgm 的设置，外面设置loop 和 playOnAwake 等
 */

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; // 写成单例的，能够让各种不同的源头调用这个组件，这里就变成了音频的集合

    public AudioSource audioSource;      // 单一播放器

    [SerializeField]    // 这个是可以让变量在 private 的情况下，unity 那边仍然能看到并拖拽赋值的标注。
    private AudioClip jumpAudio, hurtAudio, deadAudio; // 所有音源。
                                                       // 注意：单例模式只有一个播放出口，导致他不能装入一直需要循环播放的声音，比如背景bgm；
                                                       // 也导致没有办法播放可能会同时出现的声音，比如己方声音和敌方声音

    private void Awake()
    {
        instance = this; // 单例的初始化
    }

    public void JumpAudio() // public 是为了能让其他的对象使用此方法
    {
        audioSource.clip = jumpAudio; // 把jumpAudio 装给audioSource，然后play audioSource。这样的结构能保证统一管理audioSources，缺点是【一次只能播一个音频】
        audioSource.Play();
    }

    public void HurtAudio() // public 是为了能让其他的对象使用此方法
    {
        audioSource.clip = hurtAudio; 
        audioSource.Play();
    }

    public void DeadAudio() // public 是为了能让其他的对象使用此方法
    {
        audioSource.clip = deadAudio; 
        audioSource.Play();
    }
}
