using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ILFramework
{
    public interface ICommand 
    {
        void Execute(IMessage message);
    }
}

