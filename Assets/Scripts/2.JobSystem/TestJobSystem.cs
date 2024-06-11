using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class TestJobSystem : MonoBehaviour
{
    //Run(); 在主线程运行
    //Schedule(); 在工作线程执行
    //ScheduleParallel(); 在x个工作线程中完成各自的任务
    // Start is called before the first frame update
    private NativeArray<int> nums; // Unity.Collection里的容器 都是相当于指针操作器 虽然是结构体但是传值后发生变化 因为指针的指向不会向结构体那样数值无关
    private MyJob job;
    void Start()
    {
        Debug.Log("z"+System.Threading.Thread.CurrentThread.ManagedThreadId);
        nums = new NativeArray<int>(100, Allocator.Persistent);//分配器
        job = new MyJob() { nums=nums};
        job.ScheduleParallel(100,10,default(JobHandle));//一个组 完成10个任务 每个组使用不同的线程
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
    //job实际会执行的线程
    public void Execute(int index)
    {
        nums[index]= index;
        //num += 1;
        //Debug.Log(num);
        //Debug.Log("j" + System.Threading.Thread.CurrentThread.ManagedThreadId+"-index:"+index);
    }
}