using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimultaneousUIDisplayPermission<T> : SingletonMonoBehaviour<T> where T : MonoBehaviour
{
    private UIManager _uIManager;

    private SortedList<UIManager.DisplayModeEnum, string[]> _permissionList = new SortedList<UIManager.DisplayModeEnum, string[]>();

    protected new void Awake()
    {
        base.Awake();

        _uIManager = GetComponent<UIManager>();

        PermissionListAdd(UIManager.DisplayModeEnum.Title,
           new string[2]
           {
            "TitleUI",
            "TitleOperationExplanation_0"
           });
        
        PermissionListAdd(UIManager.DisplayModeEnum.SetByTitle,
           new string[1]
           {
            "SettingUI",
           });

        PermissionListAdd(UIManager.DisplayModeEnum.NormalThirdPersonPerspective,
            new string[3]
            {
            "TargetPhysicalFitnessDisplayUI",
            "PlayerPhysicalFitnessDisplayUI",
            "BattalSceneOperationExplanation_0"
            });

        PermissionListAdd(UIManager.DisplayModeEnum.FirstPersonView,
            new string[2]
            {
            "TargetPhysicalFitnessDisplayUI",
            "BattalSceneOperationExplanation_1"
            });
        
        PermissionListAdd(UIManager.DisplayModeEnum.OverlookFromSky,
            new string[1]
            {
            "PlayerCommandToTheTeamUI",
            });

        PermissionListAdd(UIManager.DisplayModeEnum.GameClear,
          new string[1]
          {
            "GameClearUI",
          });

        PermissionListAdd(UIManager.DisplayModeEnum.GameOver,
          new string[1]
          {
            "GameOverUI",
          });
    }

    /// <summary>
    /// そのモードの時に、表示して良いUIの名前を登録する。※既に存在している場合は上書きする
    /// </summary>
    /// <param name="displayModeEnum"></param>
    /// <param name="permissionUIs"></param>
    public void PermissionListAdd(UIManager.DisplayModeEnum displayModeEnum, string[] permissionUIs)
    {

        if(_permissionList.ContainsKey(displayModeEnum))
        {
            //上書き。mainUInameは変わらないのでそのまま
            _permissionList[displayModeEnum] = permissionUIs;
        }
        else
        {
            //mainUInameがなければ新規作成
            _permissionList.Add(displayModeEnum, permissionUIs);
        }
    }

    /// <summary>
    /// 現在のメインUIと同時にcanvasに表示することを許可されているか確認する
    /// </summary>
    /// <param name="confirmationUIName"></param>
    /// <returns>許可:true 却下:false</returns>
    protected bool PermissionConfirmationDisplayUI(string confirmationUIName, UIManager.DisplayModeEnum displayModeEnum)
    {
        //CurrentDisplayModeのキーがない場合は競合しないかどうかの判断が出来ないのでfalseを返す
        if (!_permissionList.ContainsKey(displayModeEnum)) return false;
       

        //許可されている名前と一致したらtrueを返す
        foreach (var permissionName in _permissionList[displayModeEnum])
        {
            if (permissionName == confirmationUIName) return true;
        }
        
        //許可された名前と全て一致しなかった場合は許可されていないのでfalseを返す
        return false;
    }

    /// <summary>
    /// 現在のUIモードにcanvasに表示することを許可されていないUIオブジェクトを削除する
    /// </summary>
    protected void RemoveUnauthorizedUI()
    {
        //子のオブジェクトから許可されていないオブジェクトを全て削除する
        for (int canvasChild_i = 0; canvasChild_i < transform.childCount; canvasChild_i++)
        {
            if (!PermissionConfirmationDisplayUI(transform.GetChild(canvasChild_i).name,GetComponent<UIManager>().CurrentDisplayMode))
            {
                //UIObjectParentDeleteExclusionがtrueの場合はそのオブジェクトの子オブジェクトのみ削除
                if (transform.GetChild(canvasChild_i).gameObject.GetComponent<CanvasChildUIBase>().UIObjectParentDeleteExclusion==true)
                {
                    for (int uIChild_i = 0; uIChild_i < transform.GetChild(canvasChild_i).childCount; uIChild_i++)
                    {
                        Destroy(transform.GetChild(canvasChild_i).GetChild(uIChild_i).gameObject);
                    }
                }
                else
                {
                    Destroy(transform.GetChild(canvasChild_i).gameObject);
                }

            }
        }
    }

}
