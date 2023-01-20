using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCASystem
{
    public class SingleFlagCheck : CheckObject
    {
        public string flag = "";
        public bool expectedValue = true;
        public override bool Check()
        {
            return FlagManager.CheckFlag(flag) == expectedValue;
        }
    }
}
