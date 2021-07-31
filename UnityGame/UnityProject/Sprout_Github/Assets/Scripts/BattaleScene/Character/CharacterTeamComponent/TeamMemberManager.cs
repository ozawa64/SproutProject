using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TeamMemberManager : TeamReorganization 
{
    /// <summary>陣営のリーダーのチームデータ</summary>
    public CharacterTeamsData.TeamData LeaderTeamData;

    public void CharacterLeaderTeamsDataSet(ref CharacterTeamsData.TeamData teamData)
    {
        LeaderTeamData = teamData;
    }

    /// <summary>
    /// 参照型のTeamDataをセットする
    /// </summary>
    public void SetTeamData(ref CharacterTeamsData.TeamData teamData)
    {
        ThisTeamData = teamData;
    }

    /// <summary>
    /// リーダーの初期設定
    /// </summary>
    public void LeaderInitialSetting()
    {
        if (ThisTeamData == null) Debug.LogError("変数:ThisTeamDataがnullです");

        //リーダーオブジェクトにリーダーのBasicStatusData(TeamData)をセットする
        ThisTeamData.LeaderObject.GetComponent<BasicStatusDataAccess>().CharacterTeamsDataSet(ref ThisTeamData, BasicStatusDataAccess.BasicStatusDataType.Leader);

        //CharacterTeamsDataのAttackTargetTags(ThisTeamData)が必要なので設定する
        ThisTeamData.LeaderObject.GetComponent<AttackManager>().CharacterTeamsDataSet(ref ThisTeamData);

        //ThisTeamDataはリーダーオブジェクトの姿を消す(削除の)際に同時にリーダーのBasicStatusDataも削除する際に使用する
        ThisTeamData.LeaderObject.GetComponent<Disappear>().CharacterTeamsDataSet(ref ThisTeamData);

        //チームとリーダーオブジェクトの親子関係を設定
        ThisTeamData.LeaderObject.transform.parent = transform;
    }

    /// <summary>
    /// FollowCharactersManagerの初期設定
    /// </summary>
    public void FollowCharactersManagerInitialSetting()
    {
        if (ThisTeamData == null) Debug.LogError("変数:ThisTeamDataがnullです");

        // チームとフォローマネージャーオブジェクトの親子関係を設定
        ThisTeamData.FollowCharactersManagerObject.transform.parent =transform;

        //FollowCharactersManagerObjectはTeamDataを使用するので設定
        ThisTeamData.FollowCharactersManagerObject.GetComponent<FollowCharactersManager>().TeamDataSet(ref ThisTeamData);
    }


}
