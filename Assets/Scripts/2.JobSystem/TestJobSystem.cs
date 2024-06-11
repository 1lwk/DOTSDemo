using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class TestJobSystem : MonoBehaviour
{
    //Run(); �����߳�����
    //Schedule(); �ڹ����߳�ִ��
    //ScheduleParallel(); ��x�������߳�����ɸ��Ե�����
    // Start is called before the first frame update
    private NativeArray<int> nums; // Unity.Collection������� �����൱��ָ������� ��Ȼ�ǽṹ�嵫�Ǵ�ֵ�����仯 ��Ϊָ���ָ�򲻻���ṹ��������ֵ�޹�
    private MyJob job;
    void Start()
    {
        Debug.Log("z"+System.Threading.Thread.CurrentThread.ManagedThreadId);
        nums = new NativeArray<int>(100, Allocator.Persistent);//������
        job = new MyJob() { nums=nums};
        job.ScheduleParallel(100,10,default(JobHandle));//һ���� ���10������ ÿ����ʹ�ò�ͬ���߳�
        Invoke("Test",3);
    }

    void Test()
    {
    //    Debug.Log(job.num);
        for(int i=0;i<nums.Length;i++)
        {
            Debug.Log(nums[i]);
        }
        nums.Dispose();
    }
}


public struct MyJob : IJobFor
{
    public NativeArray<int> nums;
    //jobʵ�ʻ�ִ�е��߳�
    public void Execute(int index)
    {
        nums[index]= index;
        //num += 1;
        //Debug.Log(num);
        //Debug.Log("j" + System.Threading.Thread.CurrentThread.ManagedThreadId+"-index:"+index);
    }
}