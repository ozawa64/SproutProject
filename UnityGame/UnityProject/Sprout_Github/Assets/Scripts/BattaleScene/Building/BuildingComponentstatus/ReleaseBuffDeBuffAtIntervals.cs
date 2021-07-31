using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseBuffDeBuffAtIntervals : MonoBehaviour
{
    //放つバフデバフ達の詳細//
    [SerializeField] private GivenBuffAndDeBuffManager.BuffAndDeBuffEnum[] m_releaseBuffDeBuffs;
    [SerializeField] private float[] m_effectingTime;
    [SerializeField] private int[] m_effectPower;
    [SerializeField] private bool[] m_duplicate;

    [SerializeField] private float m_intervalTime=3;
    [SerializeField] private float m_sphereReleaseAreaRadius = 40;
    [SerializeField] private string[] m_givenTags;

    private float _intervalTimeCount=0;
    private TargetSearch _targetSearch;

    private void Start()
    {
        _targetSearch = gameObject.SearchByTagName("SingletonManager", "TargetSearchs").GetComponent<TargetSearch>();
    }

    private void FixedUpdate()
    {
        _intervalTimeCount += Time.deltaTime;

        if(_intervalTimeCount >= m_intervalTime)
        {
            _intervalTimeCount = 0;

            ReleaseBuffDeBuff();
        }
    }

    /// <summary>
    /// 指定されたバフデバフを放つ
    /// </summary>
    private void ReleaseBuffDeBuff()
    {
        //バフデバフを周囲に放つ(周囲のコライダーを取得する)
        foreach (var givenObjectOfCollider in _targetSearch.SphereSearch(transform.Find("BodyCollider").position, m_sphereReleaseAreaRadius, m_givenTags))
        {
            //バフデバフを保持出来るクラス(GivenBuffAndDeBuffManager)があれば与える
            if (givenObjectOfCollider.gameObject.GetComponent<GivenBuffAndDeBuffManager>()!=null)
            {
                //複数のバフデバフがある場合はループして一つずつ与える
                for (int i = 0; i < m_releaseBuffDeBuffs.Length; i++)
                {
                    givenObjectOfCollider.gameObject.GetComponent<GivenBuffAndDeBuffManager>().GivenBuffOrDeBuff(gameObject,m_releaseBuffDeBuffs[i], m_effectingTime[i], m_effectPower[i],m_duplicate[i]);
                }
            }
        }  
    }

}
