using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CommandToTheTeam<T> : CharacterTeamSeparationAndIntegration<T> where T : MonoBehaviour
{
    protected CharacterTeamsData characterTeamsData;
    //イベント地点に設置するオブジェクト。イベント地点にターゲットが到着した際に行われる処理を扱うオブジェクト
    private GameObject _standbyEvent;
    private GameObject _architectureEvent;

    private GridData _gridData;
    private GridCalculation _gridCalculation;


    protected void Start()
    {
        //使用するオブジェクトの取得
        foreach (var child in transform.returnAllChild())
        {
            if (child.name == "StandbyEvent") _standbyEvent = child;
            if (child.name == "ArchitectureEvent") _architectureEvent = child;
        }

        characterTeamsData = GetComponent<CharacterTeamsData>();
        _gridCalculation = gameObject.SearchByTagName("SingletonManager", "GridManager").GetComponent<GridCalculation>();
        _gridData = gameObject.SearchByTagName("SingletonManager", "GridManager").GetComponent<GridData>();
    }

   public void MemberGather(ref CharacterTeamsData.TeamData destinationTeamData)
    {
        //同じ陣営のチームメンバーを探す
        string gatherCamp = destinationTeamData.Camp.ToString();
        var cTD_LINQ = characterTeamsData.TeamDatas
           .Where(tD => tD.Camp.ToString() == gatherCamp)
           .Select(tD => tD);

        //同じ陣営のメンバーの目的リストにリーダーに集まるを追加する
        foreach (var gatherTeamData in cTD_LINQ)
        {
            gatherTeamData.TeamObject.GetComponent<TeamMemberManager>().Purposes.Add((destinationTeamData.LeaderObject.transform.Find("BodyCollider"), TeamMemberManager.PurposeEnum.GatherToLeader));
            gatherTeamData.TeamObject.GetComponent<TeamMemberManager>().CharacterLeaderTeamsDataSet(ref destinationTeamData);
        }
    }

    /// <summary>
    /// チームメンバーを分断させ、指定した場所に待機させる
    /// </summary>
    /// <param name="teamData"></param>
    /// <param name="grid"></param>
    /// <param name="numberOfPeopleToRun"></param>
    /// <returns>処理出来ない場合はfalseを返す</returns>
    public bool StandbyCommand(ref CharacterTeamsData.TeamData teamData,(int,int,int)grid, int numberOfPeopleToRun)
    {
        //動く人がいない
        if (numberOfPeopleToRun <= 0) return false;
        //待機させたい人数が現在のチーム人数より多い場合は処理せずにfalseを返す。
        if (teamData.FollowCharacters_BasicStatusDatas.Count < numberOfPeopleToRun) return false;

        //チームを分断させ、分断先のチームの参照型のTeamDataを取得する。
        CharacterTeamsData.TeamData separationDestinationTeamData = GetComponent<CharacterTeamsData>().TeamDatas.ElementAt((int)GetComponent<CharacterTeamsData>().TeamDataIndexInSearchByTeamObject(TeamSeparation(ref teamData, numberOfPeopleToRun, (CharacterGenerate<CharacterGenerateManager>.Camp)teamData.Camp)));

        //分断したチームのリーダーを分断元のリーダーの場所に置く
        separationDestinationTeamData.LeaderObject.transform.Find("BodyCollider").position = teamData.LeaderObject.transform.Find("BodyCollider").position;

       GameObject eventGo = Instantiate(_standbyEvent);

        //イベントオブジェクトの大きさをグリッド一マスの大きさにする
        eventGo.transform.localScale = transform.InverseTransformVector(_gridData.OneSquareSize);

        //イベントオブジェクトの場所を指定されたグリッドにする
        Vector3 eventPosition = _gridCalculation.Vector3PositionGet(grid);
        eventPosition.y += _gridData.OneSquareSize.y / 2;
        eventGo.transform.position = eventPosition;

        //引き金の対象となるチームデータを設定する
        eventGo.GetComponent<EventBase>().TriggerTeamData(ref separationDestinationTeamData);

        //表示する
        eventGo.SetActive(true);

        //分断先のチームの目的に待機を追加する
        separationDestinationTeamData.TeamObject.GetComponent<TeamMemberManager>().Purposes.Add((eventGo.transform, TeamMemberManager.PurposeEnum.Standby));
        
        //無事に処理できたのでtrueを返す
        return true;
    }

    /// <summary>
    /// チームメンバーを分断させ、指定した場所で建築させる
    /// </summary>
    /// <param name="teamData"></param>
    /// <param name="grid"></param>
    /// <param name="numberOfPeopleToRun"></param>
    /// <returns>処理出来ない場合はfalseを返す</returns>
    public bool ArchitectureCommand(ref CharacterTeamsData.TeamData teamData , (int, int, int) grid, BuildingGenerateManager.Buildings building,int numberOfPeopleToRun)
    {
        //動く人がいない
        if(numberOfPeopleToRun<=0) return false;
        //待機させたい人数が現在のチーム人数より多い場合は処理せずにfalseを返す。
        if (teamData.FollowCharacters_BasicStatusDatas.Count < numberOfPeopleToRun) return false;

        //チームを分断させ、分断先のチームの参照型のTeamDataを取得する。
        CharacterTeamsData.TeamData separationDestinationTeamData = GetComponent<CharacterTeamsData>().TeamDatas.ElementAt((int)GetComponent<CharacterTeamsData>().TeamDataIndexInSearchByTeamObject(TeamSeparation(ref teamData, numberOfPeopleToRun, (CharacterGenerate<CharacterGenerateManager>.Camp)teamData.Camp)));

        //分断したチームのリーダーを分断元のリーダーの場所に置く
        separationDestinationTeamData.LeaderObject.transform.Find("BodyCollider").position = teamData.LeaderObject.transform.Find("BodyCollider").position;

        GameObject eventGo = Instantiate(_architectureEvent);

        //引き金の対象となるチームデータを設定する
        eventGo.GetComponent<EventBase>().TriggerTeamData(ref separationDestinationTeamData);

        //建築する建物が何かを設定する
        eventGo.GetComponent<ArchitectureEventManager>().ArchitectureBuilding=building;
        
        //建築するグリッド
        eventGo.GetComponent<ArchitectureEventManager>().ArchitectureGird= grid;

        //イベントオブジェクトの大きさをグリッド一マスの大きさにする
        eventGo.transform.localScale = transform.InverseTransformVector(_gridData.OneSquareSize);

        //イベントオブジェクトの場所を指定されたグリッドにする
        Vector3 eventPosition = _gridCalculation.Vector3PositionGet(grid);
        eventPosition.y += _gridData.OneSquareSize.y / 2;
        eventGo.transform.position = eventPosition;

        //表示する
        eventGo.SetActive(true);

        //分断先のチームの目的に待機を追加する
        separationDestinationTeamData.TeamObject.GetComponent<TeamMemberManager>().Purposes.Add((eventGo.transform, TeamMemberManager.PurposeEnum.Architecture));

        //無事に処理できたのでtrueを返す
        return true;
    }

    
}
