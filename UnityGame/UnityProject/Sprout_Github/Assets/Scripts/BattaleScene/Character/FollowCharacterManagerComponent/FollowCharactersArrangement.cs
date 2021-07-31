using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCharactersArrangement : FollowCharactersDisplay
{
    /// <summary>フォローキャラクター達の配置場所※配列のサイズは変わらないが人数に応じて要素がVector3かnullになる</summary>
    public Vector3?[] Arrangements { get; private set; }
    /// <summary>リーダーとメンバーの間の距離</summary>
    protected float WidthWithTheLeader { get; set; } = 5;

    protected new void Awake()
    {
        base.Awake();
        
        //配置場所の最大数は最大表示人数に合わせる
        Arrangements = new Vector3?[m_MaxDisplayNumberOfPeople];
    }

    /// <summary>
    /// 配置の更新※毎フレーム更新推奨
    /// </summary>
    /// <param name="numberOfPeople">配置人数</param>
    protected void ArrangementUpdate(int numberOfPeople, Vector3 leaderPosition)
    {
        //numberOfPeopleが配置の配列より多い場合は調整する
        if (numberOfPeople > Arrangements.Length) numberOfPeople = Arrangements.Length;

        //numberOfPeople(配置人数)が0の場合はArrangementsの要素を全てnullにする
        if (numberOfPeople == 0)
        {
            for (int i = 0; i < Arrangements.Length; i++)
            {
                Arrangements[i] = null;
            }

            return;
        }

        //↓リーダーを囲んで円状に集まる配置にする↓
        //メンバー同士が開ける幅(角度)
        float angleOfOnePerson = (360f / numberOfPeople);
        for (int i = 0; i < numberOfPeople; i++)
        {
            //現在のループのメンバーの角度
            float angle = angleOfOnePerson * i;

            //xzの角度と距離間を計算
            float radian_xz = angle * Mathf.Deg2Rad;
            Vector3 velocity_xz = new Vector3(Mathf.Cos(radian_xz), 0, Mathf.Sin(radian_xz));
            Vector3 destination = velocity_xz * WidthWithTheLeader;//リーダーとの幅

            //リーダーを中心にするためリーダーの座標を足す
            destination += leaderPosition;

            //計算した座標を配置配列に代入
            Arrangements[i] = destination;

        }



    }

}
