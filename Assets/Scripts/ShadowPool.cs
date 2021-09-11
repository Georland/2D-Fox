using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowPool : MonoBehaviour
{
    // 为了允许其他代码对他进行访问，所以使用单例模式（暂时不理解）
    public static ShadowPool instance;

    public GameObject shadowPrefab;

    // 希望对象池中有多少个对象
    public int shadowCount;

    // 队列是用来存放对象池对象的，整个对象池 的数据结构就是 队列
    private Queue<GameObject> availableObjects = new Queue<GameObject>(); 

    private void Awake()
    {
        instance = this;

        // 初始化对象池
    }

    public void FillPool()
    {
        for (int i = 0; i < shadowCount; i++) 
        {
            var newShadow = Instantiate(shadowPrefab); // 通过instanciate 把预制体生成成object 放入游戏中。var 关键字可以引用任何unity 场景中的实体
            // 固定生成的newShadow 所在的位置，即规定其parent，从而可以让他变成指定parent 下面的子集。
            // 这里newShadow 的 parent 设置成 this，this 是挂载该c# 文件的shadowPool。（不理解为什么需要transform，可能是固定在这里写好的方法
            newShadow.transform.SetParent(this.transform);

            // 取消启用，返回对象池
            ReturnPool(newShadow);
        }

    }

    // 将使用完的GameObject 放回pool 中
    // 这个function 写成单例的，还加上public， 就可以让外面的Shadow Sprite 对他进行使用，比如在外面调用这个Return Pool 函数
    public void ReturnPool(GameObject shadow)
    {
        shadow.SetActive(false); // shadow 设置成关闭
        availableObjects.Enqueue(shadow); // 放入 对象池队列 中
    }

    // 从pool中 取出一个GameObject 进行使用
    public void GetFromPool()
    {
        if (availableObjects.Count == 0) // 如果池子里的所有object 都用完了，就需要再次填充。这里可以直接用上面的方式填充：
        {
            FillPool(); // fillPool 的缺点是，填充的量 shadowCount 是固定的，所以有可能填充之后的对象不能全部充分利用。但优点是，一次添加完，所以只要shadowCount 比较小，也不会浪费 
        }

        var newShadow = availableObjects.Dequeue(); // 取出，var 可以接收unity 场景内的任意实体
        newShadow.SetActive(true); // 激活newSHadow，又有此shadow 是 shadowPrefab 生成的，本质上由 shadowSprite 生成，所以active 时会自动调用OnEnable，并开始执行update，直到执行到自己销毁
                                   // 这句话可以直接让生成的 shadowSprite 全自动地完成 从生到死的全过程
    }
}
