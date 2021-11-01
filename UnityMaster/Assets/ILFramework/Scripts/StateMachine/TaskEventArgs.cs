using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILFramework
{
    public class TaskEventArgs : EventArgs
    {
        public int task_id;//当前开始id
        public bool isFull;//是否结束

    }
}

