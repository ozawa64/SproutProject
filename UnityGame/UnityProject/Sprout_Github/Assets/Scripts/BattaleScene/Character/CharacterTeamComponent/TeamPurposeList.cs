using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamPurposeList : MonoBehaviour
{
    public CharacterTeamsData.TeamData ThisTeamData;

    /// <summary>目的リスト、(目的の場所(Transform),その場所で行う事(enum))</summary>
    public List<(Transform destinationTf, PurposeEnum purposeEnum)> Purposes { get; set; } = new List<(Transform, PurposeEnum)>();

    /// <summary>チームの目的</summary>
    public enum PurposeEnum : int
    {
        Standby = 0,
        AttackTarget,
        Architecture,
        GatherToLeader,
    }


    /// <summary>
    /// 目的があるかどうか
    /// </summary>
    /// <returns></returns>
    public bool PurposeIsThere()
    {
        if (Purposes.Count == 0) return false;

        return true;
    }

}