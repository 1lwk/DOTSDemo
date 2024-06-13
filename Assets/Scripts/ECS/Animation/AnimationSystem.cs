using Unity.Burst;
using Unity.Entities;

// 定义部分结构体 AnimationSystem，实现 ISystem 接口
public partial struct AnimationSystem : ISystem
{
    // 定义 OnUpdate 方法，更新系统状态
    public void OnUpdate(ref SystemState state)
    {
        new AnimationJob() // 创建新的 AnimationJob 实例
        {
            detaTime = state.WorldUnmanaged.Time.DeltaTime // 设置 detaTime 为未托管世界时间的 DeltaTime
        }.ScheduleParallel(); // 并行调度 AnimationJob
    }

    [BurstCompile] // 指定该结构体应使用 Burst 编译
    // 定义部分结构体 AnimationJob，实现 IJobEntity 接口 用于编写并行任务的接口，允许你定义一个工作单元，该单元将在每个匹配的实体上执行一次
    public partial struct AnimationJob : IJobEntity
    {
        public float detaTime; // 定义公共浮点变量 detaTime，表示每帧的时间增量

        // 定义 Execute 方法，处理动画帧更新
        public void Execute(in AnimationSharedData animationData, ref AnimationFrameIndex frameIndex)
        {
            float newIndex = frameIndex.value + detaTime * animationData.frameRate; // 计算新的帧索引
            while (newIndex > animationData.frameCount) // 如果新的帧索引超过总帧数
            {
                newIndex -= animationData.frameCount; // 减去总帧数，以保持帧索引在有效范围内
            }
            frameIndex.value = newIndex; // 更新帧索引
        }
    }
}
