using System.Collections.Generic;
using Bite.Runtime.Functions;
using Bite.Runtime.Memory;
using UnityEngine;

namespace BiteUnity
{

public class UnityDeltaTimeVmCallable: IBiteVmCallable
{
    public object Call( List < DynamicBiteVariable > arguments )
    {
        return Time.deltaTime;
    }
}

}
