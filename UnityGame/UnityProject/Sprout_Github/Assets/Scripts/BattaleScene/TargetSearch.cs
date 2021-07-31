using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSearch : SingletonMonoBehaviour<TargetSearch>
{
    /// <summary>
    /// 指定した場所を中心にサーチして指定したタグのGameObjectを返す。
    /// </summary>
    /// <param name="searchPosition"></param>
    /// <param name="searchArea_Radius"></param>
    /// <param name="searchTags"></param>
    /// <returns>返すGameObjectはBodyColliderオブジェクトの親</returns>
    public GameObject[] SphereSearch(Vector3 searchPosition, float searchArea_Radius, string[] searchTags)
    {
        List<GameObject> returnObjects = new List<GameObject>();

        //デバッグ用
        SphereSearchDebug();

        //サーチで取得したいオブジェクトはキャラクターや建物オブジェクトのデータアクセススクリプトの付いているGameObjectなのでifで選別していく
        foreach (var collider in Physics.OverlapSphere(searchPosition, searchArea_Radius))
        {
            if (collider.gameObject.name == "BodyCollider")
            {
                if (collider.transform.parent != null)
                {
                    foreach (var targetTag in searchTags)//サーチ対象のタグオブジェクトか
                    {
                        if (collider.transform.parent.gameObject.CompareTag(targetTag))
                        {
                            returnObjects.Add(collider.transform.parent.gameObject);
                            break;
                        }
                    }
                }
            }
        }

        return returnObjects.ToArray();

        //デバッグ用
        void SphereSearchDebug()
        {
            Debug.DrawRay(searchPosition, Vector3.forward * searchArea_Radius, Color.red, 1, false);
            Debug.DrawRay(searchPosition, Vector3.right * searchArea_Radius, Color.red, 1, false);
            Debug.DrawRay(searchPosition, Vector3.back * searchArea_Radius, Color.red, 1, false);
            Debug.DrawRay(searchPosition, Vector3.left * searchArea_Radius, Color.red, 1, false);
            Debug.DrawRay(searchPosition, Vector3.up * searchArea_Radius, Color.red, 1, false);
            Debug.DrawRay(searchPosition, Vector3.down * searchArea_Radius, Color.red, 1, false);
        }
    }

    /// <summary>
    /// 指定した場所を始まりにサーチして指定したタグのGameObjectを返す。
    /// サーチは長方形(▯)の四角の面の中心(▣)から始まる。
    /// </summary>
    /// <param name="searchPosition"></param>
    /// <param name="searchArea_Radius"></param>
    /// <param name="searchTags"></param>
    public GameObject[] RectangleSearch(Vector3 searchPosition, Vector3 searchArea_Radius,Transform searchAreaRotation, string[] searchTags)
    {

        //引数は「サーチはオブジェクトの正面から始まり長方形(▯)の四角の面の中心(▣)から長方形に伸びる」事を想定して渡しているのでそうなるように調整する
        searchPosition += searchAreaRotation.forward*searchArea_Radius.z;

        //デバッグ用
        RectangleSearchDebug();

        List<GameObject> returnObjects = new List<GameObject>();

        

        //サーチで取得したいオブジェクトはキャラクターや建物オブジェクトのデータアクセススクリプトの付いているGameObjectなのでifで選別していく
        foreach (var collider in Physics.OverlapBox(searchPosition, searchArea_Radius, searchAreaRotation.rotation))
        {
            if (collider.gameObject.name == "BodyCollider")
            {
                if (collider.transform.parent != null)
                {
                    foreach (var targetTag in searchTags)//サーチ対象のタグオブジェクトか
                    {
                        if (collider.transform.parent.gameObject.CompareTag(targetTag))
                        {
                            returnObjects.Add(collider.transform.parent.gameObject);
                            break;
                        }
                    }
                }
            }
        }

        return returnObjects.ToArray();

        //デバッグ用
        void RectangleSearchDebug()
        {
            Debug.DrawRay(searchPosition, searchAreaRotation.forward * searchArea_Radius.z, Color.red, 1, false);
            Debug.DrawRay(searchPosition, -searchAreaRotation.forward * searchArea_Radius.z, Color.red, 1, false);
            Debug.DrawRay(searchPosition, searchAreaRotation.up * searchArea_Radius.y, Color.red, 1, false);
            Debug.DrawRay(searchPosition, -searchAreaRotation.up * searchArea_Radius.y, Color.red, 1, false);
            Debug.DrawRay(searchPosition, searchAreaRotation.right * searchArea_Radius.x, Color.red, 1, false);
            Debug.DrawRay(searchPosition, -searchAreaRotation.right * searchArea_Radius.x, Color.red, 1, false);

            
        }
    }




}
