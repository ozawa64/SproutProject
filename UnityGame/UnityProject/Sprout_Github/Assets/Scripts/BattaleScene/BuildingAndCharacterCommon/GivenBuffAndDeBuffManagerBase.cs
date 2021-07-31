using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivenBuffAndDeBuffManagerBase : MonoBehaviour
{

    protected BasicStatusDataAccess basicStatusDataAccess;

    protected void Start()
    {
        basicStatusDataAccess = GetComponent<BasicStatusDataAccess>();
    }




}
