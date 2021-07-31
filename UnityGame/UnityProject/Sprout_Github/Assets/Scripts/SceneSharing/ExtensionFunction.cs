using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System;


/// <summary>
/// C# やUnityの数学関数にないあったら便利な計算や機能をまとめたクラス
/// </summary>
public static class ExtensionFunction
{

    /// <summary>
    /// Vector3のポジションが特定の範囲内に入っているか確認する。
    /// </summary>
    /// <param name="mathf"></param>
    /// <param name="range"></param>
    /// <param name="searchPosition"></param>
    /// <param name="targetPosition"></param>
    /// <returns>入っている場合true。入っていない場合はfalse。</returns>
    public static bool CheckIfItIsWithinTheRange(this Vector3 targetPosition, Vector3 searchPosition,Vector3 range)
    {

        //==の値が異なる(ターゲットがポジション外)とtrueになる。
        if (!(targetPosition.x.ToString("F2") == (Mathf.Clamp(targetPosition.x, (searchPosition.x - (range.x/2)), (searchPosition.x + (range.x/2)))).ToString("F2")))return false;
        if (!(targetPosition.y.ToString("F2") == (Mathf.Clamp(targetPosition.y, (searchPosition.y - (range.y/2)), (searchPosition.y + (range.y/2)))).ToString("F2")))return false;
        if (!(targetPosition.z.ToString("F2") == (Mathf.Clamp(targetPosition.z, (searchPosition.z - (range.z/2)), (searchPosition.z + (range.z/2)))).ToString("F2")))return false;

        //上のifを抜けたということは全ての座標で範囲内ということなのでtrueを出す。
        return true;

    }

    /// <summary>
    /// タグ検索して見つけたオブジェクトを戻り値として返す。
    /// </summary>
    /// <param name="target"></param>
    /// <param name="tagName"></param>
    /// <param name="targetGameObjectName"></param>
    /// <returns></returns>
    public static GameObject SearchByTagName( this GameObject targetGO,string tagName,string gameObjectName)
    {
        GameObject[] tagGos = GameObject.FindGameObjectsWithTag(tagName);

        foreach (var item in tagGos)
        {
            if(item.name == gameObjectName)
            {
                return item;
            }
        }

        return null;
    }

    /// <summary>
    /// 最上階の親オブジェクトから(num)下層下のTransformを取得する。
    ///  <para></para>
    /// ※特定の階層に複数のTransformがある場合は現在のTransformに繋がるTransformを参照する。
    /// </summary>
    /// <param name="bottomLayerTransform">最下層のTransform</param>
    /// <param name="num_LowerHierarchyGet">最上階から何下層したのTransformを取得するか。0で最上階、最下層以下は現在の階層</param>
    /// <returns></returns>
    public static Transform GetTransformOfSpecifiedHierarchy(this Transform bottomLayerTransform, int num_LowerHierarchyGet )
    {
        if (bottomLayerTransform == null) return null;

        List<Transform> transformInEachHierarchy = new List<Transform>();

        transformInEachHierarchy.Add(bottomLayerTransform);

        //リストに親Transformを最下層(thisTransform)のTransformから最上階まで追加していく
        while (true)
        {
            if(transformInEachHierarchy[transformInEachHierarchy.Count-1].parent==null)
            {
                break;
            }
            else
            {
                transformInEachHierarchy.Add(transformInEachHierarchy[transformInEachHierarchy.Count - 1].parent);
            }
        }

        //リストの要素数(0を含めるため-1している)より取得したい階層番号が大きい場合は最下層より下を取得しようとしている状態なので最下層のTransformを返し、警告する。
        if (num_LowerHierarchyGet > (transformInEachHierarchy.Count-1))
        {
            Debug.LogWarning("最下層より下の階層のTransformを指定しています。");

            return bottomLayerTransform;
        }

        //0より下の数の場合、最上階より上を指定している状態なので最上階のTransformを返し警告する。
        if(num_LowerHierarchyGet<0)
        {
            Debug.LogWarning("0 から 階層数で指定することをお勧めします。");

            return transformInEachHierarchy[transformInEachHierarchy.Count - 1];
        }

        //この関数を使う人は、配列の0が最上階で配列の末尾が最下層と認識して使用するのでこのプログラムを読んでいる人には紛らわしいが減法を利用して反転する。
        return transformInEachHierarchy[(transformInEachHierarchy.Count-1)- num_LowerHierarchyGet];

    }

    /// <summary>
    /// 配列内をアクセスする番号か確認する(三次元配列)。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>配列内をアクセスしている場合はtrue</returns>
    public static bool CheckIfIsOutIndexNumber(this GameObject[,,] extension,int x,int y,int z)
    {
            //配列外のアクセス番号の場合、falseを返す。
            if (Mathf.Clamp(x, 0, (extension.GetLength(0) - 1)) != x) return false;
            if (Mathf.Clamp(y, 0, (extension.GetLength(1) - 1)) != y) return false;
            if (Mathf.Clamp(z, 0, (extension.GetLength(2) - 1)) != z) return false;

            //全てのアクセス番号が配列内の場合trueを返す。
            return true;
    }

