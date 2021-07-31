using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterTeamsData : SingletonMonoBehaviour<CharacterTeamsData>
{
    /// <summary>チーム型の全てのキャラクターのGameObjectとBasicStatusDataを管理する</summary>
    public ICollection<TeamData> TeamDatas { get; set; } = new List<TeamData>();

    public class TeamData
    {
        public GameObject TeamObject { get; set; }
        public GameObject LeaderObject { get; set; }
        public GameObject FollowCharactersManagerObject { get; set; }

        /// <summary>リーダーを除く</summary>
        public ICollection<BasicStatusData> FollowCharacters_BasicStatusDatas = new List<BasicStatusData>();
       
        public BasicStatusData Leader_BasicStatusData;
        /// <summary>チームの陣営</summary>
       public CharacterGenerateManager.Camp? Camp { get; set; }=null;
        /// <summary>チームの建築物のタグ名</summary>
        public string ArchitecturalBuildingTagName { get; set; }
        /// <summary>チームの攻撃対象のオブジェクトタグ達</summary>
        public List<string> AttackTargetTags { get; set; } = new List<string>();

    }

    /// <summary>
    /// TeamObjectからそのオブジェクトが保存されているTeamDataをTeamDatasから探し見つけた場合にその場所のインデックス番号をintで返す
    /// </summary>
    /// <param name="teamObject"></param>
    /// <returns></returns>
    public int? TeamDataIndexInSearchByTeamObject(GameObject teamObject)
    {
        //teamObjectが保存されているTeamDataを探し、見つけた場合はtds変数にそのオブジェクトが保存されているTeamDataを代入する
        var tds_LINQ = TeamDatas
            .Where(td => td.TeamObject.Equals(teamObject))
            .Select(td => td);
        TeamData tds = null;
        foreach (var item in tds_LINQ)
        {
            tds = item;
            break;
        }

        //teamObjectが保存されているTeamDataのインデックス番号を返す
        for (int i = 0; i < TeamDatas.Count; i++)
        {
            if (ReferenceEquals(TeamDatas.ElementAt(i), tds))
            {
                return i;
            }
        }

        Debug.LogWarning("ここに保存されていないオブジェクトです");
        return null;
    }

    
}