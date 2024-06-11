using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour // ���� EnemyAuthoring �࣬�̳��� MonoBehaviour
{
    private Vector3 scale = Vector3.one; // ����˽�б��� scale����ʼֵΪ Vector3.one
    public float moveSpeed = 1; // ���幫������ moveSpeed����ʼֵΪ 1

    /// <summary>
    /// ����Ƕ���� EnemyBaker���̳��� Baker<EnemyAuthoring>
    /// </summary>
    public class EnemyBaker : Baker<EnemyAuthoring> 
    {
        /// <summary>
        ///  ��д Bake ����������Ϊ EnemyAuthoring
        /// </summary>
        /// <param name="authoring"></param>
        public override void Bake(EnemyAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic); // ��ȡʵ�壬ʹ�ö�̬�任��־
            AddComponent<RendererSortTag>(entity); // Ϊʵ����� RendererSortTag ���
            SetComponentEnabled<RendererSortTag>(entity, true); // ����ʵ��� RendererSortTag ���

            AddComponent<EnemyData>(entity, new EnemyData() { die = false }); // Ϊʵ����� EnemyData �������ʼ�� die Ϊ false
            SetComponentEnabled<EnemyData>(entity, true); // ����ʵ��� EnemyData ���

            AddSharedComponent<EnemySharedData>(entity, new EnemySharedData() // Ϊʵ����ӹ������ EnemySharedData
            {
                moveSpeed = authoring.moveSpeed, // ���� moveSpeed Ϊ authoring.moveSpeed
                scale = (Vector2)authoring.transform.localScale // ���� scale Ϊ authoring.transform.localScale �Ķ�ά����
            });
        }
    }
}
