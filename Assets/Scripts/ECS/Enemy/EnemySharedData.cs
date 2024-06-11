using Unity.Entities;
using Unity.Mathematics;

// 定义 EnemySharedData 结构体，实现 ISharedComponentData 接口
public struct EnemySharedData : ISharedComponentData
{
    public float moveSpeed; // 定义公共浮点变量 moveSpeed，表示敌人的移动速度
    public float2 scale; // 定义公共变量 scale，类型为 float2，表示敌人的缩放比例
}
