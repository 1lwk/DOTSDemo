using Unity.Entities;
using Unity.Mathematics;

// 定义 BulletCreateInfo 结构体，实现 IBufferElementData 接口
public struct BulletCreateInfo : IBufferElementData
{
    public float3 position; // 定义公共变量 position，类型为 float3，表示子弹的位置
    public quaternion rotation; // 定义公共变量 rotation，类型为 quaternion，表示子弹的旋转
}
