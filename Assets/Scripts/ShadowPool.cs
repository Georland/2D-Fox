using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowPool : MonoBehaviour
{
    // Ϊ��������������������з��ʣ�����ʹ�õ���ģʽ����ʱ����⣩
    public static ShadowPool instance;

    public GameObject shadowPrefab;

    // ϣ����������ж��ٸ�����
    public int shadowCount;

    // ������������Ŷ���ض���ģ���������� �����ݽṹ���� ����
    private Queue<GameObject> availableObjects = new Queue<GameObject>(); 

    private void Awake()
    {
        instance = this;

        // ��ʼ�������
    }

    public void FillPool()
    {
        for (int i = 0; i < shadowCount; i++) 
        {
            var newShadow = Instantiate(shadowPrefab); // ͨ��instanciate ��Ԥ�������ɳ�object ������Ϸ�С�var �ؼ��ֿ��������κ�unity �����е�ʵ��
            // �̶����ɵ�newShadow ���ڵ�λ�ã����涨��parent���Ӷ������������ָ��parent ������Ӽ���
            // ����newShadow �� parent ���ó� this��this �ǹ��ظ�c# �ļ���shadowPool���������Ϊʲô��Ҫtransform�������ǹ̶�������д�õķ���
            newShadow.transform.SetParent(this.transform);

            // ȡ�����ã����ض����
            ReturnPool(newShadow);
        }

    }

    // ��ʹ�����GameObject �Ż�pool ��
    // ���function д�ɵ����ģ�������public�� �Ϳ����������Shadow Sprite ��������ʹ�ã�����������������Return Pool ����
    public void ReturnPool(GameObject shadow)
    {
        shadow.SetActive(false); // shadow ���óɹر�
        availableObjects.Enqueue(shadow); // ���� ����ض��� ��
    }

    // ��pool�� ȡ��һ��GameObject ����ʹ��
    public void GetFromPool()
    {
        if (availableObjects.Count == 0) // ��������������object �������ˣ�����Ҫ�ٴ���䡣�������ֱ��������ķ�ʽ��䣺
        {
            FillPool(); // fillPool ��ȱ���ǣ������� shadowCount �ǹ̶��ģ������п������֮��Ķ�����ȫ��������á����ŵ��ǣ�һ������꣬����ֻҪshadowCount �Ƚ�С��Ҳ�����˷� 
        }

        var newShadow = availableObjects.Dequeue(); // ȡ����var ���Խ���unity �����ڵ�����ʵ��
        newShadow.SetActive(true); // ����newSHadow�����д�shadow �� shadowPrefab ���ɵģ��������� shadowSprite ���ɣ�����active ʱ���Զ�����OnEnable������ʼִ��update��ֱ��ִ�е��Լ�����
                                   // ��仰����ֱ�������ɵ� shadowSprite ȫ�Զ������ ����������ȫ����
    }
}
