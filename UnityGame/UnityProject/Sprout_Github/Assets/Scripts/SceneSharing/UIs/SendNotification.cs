using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendNotification : MonoBehaviour
{
    [SerializeField]private string m_destinationObjectName;
   private CanvasChildUIBase _canvasChildUIBase;

    public void Send()
    {
        //送信先のクラス参照変数がnullの場合は探す
        if(_canvasChildUIBase==null) _canvasChildUIBase = gameObject.SearchByTagName("UI", m_destinationObjectName).GetComponent<CanvasChildUIBase>();

        _canvasChildUIBase.GetButtonNotifications(gameObject.name);
    }


}
