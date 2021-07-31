using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataReset : MonoBehaviour
{
    

    public void DataReset()
    {

        SaveDataManager saveDataManager = gameObject.SearchByTagName("SingletonManager", "SaveDataManager").GetComponent<SaveDataManager>();

        SaveDataManager.SaveData[0] = new SaveDataManager.SaveDataClass();

        saveDataManager.Save();
    }

}
