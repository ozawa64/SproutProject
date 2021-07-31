using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPhysicalFitnessDisplayUI : CanvasChildUIBase
{
    /// <summary>プレイヤーの名前のUIテキスト</summary>
    private Text _playerNameText;
    /// <summary>体力を表示するスライダー</summary>
    private Slider _physicalFitnessSlider;
    /// <summary>スライダーの子オブジェクトの残り体力を表示するオブジェクト</summary>
    private GameObject _fillAreaOfSlider;
    private CharacterTeamsData.TeamData _teamData;

    public void CharacterTeamsDataSet(ref CharacterTeamsData.TeamData teamData)
    {
        _teamData = teamData;
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
        _playerNameText = findObjects[0].GetComponent<Text>();
        _physicalFitnessSlider = findObjects[1].GetComponent<Slider>();
        _fillAreaOfSlider = findObjects[2];
    }

    /// <summary>
    /// UIオブジェクト達を更新可能か確認する(オブジェクトがnullではないか)。
    /// </summary>
    /// <returns>true:可能 false:不可能</returns>
    protected override bool UIObjectContainsNull()
    {
        if (_playerNameText == null) return true;
        if (_physicalFitnessSlider == null) return true;
        if (_fillAreaOfSlider == null) return true;
        return false;
    }


    /// <summary>
    /// UI情報の更新
    /// </summary>
    private void UIInformationUpdate()
    {
        //以下の処理で更新出来なかった場合は情報にアクセス出来ない状態ということなのでUI上では非表示にする
        _playerNameText.text = "";
        _physicalFitnessSlider.gameObject.SetActive(false);

        //参照するプレイヤーデータがnullではないか確認。nullの場合は処理せず終了する
        if (_teamData == null) return;
        if (_teamData.LeaderObject == null) return;
        if (_teamData.LeaderObject.GetComponent<BasicStatusDataAccess>().Access == false) return;

            //情報を更新する
            _playerNameText.text = _teamData.LeaderObject.GetComponent<BasicStatusDataAccess>().Name;
            _physicalFitnessSlider.maxValue = _teamData.LeaderObject.GetComponent<BasicStatusDataAccess>().MaxPhysicalFitness;
            _physicalFitnessSlider.value = _teamData.LeaderObject.GetComponent<BasicStatusDataAccess>().PhysicalFitness;
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
