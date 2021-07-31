using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FollowCharactersDisappear : FollowCharactersArrangement
{
   //public AllCharacter

    /// <summary>
    /// 体力が0以下のBasicStatusDataとそのオブジェクトを削除
    /// </summary>
    /// <param name="teamData"></param>
    /// <param name="followCharacters"></param>
    /// <returns></returns>
    protected bool PhysicalFitnessZeroObjectIsDisappear(ref CharacterTeamsData.TeamData teamData,GameObject[] followCharacters)
    {
        bool isObjectDestroyed = false;

        //体力が0以下のオブジェクトのBasicStatusDataAccessの削除準備関数を実行
        foreach (var followCharacter in followCharacters)
        {
            //nullの場合は次に
            if (followCharacter == null) continue;

            if (followCharacter.GetComponent<BasicStatusDataAccess>().PhysicalFitness <= 0)
            {
                followCharacter.GetComponent<BasicStatusDataAccess>().DeletePreparation();

                //GameObjectは削除
                Destroy(followCharacter);
                isObjectDestroyed = true;
            }
        }


        //体力が0以下のBasicStatusDataを取得
        var td_LINQ = teamData.FollowCharacters_BasicStatusDatas
           .Where(basicData => basicData.PhysicalFitness<=0) 
           .Select(basicData => basicData);
        List<BasicStatusData> deleteBasicStatusDatas = new List<BasicStatusData>();
        foreach (var basicStatusData in td_LINQ)
        {
            deleteBasicStatusDatas.Add(basicStatusData);
        }
       
        //体力が0以下のBasicStatusDataを全て削除
        foreach (var basicData in deleteBasicStatusDatas)
        {
            teamData.FollowCharacters_BasicStatusDatas.Remove(basicData);
        }

        return isObjectDestroyed;
    }


}
