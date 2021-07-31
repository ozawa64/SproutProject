using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : SingletonMonoBehaviour<PlayerInputManager>
{
    public bool RejectPlayerInput { get; set; } = true;
    public GameObject OperationNormalCharacterObject { get;private set; }

    private CharacterRunOrWalkMove _operation_CharacterRunOrWalkMove; 
    private CameraManager _cameraManager;
    private UIManager _uIManager;
    private NormalCharacterAnimationManager _normalCharacterAnimationManager;
    private TargetingByThePlayer _targetingByThePlayer;
    private bool _releaseTheJumpKye = true;
    private MouseCursorManager _mouseCursorManager;
    private CharacterTeamsData.TeamData _operationTeamData;
    private CharacterTeamManager _characterTeamManager;

    private void Start()
    {
        _cameraManager = gameObject.SearchByTagName("SingletonManager", "CamaratManager").GetComponent<CameraManager>();
        _uIManager = gameObject.SearchByTagName("SingletonManager", "CanvasAndUIManager").GetComponent<UIManager>();
        _mouseCursorManager = gameObject.SearchByTagName("SingletonManager", "MouseCursorManager").GetComponent<MouseCursorManager>();
        _characterTeamManager = gameObject.SearchByTagName("SingletonManager", "CharactersManager").GetComponent<CharacterTeamManager>();
        _targetingByThePlayer = GetComponent<TargetingByThePlayer>();
    }

    /// <summary>
    /// Playerが操作するチームオブジェクトを設定する
    /// </summary>
    /// <param name="normalCharacter"></param>
    public void OperationTeamDataSet(ref CharacterTeamsData.TeamData teamData)
    {
        _operationTeamData = teamData;

        OperationNormalCharacterObject = _operationTeamData.LeaderObject;
        _operation_CharacterRunOrWalkMove = OperationNormalCharacterObject.GetComponent<CharacterRunOrWalkMove>();
        _normalCharacterAnimationManager = OperationNormalCharacterObject.GetComponent<NormalCharacterAnimationManager>(); 
    }

    private void Update()
    {
        if (RejectPlayerInput) return;

        //操作するオブジェクトがない場合は何もしない
        if (OperationNormalCharacterObject == null) return;

        //-------------------仲間を呼ぶ-------------------//
       if(Input.GetKeyDown(KeyCode.C))_characterTeamManager.MemberGather(ref _operationTeamData);

            //-------------------攻撃-------------------//
        if (_cameraManager.CurrentCameraMode != CameraManager.CameraMode.OverlookFromSky && Input.GetButtonDown("Fire1")) _normalCharacterAnimationManager.AttackAnimation();

        //-------------------カメラ-------------------//
        //カメラモード
        if (_cameraManager.CurrentCameraMode == CameraManager.CameraMode.Tracking && Input.GetMouseButtonDown(1)) _cameraManager.CurrentCameraMode = CameraManager.CameraMode.FirstPersonView;
        if (_cameraManager.CurrentCameraMode == CameraManager.CameraMode.FirstPersonView && Input.GetMouseButtonUp(1)) _cameraManager.CurrentCameraMode = CameraManager.CameraMode.Tracking;
        //GetKeyDownを使用していないのはUIクラス(PlayerCommandToTheTeamUI)の方で長押しに関する処理があるため
        if (_cameraManager.CurrentCameraMode == CameraManager.CameraMode.Tracking && Input.GetKeyUp(KeyCode.M))_cameraManager.CurrentCameraMode = CameraManager.CameraMode.OverlookFromSky;
        else if(_cameraManager.CurrentCameraMode == CameraManager.CameraMode.OverlookFromSky && Input.GetKeyUp(KeyCode.M)) _cameraManager.CurrentCameraMode = CameraManager.CameraMode.Tracking;


        //-------------------ジャンプ-------------------//
        //ひとつ前のフレームでジャンプキーが離されていて、かつ今のフレームで入力されている場合にジャンプ命令をする
        if (_releaseTheJumpKye && Input.GetKey(KeyCode.Space))
        {
            _releaseTheJumpKye = false;
            _normalCharacterAnimationManager.JumpAnimation();
        }
        //ジャンプキーが離されたら、その情報を扱う変数を変化させる
        if (Input.GetKeyUp(KeyCode.Space) || !Input.GetKey(KeyCode.Space)) _releaseTheJumpKye = true;
    }

    private void FixedUpdate()
    {
        if (RejectPlayerInput) return;

        //操作するオブジェクトがない場合は何もしない
        if (OperationNormalCharacterObject == null) return;


        //-------------------プレイヤーオブジェクトの移動-------------------//
        _operation_CharacterRunOrWalkMove.Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if(_cameraManager.CurrentCameraMode == CameraManager.CameraMode.OverlookFromSky)_operation_CharacterRunOrWalkMove.RelatedToCameraOrientation = false;
        if(_cameraManager.CurrentCameraMode != CameraManager.CameraMode.OverlookFromSky)_operation_CharacterRunOrWalkMove.RelatedToCameraOrientation = true;
        //移動制御の処理は攻撃アニメーションイベントの場所

        //-------------------カメラ-------------------//
        _cameraManager.InputToCamera(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
        

        //-------------------UI操作-------------------//
        switch (_cameraManager.CurrentCameraMode)
        {
            case CameraModeSwitching<CameraManager>.CameraMode.Tracking:
                //既に切り替えたいモードになっている場合は切り替え処理をしない
                if (_uIManager.CurrentDisplayMode == UIManager.DisplayModeEnum.NormalThirdPersonPerspective) break;
                //モード切り替え
                _uIManager.DisplayModeSwitching(UIManager.DisplayModeEnum.NormalThirdPersonPerspective);
                break;
            case CameraModeSwitching<CameraManager>.CameraMode.FirstPersonView:
                //既に切り替えたいモードになっている場合は切り替え処理をしない
                if (_uIManager.CurrentDisplayMode == UIManager.DisplayModeEnum.FirstPersonView) break;
                //モード切り替え
                _uIManager.DisplayModeSwitching(UIManager.DisplayModeEnum.FirstPersonView);
                break;
            case CameraModeSwitching<CameraManager>.CameraMode.OverlookFromSky:
                //既に切り替えたいモードになっている場合は切り替え処理をしない
                if (_uIManager.CurrentDisplayMode == UIManager.DisplayModeEnum.OverlookFromSky) break;
                //モード切り替え
                _uIManager.DisplayModeSwitching(UIManager.DisplayModeEnum.OverlookFromSky);
                break;
        }


        //-------------------マウスカーソル-------------------//
        if (_uIManager.CurrentDisplayMode == UIManager.DisplayModeEnum.OverlookFromSky)_mouseCursorManager.MouseCursor(true);
        if (_uIManager.CurrentDisplayMode != UIManager.DisplayModeEnum.OverlookFromSky)_mouseCursorManager.MouseCursor(false);


        //-------------------オブジェクトをターゲッティング-------------------//
       if (_cameraManager.CurrentCameraMode == CameraManager.CameraMode.FirstPersonView && Input.GetMouseButton(2)) _targetingByThePlayer.TargetInput();


    }
}
