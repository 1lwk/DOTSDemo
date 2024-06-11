using Unity.Entities;

// ����һ��������� AnimationSharedData
public struct AnimationSharedData : ISharedComponentData
{
    public float frameRate; // ����һ������������� frameRate����ʾ����֡����
    public int frameCount; // ����һ�������������� frameCount����ʾ����֡������
}
