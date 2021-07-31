using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif

/// <summary>
///  
///長方形
///attackStartPosition: ▯ で見た時下の四角形の面の中心からz軸にかけて伸びる
///attackArea:xyzのそのままの大きさ
///
///丸形
///         attackStartPosition:丸の中心
///         attackArea:Vector3のx軸のみ参照するx軸は半径の大きさ
/// </summary>
public class SelectAttackTarget : AttackTargetHitConfirmation
{
    enum AttackAreaForm
    {
        Sphere = 0,
        Rectangle
    }


   // [SerializeField] string[] m_attackTags = null;
    [SerializeField] private List<Vector3> m_attackStartPosition = new List<Vector3>();
    [SerializeField] private List<Vector3> m_attackArea = new List<Vector3>();
    [SerializeField] protected List<float> m_damageMagnification = new List<float>();
    [SerializeField] private List<AttackAreaForm> m_attackAreaForm = new List<AttackAreaForm>();
    [SerializeField] protected List<DamageManager.DamageReactionEnum> m_damageReactionnum = new List<DamageManager.DamageReactionEnum>();

    private CharacterTeamsData.TeamData _teamData;

   public void CharacterTeamsDataSet(ref CharacterTeamsData.TeamData teamData)
    {
        _teamData = teamData;
    }

    /// <summary>
    /// 攻撃に当たりBasicStatusDataAccessを所持しているオブジェクトを返す
    /// </summary>
    /// <param name="attackNum"></param>
    /// <returns></returns>
    protected GameObject[] NormalAttackTargets(int attackNum)
    {
        GameObject thisBodyObject = transform.Find("BodyCollider").gameObject;

        switch (m_attackAreaForm[attackNum])
        {
            case AttackAreaForm.Sphere:
               return DeleteThatDoNoHaveBasicStatusData(HitConfirmation_Sphere(thisBodyObject.transform.position+m_attackStartPosition[attackNum], m_attackArea[attackNum].x, _teamData.AttackTargetTags.ToArray()));
            case AttackAreaForm.Rectangle:
                return HitConfirmation_Rectangle(thisBodyObject.transform.position+m_attackStartPosition[attackNum], m_attackArea[attackNum], thisBodyObject.transform, _teamData.AttackTargetTags.ToArray());
        }

        return null;

        //BasicStatusDataAccessコンポーネントを持たないオブジェクトを除外する
        GameObject[] DeleteThatDoNoHaveBasicStatusData(GameObject[] gameObjects)
        {
            List<GameObject> gameObjects_list=new List<GameObject>();

            foreach (var item in gameObjects)
            {
                if (item.GetComponent<BasicStatusDataAccess>()) gameObjects_list.Add(item);
            }

            return gameObjects_list.ToArray();

        }

    }

    // private void SpecialAttack()

   



    /*
#if UNITY_EDITOR
    //どのコンポーネントのエディターとして機能するべきかをUnityに伝える。
    [CustomEditor(typeof(AttackManager))]
    //複数のオブジェクトを選択して編集する場合があることをUnityに伝える
    //[CanEditMultipleObjects]
    public class AttackManagerEditor : Editor
    {

        SerializedProperty attackStartPositionProperty;
        SerializedProperty attackAreaProperty;
        SerializedProperty damageMagnificationProperty;
        SerializedProperty attackAreaFormProperty;
        SerializedProperty damageReactionnumProperty;
        bool attackStatusFolding = false;

        void OnEnable()
        {
            //プロパティの取得
            attackStartPositionProperty = serializedObject.FindProperty("m_attackStartPosition");
            attackAreaProperty = serializedObject.FindProperty("m_attackArea");
            damageMagnificationProperty = serializedObject.FindProperty("m_damageMagnification");
            attackAreaFormProperty = serializedObject.FindProperty("m_attackAreaForm");
            damageReactionnumProperty = serializedObject.FindProperty("m_damageReactionnum");

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //Unityのインスペクターから編集されたプロパティ情報を更新(取得)する
            serializedObject.Update();


            // 要素を追加
            if (GUILayout.Button("追加"))
            {
                //インデックスを一つ増やす
                attackStartPositionProperty.InsertArrayElementAtIndex(attackStartPositionProperty.arraySize);
                attackAreaProperty.InsertArrayElementAtIndex(attackAreaProperty.arraySize);
                damageMagnificationProperty.InsertArrayElementAtIndex(damageMagnificationProperty.arraySize);
                attackAreaFormProperty.InsertArrayElementAtIndex(attackAreaFormProperty.arraySize);
                damageReactionnumProperty.InsertArrayElementAtIndex(damageReactionnumProperty.arraySize);
            }

            // 要素を削除
            if (GUILayout.Button("末尾の要素を削除"))
            {
                //配列のインデックスを末尾から一つ減らす

                if (attackStartPositionProperty.arraySize != 0)
                {
                    attackStartPositionProperty.DeleteArrayElementAtIndex(attackStartPositionProperty.arraySize - 1);
                }

                if (attackAreaProperty.arraySize != 0)
                {
                    attackAreaProperty.DeleteArrayElementAtIndex(attackAreaProperty.arraySize - 1);
                }

                if (damageMagnificationProperty.arraySize != 0)
                {
                    damageMagnificationProperty.DeleteArrayElementAtIndex(damageMagnificationProperty.arraySize - 1);
                }

                if (attackAreaFormProperty.arraySize != 0)
                {
                    attackAreaFormProperty.DeleteArrayElementAtIndex(attackAreaFormProperty.arraySize - 1);
                } 
                
                if (damageReactionnumProperty.arraySize != 0)
                {
                    damageReactionnumProperty.DeleteArrayElementAtIndex(damageReactionnumProperty.arraySize - 1);
                }
            }

            //折りたたみの状態とラベルをUnityに伝える
            attackStatusFolding = EditorGUILayout.Foldout(attackStatusFolding, "攻撃詳細のまとめ表示");
            //折り畳むか
            if (attackStatusFolding)
            {
                //配列サイズ達
                int[] arraySizes =
                {
                    attackStartPositionProperty.arraySize,
                    attackAreaProperty.arraySize,
                    damageMagnificationProperty.arraySize,
                    attackAreaFormProperty.arraySize,
                    damageReactionnumProperty.arraySize
                };



                for (int i = 0; i < Mathf.Min(arraySizes); i++)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("----------攻撃情報[" + i + "]----------");
                    EditorGUILayout.LabelField("攻撃オブジェクトからの攻撃");
                    EditorGUILayout.LabelField("開始位置　　　　　　　" + attackStartPositionProperty.GetArrayElementAtIndex(i).vector3Value);
                    EditorGUILayout.LabelField("範囲　　　　　　　　　" + attackAreaProperty.GetArrayElementAtIndex(i).vector3Value); 
                    EditorGUILayout.LabelField("範囲の形　　　　　　　" + attackAreaFormProperty.enumNames[attackAreaFormProperty.GetArrayElementAtIndex(i).enumValueIndex]);
                    EditorGUILayout.LabelField("倍率　　　　　　　　　" + damageMagnificationProperty.GetArrayElementAtIndex(i).floatValue);
                    EditorGUILayout.LabelField("攻撃相手のリアクション" + damageReactionnumProperty.enumNames[damageReactionnumProperty.GetArrayElementAtIndex(i).enumValueIndex]);
                }
            }

            //プロパティの編集を反映
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
    */
}
