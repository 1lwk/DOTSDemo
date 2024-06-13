using Unity.Burst;
using Unity.Entities;

// ���岿�ֽṹ�� AnimationSystem��ʵ�� ISystem �ӿ�
public partial struct AnimationSystem : ISystem
{
    // ���� OnUpdate ����������ϵͳ״̬
    public void OnUpdate(ref SystemState state)
    {
        new AnimationJob() // �����µ� AnimationJob ʵ��
        {
            detaTime = state.WorldUnmanaged.Time.DeltaTime // ���� detaTime Ϊδ�й�����ʱ��� DeltaTime
        }.ScheduleParallel(); // ���е��� AnimationJob
    }

    [BurstCompile] // ָ���ýṹ��Ӧʹ�� Burst ����
    // ���岿�ֽṹ�� AnimationJob��ʵ�� IJobEntity �ӿ� ���ڱ�д��������Ľӿڣ������㶨��һ��������Ԫ���õ�Ԫ����ÿ��ƥ���ʵ����ִ��һ��
    public partial struct AnimationJob : IJobEntity
    {
        public float detaTime; // ���幫��������� detaTime����ʾÿ֡��ʱ������

        // ���� Execute ������������֡����
        public void Execute(in AnimationSharedData animationData, ref AnimationFrameIndex frameIndex)
        {
            float newIndex = frameIndex.value + detaTime * animationData.frameRate; // �����µ�֡����
            while (newIndex > animationData.frameCount) // ����µ�֡����������֡��
            {
                newIndex -= animationData.frameCount; // ��ȥ��֡�����Ա���֡��������Ч��Χ��
            }
            frameIndex.value = newIndex; // ����֡����
        }
    }
}
