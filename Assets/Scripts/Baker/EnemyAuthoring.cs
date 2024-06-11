using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour // 定义 EnemyAuthoring 类，继承自 MonoBehaviour
{
    private Vector3 scale = Vector3.one; // 定义私有变量 scale，初始值为 Vector3.one
    public float moveSpeed = 1; // 定义公共变量 moveSpeed，初始值为 1

    /// <summary>
    /// 定义嵌套类 EnemyBaker，继承自 Baker<EnemyAuthoring>
    /// </summary>
    public class EnemyBaker : Baker<EnemyAuthoring> 
    {
        /// <summary>
        ///  重写 Bake 方法，参数为 EnemyAuthoring
        /// </summary>
        /// <param name="authoring"></param>
        public override void Bake(EnemyAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic); // 获取实体，使用动态变换标志
            AddComponent<RendererSortTag>(entity); // 为实体添加 RendererSortTag 组件
            SetComponentEnabled<RendererSortTag>(entity, true); // 启用实体的 RendererSortTag 组件

            AddComponent<EnemyData>(entity, new EnemyData() { die = false }); // 为实体添加 EnemyData 组件，初始化 die 为 false
            SetComponentEnabled<EnemyData>(entity, true); // 启用实体的 EnemyData 组件

            AddSharedComponent<EnemySharedData>(entity, new EnemySharedData() // 为实体添加共享组件 EnemySharedData
            {
                moveSpeed = authoring.moveSpeed, // 设置 moveSpeed 为 authoring.moveSpeed
                scale = (Vector2)authoring.transform.localScale // 设置 scale 为 authoring.transform.localScale 的二维向量
            });
        }
    }
}
