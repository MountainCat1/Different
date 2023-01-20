using UnityEngine;

namespace TCASystem
{
    public abstract class ActionObject : TCAObject
    {
        public bool playOnce = false;

        private bool fired = false;

        public void RunAction()
        {
            if (playOnce && fired)
                return;
            fired = true;

            Action();
        }

        public abstract void Action();
    }
}
