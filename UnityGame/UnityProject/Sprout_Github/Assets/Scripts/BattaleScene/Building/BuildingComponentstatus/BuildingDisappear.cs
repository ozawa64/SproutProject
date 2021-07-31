using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDisappear : MonoBehaviour
{
    public bool Disappearing { get; private set; } = false;

    private BasicStatusDataAccess _basicStatusDataAccess;
    private BuildingAnimationManager _buildingAnimationManager;
    private StageBuildingData _stageBuildingData;
    private BuildingGenerateManager _buildingGenerateManager;

    private void Start()
    {
        _basicStatusDataAccess = gameObject.GetComponent<BasicStatusDataAccess>();
        _buildingAnimationManager = GetComponent<BuildingAnimationManager>();
        _stageBuildingData = gameObject.SearchByTagName("SingletonManager", "StageBuildingManager").GetComponent<StageBuildingData>();
        _buildingGenerateManager = gameObject.SearchByTagName("SingletonManager", "StageBuildingManager").GetComponent<BuildingGenerateManager>();

    }

    private void FixedUpdate()
    {
        if (!_basicStatusDataAccess.Access) return;
        //体力が0以下でステージから退場し始める
        if (_basicStatusDataAccess.PhysicalFitness <= 0 && Disappearing == false) StartDisappear();
    }

    public void StartDisappear()
    {
        Disappearing = true;

        //削除するので当たり判定を停止(非アクティブ化)する
        transform.Find("BodyCollider").gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    public void GameObjectAndBasicStatusDataDelete()
    {

        _buildingGenerateManager.Generate(BuildingGenerateManager.Buildings.Air, ((int)_basicStatusDataAccess.BasicStatusDataIndex_3d.Value.Item1, (int)_basicStatusDataAccess.BasicStatusDataIndex_3d.Value.Item2, (int)_basicStatusDataAccess.BasicStatusDataIndex_3d.Value.Item3));

        //nullをセットしてBasicStatusDataを削除
       // _stageBuildingData.DataSetAndGenerate(null,((int)_basicStatusDataAccess.BasicStatusDataIndex_3d.Value.Item1, (int)_basicStatusDataAccess.BasicStatusDataIndex_3d.Value.Item2, (int)_basicStatusDataAccess.BasicStatusDataIndex_3d.Value.Item3));

        //Destroy(gameObject);
    }
}
