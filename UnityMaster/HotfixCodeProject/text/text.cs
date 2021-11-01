using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class text
    {
        GameObject curGo;
        SkeletonAnimation _skeletonGraphic;
        string slot;
        Sprite texture;
        Sprite texture1;
        Sprite texture2;
        GameObject btn;
        RawImage img;
        Shader curshader;
        void Start(object o)
        {
            curGo = (GameObject)o;
            _skeletonGraphic = curGo.transform.Find("boy").GetComponent<SkeletonAnimation>();
            slot = "boy1";
            GameObject go = curGo.transform.Find("tex").gameObject;
            img = curGo.transform.Find("RawImage").GetComponent<RawImage>();
            texture1 = go.transform.Find("tex1").GetComponent<Image>().sprite;
            texture2 = go.transform.Find("tex2").GetComponent<Image>().sprite;
            btn = curGo.transform.Find("btn").gameObject;
            Util.AddBtnClick(btn, SetSkin);
             curshader = _skeletonGraphic.skeletonDataAsset.atlasAssets[0].materials[0].shader;
            // img.gameObject.SetActive(false);
            texture = texture1;
            //curGo.GetComponent<MonoBehaviour>().StartCoroutine(wait());
            pushMaterial();
            //SpineManager.instance.CreateRegionAttachmentByTexture(_skeletonGraphic.skeletonDataAsset.atlasAssets[0],slot,curGo,img.gameObject, curshader);
        }
        IEnumerator wait()
        {
            yield return new WaitForSeconds(Time.deltaTime);
            pushMaterial();
        }
        void SetSkin(GameObject btn)
        {
            pushMaterial();
            //curshader = _skeletonGraphic.skeletonDataAsset.atlasAssets[0].materials[0].shader;
            //SpineManager.instance.CreateRegionAttachmentByTexture(_skeletonGraphic, slot, texture, curshader);
            //texture = texture == texture1 ? texture2 : texture1;
        }
        void pushMaterial()
        {
            _skeletonGraphic.GetComponent<MeshRenderer>().material.shader = Shader.Find("Spine/Skeleton");
            _skeletonGraphic.GetComponent<MeshRenderer>().material.shader.name = curshader.name;
        }
    }
}
