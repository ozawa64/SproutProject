using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TargetPhysicalFitnessDisplayUI : CanvasChildUIBase
{
    /// <summary>対象の名前のUIテキスト</summary>
     private Text _targetNameText;
    /// <summary>体力を表示するスライダー</summary>
    private Slider _physicalFitnessSlider; 
    /// <summary>スライダーの子オブジェクトの残り体力を表示するオブジェクト</summary>
     private GameObject _fillAreaOfSlider;
    
    private TeamMemberManager _teamMemberManager;
    
   /// <summary>
   /// セットされたチームが注目している対象の情報を表示する。※セットしたチームの情報を表示するのではない
   /// </summary>
   /// <param name="teamMemberManager"></param>
    public void SetTeamMemberManager(TeamMemberManager teamMemberManager)
    {
        _teamMemberManager = teamMemberManager;
    }

    private void FixedUpdate()
    {
        if (!ThisUIDisplayModeCheck(transform.parent.GetComponent<UIManager>().CurrentDisplayMode)) return;

        if (UIObjectContainsNull())
        {
            FindUIObjects();
        }
        else
        {
            UIInformationUpdate();
        }
    }


    protected override void FindUIObjects()
    {
        GameObject[] findObjects = FindsTheObjectWithTheSpecifiedName();
        _targetNameText = findObjects[0].GetComponent<Text>();
        _physicalFitnessSlider = findObjects[1].GetComponent<Slider>();
        _fillAreaOfSlider = findObjects[2];
    }

    /// <summary>
    /// UIオブジェクト達を更新可能か確認する(オブジェクトがnullではないか)。
    /// </summary>
    /// <returns>true:可能 false:不可能</returns>
    protected override bool UIObjectContainsNull()
    {
        if (_targetNameText == null) return true;
        if (_physicalFitnessSlider == null) return true;
        if (_fillAreaOfSlider == null) return true;
        return false;
    }
       

        /// <summary>
        /// UI情報の更新
        /// </summary>
    private void UIInformationUpdate()
    {
        //下の処理で更新出来なかった場合は情報にアクセス出来ない状態ということなのでUI上では非表示にする
        _targetNameText.text = "";
        _physicalFitnessSlider.gameObject.SetActive(false);

        //UIに更新して代入したい情報がnullではないか確認してnullではない場合は更新情報を代入する
        if (_teamMemberManager != null)
        {
            if (_teamMemberManager.AttentionTarget == null) return;
            if (_teamMemberManager.AttentionTarget.GetComponent<BasicStatusDataAccess>() == null) return;
            if (_teamMemberManager.AttentionTarget.GetComponent<BasicStatusDataAccess>().Access == false) return;

            //情報を更新する
            _targetNameText.text = _teamMemberManager.AttentionTarget.GetComponent<BasicStatusDataAccess>().Name;
            _physicalFitnessSlider.maxValue = _teamMemberManager.AttentionTarget.GetComponent<BasicStatusDataAccess>().MaxPhysicalFitness;
            _physicalFitnessSlider.value = _teamMemberManager.AttentionTarget.GetComponent<BasicStatusDataAccess>().PhysicalFitness;
            _physicalFitnessSlider.gameObject.SetActive(true);

            if (_physicalFitnessSlider.value <= 0)
            {//体力表示スライダーの残り体力(value)が0の場合は残り体力を表示するUIを非表示にする
                _fillAreaOfSlider.SetActive(false);
            }
            else
            {//体力表示スライダーを非表示にした後復活したり別の対象に変えた際に再び残り体力UIを表示する
                _fillAreaOfSlider.SetActive(true);
            }

        }
    }

  
}
