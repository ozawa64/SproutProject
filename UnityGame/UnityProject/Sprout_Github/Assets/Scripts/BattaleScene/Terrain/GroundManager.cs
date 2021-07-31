using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : SingletonMonoBehaviour<GroundManager>
{
    //参照変数
    //地面のサイズと配置場所の調整に使用
    [SerializeField] GridData m_GridData;

    /// <summary>地面オブジェクトの配置される高さ</summary>
    [SerializeField] float m_Height=0;
    /// <summary>地面オブジェクトのサイズ比率を除算で調整</summary>
    [SerializeField] Vector3 m_ObjectRatioAdjustment_Division = new Vector3(10, 10, 10);
    [SerializeField] private GameObject m_groundPrefab;

    /// <summary>生成した地面GameObjectのリスト配列</summary>
    private List<GameObject> groundObjects;

    private new void Awake()
    {
        base.Awake();

        groundObjects = new List<GameObject>();
    }

    /// <summary>
    /// 地面を生成する。生成する際に既に地面が生成されていた場合、その地面を削除して新しく生成する
    /// </summary>
    public void Generate()
    {
        //生成するにあたり既存の地面オブジェクトがないか確認
        if (groundObjects.Count == 0)
        {//ない場合

            NotHollowedOutGenerate();
        }
        else
        {//ある場合

            Remove();

            NotHollowedOutGenerate();
        }
    }

    /// <summary>
    /// 地面の削除
    /// </summary>
    public void Remove()
    {
        foreach (var item in groundObjects)
        {
            Destroy(item);
        }
    }

    /// <summary>
    /// 穴の開いた地面を生成する
    /// </summary>
    private void HollowedOutGenerate()
    {

    }

    /// <summary>
    /// 穴の開いていない地面を生成する
    /// </summary>
    private void NotHollowedOutGenerate()
    {
        //地面オブジェクトのプレハブを参照
        GameObject m_groundPrefab =  (GameObject)Resources.Load("BattaleScene/Grounds/Ground"); 

        //地面の生成
         GameObject ground_GO = Instantiate(m_groundPrefab);

        //地面の配置場所の計算
        Vector3 groundPosition;//グリッドの左下の一マス目の中心はxz、それぞれで0の座標にするのでそれに合わせて地面オブジェクトも調整する。
        groundPosition.x = (m_GridData.OneSquareSize.x * (m_GridData.GridSize.x-1))/2;
        groundPosition.y = m_Height;
        groundPosition.z = (m_GridData.OneSquareSize.z * (m_GridData.GridSize.z-1))/2;
        //場所の代入
        ground_GO.transform.position = groundPosition;

        //地面のサイズ計算
        Vector3 groundSize;
        groundSize.x = (m_GridData.OneSquareSize.x * m_GridData.GridSize.x) / m_ObjectRatioAdjustment_Division.x;
        groundSize.y = 1;//パネル型の地面なのでサイズは1固定
        groundSize.z = (m_GridData.OneSquareSize.z * m_GridData.GridSize.z) / m_ObjectRatioAdjustment_Division.z;
        //サイズの代入
        ground_GO.transform.localScale = groundSize;
    }


}
