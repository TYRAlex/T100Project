using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILFramework
{
    public interface IView
    {
        void OnMessage(IMessage message);
    }
}
