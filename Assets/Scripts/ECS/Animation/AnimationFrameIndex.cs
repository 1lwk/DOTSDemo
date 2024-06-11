using Unity.Entities;
using Unity.Rendering;

// 定义一个材质属性 "_Index"
[MaterialProperty("_Index")]
public struct AnimationFrameIndex : IComponentData, IEnableableComponent // 定义 AnimationFrameIndex 结构体，实现 IComponentData 和 IEnableableComponent 接口
{
    public float value; // 定义一个公共浮点变量 value，表示动画帧索引
}
