using System;


namespace TCASystem
{
    public class SetFlagAction : ActionObject
    {
        public string flag;
        public bool value = true;
        
        public override void Action()
        {
             FlagManager.SetFlag(flag, value);              
        }
    }
}
