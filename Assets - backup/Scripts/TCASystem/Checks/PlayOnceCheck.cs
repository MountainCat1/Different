using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace TCASystem
{
    public class PlayOnceCheck : CheckObject
    {
        public static HashSet<string> flags = new HashSet<string>();

        private string flag;

        private void Awake()
        {
            flag = gameObject.name + SceneManager.GetActiveScene().name;
        }

        public override bool Check()
        {
            if (!flags.Contains(flag))
            {
                flags.Add(flag);
                return true;
            }
            return false;
        }
    }
}
