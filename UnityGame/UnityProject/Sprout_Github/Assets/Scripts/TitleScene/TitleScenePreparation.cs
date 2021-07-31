using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScenePreparation : MonoBehaviour
{
    /// <summary>現在のステップ</summary>
    private Steps _currentStep=Steps.MouseCursor;
    private UIManager _uIManager;
    private MouseCursorManager _mouseCursorManager;


    enum Steps : int
    {
        MouseCursor,
        TitleUIsPreparation,
        StepEnd
    }

    private void Start()
    {
        _uIManager = gameObject.SearchByTagName("SingletonManager", "CanvasAndUIManager").GetComponent<UIManager>();
        _mouseCursorManager = gameObject.SearchByTagName("SingletonManager", "MouseCursorManager").GetComponent<MouseCursorManager>();
    }

    private void FixedUpdate()
    {
        Execution();
    }

    /// <summary>
    /// 準備を一ステップごとに順番に実行していく
    /// </summary>
    private void Execution()
    {
        switch (_currentStep)
        {
            case Steps.MouseCursor:

                _mouseCursorManager.MouseCursor(false);

                //次
                _currentStep = Steps.TitleUIsPreparation;
                break;

            case Steps.TitleUIsPreparation:

                _uIManager.FixedDisplayUISet(UIManager.DisplayModeEnum.Title, "TitleUI");
                _uIManager.FixedDisplayUISet(UIManager.DisplayModeEnum.Title, "TitleOperationExplanation_0");
                _uIManager.FixedDisplayUISet(UIManager.DisplayModeEnum.SetByTitle, "SettingUI");

                _uIManager.DisplayModeSwitching(UIManager.DisplayModeEnum.Title);
                
                //次
                _currentStep = Steps.StepEnd;
                break;
            case Steps.StepEnd:
                Destroy(gameObject);
                break;
        }
    }
}