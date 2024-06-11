using Unity.Entities;
using Unity.Mathematics;

// 定义 BulletSharedData 结构体，实现 ISharedComponentData 接口
public struct BulletSharedData : ISharedComponentData
{
    public float moveSpeed; // 定义公共浮点变量 moveSpeed，表示子弹的移动速度
    public float destroyTimer; // 定义公共浮点变量 destroyTimer，表示子弹销毁的计时器
    public float2 colloderOffset; // 定义公共变量 colloderOffset，类型为 float2，表示碰撞器的偏移
    public float3 colliderHalfExtents; // 定义公共变量 colliderHalfExtents，类型为 float3，表示碰撞器的半尺寸
}
