using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// 定义一个继承自MonoBehaviour的类BulletAuthoring，用于在Unity编辑器中配置子弹参数
public class BulletAuthoring : MonoBehaviour
{
    // 子弹的移动速度
    public float moveSpeed;
    // 子弹的销毁时间
    public float destoryTime;

    // 定义一个嵌套类BulletBaker，继承自Baker<BulletAuthoring>，用于将BulletAuthoring组件的数据转化为实体组件数据
    public class BulletBaker : Baker<BulletAuthoring>
    {
        // 重写Bake方法，将BulletAuthoring的组件数据转化为实体组件数据
        public override void Bake(BulletAuthoring authoring)
        {
            // 获取一个动态变换的实体
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            // 为实体添加一个RendererSortTag组件
            AddComponent<RendererSortTag>(entity);
            // 启用RendererSortTag组件
            SetComponentEnabled<RendererSortTag>(entity, true);

            // 为实体添加一个BulletData组件，并初始化其destroyTimer属性
            AddComponent<BulletData>(entity, new BulletData()
            {
                destroyTimer = authoring.destoryTime  // 设置子弹销毁时间
            });
            // 启用BulletData组件
            SetComponentEnabled<BulletData>(entity, true);

            // 获取BoxCollider2D组件的大小并除以2，得到碰撞体的半大小
            float2 colliderSize = authoring.GetComponent<BoxCollider2D>().size / 2;

            // 为实体添加一个共享组件BulletSharedData，并设置其属性
            AddSharedComponent<BulletSharedData>(entity, new BulletSharedData()
            {
                moveSpeed = authoring.moveSpeed,  // 设置子弹的移动速度
                destroyTimer = authoring.destoryTime,  // 设置子弹的销毁时间
                colloderOffset = authoring.GetComponent<BoxCollider2D>().offset,  // 设置碰撞体的偏移量
                colliderHalfExtents = new float3(colliderSize.x, colliderSize.y, 10000)  // 设置碰撞体的半大小，z轴方向设为10000
            });
        }
    }
}
