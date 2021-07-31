using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterTeamSeparationAndIntegration<T> : SingletonMonoBehaviour<T> where T : MonoBehaviour
{

    /// <summary>
    /// チームを分離する
    /// </summary>
    /// <param name="separationTeam"></param>
    /// <param name="separationNumberOfPeople"></param>
    /// <param name="camp"></param>
    /// <returns>分断先のチームオブジェクト</returns>
    public GameObject TeamSeparation(ref CharacterTeamsData.TeamData separationTeam, int separationNumberOfPeople, CharacterGenerate<CharacterGenerateManager>.Camp camp)
    {
        //分離出来る人数がいない場合は処理しない
        if (separationTeam.FollowCharacters_BasicStatusDatas.Count <= 0) return null;

        //新しいチームを作る、リーダーはFollowCharactersリストの最初の人。移動させるので既存のチームからは削除
        CharacterTeamsData.TeamData newTeamData = GetComponent<CharacterGenerateManager>().NewTeamAndLeaderGenerate(camp, separationTeam.TeamObject.GetComponent<TeamMemberManager>().ThisTeamFollowCharactersDelete(1)[0]);

        //残り分離人数
        separationNumberOfPeople--;

        //新しいチームの参照型のTeamDataを取得する
        newTeamData = GetComponent<CharacterTeamsData>().TeamDatas.ElementAt((int)GetComponent<CharacterTeamsData>().TeamDataIndexInSearchByTeamObject(newTeamData.TeamObject));

        //メンバー以外の情報も引き継ぐ
        newTeamData.AttackTargetTags=new List<string>(separationTeam.AttackTargetTags.ToArray());
        newTeamData.Camp = separationTeam.Camp;
        newTeamData.ArchitecturalBuildingTagName = separationTeam.ArchitecturalBuildingTagName.ToString();

        //既存のチームのメンバーを削除して新しいチームに移動させる
        foreach (var basicStatusData in separationTeam.TeamObject.GetComponent<TeamMemberManager>().ThisTeamFollowCharactersDelete(separationNumberOfPeople))
        {
            GetComponent<CharacterGenerateManager>().FollowCharacterGenerate_BasicStatusData(ref newTeamData, basicStatusData);
        }

        return  newTeamData.TeamObject;
    }


    /// <summary>
    /// チームを統合する
    /// </summary>
    public void TeamIntegration(ref CharacterTeamsData.TeamData integrationTeamData, ref CharacterTeamsData.TeamData integrationDestinationTeamData)
    {
        //リーダーがいたら移動させる
        if (integrationTeamData.Leader_BasicStatusData != null) GetComponent<CharacterGenerateManager>().FollowCharacterGenerate_BasicStatusData(ref integrationDestinationTeamData ,integrationTeamData.TeamObject.GetComponent<TeamMemberManager>().ThisTeamLeaderDelete());

        //フォローキャラクターを移動させる
        foreach (var basicStatusData in integrationTeamData.TeamObject.GetComponent<TeamMemberManager>().ThisTeamFollowCharactersAllDelete())
        {
            GetComponent<CharacterGenerateManager>().FollowCharacterGenerate_BasicStatusData( ref integrationDestinationTeamData, basicStatusData);
        }

    }
}
