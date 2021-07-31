using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterGenerateManager : CharacterGenerate<CharacterGenerateManager>
{

    private CharacterTeamsData _characterTeamsData;

    private void Start()
    {
        //それぞれ必要なクラスやGameObjectを準備
        _characterTeamsData = gameObject.SearchByTagName("SingletonManager", "CharactersManager").GetComponent<CharacterTeamsData>();

    }



    /// <summary>
    /// 新しいチームとそのリーダーを生成する。※同時にCharacterTeamsDataにデータを登録します
    /// </summary>
    /// <param name="camp">生成するキャラクターの陣営</param>
    /// <param name="leader_BasicStatusData">リーダーが引き継ぐBasicStatusData</param>
    /// <returns>※参照ではなくコピー</returns>
    public  CharacterTeamsData.TeamData NewTeamAndLeaderGenerate(Camp camp, BasicStatusData leader_BasicStatusData)
    {
        //チームオブジェクトとリーダーオブジェクトとそのステータスをキャラクターデータ保存場所に登録しながら生成する
        _characterTeamsData.TeamDatas.Add(new CharacterTeamsData.TeamData
        {
            TeamObject = Generate(camp, CharacterType.CharacterTeam),
            LeaderObject = Generate(camp, CharacterType.NormalCharacter),
            Leader_BasicStatusData = leader_BasicStatusData,
            Camp = camp
        });

        //新しいチームの参照型のTeamDataを取得する
        CharacterTeamsData.TeamData generateTeamData = _characterTeamsData.TeamDatas.ElementAt((_characterTeamsData.TeamDatas.Count - 1));
        
        //TeamObjectはTeamDataが必要なので設定する
        generateTeamData.TeamObject.GetComponent<TeamMemberManager>().SetTeamData(ref generateTeamData);

        //生成したリーダーの初期設定をする
        generateTeamData.TeamObject.GetComponent<TeamMemberManager>().LeaderInitialSetting();


        //追加生成したTeamDataのコピーを返す。※TeamDataの中身のGameオブジェクトの参照先は同じ
        return _characterTeamsData.TeamDatas.ElementAt((_characterTeamsData.TeamDatas.Count-1));
    }

    /// <summary>
    /// リーダーの生成とBasicStatusDataをCharacterTeamsDataに登録
    /// </summary>
    /// <param name="leader_BasicStatusData">リーダーが引き継ぐBasicStatusData</param>
    /// <param name="camp">生成するキャラクターの陣営</param>
    /// <param name="teamData"></param>
    ///  /// <returns>生成したリーダーオブジェクト</returns>
    public GameObject NewLeaderGenerate(ref CharacterTeamsData.TeamData teamData, BasicStatusData leader_BasicStatusData)
    {
        //オブジェクトを改めて作る
        teamData.LeaderObject = Generate((Camp)teamData.Camp, CharacterType.NormalCharacter);

        //新しいBasicStatusDataを設定する
        teamData.Leader_BasicStatusData = leader_BasicStatusData;

        //生成したリーダーの初期設定をする
        teamData.TeamObject.GetComponent<TeamMemberManager>().LeaderInitialSetting();

        return teamData.TeamObject;
    }


    /// <summary>
    /// フォローキャラクターの生成(チームに追加(BasicStatusDatasのみ))
    /// </summary>
    /// <param name="characterPushTeamData">CharacterTeamsDataから参照していることを想定</param>
    /// <param name="addBasicStatusData"></param>
    public void FollowCharacterGenerate_BasicStatusData( ref CharacterTeamsData.TeamData characterPushTeamData, BasicStatusData addBasicStatusData)
    {

        //FollowCharactersManagerObjectがない場合は生成する
        if (characterPushTeamData.FollowCharactersManagerObject==null)
        {
            characterPushTeamData.FollowCharactersManagerObject = Generate((Camp)characterPushTeamData.Camp, CharacterType.FollowCharactersManager);
            
            //生成したフォローキャラクターマネジャーの初期設定をする
            characterPushTeamData.TeamObject.GetComponent<TeamMemberManager>().FollowCharactersManagerInitialSetting();
            
        }

        //フォローキャラクターの追加
        characterPushTeamData.FollowCharacters_BasicStatusDatas.Add(addBasicStatusData);
    }

    /// <summary>
    /// FollowCharacterのGameObjectを生成する。※FollowCharactersManager用の関数
    /// </summary>
    /// <param name="camp"></param>
    /// <returns></returns>
    public GameObject FollowCharacterGenerate_GameObject(Camp camp)
    {
        return Generate(camp, CharacterType.FollowCharacter);
    }


}
