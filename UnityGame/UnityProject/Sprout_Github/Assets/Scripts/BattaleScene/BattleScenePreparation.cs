using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// <summary>
/// オブジェクトの生成や配置などの準備を段階ごとにしていくクラス
/// </summary>
public class BattleScenePreparation : SingletonMonoBehaviour<BattleScenePreparation>
{
    [SerializeField] private GameObject m_loadingScreen;
    //参照するクラスの変数達
    private GroundManager _groundManager;
    private RandomBuildingCreation _RandomBuildingCreation;
    private CharacterGenerateManager _characterGenerateManager;
    private PlayerInputManager _playerInputManager;
    private CameraManager _cameraManager;
    private CharacterTeamsData _characterTeamsData;
    private GridData _gridData;
    private UIManager _uIManager;
    private TargetingByThePlayer _targetingByThePlayer;
    private MouseCursorManager _mouseCursorManager;
    private BattleSceneEndProcedure _battleSceneEndProcedure;

    /// <summary>現在の準備段階を表すint番号</summary>
    private Steps _step = 0;

    private CharacterTeamsData.TeamData _generatePlayerTeamData;
    private CharacterTeamsData.TeamData _generateBossEnemyTeamData;

    private void Start()
    {

        //このクラスで使用するクラスを探して変数に代入する
        _groundManager = gameObject.SearchByTagName("SingletonManager", "GroundManager").GetComponent<GroundManager>();
        _RandomBuildingCreation = gameObject.SearchByTagName("SingletonManager", "StageBuildingManager").GetComponent<RandomBuildingCreation>();
        _characterGenerateManager = gameObject.SearchByTagName("SingletonManager", "CharactersManager").GetComponent<CharacterGenerateManager>();
        _playerInputManager = gameObject.SearchByTagName("SingletonManager", "PlayerOnlyManager").GetComponent<PlayerInputManager>();
        _targetingByThePlayer = gameObject.SearchByTagName("SingletonManager", "PlayerOnlyManager").GetComponent<TargetingByThePlayer>();
        _cameraManager = gameObject.SearchByTagName("SingletonManager", "CamaratManager").GetComponent<CameraManager>();
        _characterTeamsData = gameObject.SearchByTagName("SingletonManager", "CharactersManager").GetComponent<CharacterTeamsData>();
        _gridData = gameObject.SearchByTagName("SingletonManager", "GridManager").GetComponent<GridData>();
        _uIManager = gameObject.SearchByTagName("SingletonManager", "CanvasAndUIManager").GetComponent<UIManager>();
        _mouseCursorManager = gameObject.SearchByTagName("SingletonManager", "MouseCursorManager").GetComponent<MouseCursorManager>();
        _battleSceneEndProcedure = gameObject.SearchByTagName("SingletonManager", "BattleSceneEndProcedure").GetComponent<BattleSceneEndProcedure>();

        //ゲーム開始準備中はカーソルを使用しないので非表示
        _mouseCursorManager.MouseCursor(false);
    }

    enum Steps : int
    {
        GroundGenerate = 0,
        GroundGenerateing,
        NaturalStructureGenerate,
        NaturalStructureGenerateing,
        PlayerGenerateRelated,
        CompanionOfPlayerGenerateRelated,
        CompanionOfPlayerGenerateRelateding,
        BossEnemyGenerate,
        BossEnemyGenerateing,
        Camera,
        UIsSetting,
        BattleSceneEndProcedureSeting,
        CloseLoadingScreen,
        CloseLoadingScreening,
        StepEnd
    }

    private void FixedUpdate()
    {
        Execution();
    }

