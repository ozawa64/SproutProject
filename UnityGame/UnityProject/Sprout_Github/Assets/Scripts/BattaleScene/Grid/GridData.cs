using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// グリッドの基本情報を管理する
/// </summary>
public class GridData : SingletonMonoBehaviour<GridData>
{
    /// <summary>グリッド一マスの大きさ</summary>
     public Vector3 OneSquareSize { get; } = new Vector3(40, 25, 40);
    /// <summary>グリッドの範囲(マスの個数)</summary>
    public (int x,int y,int z) GridSize { get; } = (6, 3, 6);//(4, 3, 4)

}
