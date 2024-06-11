using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

public partial struct RendererSortSystem : ISystem // 定义部分结构体 RendererSortSystem，实现 ISystem 接口
{
    // 系统更新方法
    public void OnUpdate(ref SystemState state)
    {
        new RendererSortJob() { }.ScheduleParallel(); // 调度并行执行 RendererSortJob
    }

    // 使用 Burst 编译
    [BurstCompile]
    public partial struct RendererSortJob : IJobEntity // 定义部分结构体 RendererSortJob，实现 IJobEntity 接口
    {
        // Execute 方法用于处理每个实体
        private void Execute(in RendererSortTag sortTag, ref LocalTransform localTransform)
        {
            localTransform.Position.z = localTransform.Position.y; // 将 Z 轴位置设置为 Y 轴位置，实现渲染排序
        }
    }
}
