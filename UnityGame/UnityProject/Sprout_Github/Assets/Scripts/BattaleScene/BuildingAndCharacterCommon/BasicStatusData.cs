using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicStatusData
{
    /// <summary>名前</summary>
    public string Name = "名前";
    /// <summary>体力</summary>
    public int PhysicalFitness = 1000; 
    /// <summary>最大体力</summary>
    public int MaxPhysicalFitness = 1000;
    /// <summary>防御力</summary>
    public int DefensePower  = 50;
    /// <summary>攻撃力</summary>
    public int OffensivePower  = 100;
    /// <summary>移動の最高速度</summary>
    public float MoveMaxSpeed  = 20;
    /// <summary>移動速度</summary>
    public float MoveSpeed = 30;
    /// <summary>ジャンプ回数※累積ではなく地面から跳ぶ際の回数</summary>
    public float JumpFrequency  = 1;
    
}
