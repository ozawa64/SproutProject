using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// グリッドに関係する計算クラス
/// </summary>
public class GridCalculation : SingletonMonoBehaviour<GridCalculation>
{
    private GridData _gridData;

    private new void Awake()
    {
        base.Awake();

        //それぞれ必要なクラスやGameObjectを準備
        _gridData = gameObject.SearchByTagName("SingletonManager", "GridManager").GetComponent<GridData>();
    }

    /// <summary>
    /// xyzのグリッド番号に応じたVector3の場所情報を返す
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns></returns>
    public Vector3 Vector3PositionGet((int x,int y,int z)gridPosition)
    {
        Vector3 returnPosition = Vector3.zero;

        returnPosition.x = _gridData.OneSquareSize.x * gridPosition.x;
        returnPosition.y = _gridData.OneSquareSize.y * gridPosition.y;
        returnPosition.z = _gridData.OneSquareSize.z * gridPosition.z;

        return returnPosition;
    } 

    /// <summary>
    /// Vector3の場所情報をグリッドの番号に変換して返す
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public (int x, int y, int z) GridNumberGet(Vector3 position)
    {
        (int x, int y, int z) returnPosition=(0,0,0);

        returnPosition.x = int.Parse((position.x / _gridData.OneSquareSize.x).ToString("F0"));
        returnPosition.y = int.Parse((position.y / _gridData.OneSquareSize.y).ToString("F0"));
        returnPosition.z = int.Parse((position.z / _gridData.OneSquareSize.z).ToString("F0"));

        return returnPosition;
    }






}
