using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct EnemySystem : ISystem // ���岿�ֽṹ�� EnemySystem��ʵ�� ISystem �ӿ�
{
    // �����������ṹ�壬���ڹ���̬����
    //������ key1��key2 �� key3 ��������Ϊ SharedStatic ��̬�����ļ��ġ�
    //SharedStatic �� Unity ��һ�����ԣ������ڶ����ҵ���߳�֮�乲��̬���ݡ�
    //��Щ���ṹ��ʵ������Ψһ��ʶ���������ֲ�ͬ�� SharedStatic ʵ����
    public struct key1 { }
    public struct key2 { }
    public struct key3 { }

    // ���干��̬����
    public readonly static SharedStatic<int> createdCount = SharedStatic<int>.GetOrCreate<key1>();
    public readonly static SharedStatic<int> createCount = SharedStatic<int>.GetOrCreate<key2>();
    public readonly static SharedStatic<Random> random = SharedStatic<Random>.GetOrCreate<key3>();

    public float spawnEnemyTimer; // �������ɵ��˵ļ�ʱ��
    public const int maxEnemys = 10000; // ������������

    // ϵͳ����ʱ�ĳ�ʼ������
    public void OnCreate(ref SystemState state)
    {
        createdCount.Data = 0; // ��ʼ���Ѵ���������
        createCount.Data = 0; // ��ʼ��������������
        random.Data = new Random((uint)System.DateTime.Now.GetHashCode()); // ��ʼ�������������
        SharedData.GameSharedInfo.Data.deadCounter = 0; // ��ʼ������������
    }

    // ϵͳ���·���
    public void OnUpdate(ref SystemState state)
    {
        spawnEnemyTimer -= SystemAPI.Time.DeltaTime; // �������ɵ��˵ļ�ʱ��
        if (spawnEnemyTimer <= 0)
        {
            spawnEnemyTimer = SharedData.GameSharedInfo.Data.spawnInterval; // �������ɼ�ʱ��
            createCount.Data += SharedData.GameSharedInfo.Data.spawnCount; // ���Ӵ�����������
        }

        // ��ȡ�����������
        EntityCommandBuffer.ParallelWriter ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        float2 playerPos = SharedData.playerPos.Data; // ��ȡ���λ��

        // ���Ȳ���ִ�� EnemyJob
        new EnemyJob()
        {
            detaTime = SystemAPI.Time.DeltaTime, // ÿ֡ʱ������
            playerPos = playerPos, // ���λ��
            ecb = ecb, // �����������
            time = SystemAPI.Time.ElapsedTime, // �Ѿ�����ʱ��
        }.ScheduleParallel();
        state.CompleteDependency(); // �������

        // ����д������ĵ�����δ�ﵽ��������
        if (createCount.Data > 0 && createdCount.Data < maxEnemys)
        {
            NativeArray<Entity> newEnemys = new NativeArray<Entity>(createCount.Data, Allocator.Temp); // �����µĵ���ʵ������
            ecb.Instantiate(int.MinValue, SystemAPI.GetSingleton<GameConfigData>().enemyPortotype, newEnemys); // ʵ�����µĵ���

            for (int i = 0; i < newEnemys.Length && createdCount.Data < maxEnemys; i++)
            {
                createdCount.Data += 1; // �����Ѵ���������
                float2 offset = random.Data.NextFloat2Direction() * random.Data.NextFloat(5f, 10); // �������ƫ����
                ecb.SetComponent<LocalTransform>(newEnemys[i].Index, newEnemys[i], new LocalTransform()
                {
                    Position = new float3(playerPos.x + offset.x, playerPos.y + offset.y, 0), // ���õ���λ��
                    Rotation = quaternion.identity, // ���õ�����ת
                    Scale = 1, // ���õ�������
                });
            }
            createCount.Data = 0; // ���ô�����������
            newEnemys.Dispose(); // �ͷ��µ�������
        }
    }

    // ʹ�� EntityQueryOptions.IgnoreComponentEnabledState �����������״̬
    // ʹ��BurstCompile���� ����Unity Burst�������Ż�����ִ������
    // Burst�������Ὣ��ǵĴ������ɸ߶��Ż��ı��ش��루native code��
    // ���������ʱ������ ��Ƶ��ִ�еĴ���Ƭ������Ϊ��Ҫ
    // ��Ϸ������� ���ݼ��� ·������
    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    [BurstCompile]
    public partial struct EnemyJob : IJobEntity // ���岿�ֽṹ�� EnemyJob��ʵ�� IJobEntity �ӿ�
    {
        public float detaTime; // ÿ֡ʱ������
        public double time; // �Ѿ�����ʱ��
        public float2 playerPos; // ���λ��
        public EntityCommandBuffer.ParallelWriter ecb; // �����������

        // Execute �������ڴ���ÿ��ʵ��
        private void Execute(
            EnabledRefRW<EnemyData> enableState,
            EnabledRefRW<RendererSortTag> rendererSortEnableState,
            EnabledRefRW<AnimationFrameIndex> aniamtionEnableState,
            ref EnemyData enemyData,
            in EnemySharedData enemySharedData,
            ref LocalTransform localTransform,
            ref LocalToWorld localToWorld)
        {
            // �����ǰ�����ǷǼ���״̬��ͬʱ��Ҫ��������
            if (enableState.ValueRO == false)
            {
                if (createCount.Data > 0)
                {
                    createCount.Data -= 1; // ���´�����������
                    float2 offset = random.Data.NextFloat2Direction() * random.Data.NextFloat(5f, 10); // �������ƫ����
                    localTransform.Position = new float3(playerPos.x + offset.x, playerPos.y + offset.y, 0); // ���õ���λ��
                    enableState.ValueRW = true; // ���õ���
                    rendererSortEnableState.ValueRW = true; // ������Ⱦ�����ǩ
                    aniamtionEnableState.ValueRW = true; // ���ö���֡����
                    localTransform.Scale = 1; // ���õ�������
                }
                return;
            }

            // �����������
            if (enemyData.die)
            {
                SharedData.GameSharedInfo.Data.playHitAudio = true; // ���Ż�����Ч
                SharedData.GameSharedInfo.Data.deadCounter += 1; // ��������������
                SharedData.GameSharedInfo.Data.playHitAudioTime += time; // ���»�����Чʱ��
                enemyData.die = false; // ���õ���������־
                enableState.ValueRW = false; // ���õ���
                rendererSortEnableState.ValueRW = false; // ������Ⱦ�����ǩ
                aniamtionEnableState.ValueRW = false; // ���ö���֡����
                localTransform.Scale = 0; // ����������Ϊ 0
                localToWorld.Value.c1.y = enemySharedData.scale.y; // ���� LocalToWorld ������
                return;
            }

            // �����ƶ��߼�
            float2 dir = math.normalize(playerPos - new float2(localTransform.Position.x, localTransform.Position.y)); // ���㷽������
            localTransform.Position += detaTime * enemySharedData.moveSpeed * new float3(dir.x, dir.y, 0); // ���µ���λ��

            // ���� LocalToWorld ������
            localToWorld.Value.c0.x = localTransform.Position.x < playerPos.x ? -enemySharedData.scale.x : enemySharedData.scale.x;
            localToWorld.Value.c1.y = enemySharedData.scale.y;
        }
    }
}
