using Unity.Entities;
using Unity.Rendering;

// ����һ���������� "_Index"
[MaterialProperty("_Index")]
public struct AnimationFrameIndex : IComponentData, IEnableableComponent // ���� AnimationFrameIndex �ṹ�壬ʵ�� IComponentData �� IEnableableComponent �ӿ�
{
    public float value; // ����һ������������� value����ʾ����֡����
}
