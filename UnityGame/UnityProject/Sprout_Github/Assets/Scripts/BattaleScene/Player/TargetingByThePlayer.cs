using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingByThePlayer : MonoBehaviour
{
    /// <summary>ターゲッティング出来るレイヤー達</summary>
    [SerializeField] private LayerMask m_targetingLayer;
    /// <summary>ターゲッティング出来る最大距離</summary>
    [SerializeField] private float m_range=100;

    private CharacterTeamsData.TeamData _teamData;
    
    public void CharacterTeamsDataSet(ref CharacterTeamsData.TeamData teamData)
    {
        _teamData = teamData;
    }

    /// <summary>
    /// カメラの正面から直線上にレイを飛ばし衝突した対象がターゲット対象の場合はチームのターゲット対象に設定する。対象でなければnullを設定する
    /// </summary>
    public void TargetInput()
    {
        //_teamDataがないと処理できない
        if (_teamData == null) return;

       GameObject target = FlyARayFromTheCamera();

        //ターゲットが見つからない場合はnullを設定する。※以下の処理でターゲット対象が見つかればAttentionTargetに代入され見つからない場合は代入されないので見つからないことを想定してnullを代入
        _teamData.TeamObject.GetComponent<TeamMemberManager>().AttentionTarget = null;

        //ターゲットがいない(null)場合は処理終了
        if (target == null) return;

        //ターゲッティング対象のオブジェクトタグか判断し、対象のタグの場合はプレイヤーのチーム情報にターゲットとして設定
        foreach (var targetTag in _teamData.AttackTargetTags)
        {
            if (target.CompareTag(targetTag))
            {
                _teamData.TeamObject.GetComponent<TeamMemberManager>().AttentionTarget = target;
                return;
            }
        }

    }

    /// <summary>
    /// Rayをカメラの正面から飛ばす。Rayがヒットしたらコライダーの親オブジェクトを返す、なければnullを返す。
    /// </summary>
    /// <returns></returns>
    private GameObject FlyARayFromTheCamera()
    {
        //rayを飛ばす方向//カメラの正面から
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        //デバッグ用
        Debug.DrawRay(ray.origin, ray.direction * m_range, Color.blue);

        RaycastHit hit;
        //ヒットしたらそのコライダーの親オブジェクトを返し、それ以外はnullを返す。
        if (Physics.Raycast(ray, out hit, m_range, m_targetingLayer))
        {
            //親オブジェクトがなければnullを返す。
            return (hit.collider.transform.parent == null)? null: hit.collider.transform.parent.gameObject;
        }

        //ヒットしなければnullを返す。
            return null;
    }
}
