using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandbyEventManager : EventBase
{
    private void OnTriggerEnter(Collider other)
    {
        //引き金となるチームが存在しなくなった場合はイベントを終了させる
        if (triggerObject_TeamData == null) EventForcedTermination();
        //リーダーがいない場合は以下の処理はしない(出来ない)。※リーダーがいないだけの場合はまだフォローキャラクターがリーダーの代理になる可能性があるのでイベントは終了させない
        if (triggerObject_TeamData.LeaderObject == null) return;
     
        //引き金となるチームのリーダーがイベント判定に入ったか
        if (triggerObject_TeamData.LeaderObject.transform.Find("BodyCollider").gameObject == other.gameObject)
        {
            //待機命令の目的を達成したので目的リストから削除する
            triggerObject_TeamData.TeamObject.GetComponent<TeamMemberManager>().Purposes.Remove((transform, TeamMemberManager.PurposeEnum.Standby));

            //全てのイベント処理が終了したらこのオブジェクトは削除する
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        //引き金となるチームが存在しなくなった場合はイベントを終了させる
        if (triggerObject_TeamData == null) EventForcedTermination();
        //リーダーがいない場合は以下の処理はしない(出来ない)。※リーダーがいないだけの場合はまだフォローキャラクターがリーダーの代理になる可能性があるのでイベントは終了させない
        if (triggerObject_TeamData.LeaderObject == null) return;
    }

    public override void EventForcedTermination()
    {
        //待機命令の目的を目的リストから削除する
        triggerObject_TeamData.TeamObject.GetComponent<TeamMemberManager>().Purposes.Remove((transform, TeamMemberManager.PurposeEnum.Standby));

        //イベント強制終了処理が終了したらこのオブジェクトは削除する
        Destroy(gameObject);

    }

}
