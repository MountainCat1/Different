using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCASystem
{
    public class CheckFlag : CheckObject
    {
        public List<string> flags = new List<string>() { "" };
        public LogicalType logicalType;
        public override bool Check()
        {
            if (logicalType == LogicalType.And)
            {
                foreach (string flag in flags)
                {
                    if (!FlagManager.CheckFlag(flag))
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (logicalType == LogicalType.Or)
            {
                foreach (string flag in flags)
                {
                    if (!FlagManager.CheckFlag(flag))
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (logicalType == LogicalType.Not)
            {
                foreach (string flag in flags)
                {
                    if (FlagManager.CheckFlag(flag))
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (logicalType == LogicalType.Dif)
            {
                for (int i = 0; i < flags.Count - 1; i += 2)
                {
                    if (FlagManager.CheckFlag(flags[i]) == FlagManager.CheckFlag(flags[i + 1]))
                        return false;
                }
                return true;
            }
            else if (logicalType == LogicalType.Same)
            {
                for (int i = 0; i < flags.Count - 1; i += 2)
                {
                    if (FlagManager.CheckFlag(flags[i]) != FlagManager.CheckFlag(flags[i + 1]))
                        return false;
                }
                return true;
            }
            else
                throw new NotImplementedException();
        }
    }
}
