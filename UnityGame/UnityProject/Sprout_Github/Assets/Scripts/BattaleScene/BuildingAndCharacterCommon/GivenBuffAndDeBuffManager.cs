using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivenBuffAndDeBuffManager : RecoveryBuff
{
    [SerializeField] private float m_buffDeBuffIntervalTime = 1;
     private float buffDeBuffIntervalTimeCount = 0;
   /// <summary>効力の働いているバフとデバフのリスト</summary>
    private List<(GameObject sentGameObject, List<BuffAndDeBuffEnum> buffAndDeBuffEnums,List<float> effectingTime,List<int> effectPower)> _effectingBuffOrDeBuffList =new List<(GameObject sentGameObject, List<BuffAndDeBuffEnum> buffAndDeBuffEnums, List<float> effectingTime, List<int> effectPower)>();

   public enum BuffAndDeBuffEnum
    {
        RecoveryBuff,
    }

    /// <summary>
    /// バフデバフを指定した時間与え続ける
    /// </summary>
    /// <param name="sentGameObject"></param>
    /// <param name="buffOrDeBuffEnum"></param>
    /// <param name="effectingTime"></param>
    /// <param name="duplicate"></param>
    public void GivenBuffOrDeBuff(GameObject sentGameObject, BuffAndDeBuffEnum buffOrDeBuffEnum, float effectingTime,int effectPower, bool duplicate=true)
    {
        //既存の要素にバフデバフを追加する時にどのインデックスにアクセスするかを求める
        //のと
        //Listに新しく要素を追加するかどうかをsentGameObjectで判断する
        int accessIndex = 0;
        bool newAddGameObject = true;
        for (int i = 0; i < _effectingBuffOrDeBuffList.Count; i++)
        {
            if (_effectingBuffOrDeBuffList[i].sentGameObject == sentGameObject)
            {
                newAddGameObject = false;
                accessIndex=i;
                break;
            }
        }

        //新規追加する場合の処理
        if (newAddGameObject)
        {
            _effectingBuffOrDeBuffList.Add((sentGameObject, new List<BuffAndDeBuffEnum> { buffOrDeBuffEnum }, new List<float> { effectingTime },new List<int> { effectPower}));
            return;//以下の処理は新規追加ではない場合の処理なのでここで終了
        }

        //既存のバフデバフにアクセスする際にどのインデックスにアクセスするかを求める
        //のと
        //同じ相手から同じバフデバフが既に送られて効力中であるかどうかを求める
        int accessIndex_BuffOrDeBuff = 0;
        bool existingBuffOrDeBuff = false;
        for (int i = 0; i < _effectingBuffOrDeBuffList[accessIndex].buffAndDeBuffEnums.Count; i++)
        {
            if (buffOrDeBuffEnum == _effectingBuffOrDeBuffList[accessIndex].buffAndDeBuffEnums[i])
            {
                existingBuffOrDeBuff = true;
                accessIndex_BuffOrDeBuff = i;
                break;
            }
        }

        //既存のバフデバフがない場合
        if(existingBuffOrDeBuff==false)
        {
            _effectingBuffOrDeBuffList[accessIndex].buffAndDeBuffEnums.Add(buffOrDeBuffEnum);
            _effectingBuffOrDeBuffList[accessIndex].effectingTime.Add(effectingTime);
            _effectingBuffOrDeBuffList[accessIndex].effectPower.Add(effectPower);
            return; //以下の処理は新規追加ではない場合の処理なのでここで終了
        }

        //バフデバフの効果を重複させるかどうか
        if(duplicate)
        {
            _effectingBuffOrDeBuffList[accessIndex].effectingTime[accessIndex_BuffOrDeBuff] = effectingTime;
            _effectingBuffOrDeBuffList[accessIndex].effectPower[accessIndex_BuffOrDeBuff] = effectPower;
        }
        else
        {
            //--※後で修正、多分二次関数を使う--//
            _effectingBuffOrDeBuffList[accessIndex].effectingTime[accessIndex_BuffOrDeBuff] += effectingTime;
            _effectingBuffOrDeBuffList[accessIndex].effectingTime[accessIndex_BuffOrDeBuff] += effectPower;
        }

    }

    private void FixedUpdate()
    {
        InvokingBuffDeBuffAndEffectingTimeCount();
    }

    /// <summary>
    /// バフデバフの実行処理と効果時間カウント※毎フレーム更新推奨
    /// </summary>
    private void InvokingBuffDeBuffAndEffectingTimeCount()
    {
        buffDeBuffIntervalTimeCount += Time.deltaTime;

        //与えられたバフデバフを実行していく。※バフデバフが重複していても送っているオブジェクトが異なる場合は別々に実行していく
        for (int accessIndex = 0; accessIndex < _effectingBuffOrDeBuffList.Count; accessIndex++)//送ったオブジェクト
        {
            for (int accessIndex_BuffDeBuff = 0; accessIndex_BuffDeBuff < _effectingBuffOrDeBuffList[accessIndex].buffAndDeBuffEnums.Count; accessIndex_BuffDeBuff++)//送ったオブジェクトが送ってきたバフデバフ達
            {
                //効果時間のカウント
                _effectingBuffOrDeBuffList[accessIndex].effectingTime[accessIndex_BuffDeBuff] -= Time.deltaTime;

                if (_effectingBuffOrDeBuffList[accessIndex].effectingTime[accessIndex_BuffDeBuff] <= 0)
                {
                    //効果時間が切れたら切れたバフデバフのEnumとTimeリストの要素を削除
                    _effectingBuffOrDeBuffList[accessIndex].effectingTime.RemoveAt(accessIndex_BuffDeBuff);
                    _effectingBuffOrDeBuffList[accessIndex].buffAndDeBuffEnums.RemoveAt(accessIndex_BuffDeBuff);
                }
                else
                {
                    //バフデバフを実行する時間周期ではない場合は実行しない
                    if (buffDeBuffIntervalTimeCount < m_buffDeBuffIntervalTime) continue;

                    //効果時間中ならバフデバフの実行
                    BuffDeBuffExecution(_effectingBuffOrDeBuffList[accessIndex].buffAndDeBuffEnums[accessIndex_BuffDeBuff], _effectingBuffOrDeBuffList[accessIndex].effectPower[accessIndex_BuffDeBuff]);
                }
            }
        }

        //バフデバフを実行する時間周期になっている場合は次の周期までカウントするためにカウント変数をリセットする
        if (buffDeBuffIntervalTimeCount >= m_buffDeBuffIntervalTime)buffDeBuffIntervalTimeCount = 0;

        //バフデバフの効果を実行
        void BuffDeBuffExecution(BuffAndDeBuffEnum buffAndDeBuffEnum, int effectPower)
        {
            switch (buffAndDeBuffEnum)
            {
                case BuffAndDeBuffEnum.RecoveryBuff:
                    Recovery_Buff(effectPower);
                    break;
            }
        }
    }


    //StartCoroutine("FindThe_TargetScript_AttachedToThePlayer");

    /*
    private IEnumerator FindThe_TargetScript_AttachedToThePlayer()
    {
        
        while (true)
        {
            yield break;
            yield return new WaitForSeconds(5);
        }
        
    }
    */
}
