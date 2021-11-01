using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILFramework
{
    public class GetCurrenAndroidVersion
    {
        
        public static int GetVersionInt()
        {
            using (var version=new AndroidJavaClass("android.os.Build$VERSION"))
            {
                return version.GetStatic<int>("SDK_INT");
            }
        }
    }
}

