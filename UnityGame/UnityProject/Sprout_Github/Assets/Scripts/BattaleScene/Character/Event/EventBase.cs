using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBase : MonoBehaviour
{
    /// <summary>イベントを発生させる引き金となるチームのデータ</summary>
    protected CharacterTeamsData.TeamData triggerObject_TeamData;

    /// <summary>
    /// イベントを発生させる引き金となるチームデータを設定する
    /// </summary>
    public void TriggerTeamData(ref CharacterTeamsData.TeamData teamData)
    {
        triggerObject_TeamData = teamData;
    }

    /// <summary>
    /// イベント処理を強制終了する。※このイベントオブジェクトも消える
    /// </summary>
    virtual public void EventForcedTermination()
    {
        Destroy(gameObject);
    }


}
