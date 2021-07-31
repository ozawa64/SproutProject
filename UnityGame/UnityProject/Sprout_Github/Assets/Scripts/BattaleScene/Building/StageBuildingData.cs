using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ上の建物のデータを保管するクラス
/// </summary>
public class StageBuildingData: SingletonMonoBehaviour<StageBuildingData>
{
    /// <summary>ステージ上に存在する全ての建物のデータ達</summary>
    private GameObject[,,] _data_GameObject;
    /// <summary>ステージ上に存在する全ての建物のBasicStatusData達</summary>
    private BasicStatusData[,,] _data_BasicStatusData;
    /// <summary>グリッドの基本データ</summary>
    private GridData _gridData;

    private new void Awake()
    {
        base.Awake();

        //このクラスで使用するクラスを探して変数に代入する
        _gridData= gameObject.SearchByTagName("SingletonManager", "GridManager").GetComponent<GridData>();
       
        //グリッドのマスの個数に応じて建物保管データのサイズを決める。
        _data_GameObject = new GameObject[_gridData.GridSize.x, _gridData.GridSize.y, _gridData.GridSize.z];

        //_data_GameObjectに合わせる
        _data_BasicStatusData = new BasicStatusData[_data_GameObject.GetLength(0), _data_GameObject.GetLength(1), _data_GameObject.GetLength(2)];
    }


    /// <summary>
    /// GameObjectを取得する。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>指定されたグリッドのGameObjectを返す</returns>
    public GameObject GameObjectDataGet((int x, int y, int z)grid)
    {
        if(_data_GameObject.CheckIfIsOutIndexNumber(grid.x, grid.y, grid.z))
        {
            return this._data_GameObject[grid.x, grid.y, grid.z];
        }
        else
        {
            return null;
        }
    }


    /// <summary>
    /// GameObjectをセットする。既にある場合は既存のオブジェクトを削除してセットする。
    /// </summary>
    /// <param name="setGO"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>セットできない場合はfalseを返す</returns>
    public bool DataSet(GameObject setGO , (int x, int y, int z)grid)
    {
        if (_data_GameObject.CheckIfIsOutIndexNumber(grid.x, grid.y, grid.z))
        {

            if (this._data_GameObject[grid.x, grid.y, grid.z] != null) Destroy(this._data_GameObject[grid.x, grid.y, grid.z]);

            this._data_GameObject[grid.x, grid.y, grid.z]= setGO;

            //thisTransformを親に設定
            setGO.transform.parent = transform;

            return true;
        }
        else
        {
            Debug.LogWarning("セットするGameObjectが配列外を指定しています");

            return false;

        }

    }

    /// <summary>
    /// BasicStatusDataを取得する。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>指定されたグリッドのBasicStatusDataを返す</returns>
    public ref BasicStatusData BasicStatusDataGet((int x, int y, int z)gird)
    {
        if (_data_BasicStatusData.CheckIfIsOutIndexNumber(gird.x, gird.y, gird.z))
        {
            return ref this._data_BasicStatusData[gird.x, gird.y, gird.z];
        }
        else
        {
            Debug.LogError("インデックス番号が配列外です");
            return ref this._data_BasicStatusData[gird.x, gird.y, gird.z];
        }
    }


    /// <summary>
    /// BasicStatusDataをセットする。既にある場合は上書きする。※削除する場合はnullをセットする
    /// </summary>
    /// <param name="setGO"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>セットできない場合はfalseを返す</returns>
    public bool DataSetAndGenerate(BasicStatusData setBasicStatusData, (int x, int y, int z) grid)
    {
        if (_data_BasicStatusData.CheckIfIsOutIndexNumber(grid.x, grid.y, grid.z))
        {
            this._data_BasicStatusData[grid.x, grid.y, grid.z] = setBasicStatusData;

            return true;
        }
        else
        {
            Debug.LogWarning("セットするBasicStatusDataが配列外を指定しています");

            return false;

        }

    }



}
