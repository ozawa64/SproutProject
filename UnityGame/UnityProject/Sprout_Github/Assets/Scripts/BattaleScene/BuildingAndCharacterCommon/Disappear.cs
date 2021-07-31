using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    public bool Disappearing { get; private set; } = false;

    private BasicStatusDataAccess _basicStatusDataAccess;
    private NormalCharacterAnimationManager _normalCharacterAnimationManager;
    private CharacterTeamsData.TeamData _teamData;

    public void CharacterTeamsDataSet(ref CharacterTeamsData.TeamData teamData)
    {
        _teamData = teamData;
    }

    private void Start()
    {
        _normalCharacterAnimationManager = GetComponent<NormalCharacterAnimationManager>();
        _basicStatusDataAccess = gameObject.GetComponent<BasicStatusDataAccess>();
    }

    private void FixedUpdate()
    {

        if (!_basicStatusDataAccess.Access) return;

        //体力が0以下でステージから退場し始める
        if (_basicStatusDataAccess.PhysicalFitness <= 0&&Disappearing==false) StartDisappear();
    }

    /// <summary>
    /// 姿を消す(動きの)処理をする
    /// </summary>
    public void StartDisappear()
    {
        Disappearing = true;

        _normalCharacterAnimationManager.DisappearAnimation();
    }

    /// <summary>
    /// StartDisappearで処理した動きとは違い実際にデータを削除していく関数
    /// </summary>
    public void DisappearExecution()
    {
        //リーダーのBasicStatusDataとオブジェクトを削除
        _teamData.Leader_BasicStatusData = null;
         Destroy(this.gameObject);
    }
}
