using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILFramework
{
    public class View : Base, IView
    {
        public virtual void OnMessage(IMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
