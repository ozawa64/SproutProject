using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif


public class AttackManager : SelectAttackTarget
{
    /// <summary>攻撃処理を一時停止</summary>
    public bool Suspension = false;

    private DamageManager _damageManager;


    /// <summary>
    /// 攻撃処理の実行。※アニメーションは別
    /// </summary>
    /// <param name="attackNum"></param>
    public void AttackExecution(int attackNum)
    {
        foreach (var damageTarget in NormalAttackTargets(attackNum))
        {
            _damageManager.CauseDamage(damageTarget,gameObject, m_damageReactionnum[attackNum], m_damageMagnification[attackNum]);
        }
    }

    private new void Start()
    {
        base.Start();

        _damageManager = gameObject.SearchByTagName("SingletonManager", "DamageManager").GetComponent<DamageManager>();
    }
}
   

