using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// ����һ����̬�� EditorTool�����ڴ���Զ���༭������
public static class EditorTool
{
    // ����һ���˵���ò˵���λ�� "Assets/CustomTool/MergeSprite"
    [MenuItem("Assets/CustomTool/MergeSprite")]
    public static void MergeSprite()
    {
        // ��ȡ��ǰѡ�е���Դ GUID ����
        string[] spriteGUIDs = Selection.assetGUIDs;

        // ���û��ѡ����Դ����ѡ�е���Դ�����������򷵻�
        if (spriteGUIDs == null || spriteGUIDs.Length <= 1) return;

        // ����һ���б����ڴ��ѡ����Դ��·��
        List<string> spritePathList = new List<string>(spriteGUIDs.Length);
        for (int i = 0; i < spriteGUIDs.Length; i++)
        {
            // ���� GUID ��ȡ��Դ·������ӵ��б���
            string assetPath = AssetDatabase.GUIDToAssetPath(spriteGUIDs[i]);
            spritePathList.Add(assetPath);
        }
        // ����Դ·���б��������
        spritePathList.Sort();

        // ���ص�һ���������ڻ�ȡ����ĸ߶ȺͿ��
        Texture2D firstTex = AssetDatabase.LoadAssetAtPath<Texture2D>(spritePathList[0]);
        int unitHeight = firstTex.height;
        int unitWidth = firstTex.width;

        // ����һ���µ���������ǵ��������ȵ��ܺͣ��߶��뵥��������ͬ
        Texture2D outputTex = new Texture2D(unitWidth * spritePathList.Count, unitHeight);
        for (int i = 0; i < spritePathList.Count; i++)
        {
            // ����ÿһ��ѡ�е�����
            Texture2D temp = AssetDatabase.LoadAssetAtPath<Texture2D>(spritePathList[i]);
            // ��ȡ�������������
            Color[] colors = temp.GetPixels();
            // �������������õ�����������Ӧλ��
            outputTex.SetPixels(i * unitWidth, 0, unitWidth, unitHeight, colors);
        }

        // ������������Ϊ PNG ��ʽ���ֽ�����
        byte[] bytes = outputTex.EncodeToPNG();
        // ���ֽ�����д���ļ����ļ���Ϊ "MergeSprite.png"
        File.WriteAllBytes(spritePathList[0].Remove(spritePathList[0].LastIndexOf(firstTex.name)) + "MergeSprite.png", bytes);
        // ������Դ��ˢ���ʲ�����
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

