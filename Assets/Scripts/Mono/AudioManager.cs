using UnityEngine;

// ���� AudioManager �࣬�̳��� MonoBehaviour
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // ���徲̬ʵ��������ʵ�ֵ���ģʽ

    // Awake �����ڽű�ʵ����ʱ����
    private void Awake()
    {
        instance = this; // ���þ�̬ʵ��Ϊ��ǰ����
    }

    public AudioSource audioSource; // ������ƵԴ���
    public AudioClip shootAudioClip; // ���������Ƶ����
    public AudioClip hitAudioClip; // ���������Ƶ����
    public float playHitAudioInterval = 0.2f; // ���岥�Ż�����Ƶ��ʱ����
    public float lastPlayerHitAudioTime = 0.1f; // ������һ�β��Ż�����Ƶ��ʱ��

    // ���������Ƶ�ķ���
    public void PlayShootAudio()
    {
        audioSource.PlayOneShot(shootAudioClip); // ʹ����ƵԴ���������Ƶ����
    }

    // ���Ż�����Ƶ�ķ���
    public void PlayHitAudio()
    {
        audioSource.PlayOneShot(hitAudioClip); // ʹ����ƵԴ���Ż�����Ƶ����
    }

    // Update ������ÿ֡����
    private void Update()
    {
        // ����Ƿ���Ҫ���Ż�����Ƶ
        if (SharedData.GameSharedInfo.Data.playHitAudio
            && Time.time - lastPlayerHitAudioTime > playHitAudioInterval
            && Time.time - SharedData.GameSharedInfo.Data.playHitAudioTime < Time.deltaTime)
        {
            lastPlayerHitAudioTime = Time.deltaTime; // ������һ�β��Ż�����Ƶ��ʱ��
            PlayHitAudio(); // ���Ż�����Ƶ
            SharedData.GameSharedInfo.Data.playHitAudio = false; // ���ò��Ż�����Ƶ��־
        }
    }
}
