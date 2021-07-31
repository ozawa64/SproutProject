using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchitectureEventManager : PushToEmptyGrid
{
    public BuildingGenerateManager.Buildings ArchitectureBuilding { get; set; }
    /// <summary>建築するグリッド(場所)</summary>
    public (int x, int y, int z) ArchitectureGird { get; set; }

    /// <summary>建築完了にかかる時間</summary>
    public float ArchitectureTime { get; set; } = 30f;

    [SerializeField] private float m_pushTime=5f;
    
    private float _pushTimeCount=0;
    private float _architectureTimeCount = 0;
    private BuildingGenerateManager _buildingGenerateManager;
    /// <summary>建築処理の現在の段階</summary>
    private ArchitectureStepEnum _currentArchitectureStep = ArchitectureStepEnum.WaitingForArrival;
    private BasicStatusData _architectureBuildingBasicStatusData;
    /// <summary>建築する建物の名前。※建築中は「建築中」と名前に付けるので元の名前をこの変数で保持</summary>
    private string _architectureBuildingName;
    /// <summary>建築による体力の増加量(倍率)</summary>
    private float _physicalFitnessIncreaseAmount =0;
    enum ArchitectureStepEnum
    {
        WaitingForArrival,
        OutsiderPushToEmptyGrid,
        BuildingPreparation,
        UnderConstruction,
        ConstructionCompleted,
    }

    public override void EventForcedTermination()
    {
        //建築命令の目的を目的リストから削除する
        triggerObject_TeamData.TeamObject.GetComponent<TeamMemberManager>().Purposes.Remove((transform, TeamMemberManager.PurposeEnum.Architecture));

        //BasicStatusDataが存在していたら名前から「(建築中)」を取り除いた元の名前を表示する
        if (_architectureBuildingBasicStatusData!=null) _architectureBuildingBasicStatusData.Name = _architectureBuildingName;

        //建物から出たことを表現するため表示する
        triggerObject_TeamData.TeamObject.SetActive(true);

        //イベント強制終了処理が終了したらこのオブジェクトは削除する
        Destroy(gameObject);

    }

    protected new void Start()
    {
        base.Start();

        _buildingGenerateManager = gameObject.SearchByTagName("SingletonManager", "StageBuildingManager").GetComponent<BuildingGenerateManager>();
    }

    private void FixedUpdate()
    {
        //引き金となるチームが存在しなくなった場合はイベントを終了させる
        if (triggerObject_TeamData == null) EventForcedTermination();
        //リーダーがいない場合は以下の処理はしない(出来ない)。※リーダーがいないだけの場合はまだフォローキャラクターがリーダーの代理になる可能性があるのでイベントは終了させない
        if (triggerObject_TeamData.LeaderObject == null) return;

        switch (_currentArchitectureStep)
        {
            case ArchitectureStepEnum.OutsiderPushToEmptyGrid:

                PushFromTheGrid(ArchitectureGird);

                //押し出す時間が終了したら次の段階に移る
                _pushTimeCount += Time.deltaTime;
                if(_pushTimeCount >= m_pushTime)
                {
                    _currentArchitectureStep = ArchitectureStepEnum.BuildingPreparation;
                }

                break;


            case ArchitectureStepEnum.BuildingPreparation:

                //建物を建築するグリッドに生成して、生成した建物オブジェクトのタグを変更する
                _buildingGenerateManager.Generate(ArchitectureBuilding,ArchitectureGird).transform.AllChildObjectsDelegateProcess((GameObject childGo) =>
                {
                    childGo.tag = triggerObject_TeamData.ArchitecturalBuildingTagName;
                }, true);

                //建築する建物のBasicStatusDataを取得する
                _architectureBuildingBasicStatusData = _stageBuildingData.BasicStatusDataGet(ArchitectureGird);

                //建物の元の名前を保持
                _architectureBuildingName = _architectureBuildingBasicStatusData.Name.ToString();
                //名前に「(建築中)」を追加
                _architectureBuildingBasicStatusData.Name += " (建築中)";

                //建築時間の計測。以下の理由でUnderConstructionで開始していないことが分かる
                _architectureTimeCount += Time.deltaTime;
                //建築の完成が近づくほど体力が最大体力に近づく。※体力を0の状態にすると建物の消滅処理が実行されてしまうので最初に少しだけ建築時間を進めて開始する
                _architectureBuildingBasicStatusData.PhysicalFitness = Mathf.CeilToInt(_architectureBuildingBasicStatusData.PhysicalFitness * (_architectureTimeCount / ArchitectureTime));

                //次の段階に移る
                _currentArchitectureStep = ArchitectureStepEnum.UnderConstruction;
                break;


            case ArchitectureStepEnum.UnderConstruction:


                //建築する建物のBasicStatusDataを取得する
                _architectureBuildingBasicStatusData = _stageBuildingData.BasicStatusDataGet(ArchitectureGird);

                //建築中の建物のBasicStatusDataがnull(移動や破壊)になった場合はイベントを強制終了させる
                if (_architectureBuildingBasicStatusData == null)
                {
                    EventForcedTermination();
                    return;
                }

                //建築時間の計測。
                _architectureTimeCount += Time.deltaTime;

                //体力の増加倍率は毎回更新する(フレーム間の時間は変化することがあるため)
                _physicalFitnessIncreaseAmount=(Time.deltaTime / ArchitectureTime);

                //建築時間が進んだ分だけ体力を増加させる
                _architectureBuildingBasicStatusData.PhysicalFitness += Mathf.CeilToInt(_architectureBuildingBasicStatusData.MaxPhysicalFitness * _physicalFitnessIncreaseAmount);
                //増加した体力が最大体力を超えた場合は最大体力にする
                if (_architectureBuildingBasicStatusData.PhysicalFitness > _architectureBuildingBasicStatusData.MaxPhysicalFitness) _architectureBuildingBasicStatusData.PhysicalFitness = _architectureBuildingBasicStatusData.MaxPhysicalFitness;

                //建築時間が終了したら次の段階に移る
                if (_architectureTimeCount >= ArchitectureTime)
                {
                    _currentArchitectureStep = ArchitectureStepEnum.ConstructionCompleted;
                }
                break;


            case ArchitectureStepEnum.ConstructionCompleted:

                //強制終了関数を使用しているが通常終了と処理内容が一緒なのでそのまま使う
                EventForcedTermination();
                
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //引き金となるチームが存在しなくなった場合はイベントを終了させる
        if (triggerObject_TeamData == null) EventForcedTermination();
        //リーダーがいない場合は以下の処理はしない(出来ない)。※リーダーがいないだけの場合はまだフォローキャラクターがリーダーの代理になる可能性があるのでイベントは終了させない
        if (triggerObject_TeamData.LeaderObject == null) return;
        //イベントの段階が既に進んでいる場合は処理しない
        if (_currentArchitectureStep != ArchitectureStepEnum.WaitingForArrival) return;

        //引き金となるチームのリーダーがイベント判定に入ったか
        if (triggerObject_TeamData.LeaderObject.transform.Find("BodyCollider").gameObject == other.gameObject)
        {
            //建築場所にいる部外者を空きのグリッドに押し出す処理をする
            _currentArchitectureStep = ArchitectureStepEnum.OutsiderPushToEmptyGrid;

            //建築チームを非表示にして建物の中で建築している表現にする
            triggerObject_TeamData.TeamObject.SetActive(false);
        }
    }

}
