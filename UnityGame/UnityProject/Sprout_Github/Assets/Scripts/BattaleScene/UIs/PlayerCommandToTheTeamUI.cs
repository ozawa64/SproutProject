using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerCommandToTheTeamUI : CanvasChildUIBase
{


    private class InputCommands
    {
        public (int, int, int)? SelectGrid=null;
        public BuildingGenerateManager.Buildings Building;
        public int SelectNumberOfPeople=8;
    }

    enum DisplayUISceneEnum:int
    {
        GridSelection=0,
        BuildingOrWaitingSelection,
        BuildingSelect,
        NumberOfBuildersSelect,
        NumberOfStandbySelect,
        None
    }

    /// <summary>今までの入力情報を削除する削除入力があった時にどれくらいの長押しをしたら実行されるかの時間</summary>
    [SerializeField] private float m_inputCommandDleteCountTime=2f;
    
    /// <summary>マウスカーソルでグリッドを選択する際に衝突出来るレイヤー達</summary>
    [SerializeField] private LayerMask m_rayCollisionLayerMask;
    /// <summary>配列の順番はDisplayUISceneEnumと同じにする</summary>
    private GameObject[] _uIScenes;
    /// <summary>時間の計測用</summary>
    private float m_inputCommandDleteCountTimeCount = 0;
    /// <summary>現在表示している(する)UIシーン</summary>
    private DisplayUISceneEnum _currentDisplayScene = DisplayUISceneEnum.GridSelection;
    private CharacterTeamManager _characterTeamManager;
    private GridCalculation _gridCalculation;
    private CharacterTeamsData.TeamData _teamData;
    private GameObject _gridSelectEffectCube;
    private GridData _gridData;
    private InputCommands _inputCommands = new InputCommands();
    private Text[] _teamNumberOfPeopleText=new Text[2];
    private Text[] _selectNumberOfPeopleText=new Text[2];

    enum ButtonNotificationsEnum
    {
        StandbyButton,
        ArchitectureButton,
        ReturnBackButton,
        Vegetation_RecoveryButton,
        BigRockButton,
        UpButton,
        DownButton,
        ExecutionButton
    }

    public override void GetButtonNotifications(string notifications)
    {
       
        switch (_currentDisplayScene)
        {
            case DisplayUISceneEnum.GridSelection:
                break;
            case DisplayUISceneEnum.BuildingOrWaitingSelection:

                //建築か待機かを選択する画面のUI達を非表示にする
                _uIScenes[(int)_currentDisplayScene].SetActive(false);

                //待機ボタンが押されたら、待機人数のUI画面に移る
                if (ButtonNotificationsEnum.StandbyButton.ToString()== notifications)_currentDisplayScene = DisplayUISceneEnum.NumberOfStandbySelect;

                //建築ボタンが押されたら、 建物選択のUI画面に移る
                if (ButtonNotificationsEnum.ArchitectureButton.ToString() == notifications)_currentDisplayScene = DisplayUISceneEnum.BuildingSelect;

                //戻るボタンが押されたら、グリッド選択画面に移る
                if (ButtonNotificationsEnum.ReturnBackButton.ToString() == notifications)_currentDisplayScene = DisplayUISceneEnum.GridSelection;

                break;
            case DisplayUISceneEnum.BuildingSelect:

                //建物選択画面のUI達を非表示にする
                _uIScenes[(int)_currentDisplayScene].SetActive(false);

                //戻るボタンが押されたら、待機と建築の選択画面に移る
                if (ButtonNotificationsEnum.ReturnBackButton.ToString() == notifications) _currentDisplayScene = DisplayUISceneEnum.BuildingOrWaitingSelection;

                //回復植物ボタンが押されたら
                if (ButtonNotificationsEnum.Vegetation_RecoveryButton.ToString() == notifications)
                {
                    //建築人数のUI画面に移る
                    _currentDisplayScene = DisplayUISceneEnum.NumberOfBuildersSelect;
                    //プレイヤーが選択した建物情報を保存(代入)
                    _inputCommands.Building = BuildingGenerateManager.Buildings.Vegetation_Recovery;
                }

                //大岩ボタンが押されたら、建築人数のUI画面に移る
                if (ButtonNotificationsEnum.BigRockButton.ToString() == notifications)
                {
                    //建築人数のUI画面に移る
                    _currentDisplayScene = DisplayUISceneEnum.NumberOfBuildersSelect;
                    //プレイヤーが選択した建物情報を保存(代入)
                    _inputCommands.Building = BuildingGenerateManager.Buildings.BigRock;
                }


                break;
            case DisplayUISceneEnum.NumberOfBuildersSelect:
               
                //戻るボタンが押されたら...
                if (ButtonNotificationsEnum.ReturnBackButton.ToString() == notifications)
                {
                    //建築人数選択画面のUI達を非表示にする
                    _uIScenes[(int)_currentDisplayScene].SetActive(false);

                    //建物選択画面に移る
                    _currentDisplayScene = DisplayUISceneEnum.BuildingSelect;
                }

                //人数の上下ボタンが押されたら一人ずつ変動させる
                if (ButtonNotificationsEnum.UpButton.ToString() == notifications) _inputCommands.SelectNumberOfPeople++;
                if (ButtonNotificationsEnum.DownButton.ToString() == notifications) _inputCommands.SelectNumberOfPeople--;

                //決定ボタンが押されたら
                if (ButtonNotificationsEnum.ExecutionButton.ToString() == notifications)
                {
                    //待機人数選択画面のUI達を非表示にする
                    _uIScenes[(int)_currentDisplayScene].SetActive(false);

                    //チームメンバーに指定した場所に待機する命令を出す。出来なければfalseを出す
                    if (_characterTeamManager.ArchitectureCommand(ref _teamData, ((int, int, int))_inputCommands.SelectGrid, _inputCommands.Building, _inputCommands.SelectNumberOfPeople))
                    {
                        //命令が完了したので最初の画面に戻る
                        _currentDisplayScene = DisplayUISceneEnum.GridSelection;

                        //入力情報をリセット
                        _inputCommands = new InputCommands();
                    }
                    else
                    {//チームメンバーに命令が出せなかった場合の処理

                        //待機人数選択画面のUI達を表示にする(非表示をキャンセル)
                        _uIScenes[(int)_currentDisplayScene].SetActive(true);
                    }
                }

                    break;

            case DisplayUISceneEnum.NumberOfStandbySelect:

                //戻るボタンが押されたら...
                if (ButtonNotificationsEnum.ReturnBackButton.ToString() == notifications)
                {
                    //待機人数選択画面のUI達を非表示にする
                    _uIScenes[(int)_currentDisplayScene].SetActive(false);

                    //待機と建築の選択画面に移る
                    _currentDisplayScene = DisplayUISceneEnum.BuildingOrWaitingSelection;
                }

                //人数の上下ボタンが押されたら一人ずつ変動させる
                if (ButtonNotificationsEnum.UpButton.ToString() == notifications) _inputCommands.SelectNumberOfPeople++;
                if (ButtonNotificationsEnum.DownButton.ToString() == notifications) _inputCommands.SelectNumberOfPeople--;

                //決定ボタンが押されたら
                if (ButtonNotificationsEnum.ExecutionButton.ToString() == notifications)
                {
                    //待機人数選択画面のUI達を非表示にする
                    _uIScenes[(int)_currentDisplayScene].SetActive(false);

                    //チームメンバーに指定した場所に待機する命令を出す。出来なければfalseを出す
                   if(_characterTeamManager.StandbyCommand(ref _teamData, ((int, int, int))_inputCommands.SelectGrid, _inputCommands.SelectNumberOfPeople))
                    {

                        //命令が完了したので最初の画面に戻る
                        _currentDisplayScene = DisplayUISceneEnum.GridSelection;

                        //入力情報をリセット
                        _inputCommands = new InputCommands();
                    }
                   else
                    {//チームメンバーに命令が出せなかった場合の処理

                        //待機人数選択画面のUI達を表示する(非表示をキャンセル)
                        _uIScenes[(int)_currentDisplayScene].SetActive(true);

                    }

                  
                }

                break;
        }

        SelectNumberOfPeopleUIUpdate();

       
    }

    private void Start()
    {
        _characterTeamManager = gameObject.SearchByTagName("SingletonManager", "CharactersManager").GetComponent<CharacterTeamManager>();
        _gridCalculation = gameObject.SearchByTagName("SingletonManager", "GridManager").GetComponent<GridCalculation>();
        _gridData = gameObject.SearchByTagName("SingletonManager", "GridManager").GetComponent<GridData>();

        //Playerが率いるTeamDataを取得する
        CharacterTeamsData _characterTeamsData = gameObject.SearchByTagName("SingletonManager", "CharactersManager").GetComponent<CharacterTeamsData>();
        int playerTeamsDataIndex = (int)_characterTeamsData.TeamDataIndexInSearchByTeamObject(gameObject.SearchByTagName("Player", "CharacterTeam_Player"));
        _teamData = _characterTeamsData.TeamDatas.ElementAt(playerTeamsDataIndex);
    }

    private void FixedUpdate()
    {
        //このUIが機能するべきではないUI場面の場合の処理
        if (!ThisUIDisplayModeCheck(transform.parent.GetComponent<UIManager>().CurrentDisplayMode))
        {
            //入力時間のカウントはUIが機能していない時はリセット
            m_inputCommandDleteCountTimeCount = 0;

            //UI場面がNoneの場合は初期化。※Noneの状態を続けていると次に機能させるときに何も出来ないので。
            if (_currentDisplayScene == DisplayUISceneEnum.None) _currentDisplayScene = DisplayUISceneEnum.GridSelection;

            return;
        }

       InputCommandDleteCheckAndRun(Input.GetKey(KeyCode.M));//本来はPlayerInputManagerで処理したいが制作時間がないのでここで処理

        //以下の処理はUIの場面に何かある時に処理するものなので現在の場面が何もない状態の場合は処理しない
        if (_currentDisplayScene == DisplayUISceneEnum.None) return;

        if (UIObjectContainsNull())
        {
            FindUIObjects();
        }
        else
        {
            UIUpdate();
        }
    }

    /// <summary>
    /// 一定時間削除入力があったら今までの入力情報を削除する
    /// </summary>
    public void InputCommandDleteCheckAndRun(bool dleteInput)
    {
        //このUIが機能していない時はこの関数も機能させない
        if (!ThisUIDisplayModeCheck(transform.parent.GetComponent<UIManager>().CurrentDisplayMode)) return;

        //dleteInputがtrue(キーが押されている)時間の計測。離されたら計測変数を0にリセットする
        m_inputCommandDleteCountTimeCount = (dleteInput) ? m_inputCommandDleteCountTimeCount + Time.deltaTime : 0;
         
        //時間になったら今までの入力情報を削除してこのUIのシーンを全て非表示にする
        if (m_inputCommandDleteCountTimeCount >= m_inputCommandDleteCountTime)
        {
            //時間のリセット
            m_inputCommandDleteCountTimeCount = 0;

            //今までの入力情報の初期化
            _inputCommands = new InputCommands();

            //現在表示しているUIを非表示にする
            foreach (var scenes in _uIScenes)
            {
                scenes.SetActive(false);
            }

            //UIの場面を何もない場面に
            _currentDisplayScene = DisplayUISceneEnum.None;
}
    }

    protected override void FindUIObjects()
    {
        GameObject[] findObjects = FindsTheObjectWithTheSpecifiedName();

        //エフェクトオブジェクトの取得
        _gridSelectEffectCube = findObjects[0];

        //シーンオブジェクトの取得
        _uIScenes = new GameObject[5];
        for (int i = 1; i < 1+_uIScenes.Length ; i++)
        {
            _uIScenes[i-1] = findObjects[i];
        }

        //現在のチーム人数を表示するText
        _teamNumberOfPeopleText[0] = findObjects[6].GetComponent<Text>();
        _teamNumberOfPeopleText[1] = findObjects[7].GetComponent<Text>();
        //現在選択されている人数を表示するText
        _selectNumberOfPeopleText[0] = findObjects[8].GetComponent<Text>();
        _selectNumberOfPeopleText[1] = findObjects[9].GetComponent<Text>();

    }


    protected override bool UIObjectContainsNull()
    {
        if (_gridSelectEffectCube==null) return true;

        return false;
    }

    /// <summary>
    /// 選択している人数の調整と表示の更新
    /// </summary>
    private void SelectNumberOfPeopleUIUpdate()
    {
        //選択されている人数が...
        //0以下の場合は一人にする
        if (_inputCommands.SelectNumberOfPeople < 1) _inputCommands.SelectNumberOfPeople = 1;
        //率いる仲間の人数より多い場合は、仲間の上限人数にする
        if (_inputCommands.SelectNumberOfPeople > _teamData.FollowCharacters_BasicStatusDatas.Count) _inputCommands.SelectNumberOfPeople = _teamData.FollowCharacters_BasicStatusDatas.Count;

        //選択されている人数を表示するTextの更新
        foreach (var text in _selectNumberOfPeopleText)
        {
            text.text = _inputCommands.SelectNumberOfPeople + "人";
        }

    }

    private void UIUpdate()
    {
        //チーム人数Textの更新
        foreach (var text in _teamNumberOfPeopleText)
        {
            text.text = "現在のチーム人数 " + _teamData.FollowCharacters_BasicStatusDatas.Count +" 人";
        }

        //グリッドの場所が既に決められている際に他のUI画面(このクラス内)でも常に選択されているグリッドがどこか分かるようにエフェクトオブジェクトを表示する※グリッド選択画面の処理でグリッドは表示されるが、中断から再開した場合はグリッド選択画面をスキップすることがありその対策。
        if (_inputCommands.SelectGrid!=null&& _currentDisplayScene!= DisplayUISceneEnum.GridSelection)
        {
            //エフェクトオブジェクトの大きさをグリッド一マスの大きさにする
            _gridSelectEffectCube.transform.localScale = transform.InverseTransformVector(_gridData.OneSquareSize);

            _gridSelectEffectCube.SetActive(true);

            //エフェクトオブジェクトの場所を現在選択されているグリッドの中心にする
            Vector3 gSECPosition = _gridCalculation.Vector3PositionGet(((int, int, int))_inputCommands.SelectGrid);
            gSECPosition.y += _gridData.OneSquareSize.y / 2;
            _gridSelectEffectCube.transform.position = gSECPosition;
        }

        SelectNumberOfPeopleUIUpdate();

        switch (_currentDisplayScene)
        {
            case DisplayUISceneEnum.GridSelection:

                //グリッド選択画面のUI達を表示する
                _uIScenes[(int)_currentDisplayScene].SetActive(true);

                //現在のマウスカーソルと重なっているグリッド番号
                (int, int, int)? mouseCursorGrid = GridPositionOfMouseCursor();

                //グリッド選別エフェクトオブジェクトはグリッドを選択していない時は非表示にする
                _gridSelectEffectCube.SetActive((mouseCursorGrid == null) ? false : true);

                //マウスカーソルがグリッドと重なっていない(指していない)場合はnullになり、nullの場合は以下の処理はしない
                if (mouseCursorGrid == null) break;

                //エフェクトオブジェクトの場所を現在マウスカーソルが指しているグリッドの中心にする
                Vector3 gSECPosition = _gridCalculation.Vector3PositionGet(((int , int , int ))mouseCursorGrid);
                gSECPosition.y += _gridData.OneSquareSize.y / 2;
                _gridSelectEffectCube.transform.position = gSECPosition;

                //エフェクトオブジェクトの大きさをグリッド一マスの大きさにする
                _gridSelectEffectCube.transform.localScale = transform.InverseTransformVector(_gridData.OneSquareSize);

                if (Input.GetMouseButton(0))
                {
                    _inputCommands.SelectGrid = ((int, int, int))mouseCursorGrid;

                    //次の画面に移るので現在のUI達を非表示にする
                    _uIScenes[(int)_currentDisplayScene].SetActive(false);

                    //グリッドを選んだら次のUI画面に移る
                    _currentDisplayScene = DisplayUISceneEnum.BuildingOrWaitingSelection;
                }

                break;
            case DisplayUISceneEnum.BuildingOrWaitingSelection:

                //建築か待機かを選択する画面のUI達を表示する。※次の画面に移る際の非表示はボタン通知処理関数(GetButtonNotifications)で行う
                _uIScenes[(int)_currentDisplayScene].SetActive(true);

                break;
            case DisplayUISceneEnum.BuildingSelect:

                //建築物を選択する画面のUI達を表示する。※次の画面に移る際のUIの非表示はボタン通知処理関数(GetButtonNotifications)で行う
                _uIScenes[(int)_currentDisplayScene].SetActive(true);

                break;
            case DisplayUISceneEnum.NumberOfBuildersSelect:

                //建築人数を選択する画面のUI達を表示する。※次の画面に移る際の非表示はボタン通知処理関数(GetButtonNotifications)で行う
                _uIScenes[(int)_currentDisplayScene].SetActive(true);

                break;
            case DisplayUISceneEnum.NumberOfStandbySelect:

                //待機人数を選択する画面のUI達を表示する。※次の画面に移る際の非表示はボタン通知処理関数(GetButtonNotifications)で行う
                _uIScenes[(int)_currentDisplayScene].SetActive(true);

                break;
        }
    }

    /// <summary>
    /// 現在のマウスカーソルがどのグリッドにあるか
    /// </summary>
    /// <returns></returns>
    private (int x, int y, int z)? GridPositionOfMouseCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//※mousePositionは画面外でもPositionを返すので後で調整

        RaycastHit hit;

        //グリッド番号で返す、レイがヒットしなければnull
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_rayCollisionLayerMask))
        {
            return _gridCalculation.GridNumberGet(hit.point);
        }
        else
        {
            return null;
        }
    }
}