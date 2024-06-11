using Unity.Entities;

// 定义 BulletData 结构体，实现 IComponentData 和 IEnableableComponent 接口
public struct BulletData : IComponentData, IEnableableComponent
{
    public float destroyTimer; // 定义公共浮点变量 destroyTimer，表示子弹销毁的计时器
}
