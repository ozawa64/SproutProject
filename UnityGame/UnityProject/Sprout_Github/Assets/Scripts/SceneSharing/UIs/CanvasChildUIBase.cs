using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasChildUIBase : MonoBehaviour
{
    /// <summary>trueにするとcanvasの一段下の子オブジェクトであれば削除から除外する</summary>
    public bool UIObjectParentDeleteExclusion = false;
    /// <summary>子の重複生成(表示)を許可する</summary>
    public bool PermissionDuplicateChildObjects = false;
    /// <summary>このUIが表示(機能)されるモード</summary>
    [SerializeField]private UIManager.DisplayModeEnum[] m_thisUIDisplayModes;
    /// <summary>子オブジェクトから取得したいオブジェクトの名前達</summary>
    [SerializeField] private string[] m_childObjectToGetFromName;


    /// <summary>
    /// ボタンからの通知(クリック)を受け取る
    /// </summary>
    public virtual void GetButtonNotifications(string notifications)
    {
        Debug.Log(notifications);
    }

    /// <summary>
    /// thisTransformの子からオブジェクトを探す。UnityのインスペクターのChildObjectToGetFromName[]に指定された名前のオブジェクトを返す、配列の順番は指定した名前と一致
    /// </summary>
    protected GameObject[] FindsTheObjectWithTheSpecifiedName()
    {
        GameObject[] returnObjects=new GameObject[m_childObjectToGetFromName.Length];

        //全ての子オブジェクト配列で探す
        foreach (var child in transform.returnAllChild())
        {
            //取得したいオブジェクト(m_childObjectToGetFromName)が見つかったら名前の配列と同じインデックス(※番号の同じ、配列は別)に代入する
            for (int i = 0; i < m_childObjectToGetFromName.Length; i++)
            {
                if (m_childObjectToGetFromName[i] == child.name)
                {
                    returnObjects[i] = child.gameObject;
                    break;
                }
            }
        }

        return returnObjects;
    }

    /// <summary>
    /// 現在のDisplayModeEnumがこのUIが機能して良いEnumの場合はtrueを返す。
    /// </summary>
    /// <param name="currentDisplayModeEnum"></param>
    /// <returns></returns>
    protected bool ThisUIDisplayModeCheck(UIManager.DisplayModeEnum currentDisplayModeEnum)
    {
        //現在のDisplayModeEnumとこのUIが機能出来るDisplayModeEnum配列を照合して確認する
        foreach (var thisUIDisplayModes in m_thisUIDisplayModes)
        {
            if (thisUIDisplayModes == currentDisplayModeEnum) return true;
        }

        //照合した結果全てスルーされた場合は現在機能出来ないモードなのでfasleを返す。
        return false;
    }

    /// <summary>
    /// UIオブジェクト達の中にnullが含まれている場合にtrueを返す。
    /// </summary>
    /// <returns></returns>
    protected virtual bool UIObjectContainsNull()
    {
        return false;
    }

    /// <summary>
    /// UIの更新に必要なオブジェクト達を探す
    /// </summary>
    protected virtual void FindUIObjects()
    {

    }

}
