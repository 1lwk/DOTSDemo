using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// ����һ���̳���MonoBehaviour����BulletAuthoring��������Unity�༭���������ӵ�����
public class BulletAuthoring : MonoBehaviour
{
    // �ӵ����ƶ��ٶ�
    public float moveSpeed;
    // �ӵ�������ʱ��
    public float destoryTime;

    // ����һ��Ƕ����BulletBaker���̳���Baker<BulletAuthoring>�����ڽ�BulletAuthoring���������ת��Ϊʵ���������
    public class BulletBaker : Baker<BulletAuthoring>
    {
        // ��дBake��������BulletAuthoring���������ת��Ϊʵ���������
        public override void Bake(BulletAuthoring authoring)
        {
            // ��ȡһ����̬�任��ʵ��
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            // Ϊʵ�����һ��RendererSortTag���
            AddComponent<RendererSortTag>(entity);
            // ����RendererSortTag���
            SetComponentEnabled<RendererSortTag>(entity, true);

            // Ϊʵ�����һ��BulletData���������ʼ����destroyTimer����
            AddComponent<BulletData>(entity, new BulletData()
            {
                destroyTimer = authoring.destoryTime  // �����ӵ�����ʱ��
            });
            // ����BulletData���
            SetComponentEnabled<BulletData>(entity, true);

            // ��ȡBoxCollider2D����Ĵ�С������2���õ���ײ��İ��С
            float2 colliderSize = authoring.GetComponent<BoxCollider2D>().size / 2;

            // Ϊʵ�����һ���������BulletSharedData��������������
            AddSharedComponent<BulletSharedData>(entity, new BulletSharedData()
            {
                moveSpeed = authoring.moveSpeed,  // �����ӵ����ƶ��ٶ�
                destroyTimer = authoring.destoryTime,  // �����ӵ�������ʱ��
                colloderOffset = authoring.GetComponent<BoxCollider2D>().offset,  // ������ײ���ƫ����
                colliderHalfExtents = new float3(colliderSize.x, colliderSize.y, 10000)  // ������ײ��İ��С��z�᷽����Ϊ10000
            });
        }
    }
}
