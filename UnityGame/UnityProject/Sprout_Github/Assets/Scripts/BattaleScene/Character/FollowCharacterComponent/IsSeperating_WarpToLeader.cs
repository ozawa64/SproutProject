using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsSeperating_WarpToLeader : MonoBehaviour
{
    [SerializeField] Transform m_MonitoringTarget = null;
    [SerializeField] Vector3 m_WarpDistance = new Vector3(50,60,50); 

    private CharacterTeamsData.TeamData _teamData=null;

   public void TeamDataSet (ref CharacterTeamsData.TeamData teamData)
    {
        _teamData = teamData;
    }

    private void FixedUpdate()
    {
        //_teamDataやと監視オブジェクトとリーダーオブジェクトのいずれかがない時は以下の処理をしない(出来ない)
        if (_teamData == null || m_MonitoringTarget == null|| _teamData.LeaderObject == null) return;

        //リーダーと離れている距離
        Vector3 distance = _teamData.LeaderObject.transform.Find("BodyCollider").transform.position - m_MonitoringTarget.position;

        //離れている距離が知りたいので絶対値にする
        distance.x = Mathf.Abs(distance.x);
        distance.y = Mathf.Abs(distance.y);
        distance.z = Mathf.Abs(distance.z);

        //ワープさせる場所
        Vector3? warpPosition = null;

        //ワープさせるか判断
        if (distance.x >= m_WarpDistance.x) warpPosition = _teamData.LeaderObject.transform.Find("BodyCollider").transform.position;
        if (distance.y >= m_WarpDistance.y) warpPosition = _teamData.LeaderObject.transform.Find("BodyCollider").transform.position;
        if (distance.z >= m_WarpDistance.z) warpPosition = _teamData.LeaderObject.transform.Find("BodyCollider").transform.position;

        //ワープさせる判断が出た場合はワープさせる
        if (warpPosition != null) m_MonitoringTarget.position = (Vector3)warpPosition;

    }

}
