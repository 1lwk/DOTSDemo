using Unity.Entities;

// 定义 EnemyData 结构体，实现 IComponentData 和 IEnableableComponent 接口
public struct EnemyData : IComponentData, IEnableableComponent
{
    public bool die; // 定义公共布尔变量 die，表示敌人是否死亡
}
