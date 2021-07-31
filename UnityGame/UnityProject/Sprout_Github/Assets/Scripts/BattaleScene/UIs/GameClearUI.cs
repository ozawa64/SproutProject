using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClearUI : CanvasChildUIBase
{
    /// <summary>勝利回数を表示するテキスト</summary>
    private Text _numberOfWinsText;
    private SaveDataManager _saveDataManager;

    private void Start()
    {
        _saveDataManager = gameObject.SearchByTagName("SingletonManager", "SaveDataManager").GetComponent<SaveDataManager>();
    }

    private void Update()
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
        _numberOfWinsText = findObjects[0].GetComponent<Text>();
    }

    protected override bool UIObjectContainsNull()
    {
        if (_numberOfWinsText == null) return true;
        return false;
    }


    /// <summary>
    /// UI情報の更新
    /// </summary>
    private void UIInformationUpdate()
    {
        _numberOfWinsText.text = "勝利回数 "+ SaveDataManager.SaveData[0].TotalNumberOfWins+ " 回!!";
    }

}
