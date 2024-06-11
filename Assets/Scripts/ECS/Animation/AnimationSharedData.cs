using Unity.Entities;

// 定义一个共享组件 AnimationSharedData
public struct AnimationSharedData : ISharedComponentData
{
    public float frameRate; // 定义一个公共浮点变量 frameRate，表示动画帧速率
    public int frameCount; // 定义一个公共整数变量 frameCount，表示动画帧的数量
}
