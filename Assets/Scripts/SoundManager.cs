using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  ע�⣬������������װ����Ƶ�ĺ���ƣ���Ϊ�����Ǻ�����ͬʱ���ֵģ������������������޷�ʵ��ͬʱ���֡�
 *  ������ʵ����������չʾ �������ģʽ ���ŵ㣬���ŵ�Ϊ��
 *  �����ط����ܷ���������д���Լ��ĵ���ı���һ�����ø÷���������������д���ҷ��������Ѷ������Դ��find �����ϣ��ٵ��� 
 *  ȱ�����Լ��Ǿ�̬static �ģ����Ի�һֱ������ڴ��ռ���ڴ�ռ䡣��ռ�õ���С��ջ�ڴ棬���Ǵ�Ķ��ڴ棬��͵��µ���ռ�õ��ڴ�ռ�����ܴ���һ������˵���������ʲô����¶��������ã�
 *  ����������Դ�ϵ�ȱ�㻹�У����е�һ��������Դ�������޷�ͬʱ���Ų�ͬ�������޷�������Ҫѭ�����ŵ��������޷�����������Ƶ�Ĳ��������������ȵȣ�ֻ�ܶ�������Ƶ����ͳһ�Ĳ�������
 *  ��һ������Դ�Ľ���취�� �������д����audioSource�����Ӳ�ͬ��clip ���ɣ�Ȼ��bgm �Ͱ���bgm �����ã���������loop �� playOnAwake ��
 */

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; // д�ɵ����ģ��ܹ��ø��ֲ�ͬ��Դͷ����������������ͱ������Ƶ�ļ���

    public AudioSource audioSource;      // ��һ������

    [SerializeField]    // ����ǿ����ñ����� private ������£�unity �Ǳ���Ȼ�ܿ�������ק��ֵ�ı�ע��
    private AudioClip jumpAudio, hurtAudio, deadAudio; // ������Դ��
                                                       // ע�⣺����ģʽֻ��һ�����ų��ڣ�����������װ��һֱ��Ҫѭ�����ŵ����������米��bgm��
                                                       // Ҳ����û�а취���ſ��ܻ�ͬʱ���ֵ����������缺�������͵з�����

    private void Awake()
    {
        instance = this; // �����ĳ�ʼ��
    }

    public void JumpAudio() // public ��Ϊ�����������Ķ���ʹ�ô˷���
    {
        audioSource.clip = jumpAudio; // ��jumpAudio װ��audioSource��Ȼ��play audioSource�������Ľṹ�ܱ�֤ͳһ����audioSources��ȱ���ǡ�һ��ֻ�ܲ�һ����Ƶ��
        audioSource.Play();
    }

    public void HurtAudio() // public ��Ϊ�����������Ķ���ʹ�ô˷���
    {
        audioSource.clip = hurtAudio; 
        audioSource.Play();
    }

    public void DeadAudio() // public ��Ϊ�����������Ķ���ʹ�ô˷���
    {
        audioSource.clip = deadAudio; 
        audioSource.Play();
    }
}
