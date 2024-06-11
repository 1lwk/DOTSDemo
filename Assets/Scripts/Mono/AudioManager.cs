using UnityEngine;

// 定义 AudioManager 类，继承自 MonoBehaviour
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // 定义静态实例，用于实现单例模式

    // Awake 方法在脚本实例化时调用
    private void Awake()
    {
        instance = this; // 设置静态实例为当前对象
    }

    public AudioSource audioSource; // 定义音频源组件
    public AudioClip shootAudioClip; // 定义射击音频剪辑
    public AudioClip hitAudioClip; // 定义击中音频剪辑
    public float playHitAudioInterval = 0.2f; // 定义播放击中音频的时间间隔
    public float lastPlayerHitAudioTime = 0.1f; // 定义上一次播放击中音频的时间

    // 播放射击音频的方法
    public void PlayShootAudio()
    {
        audioSource.PlayOneShot(shootAudioClip); // 使用音频源播放射击音频剪辑
    }

    // 播放击中音频的方法
    public void PlayHitAudio()
    {
        audioSource.PlayOneShot(hitAudioClip); // 使用音频源播放击中音频剪辑
    }

    // Update 方法在每帧调用
    private void Update()
    {
        // 检查是否需要播放击中音频
        if (SharedData.GameSharedInfo.Data.playHitAudio
            && Time.time - lastPlayerHitAudioTime > playHitAudioInterval
            && Time.time - SharedData.GameSharedInfo.Data.playHitAudioTime < Time.deltaTime)
        {
            lastPlayerHitAudioTime = Time.deltaTime; // 更新上一次播放击中音频的时间
            PlayHitAudio(); // 播放击中音频
            SharedData.GameSharedInfo.Data.playHitAudio = false; // 重置播放击中音频标志
        }
    }
}
