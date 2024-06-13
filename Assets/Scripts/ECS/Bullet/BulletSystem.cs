using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

public partial struct BulletSystem : ISystem // ���岿�ֽṹ�� BulletSystem��ʵ�� ISystem �ӿ�
{
    // ���徲ֻ̬������̬���� createBulletCountʹ��GetOrCreate���Ա����δ��� 
    // readonly static ��ֻ̬�� GetOrCreate<BulletSystem>()�������ȡһ�� "SharedStatic"��ʵ�� ��BulletSystem��� ��֤��BulletSystem����ͬһ����̬����
    public readonly static SharedStatic<int> createBulletCount = SharedStatic<int>.GetOrCreate<BulletSystem>();

    // ����ϵͳ����ʱ�ĳ�ʼ������ SystemStateϵͳ���е�״̬�Լ��������
    public void OnCreate(ref SystemState state)
    {
        // Data �������ڷ��ʻ��޸� SharedStatic<int> ʵ���е�ʵ������ֵ
        createBulletCount.Data = 0; // ��ʼ�� createBulletCount Ϊ 0
        SharedData.singtonEntity.Data = state.EntityManager.CreateEntity(typeof(BulletCreateInfo)); // ���� BulletCreateInfo ���͵�ʵ��
    }

    // ����ϵͳ���·���
    public void OnUpdate(ref SystemState state)
    {
        // ��ȡ����������� ����ʵ����ɾ��ʵ�� 
        EntityCommandBuffer.ParallelWriter ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        // ��ȡ BulletCreateInfo ������ DynamicBuffer�����ڴ���̬�����һ���ṹ ����Ϊÿ��ʵ��洢һ����̬��С�Ļ�����
        DynamicBuffer<BulletCreateInfo> bulletCreateInfoBuffer = SystemAPI.GetSingletonBuffer<BulletCreateInfo>();
        createBulletCount.Data = bulletCreateInfoBuffer.Length; // �����ӵ���������

        // ���Ȳ���ִ�� BulletJob
        new BulletJob()
        {
            enemyLayerMask = 1 << 6, // ���˲�����
            ecb = ecb, // �����������
            deltaTime = SystemAPI.Time.DeltaTime, // ÿ֡ʱ������ ��Time.DeltaTime
            bulletCreateInfoBuffer = bulletCreateInfoBuffer, // �ӵ�������Ϣ������
            collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld, // ��ײ���� һ���������ʵ��֮������ײ ȷ�������Ƿ��ڽӴ������໥Ӱ�� ����������ʹ��
        }.ScheduleParallel();//�Զ�ʹ�õ�ǰϵͳ������������job�ĵ���
        state.CompleteDependency(); // ������� ������ʽ����ɵ�ǰϵͳ��������������(jobs��

        // �����Ҫ�������ӵ��������� 0
        if (createBulletCount.Data > 0)
        {
            // �����µ��ӵ�ʵ������
            NativeArray<Entity> newBullets = new NativeArray<Entity>(createBulletCount.Data, Allocator.Temp); //����1����ĳ��� ����2��η����ڴ�����ָ������ʱ�����ڴ� ֻ�ڵ�ǰ֡��Ч Ȼ��ͬһ֡�ͷ�
            ecb.Instantiate(int.MinValue, SystemAPI.GetSingleton<GameConfigData>().bulletPortotype, newBullets); // ʵ�����µ��ӵ� ��һ������SortKey ָ�����������˳�� ԽСԽ��ִ��

            // ��ʼ��ÿ�����ӵ��� Transform ���
            for (int i = 0; i < newBullets.Length; i++)
            {
                BulletCreateInfo info = bulletCreateInfoBuffer[i];
                ecb.SetComponent<LocalTransform>(newBullets[i].Index, newBullets[i], new LocalTransform()
                {
                    Position = info.position,
                    Rotation = info.rotation,
                    Scale = 1
                });
            }
            newBullets.Dispose(); // �ͷ����ӵ�����
        }

        bulletCreateInfoBuffer.Clear(); // ����ӵ�������Ϣ������
    }

    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)] // �����������״̬��ʵ���ѯѡ��
    [BurstCompile] // ʹ�� Burst ����
    public partial struct BulletJob : IJobEntity // ���岿�ֽṹ�� BulletJob��ʵ�� IJobEntity �ӿ�
    {
        public uint enemyLayerMask; // ���˲�����
        public EntityCommandBuffer.ParallelWriter ecb; // �����������
        public float deltaTime; // ÿ֡ʱ������
        [ReadOnly] public DynamicBuffer<BulletCreateInfo> bulletCreateInfoBuffer; // ֻ�����ӵ�������Ϣ������
        [ReadOnly] public CollisionWorld collisionWorld; // ֻ������ײ����

        public void Execute(
            EnabledRefRW<BulletData> bulletEnableState,
            EnabledRefRW<RendererSortTag> sortEnableState,
            ref BulletData bulletData,
            in BulletSharedData bulletSharedData,
            in Entity entity,
            ref LocalTransform localTransform)
        {
            // �����ǰ�ӵ��ǷǼ���״̬��ͬʱ��Ҫ�����ӵ�
            if (bulletEnableState.ValueRO == false)
            {
                if (createBulletCount.Data > 0)
                {
                    int index = createBulletCount.Data -= 1; // �����ӵ���������
                    bulletEnableState.ValueRW = true; // �����ӵ�����ʱ����ʱ��
                    sortEnableState.ValueRW = true; // ������Ⱦ�����ǩ
                    localTransform.Position = bulletCreateInfoBuffer[index].position; // �����ӵ�λ��
                    localTransform.Rotation = bulletCreateInfoBuffer[index].rotation; // �����ӵ���ת
                    localTransform.Scale = 1; // �����ӵ�����
                    bulletData.destroyTimer = bulletSharedData.destroyTimer; // ��ʼ�����ټ�ʱ��
                }
                return;
            }

            // �ӵ�λ���ƶ�
            localTransform.Position += bulletSharedData.moveSpeed * deltaTime * localTransform.Up();

            // �������ټ�ʱ��
            bulletData.destroyTimer -= deltaTime;
            if (bulletData.destroyTimer <= 0)
            {
                bulletEnableState.ValueRW = false; // �����ӵ�
                sortEnableState.ValueRW = false; // ������Ⱦ�����ǩ
                localTransform.Scale = 0; // ����������Ϊ 0
                return;
            }

            // �����˺����
            NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);
            // ���������ռ��µ�һ���ṹ�� ��������������ײ���˹����
            CollisionFilter filter = new CollisionFilter()
            {
                BelongsTo = ~0u, // ���������Ĳ�
                CollidesWith = enemyLayerMask, // �����������Щ�㷢����ײ
                GroupIndex = 0 // 0��ʾû���������ײ����
            };

            // ����Ƿ��������ײ
            if (collisionWorld.OverlapBox(localTransform.Position, localTransform.Rotation, bulletSharedData.colliderHalfExtents, ref hits, filter))
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    Entity temp = hits[i].Entity; // ��ȡ��ײʵ��
                    bulletData.destroyTimer = 0; // �����ټ�ʱ������Ϊ 0
                    ecb.SetComponent<EnemyData>(temp.Index, temp, new EnemyData()
                    {
                        die = true, // ���õ���������־
                    });
                }
            }
            hits.Dispose(); // �ͷ������б�
        }
    }
}
