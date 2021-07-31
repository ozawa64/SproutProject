using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ミス(後で修正)押し出しの範囲判定を建物のコライダーの判定と合わせられていない
public class PushToEmptyGrid : EventBase
{

    [SerializeField]private float m_pushPower=10;
    [SerializeField]private LayerMask m_pushLayer;
    [SerializeField]private string[] m_pushTags;

    protected GridCalculation _gridCalculation;
    protected GridData _gridData;
    protected StageBuildingData _stageBuildingData;

    protected void Start()
    {
        _gridCalculation = gameObject.SearchByTagName("SingletonManager", "GridManager").GetComponent<GridCalculation>();
        _gridData = gameObject.SearchByTagName("SingletonManager", "GridManager").GetComponent<GridData>();
        _stageBuildingData = gameObject.SearchByTagName("SingletonManager", "StageBuildingManager").GetComponent<StageBuildingData>();
    }

    /// <summary>
    /// 指定したオブジェクトタグを指定したグリッドから空いているグリッドに押す
    /// </summary>
    protected  void PushFromTheGrid((int x, int y, int z) gridToExtrude)
    {
        (int x, int y, int z)? freeGrid = FreeGrid();

        //空いているグリッドがないと押し出せないので以下の処理はせずに終了
        if (freeGrid == null) return;

        //対象のタグとレイヤーを持っているオブジェクトのpositionをm_pushPower分空きグリッドに移動させる
        foreach (var collider in Physics.OverlapBox(_gridCalculation.Vector3PositionGet(gridToExtrude), (_gridData.OneSquareSize / 2), Quaternion.identity, m_pushLayer))
        {
            //BodyCollider以外は押し出さない
            if (collider.name != "BodyCollider")continue;

            foreach (var targetTag in m_pushTags)
            {

                if(targetTag== collider.tag)
                {

                    //空いているグリッドを押し出すオブジェクトの目的地にする
                    Vector3  destination = _gridCalculation.Vector3PositionGet(((int x, int y, int z))freeGrid);

                    //建物オブジェクトはグリッドの中心を底辺として扱っているため
                    destination.y += _gridData.OneSquareSize.y/2;

                    //移動後の位置を計算する
                    Vector3 movePosition = Vector3.MoveTowards(collider.transform.position, destination, m_pushPower);

                    //※バグ修正 y軸のみ押す力を三倍にする。重力の影響で押す力が不足するため
                    movePosition.y = Mathf.MoveTowards(collider.transform.position.y,destination.y, m_pushPower * 3);

                    //移動
                    collider.transform.position = movePosition;
                    break;
                }
            }
        }

        //隣接していて空いているグリッドを返す
       (int x,int y,int z)? FreeGrid()
        {
            //確認するグリッドに存在するGameObject又はnull
            GameObject confirmationGO;

            //確認したグリッドに空気オブジェクトが存在していた場合、そこが空いているグリッドなのでその場所のグリッド番号を返す。
            //+z軸
            confirmationGO = _stageBuildingData.GameObjectDataGet((gridToExtrude.x, gridToExtrude.y, gridToExtrude.z + 1));
            if (confirmationGO !=null&& confirmationGO.name == BuildingGenerateManager.Buildings.Air.ToString()) return (gridToExtrude.x, gridToExtrude.y, gridToExtrude.z + 1);
           
            //-z軸
            confirmationGO = _stageBuildingData.GameObjectDataGet((gridToExtrude.x, gridToExtrude.y, gridToExtrude.z + -1));
            if (confirmationGO !=null&& confirmationGO.name == BuildingGenerateManager.Buildings.Air.ToString()) return (gridToExtrude.x, gridToExtrude.y, gridToExtrude.z + -1);
            //+x軸
            confirmationGO = _stageBuildingData.GameObjectDataGet((gridToExtrude.x + 1, gridToExtrude.y, gridToExtrude.z));
            if (confirmationGO !=null&& confirmationGO.name == BuildingGenerateManager.Buildings.Air.ToString()) return (gridToExtrude.x + 1, gridToExtrude.y, gridToExtrude.z);
            //-x軸
            confirmationGO = _stageBuildingData.GameObjectDataGet((gridToExtrude.x + -1, gridToExtrude.y, gridToExtrude.z));
            if (confirmationGO !=null&& confirmationGO.name == BuildingGenerateManager.Buildings.Air.ToString()) return (gridToExtrude.x + -1, gridToExtrude.y, gridToExtrude.z);
            //+y軸
            confirmationGO = _stageBuildingData.GameObjectDataGet((gridToExtrude.x, gridToExtrude.y + 1, gridToExtrude.z));
            if (confirmationGO !=null&& confirmationGO.name == BuildingGenerateManager.Buildings.Air.ToString()) return (gridToExtrude.x, gridToExtrude.y + 1, gridToExtrude.z);


            return null;

        }


    }


}
