using Unity.Entities;
using UnityEngine;

// ����һ���̳���MonoBehaviour����AnimationAuthoring��������Unity�༭�������ö�������
public class AnimationAuthoring : MonoBehaviour
{
    // ������֡��
    public float frameRate;
    // ���������֡����
    public int frameMaxIndex;

    // ����һ��Ƕ����AnimationBaker���̳���Baker<AnimationAuthoring>�����ڽ�AnimationAuthoring���������ת��Ϊʵ���������
    public class AnimationBaker : Baker<AnimationAuthoring>
    {
        // ��дBake��������AnimationAuthoring���������ת��Ϊʵ���������
        public override void Bake(AnimationAuthoring authoring)
        {
            // ��ȡһ����̬�任��ʵ��
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            // Ϊʵ�����һ��AnimationFrameIndex���
            AddComponent<AnimationFrameIndex>(entity);

            // Ϊʵ�����һ���������AnimationSharedData����������frameRate��frameCount����
            AddSharedComponent<AnimationSharedData>(entity, new AnimationSharedData()
            {
                frameRate = authoring.frameRate,  // ���ù��������֡��
                frameCount = authoring.frameMaxIndex,  // ���ù�����������֡����
            });
        }
    }
}
