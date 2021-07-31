using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 本来行けない場所にいる場合に強制的に移動させる
/// </summary>
public class WarpDueToUnexpectedMove : MonoBehaviour
{
    [SerializeField] Transform m_MonitoringTarget = null;

    private void FixedUpdate()
    {
        if (m_MonitoringTarget == null) return;

        //想定外の場所にいたら指定した高さにワープ
        if (m_MonitoringTarget.position.y < -10)
        {
            m_MonitoringTarget.position = new Vector3(m_MonitoringTarget.position.x, 50, m_MonitoringTarget.position.z);
        }

    }

    /*
    private void Start()
    {
        StartCoroutine("Monitoring");
    }

    private IEnumerator Monitoring()
    {
        while (true)
        {
            if (m_MonitoringTarget == null) yield break;

            //想定外の場所にいたら指定した高さにワープ
            if (m_MonitoringTarget.position.y < -10)
            {
                m_MonitoringTarget.position = new Vector3(m_MonitoringTarget.position.x, 50, m_MonitoringTarget.position.z);
            }

            yield return new WaitForSeconds(5);
        }
    }
    */
}
