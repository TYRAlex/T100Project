using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ILFramework
{

    public class ChildILBehaviour : RootILBehaviour
    {
        private RootILBehaviour rootILBehaviour;
        private string childNodeName;

        public RootILBehaviour RootILBehaviour { get => rootILBehaviour; set => rootILBehaviour = value; }

        private void Start()
        {
            //Startin();
        }

        //protected override void Startin()
        //{

        //    childNodeName = this.gameObject.name;
        //    RootILBehaviour = transform.root.gameObject.GetComponent<RootILBehaviour>();
        //    StartCoroutine(OnStart());
            
        //}


        IEnumerator OnStart()
        {
            while (RootILBehaviour.RootPackage == null)
                yield return null;

            this.RootPackage = RootILBehaviour.RootPackage;
            this.gamePrefab = gameObject;
            this.InitILBehaviour(this.RootPackage.PackageDomain, childNodeName);
            Inited();
            //base.Startin();
        }

        public virtual void Inited()
        {

        }

    }
}
