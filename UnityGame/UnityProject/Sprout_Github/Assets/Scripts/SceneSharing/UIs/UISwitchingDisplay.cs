using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISwitchingDisplay<T> : UIGenerate<T> where T : MonoBehaviour
{
    /// <summary>切り替えが起こるひとつ前のモード</summary>
    public DisplayModeEnum ModeBeforeSwitching { get; private set; } = DisplayModeEnum.None;
    /// <summary>現在のディスプレイモード</summary>
    public DisplayModeEnum CurrentDisplayMode { get; private set; } = DisplayModeEnum.None;
    /// <summary>特定のモードの時に毎回固定で表示されるUIを設定する</summary>
    private SortedList<DisplayModeEnum, List<string>> _fixedDisplayList = new SortedList<DisplayModeEnum, List<string>>();

    public enum DisplayModeEnum
    {
        None,
        Title,
        SetByTitle,
        NormalThirdPersonPerspective,
        FirstPersonView,
        OverlookFromSky,
        GameOver,
        GameClear,
    }

    /// <summary>
    /// 特定のモード時に固定で表示されるUIを設定
    /// </summary>
    /// <param name="displayModeEnum"></param>
    /// <param name="fixedUIName"></param>
   public void FixedDisplayUISet(DisplayModeEnum displayModeEnum, string fixedUIName)
    {
        //まだ特定のモード用のstring枠がない場合は新しく作る
        if (!_fixedDisplayList.ContainsKey(displayModeEnum))
        {
            _fixedDisplayList.Add(displayModeEnum, new List<string>());
        }

        //固定表示リストに追加
        _fixedDisplayList[displayModeEnum].Add(fixedUIName);
    }

    /// <summary>
    /// UIを表示(生成)する。表示が許可されていない場合は表示しない
    /// </summary>
    /// <param name="displayUIname"></param>
   public void Display(string displayUIname)
    {
      if(!PermissionConfirmationDisplayUI(displayUIname, GetComponent<UIManager>().CurrentDisplayMode))Debug.LogWarning("UI "+displayUIname+"は表示する事を許可されていません");

        if (PermissionConfirmationDisplayUI(displayUIname, GetComponent<UIManager>().CurrentDisplayMode)) GenerateUI(displayUIname);
    }

    /// <summary>
    /// ディスプレイモードの切り替え
    /// </summary>
    /// <param name="switchingDisplayMode"></param>
    public void DisplayModeSwitching(DisplayModeEnum switchingDisplayMode)
    {
        //ひとつ前のモードを代入
        ModeBeforeSwitching = CurrentDisplayMode;

        CurrentDisplayMode = switchingDisplayMode;

        RemoveUnauthorizedUI();

        //固定で表示されるUIsをモード切り替え時に表示(生成)する
        if (_fixedDisplayList.ContainsKey(CurrentDisplayMode))
        {
            foreach (var fixedDisplayUIName in _fixedDisplayList[CurrentDisplayMode])
            {
                Display(fixedDisplayUIName);
            }
        }
    }
}
