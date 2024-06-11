using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

public partial struct RendererSortSystem : ISystem // ���岿�ֽṹ�� RendererSortSystem��ʵ�� ISystem �ӿ�
{
    // ϵͳ���·���
    public void OnUpdate(ref SystemState state)
    {
        new RendererSortJob() { }.ScheduleParallel(); // ���Ȳ���ִ�� RendererSortJob
    }

    // ʹ�� Burst ����
    [BurstCompile]
    public partial struct RendererSortJob : IJobEntity // ���岿�ֽṹ�� RendererSortJob��ʵ�� IJobEntity �ӿ�
    {
        // Execute �������ڴ���ÿ��ʵ��
        private void Execute(in RendererSortTag sortTag, ref LocalTransform localTransform)
        {
            localTransform.Position.z = localTransform.Position.y; // �� Z ��λ������Ϊ Y ��λ�ã�ʵ����Ⱦ����
        }
    }
}
