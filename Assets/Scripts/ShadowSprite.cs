using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSprite : MonoBehaviour
{

    private Transform player; // ��ȡ���λ�ã�private ����Ϊ������ShadowSprite ��������Ԥ���壬��Ԥ������ʵ��������Ϸ��ʱ��
                              // ��Ҫ�ֶ���קplayer �����λ�ò��ܸ�ֵ����������������ģ��ȶ��ķ�ʽ��player ��ֵ��Ҳ�Ͳ���Ҫpublic ��

    private SpriteRenderer thisSprite; // ��ȡ��ǰ�Լ���Sprite Renderer ���������ȡ��ǰ�Լ���ʾ��ͼ��
    private SpriteRenderer playerSprite; // ��ȡ֮ǰplayer ��Sprite Renderer ���������ȡ��һ֡player ��ͼ��

    private Color color; // ����ͼ���͸���ȼ���ɫ�Ȳ���

    [Header("ʱ����Ʋ���")] // ����������ǣ�������unity ����ʾ����ܹ������� ��ʱ����Ʋ����� Ϊ���ֵ�ר�����࣬���Լ�������unity ��������
    public float activeTime; // ��Ӱ������ʾʱ��
    public float activeStart; // ��ʼ��ʾʱ���

    [Header("��͸���ȿ���")]
    private float alpha; // ���Զ��ı�� ����alpha�����ڲ�����ʹ�õĲ�͸����
    public float alphaSet; // ����ص�ÿ�������ʼ��õ�alpha ֵ�����ֵ���ⲿ��unity �˻�ã�Ҫ���������alpha
    public float alphaMultiplier; // 0-1 ֮���һ������ֵ��alpha ֵ�����Դ�ֵ���Դﵽ͸���ȱ�����С��Ч���������Ļ�ӰЧ����������

    // ��ʹ��Start ��������ShadowSprite ��ֵ������Ϊ����shadowsprite �ᱻ����ʹ�ã�ÿһ��ʹ�ö��ḳֵ
    // ����ץס�����õ�ʱ�������ֵ������ȷ�ĸ�ֵ�㡣�������� OnEnable �����С����ڶ�����У��������еĶ����Ǵ���disable ״̬��ֻ���õ�ʱ��Ż���enable
    // �����������OnEnable ������еı�������ֵ
    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // GameObject ���ã���������ֱ�ӵ����侲̬������
        thisSprite = this.GetComponent<SpriteRenderer>();
        playerSprite = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet; // ���ⲿ�������ӽ���

        // ������һ֡�� player ��������
        thisSprite.sprite = playerSprite.sprite; // sprite ���ͼƬ������ĸ�ֵ���ǰ����ڵ� player sprite ͼƬ����� shadow sprite �������һ֡������ʾ
        this.transform.position = player.transform.position; // ����player ��һ֡��λ��
        this.transform.localScale = player.transform.localScale; // ����player ��һ֡�� ����
        this.transform.rotation = player.transform.rotation; // ����player ��һ֡�� ��ת�Ƕȣ�һ�㲻��仯���˴�Ϊ��ֹ��һ��

        activeStart = Time.time; // ��Ӱ��ʼ��ʾ��ʱ�䣬���ǵ�ǰʱ��
    }

    // update ��ÿһ֡������
    void Update()
    {
        // ����ÿһ֡����alpha ֵ�ͻ���Ӧ�س��ϱ�����С
        alpha *= alphaMultiplier;
        // �Ѹ��µ�alpha д��color ��rgba ��
        color = new Color(1, 1, 1, alpha); // new Color(1, 1, 1, alpha) Ϊԭɫ������͸����ͼ����ɫ�������Ҫƫ�������Խ���ɫ����һЩ����ɫ����ɫ����һЩ������new Color(0.5f, 0.5f, 1, alpha);
        //color = new Color(0.5f, 0.5f, 1, alpha); 
        thisSprite.color = color; // �滻��ǰcolor

        // ����Ƿ��Ѿ���ʱ
        if (Time.time >= activeStart + activeTime)
        {
            // ����������򽫶���Żض���� 
        }
    }
}
