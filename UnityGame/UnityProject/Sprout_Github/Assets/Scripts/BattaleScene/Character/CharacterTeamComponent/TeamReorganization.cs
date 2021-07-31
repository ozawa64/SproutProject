using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamReorganization : PaysAttentionToTheTarget
{
    private void FixedUpdate()
    {
        if(ThisTeamData.Leader_BasicStatusData==null && ThisTeamData.FollowCharacters_BasicStatusDatas.Count==0)
        {
            //リーダーとフォローキャラクターどちらもいない場合はチームを削除する
            ThisTeamDelete();
        }
        else
        {
            //リーダーがいない(BasicStatusDataがない)場合は...
            if (ThisTeamData.Leader_BasicStatusData == null)
            {
                //フォローキャラクターをリーダーにする。※フォローキャラクターがいない場合はこの関数にたどり着かない
                _characterGenerateManager.NewLeaderGenerate(ref ThisTeamData, ThisTeamFollowCharactersDelete(1)[0]);
            }
        }
    }
}
