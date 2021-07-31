using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 重複した<T>クラスがある場合、後から生成した<T>クラスを削除する
/// </summary>
/// <typeparam name="T">シングルトンデザインにするクラス</typeparam>
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{

	private static T instance;

	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (T)FindObjectOfType(typeof(T));

				if (instance == null)
				{
					Debug.LogError(typeof(T) + "が見つかりません");
				}
			}

			return instance;
		}
	}

	/// <summary>
	/// 同じクラスが既にある場合、現在のクラストGameObjectを削除する
	/// </summary>
	protected void Awake()
	{
		CheckInstance();
	}

	protected bool CheckInstance()
	{
		if (this == Instance) { return true; }

		Destroy(this.gameObject);
		
		return false;
	}

}
