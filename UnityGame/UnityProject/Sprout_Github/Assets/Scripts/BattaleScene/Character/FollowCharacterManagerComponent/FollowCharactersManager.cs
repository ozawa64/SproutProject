using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCharactersManager : FollowCharactersDisappear
{
    private CharacterTeamsData.TeamData _dataInThisTeam = null;

    /// <summary>FollowCharactersのリーダーのBody</summary>
    private GameObject leaderBodyObject=null;


    public void TeamDataSet(ref CharacterTeamsData.TeamData teamData)
    {
        _dataInThisTeam = teamData;
    }

    /// <summary>
    /// 一時的(1フレーム)に表示しているフォローキャラクターを全て削除する
    /// </summary>
    public void FollowCharactersObjectAllDelete()
    {
        ChangeTheDisplayedNumberOfPeople(0, ref _dataInThisTeam, Vector3.zero);
    }

    private void FixedUpdate()
    {
        //TeamDataがないと処理出来ないのでなければ終了
        if (_dataInThisTeam == null) return;

        PhysicalFitnessZeroObjectIsDisappear(ref _dataInThisTeam,DisplayCharacters);

        //プレイヤーオブジェクトのBody(GameObject)を取得
        if (leaderBodyObject == null) leaderBodyObject = LeaderBodyObjectGet();

        //以下の処理はleaderBodyObjectがないと処理できないのでnullの場合はここで終了
        if (leaderBodyObject == null) return;
        
        //FollowCharacters_BasicStatusDatas分フォローキャラクターを表示する、表示出来る人数を超えていたら関数側で調整されるのでそのまま引数に設定
        ChangeTheDisplayedNumberOfPeople(_dataInThisTeam.FollowCharacters_BasicStatusDatas.Count,ref _dataInThisTeam, leaderBodyObject.transform.position);

        ArrangementUpdate(CurrentNumberOfPeopleDisplayed(), leaderBodyObject.transform.position);

    }

    private GameObject LeaderBodyObjectGet()
    {
        //リーダーオブジェクトがない場合はnullを返す
        if (_dataInThisTeam.LeaderObject == null) return null;

        for (int i = 0; i < _dataInThisTeam.LeaderObject.transform.childCount; i++)
        {
            if (_dataInThisTeam.LeaderObject.transform.GetChild(i).name == "BodyCollider")
            {
                return _dataInThisTeam.LeaderObject.transform.GetChild(i).gameObject;
            }
        }

        return null;
    }
}
