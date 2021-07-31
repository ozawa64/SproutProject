using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャンバスにオブジェクトを追加する際のサポートをする
/// </summary>
public class CanvaAdditionalSupport<T> : SimultaneousUIDisplayPermission<T> where T : MonoBehaviour
{

    /// <summary>
    /// キャンバスの子オブジェクトにしながら場所、サイズを調整する。
    /// </summary>
    /// <param name="addGo"></param>
    protected void CanvasAdd(GameObject addGo)
    {
        //canvasの子オブジェクトにする
        addGo.transform.SetParent(transform);
        //サイズ調整
        addGo.transform.localScale = Vector3.one;
        addGo.GetComponent<RectTransform>().sizeDelta = Vector3.zero;
        //場所調整
        addGo.transform.position = GetComponent<RectTransform>().localPosition;

        //アンカーからの距離を調整
        addGo.GetComponent<RectTransform>().offsetMax = new Vector2(addGo.GetComponent<RectTransform>().offsetMax.x, 0);
        addGo.GetComponent<RectTransform>().offsetMin = new Vector2(addGo.GetComponent<RectTransform>().offsetMin.x, 0);
    }
}
