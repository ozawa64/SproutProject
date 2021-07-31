using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : SingletonMonoBehaviour<DamageManager>
{
   public enum DamageReactionEnum
    {
        WeakDamage,
        HeavyDamage,
        NoReaction
    }

    /// <summary>
    /// 指定したオブジェクトにダメージを与える
    /// </summary>
    public bool CauseDamage(GameObject damageObject, GameObject attackObject, DamageReactionEnum damageReactionEnum=DamageReactionEnum.NoReaction, float damageMagnification = 1)
    {
        BasicStatusDataAccess damageBSDAccess = damageObject.GetComponent<BasicStatusDataAccess>();
        BasicStatusDataAccess attackBSDAccess = attackObject.GetComponent<BasicStatusDataAccess>();

        if (damageBSDAccess.Access != true) return false;
        if (attackBSDAccess.Access != true) return false;

        float offensivePower = attackBSDAccess.OffensivePower * damageMagnification;
        float defensePower = damageBSDAccess.DefensePower;

        //与えるダメージから防御分を引く
        offensivePower -= defensePower;

        //攻撃相手の防御が攻撃力を上回る場合は1ダメージを与える
        if (offensivePower < 0)
        {
            damageBSDAccess.PhysicalFitness--;
        }
        else
        {
            //防御しきれないダメージを与える
            damageBSDAccess.PhysicalFitness -= Mathf.RoundToInt(offensivePower);
        }

        //ダメージを受けた際のリアクションアニメーションをする。※アニメーション管理スクリプトがない場合は何もしない
        CharacterAndBuildingCommonAnimation animationManagerForDamageObject= GameObjectAnimatorGet(damageObject);
       if(animationManagerForDamageObject!=null)
        {
            switch (damageReactionEnum)
            {
                case DamageReactionEnum.WeakDamage:
                    animationManagerForDamageObject.WeakDamageAnimation();
                    break;
                case DamageReactionEnum.HeavyDamage:
                    animationManagerForDamageObject.HeavyDamageAnimation();
                    break;
                case DamageReactionEnum.NoReaction:
                    break;
            }

        }
          
        return true;

        CharacterAndBuildingCommonAnimation GameObjectAnimatorGet(GameObject gameObject)
        {
            if (gameObject.GetComponent<BuildingAnimationManager>()) return gameObject.GetComponent<BuildingAnimationManager>();

            if (gameObject.GetComponent<NormalCharacterAnimationManager>()) return gameObject.GetComponent<NormalCharacterAnimationManager>();

            return null;
        }
         

    }

    
}
