using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

// ����һ����̬�� SharedData ���ڴ洢��������
public static class SharedData
{
    // ����һ������̬���� singtonEntity�����ڴ洢һ��ȫ��Ψһ��ʵ��
    public static readonly SharedStatic<Entity> singtonEntity = SharedStatic<Entity>.GetOrCreate<KeyClass1>();

    // ����һ������̬���� GameSharedInfo�����ڴ洢ȫ����Ϸ������Ϣ
    public static readonly SharedStatic<GameSharedInfo> GameSharedInfo = SharedStatic<GameSharedInfo>.GetOrCreate<GameSharedInfo>();

    // ����һ������̬���� playerPos�����ڴ洢���λ��
    public static readonly SharedStatic<float2> playerPos = SharedStatic<float2>.GetOrCreate<KeyClass2>();

    // ����һ���յĽṹ�� KeyClass1�����ڱ�ʶ singtonEntity �Ĺ���̬����
    public struct KeyClass1 { }

    // ����һ���յĽṹ�� KeyClass2�����ڱ�ʶ playerPos �Ĺ���̬����
    public struct KeyClass2 { }
}

// ����һ���ṹ�� GameSharedInfo�����ڴ洢ȫ����Ϸ������Ϣ
public struct GameSharedInfo
{
    public int deadCounter; // ��¼��������
    public float spawnInterval; // ��¼���ɵ��˵ļ��ʱ��
    public int spawnCount; // ��¼���ɵ��˵�����
    public bool playHitAudio; // ��ʶ�Ƿ񲥷Ż�����Ч
    public double playHitAudioTime; // ��¼���Ż�����Ч��ʱ��
}