    /// <summary>
    /// 準備を一ステップごとに順番に実行していく
    /// </summary>
    private void Execution()
    {
        switch (_step)
        {

            case Steps.GroundGenerate:
                //地面の生成
                _groundManager.Generate();

                //次
                _step = Steps.NaturalStructureGenerate;
                break;


            case Steps.NaturalStructureGenerate:
                _RandomBuildingCreation.CreationStart();
                //次
                _step = Steps.NaturalStructureGenerateing;
                break;
            case Steps.NaturalStructureGenerateing:
                if (!_RandomBuildingCreation.Creating)
                {
                    //次
                    _step = Steps.PlayerGenerateRelated;
                }
                break;


            case Steps.PlayerGenerateRelated:
                //プレイヤーのチームとプレイヤー本体を生成
                CharacterTeamsData.TeamData generateTeamData = _characterGenerateManager.NewTeamAndLeaderGenerate(CharacterGenerate<CharacterGenerateManager>.Camp.Player, new BasicStatusData
                {
                    Name = "あなた",
                    PhysicalFitness = 5000,
                    MaxPhysicalFitness = 5000,
                    DefensePower = 5,
                    MoveMaxSpeed = 20,
                    MoveSpeed = 60,
                    OffensivePower = 700,
                    JumpFrequency = 5
                });

                //プレイヤーチームの参照型のTeamDataを取得する
                _generatePlayerTeamData = _characterTeamsData.TeamDatas.ElementAt((int)_characterTeamsData.TeamDataIndexInSearchByTeamObject(generateTeamData.TeamObject));

                //チームが建てた建築物のタグ名
                _generatePlayerTeamData.ArchitecturalBuildingTagName = "PlayersBuilding";

                //プレイヤーチームの攻撃対象のオブジェクトタグを設定する
                _generatePlayerTeamData.AttackTargetTags.Add("Enemy");
                _generatePlayerTeamData.AttackTargetTags.Add("Building");
                _generatePlayerTeamData.AttackTargetTags.Add("EnemyBuilding");
                _generatePlayerTeamData.AttackTargetTags.Add("EnemyBoss");

                //プレイヤーの率いるチームオブジェクトのタグを"Player"にする
                _generatePlayerTeamData.TeamObject.tag = "Player";

                //デフォルトのレイヤーはCompanionOfPlayer。プレイヤーとして扱いたいのでレイヤーをPlayerにする
                _generatePlayerTeamData.LeaderObject.transform.AllChildObjectsDelegateProcess((GameObject childGos) =>
                {
                    childGos.layer = 10;
                }, true);

                //プレイヤーチームトをPlayerInputManagerにセットする
                _playerInputManager.OperationTeamDataSet(ref _generatePlayerTeamData);

                //プレイヤーのBodyを初期場所に移動させる
                _generatePlayerTeamData.LeaderObject.transform.Find("BodyCollider").gameObject.transform.position = new Vector3(0, 50, 0);

                //プレイヤーオブジェクトはプレイヤーが操作するので自動制御オブジェクト(スクリプト達)は非表示にする
                _generatePlayerTeamData.LeaderObject.transform.Find("AutomaticControlManager").gameObject.SetActive(false);

                //次
                _step = Steps.CompanionOfPlayerGenerateRelated;
                break;


            case Steps.CompanionOfPlayerGenerateRelated:

                //プレイヤーチームにプレイヤーの仲間を追加する
                for (int i = 0; i < 100; i++)
                {
                    //ランダムに決める際に現在体力と最大体力を一緒の値にしたいのでここで求める
                    int maxPhysicalFitness = Random.Range(1000, 5000);

                    _characterGenerateManager.FollowCharacterGenerate_BasicStatusData(ref _generatePlayerTeamData, new BasicStatusData
                    {
                        PhysicalFitness = maxPhysicalFitness,
                        MaxPhysicalFitness = maxPhysicalFitness,
                        DefensePower = Random.Range(0, 5),
                        MoveMaxSpeed = Random.Range(15, 20),
                        MoveSpeed = Random.Range(50, 60),
                        OffensivePower = Random.Range(600, 700),
                        JumpFrequency = Random.Range(4, 6)
                    });
                }

                //次
                _step = Steps.BossEnemyGenerate;
                break;

            case Steps.BossEnemyGenerate:

                //敵のチームと敵ボス本体を生成
                CharacterTeamsData.TeamData generateBossEnemyTeamData = _characterGenerateManager.NewTeamAndLeaderGenerate(CharacterGenerate<CharacterGenerateManager>.Camp.Enemy, new BasicStatusData
                {
                    Name = "丸形の機械兵器",
                    PhysicalFitness = 100000,
                    MaxPhysicalFitness = 100000,
                    DefensePower = 45,
                    MoveMaxSpeed = 5,
                    MoveSpeed = 60,
                    OffensivePower = 1000,
                    JumpFrequency = 0
                });

                //チームの参照型のTeamDataを取得する
                _generateBossEnemyTeamData = _characterTeamsData.TeamDatas.ElementAt((int)_characterTeamsData.TeamDataIndexInSearchByTeamObject(generateBossEnemyTeamData.TeamObject));

                //ボス敵チームの攻撃対象のオブジェクトタグを設定する
                _generateBossEnemyTeamData.AttackTargetTags.Add("CompanionOfPlayer");
                _generateBossEnemyTeamData.AttackTargetTags.Add("Building");
                _generateBossEnemyTeamData.AttackTargetTags.Add("PlayersBuilding");
                _generateBossEnemyTeamData.AttackTargetTags.Add("Player");

                //敵ボスオブジェクトのBody(GameObject)を取得
                GameObject enemyBossBodyObject = _generateBossEnemyTeamData.LeaderObject.transform.Find("BodyCollider").gameObject;

                //敵ボスのBodyを初期場所に移動させる
                enemyBossBodyObject.transform.position = new Vector3((_gridData.OneSquareSize.x * (_gridData.GridSize.x - 1)), 50, (_gridData.OneSquareSize.z * (_gridData.GridSize.z - 1)));


                //次
                _step = Steps.Camera;
                break;


            case Steps.Camera:
                //カメラのターゲットをプレイヤーのBodyに設定する
                _cameraManager.CameraTarget = _generatePlayerTeamData.LeaderObject.transform.Find("BodyCollider").gameObject.transform;

                //ゲーム開始時のカメラモード
                _cameraManager.CurrentCameraMode = CameraManager.CameraMode.Tracking;

                //次
                _step = Steps.UIsSetting;
                break;


            case Steps.UIsSetting:
                //特定のモードに固定で表示されるUIを設定していく
                _uIManager.FixedDisplayUISet(UIManager.DisplayModeEnum.NormalThirdPersonPerspective, "TargetPhysicalFitnessDisplayUI");
                _uIManager.FixedDisplayUISet(UIManager.DisplayModeEnum.NormalThirdPersonPerspective, "PlayerPhysicalFitnessDisplayUI");
                _uIManager.FixedDisplayUISet(UIManager.DisplayModeEnum.NormalThirdPersonPerspective, "BattalSceneOperationExplanation_0");
                _uIManager.FixedDisplayUISet(UIManager.DisplayModeEnum.FirstPersonView, "TargetPhysicalFitnessDisplayUI");
                _uIManager.FixedDisplayUISet(UIManager.DisplayModeEnum.FirstPersonView, "BattalSceneOperationExplanation_1");
                _uIManager.FixedDisplayUISet(UIManager.DisplayModeEnum.OverlookFromSky, "PlayerCommandToTheTeamUI");
                _uIManager.FixedDisplayUISet(UIManager.DisplayModeEnum.GameClear, "GameClearUI");
                _uIManager.FixedDisplayUISet(UIManager.DisplayModeEnum.GameOver, "GameOverUI");

                //ディスプレイモード切り替え//PlayerInputManagerの方で勝手に切り替えが行われるが以下の処理をするためにこのフレームで切り替える
                _uIManager.DisplayModeSwitching(UIManager.DisplayModeEnum.NormalThirdPersonPerspective);

                //どの対象に注目するか、の情報を参照したいので_generatePlayerTeamDataを設定
                gameObject.SearchByTagName("UI", "TargetPhysicalFitnessDisplayUI").GetComponent<TargetPhysicalFitnessDisplayUI>().SetTeamMemberManager(_generatePlayerTeamData.TeamObject.GetComponent<TeamMemberManager>());
                //リーダー(プレイヤー)の体力を参照するためにTeamDataを設定する
                gameObject.SearchByTagName("UI", "PlayerPhysicalFitnessDisplayUI").GetComponent<PlayerPhysicalFitnessDisplayUI>().CharacterTeamsDataSet(ref _generatePlayerTeamData);

                //ターゲッティング対象の選別に必要
                _targetingByThePlayer.CharacterTeamsDataSet(ref _generatePlayerTeamData);
                //次
                _step = Steps.BattleSceneEndProcedureSeting;
                break;



            case Steps.BattleSceneEndProcedureSeting:
                //ゲームの終了条件を設定する
                _battleSceneEndProcedure.TriggerSet(_generateBossEnemyTeamData.LeaderObject, _generatePlayerTeamData.LeaderObject);
                //次
                _step = Steps.CloseLoadingScreen;
                break;



            case Steps.CloseLoadingScreen:
                Destroy(m_loadingScreen);

                //次
                _step = Steps.StepEnd;
                break;


            case Steps.StepEnd:

                //準備が完了したのでプレイヤーの操作を受け付ける
                _playerInputManager.RejectPlayerInput = false;

                //全てのステップが終了したので削除
                Destroy(gameObject);
                break;
        }



    }

}