    /// <summary>
    /// 配列内をアクセスする番号か確認する(三次元配列)。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>配列内をアクセスしている場合はtrue</returns>
    public static bool CheckIfIsOutIndexNumber(this BasicStatusData[,,] extension, int x, int y, int z)
    {
        //配列外のアクセス番号の場合、falseを返す。
        if (Mathf.Clamp(x, 0, (extension.GetLength(0) - 1)) != x) return false;
        if (Mathf.Clamp(y, 0, (extension.GetLength(1) - 1)) != y) return false;
        if (Mathf.Clamp(z, 0, (extension.GetLength(2) - 1)) != z) return false;

        //全てのアクセス番号が配列内の場合trueを返す。
        return true;
    }


    /// <summary>
    /// 全ての子オブジェクトを配列で返す。※親から一段下の子オブジェクトのみではなく全て
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject[] returnAllChild(this Transform topParent)
    {
        //全ての子オブジェクトを詰めるリスト
        List<GameObject> childGameObjects = new List<GameObject>();

        addChild(topParent);

        return childGameObjects.ToArray();

        void addChild(Transform parent)
        {
            //引数に渡された親オブジェクトの一段下の子オブジェクトをchildGameObjectsに追加していく
            for (int i = 0; i < parent.childCount; i++)
            {
                //子オブジェクトの一段下に子オブジェクトが存在する(子オブジェクトが親オブジェクトとしてある)場合は更に掘り下げて追加していく
                if(parent.GetChild(i).childCount!=0)
                {
                    addChild(parent.GetChild(i));
                }

                childGameObjects.Add(parent.GetChild(i).gameObject);
            }
        }
    }

    /// <summary>
    /// 全ての子オブジェクトに引数で渡された処理をする
    /// </summary>
    /// <param name="parentGo"></param>
    /// <param name="process">全ての子オブジェクトにする処理コード</param>
    /// <param name="parentProcess">子オブジェクトにデリゲート処理をする際に親オブジェクトも含めるか</param>
    public static void AllChildObjectsDelegateProcess(this Transform parent, Action<GameObject> process, bool parentProcess=false)
    {
        //親オブジェクトも処理するか
        if (parentProcess) process(parent.gameObject);

        //全ての子オブジェクトに渡された処理をする
        foreach (var child in parent.returnAllChild())
        {
            process(child);
        }

    }

    /// <summary>
    /// Resourcesファイルからプレハブオブジェクトを探し、見つけたらオブジェクトの名前とGameObjectを返す。
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="faldaPath">Resources/+ファイルパス</param>
    /// <returns></returns>
    public static (string prefabName, GameObject prefabObject)[] FindPrefabObjectsFromFalda(this GameObject gameObject, string[] prefabPaths)
    {

        List<(string prefabName, GameObject prefabObject)> returnPrefabNameAndFaldaPath = new List<(string, GameObject)>();

        foreach (var prefabPath in prefabPaths)
        {
          //  GameObject prefabObject = (GameObject)Resources.Load(prefabPath);

          //  returnPrefabNameAndFaldaPath.Add((prefabObject.name, prefabObject));

        }

        return returnPrefabNameAndFaldaPath.ToArray();
    }

    /// <summary>
    /// Resourcesファイルからプレハブオブジェクトを探し、見つけたらオブジェクトの名前とファイルパスを返す。※faldaPath:Resources/+ファイルパス
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="faldaPath">Resources/+ファイルパス</param>
    /// <returns></returns>
    public static (string prefabName, string faldaPath)[] FindPrefabObjectsFromFalda(this GameObject gameObject,string faldaPath)
    {

        List<(string prefabName, string faldaPath)> returnPrefabNameAndFaldaPath=new List<(string, string)>();


        //探したいファルダのパス
        DirectoryInfo prefabFaldaPath = new DirectoryInfo(Application.dataPath + "/Resources/"+ faldaPath);

        //必要なファイルとだけマッチする正規表現
        Regex regex = new Regex(@"(.+)\.prefab$");

        //ファルダにあるファイルを全て配列で取得して正規表現で選別していく
        FileInfo[] prefabFaldaInFiles = prefabFaldaPath.GetFiles();
        foreach (var item in prefabFaldaInFiles)
        {

            Match regexMatch = regex.Match(item.Name);

            //マッチしない場合は次のファイルに行く
            if (!regexMatch.Success) continue;

            //マッチした物はGameObjectのプレハブなのでリストに追加していく
            returnPrefabNameAndFaldaPath.Add((regexMatch.Groups[1].ToString(), (faldaPath +"/"+ regexMatch.Groups[1])));
        }

        return returnPrefabNameAndFaldaPath.ToArray();
    }

    /*
    public static Vector3 ReturnsVectorFromAngle360(this Vector3 vector3, float angle)
    {
        float radian_xz = angle * Mathf.Deg2Rad;

        Vector3 velocity = new Vector3(Mathf.Cos(radian_xz), 0, Mathf.Sin(radian_xz));

        return velocity;
    }
    */
}
