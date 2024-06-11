using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// 定义一个静态类 EditorTool，用于存放自定义编辑器工具
public static class EditorTool
{
    // 定义一个菜单项，该菜单项位于 "Assets/CustomTool/MergeSprite"
    [MenuItem("Assets/CustomTool/MergeSprite")]
    public static void MergeSprite()
    {
        // 获取当前选中的资源 GUID 数组
        string[] spriteGUIDs = Selection.assetGUIDs;

        // 如果没有选中资源或者选中的资源少于两个，则返回
        if (spriteGUIDs == null || spriteGUIDs.Length <= 1) return;

        // 定义一个列表，用于存放选中资源的路径
        List<string> spritePathList = new List<string>(spriteGUIDs.Length);
        for (int i = 0; i < spriteGUIDs.Length; i++)
        {
            // 根据 GUID 获取资源路径并添加到列表中
            string assetPath = AssetDatabase.GUIDToAssetPath(spriteGUIDs[i]);
            spritePathList.Add(assetPath);
        }
        // 对资源路径列表进行排序
        spritePathList.Sort();

        // 加载第一个纹理，用于获取纹理的高度和宽度
        Texture2D firstTex = AssetDatabase.LoadAssetAtPath<Texture2D>(spritePathList[0]);
        int unitHeight = firstTex.height;
        int unitWidth = firstTex.width;

        // 创建一个新的纹理，宽度是单个纹理宽度的总和，高度与单个纹理相同
        Texture2D outputTex = new Texture2D(unitWidth * spritePathList.Count, unitHeight);
        for (int i = 0; i < spritePathList.Count; i++)
        {
            // 加载每一个选中的纹理
            Texture2D temp = AssetDatabase.LoadAssetAtPath<Texture2D>(spritePathList[i]);
            // 获取纹理的像素数据
            Color[] colors = temp.GetPixels();
            // 将像素数据设置到输出纹理的相应位置
            outputTex.SetPixels(i * unitWidth, 0, unitWidth, unitHeight, colors);
        }

        // 将输出纹理编码为 PNG 格式的字节数组
        byte[] bytes = outputTex.EncodeToPNG();
        // 将字节数组写入文件，文件名为 "MergeSprite.png"
        File.WriteAllBytes(spritePathList[0].Remove(spritePathList[0].LastIndexOf(firstTex.name)) + "MergeSprite.png", bytes);
        // 保存资源并刷新资产数据
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

