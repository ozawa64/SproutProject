using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryBuff : GivenBuffAndDeBuffManagerBase
{
 
    /// <summary>
    /// ThisGameObjectを回復させる
    /// </summary>
    protected void Recovery_Buff(int effectPower)
    {
        //体力の回復
        basicStatusDataAccess.PhysicalFitness += effectPower;

        //体力が最大値よりも増えている場合は最大値にする
        if (basicStatusDataAccess.PhysicalFitness > basicStatusDataAccess.MaxPhysicalFitness) basicStatusDataAccess.PhysicalFitness = basicStatusDataAccess.MaxPhysicalFitness;
    }


}
