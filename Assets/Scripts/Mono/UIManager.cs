using UnityEngine;
using UnityEngine.UI;

// 定义 UIManager 类，继承自 MonoBehaviour
public class UIManager : MonoBehaviour
{
    private int score; // 定义私有变量 score，用于存储当前得分
    public Text socreText; // 公有变量，用于在 Unity 编辑器中指定 UI Text 组件

    // Start 方法在第一次调用 Update 方法前调用
    void Start()
    {
        // 初始化逻辑可以放在这里，目前为空
    }

    // Update 方法在每帧调用
    void Update()
    {
        // 如果当前得分与共享数据中的死亡计数器不相等
        if (score != SharedData.GameSharedInfo.Data.deadCounter)
        {
            // 更新得分
            score = SharedData.GameSharedInfo.Data.deadCounter;
            // 更新 UI 文本显示得分
            socreText.text = score.ToString();
        }
    }
}
