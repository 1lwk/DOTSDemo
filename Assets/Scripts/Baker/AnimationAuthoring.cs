using Unity.Entities;
using UnityEngine;

// 定义一个继承自MonoBehaviour的类AnimationAuthoring，用于在Unity编辑器中配置动画参数
public class AnimationAuthoring : MonoBehaviour
{
    // 动画的帧率
    public float frameRate;
    // 动画的最大帧索引
    public int frameMaxIndex;

    // 定义一个嵌套类AnimationBaker，继承自Baker<AnimationAuthoring>，用于将AnimationAuthoring组件的数据转化为实体组件数据
    public class AnimationBaker : Baker<AnimationAuthoring>
    {
        // 重写Bake方法，将AnimationAuthoring的组件数据转化为实体组件数据
        public override void Bake(AnimationAuthoring authoring)
        {
            // 获取一个动态变换的实体
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            // 为实体添加一个AnimationFrameIndex组件
            AddComponent<AnimationFrameIndex>(entity);

            // 为实体添加一个共享组件AnimationSharedData，并设置其frameRate和frameCount属性
            AddSharedComponent<AnimationSharedData>(entity, new AnimationSharedData()
            {
                frameRate = authoring.frameRate,  // 设置共享组件的帧率
                frameCount = authoring.frameMaxIndex,  // 设置共享组件的最大帧索引
            });
        }
    }
}
