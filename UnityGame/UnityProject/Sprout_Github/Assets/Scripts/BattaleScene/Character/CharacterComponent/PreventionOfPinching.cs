using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventionOfPinching : MonoBehaviour
{
    [SerializeField] private Transform m_pushTransform;
    [SerializeField]private float m_pushPower=1;
    [SerializeField] private string[] m_targetObjectsTagName;

    private void OnTriggerStay(Collider other)
    {
        foreach (var tagName in m_targetObjectsTagName)
        {
            if (other.transform.parent != null && tagName == other.transform.parent.tag)
            {
                m_pushTransform.position = Vector3.MoveTowards(m_pushTransform.position, m_pushTransform.position + (-(other.transform.position- m_pushTransform.position)), m_pushPower);
                return;
            }

        }
    }

}
