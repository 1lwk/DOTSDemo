using Unity.Entities;

// 定义 GameConfigData 结构体，实现 IComponentData 接口
public struct GameConfigData : IComponentData
{
    public Entity bulletPortotype; // 定义公共变量 bulletPortotype，类型为 Entity，表示子弹的原型实体
    public Entity enemyPortotype; // 定义公共变量 enemyPortotype，类型为 Entity，表示敌人的原型实体
}
