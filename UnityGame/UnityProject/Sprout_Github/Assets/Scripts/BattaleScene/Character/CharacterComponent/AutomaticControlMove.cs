using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticControlMove : MonoBehaviour
{
    /// <summary>道のり。現在のグリッドから一マス進んだ(進むべき)グリッド</summary>
    public (int x, int y, int z)? NextGrid { get; private set; } = null;
    
    protected GridCalculation gridCalculation;

    /// <summary>コントロールするオブジェクト本体</summary>
    protected GameObject controlBody=null;
    /// <summary>到着したことにする範囲(許容範囲)</summary>
    [SerializeField] protected Vector3 m_arrivalJudgmentRange = new Vector3(5,10,5);
    /// <summary>最終目的地</summary>
    private Transform _finalDestination=null;
  


    protected void Start()
    {

        foreach (var child in transform.parent.returnAllChild())
        {
            if (child.name == "BodyCollider") controlBody = child;
        } 

        gridCalculation = gameObject.SearchByTagName("SingletonManager", "GridManager").GetComponent<GridCalculation>();

    }

    /// <summary>
    /// 移動したい方向のベクトルを返す。※プレイヤーの入力と同等の扱い
    /// </summary>
    /// <returns></returns>
    protected Vector3? MovingVector()
    {
        //目的地や移動するオブジェクトがないと計算出来ない
        if (controlBody == null) return null;
        if (_finalDestination == null) return null;

        //今行くべきグリッドが既に計算されている場合はそこに向かう
        if(NextGrid != null)
        {
            //目的地となっているグリッドに到着しているか判断
            if(controlBody.transform.position.CheckIfItIsWithinTheRange(gridCalculation.Vector3PositionGet(((int x, int y, int z))NextGrid), m_arrivalJudgmentRange))
            {
                //到着していたらNextGridをnullにして次のグリッドを計算するために以下の処理に進める
                NextGrid = null;
            }
            else
            {
                //現在向かうべき移動方向を求め返す
                return (gridCalculation.Vector3PositionGet(((int x, int y, int z))NextGrid) - controlBody.transform.position).normalized;
            }
        }

        //最終目的地のグリッドを求める
        (int x, int y, int z) finalDestination_grid = gridCalculation.GridNumberGet(_finalDestination.position);
        //現在地のグリッドを求める
        (int x, int y, int z) currentPosition_grid = gridCalculation.GridNumberGet(controlBody.transform.position);

        //次に進むべきグリッドを計算する。Y軸は除外する(飛べないので...)
        (int x, int y, int z) nextGrid ;
        nextGrid.y = 0;
        nextGrid.x = finalDestination_grid.x - currentPosition_grid.x;
        nextGrid.z = finalDestination_grid.z - currentPosition_grid.z;
        nextGrid.x = Mathf.Clamp(nextGrid.x, -1, 1);
        nextGrid.z = Mathf.Clamp(nextGrid.z, -1, 1);
        //xとz軸の両方に進みたい場合はx軸優先
        if (nextGrid.x == 1|| nextGrid.x == -1) nextGrid.z = 0;
        //現在地に進みたい方向のグリッドを足す
        nextGrid.x += currentPosition_grid.x;
        nextGrid.y += currentPosition_grid.y;
        nextGrid.z += currentPosition_grid.z;

        nextGrid.y = 0;

        NextGrid = nextGrid;


        //現在向かうべき移動方向を求め返す
        //  return (gridCalculation.Vector3PositionGet(((int x, int y, int z))NextGrid) - controlBody.transform.position).normalized;
        return Vector3.zero;//NextGridに到着したフレームは移動方向を0にする。これによって同じグリッドに最終目的地がある時に荒ぶることを防止出来る
    }

    /// <summary>
    /// 移動場所を設定する※道のりはこちらで計算
    /// </summary>
    /// <param name="locationTransform"></param>
   protected void SettingTheMovingLocation(Transform locationTransform)
    {
        _finalDestination = locationTransform;
    }


}
