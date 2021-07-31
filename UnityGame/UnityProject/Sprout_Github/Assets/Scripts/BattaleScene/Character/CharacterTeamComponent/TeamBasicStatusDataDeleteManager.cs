using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TeamBasicStatusDataDeleteManager : TeamPurposeList
{
    protected CharacterTeamsData _characterTeamsData;
    protected CharacterGenerateManager _characterGenerateManager;


    

    /// <summary>
    /// このクラスの管理するチームのリーダーのBasicStatusDataとオブジェクトを削除する
    /// </summary>
    /// <returns>削除したBasicStatusDataを返す(削除前の値)</returns>
    public BasicStatusData ThisTeamLeaderDelete()
    {
        ThisTeamData.LeaderObject.GetComponent<BasicStatusDataAccess>().DeletePreparation();
        //オブジェクトはそのまま削除
        Destroy(ThisTeamData.LeaderObject);
        //BasicStatusDataを削除する。削除前のBasicStatusDataのコピーを返す
        BasicStatusData returnBasicStatusData;
        returnBasicStatusData = ThisTeamData.Leader_BasicStatusData;
        ThisTeamData.Leader_BasicStatusData=null;
        return returnBasicStatusData;
    }

    /// <summary>
    /// このクラスの管理するチームのフォローキャラクター全員のBasicStatusDataとオブジェクトを削除する
    /// </summary>
    /// <returns>削除したBasicStatusDatasを返す(削除前の値)</returns>
    public BasicStatusData[] ThisTeamFollowCharactersAllDelete()
    {
        //FollowCharactersManagerObjectがない場合は処理せず終了
        if (ThisTeamData.FollowCharactersManagerObject == null) return new BasicStatusData[0];

        //フォローキャラクターオブジェクトはBasicStatusDataにアクセスしているため、まずはオブジェクトを削除する必要がある
            ThisTeamData.FollowCharactersManagerObject.GetComponent<FollowCharactersManager>().FollowCharactersObjectAllDelete();

        //返すBasicStatusDatasを用意してから削除する
        BasicStatusData[] returnBasicStatusDatas=new BasicStatusData[ThisTeamData.FollowCharacters_BasicStatusDatas.Count];
        ThisTeamData.FollowCharacters_BasicStatusDatas.CopyTo(returnBasicStatusDatas,0);
        ThisTeamData.FollowCharacters_BasicStatusDatas.Clear();

        return returnBasicStatusDatas;
    }

    /// <summary>
    /// このクラスの管理するチームのフォローキャラクターのBasicStatusDataとオブジェクトを指定した数、削除する
    /// </summary>
    /// <returns>削除したBasicStatusDatasを返す(削除前の値)</returns>
    public BasicStatusData[] ThisTeamFollowCharactersDelete(int deleteNumberOfPeople)
    {
        //フォローキャラクターオブジェクトはBasicStatusDataにアクセスしているため、まずはオブジェクトを削除する必要がある
        ThisTeamData.FollowCharactersManagerObject.GetComponent<FollowCharactersManager>().FollowCharactersObjectAllDelete();

        //削除したデータを格納するリスト
        List<BasicStatusData> deleteList=new List<BasicStatusData>() ;
        for (int i = 0; i < deleteNumberOfPeople; i++)
        {
            //削除する人数分ループするが途中で削除出来る人のデータがなくたった場合はそこで終了
            if (ThisTeamData.FollowCharacters_BasicStatusDatas.Count == 0) break;

            //削除予定のデータを削除リストに追加
            deleteList.Add(ThisTeamData.FollowCharacters_BasicStatusDatas.ElementAt(0));

            //データを削除
            ThisTeamData.FollowCharacters_BasicStatusDatas.Remove(ThisTeamData.FollowCharacters_BasicStatusDatas.ElementAt(0));
        }

        return deleteList.ToArray();
    }

    /// <summary>
    /// CharacterTeamsDataからこのチームの情報を削除してチームGameObjectも削除する。※リーダーとフォローキャラクターのGameObjectは削除しない
    /// </summary>
    protected void ThisTeamDelete()
    {
        _characterTeamsData.TeamDatas.Remove(ThisTeamData);

        Destroy(gameObject);
    }

    protected void Start()
    {

     _characterTeamsData =  gameObject.SearchByTagName("SingletonManager", "CharactersManager").GetComponent<CharacterTeamsData>();
        _characterGenerateManager =  gameObject.SearchByTagName("SingletonManager", "CharactersManager").GetComponent<CharacterGenerateManager>();

    }
}
