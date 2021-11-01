using System;
using System.Collections;
using System.Collections.Generic;
using ILFramework;
using UnityEngine;
using UnityEngine.UI;

namespace OneID
{
    public class PresentationManager : MonoBehaviour
    {
        private Transform _subscribePanel;

        private Dictionary<string, GameObject> _subscribeDic;
        private Dictionary<string, Text> _likeNumberDic;
        private void Awake()
        {
            _subscribeDic=new Dictionary<string, GameObject>();
            _likeNumberDic=new Dictionary<string, Text>();
            _subscribePanel = this.transform.GetTransform("SubscribePanel");
            Transform _likeNumberPanel = this.transform.GetTransform("LikeNumber");
            for (int i = 0; i < _subscribePanel.childCount; i++)
            {
                Transform target = _subscribePanel.GetChild(i);
                _subscribeDic.Add(target.name, target.gameObject);
                PointerClickListener.Get(target.GetGameObject("Click")).clickDown = ClickAndSubsribe;
            }

            for (int i = 0; i < _likeNumberPanel.childCount; i++)
            {
                Transform target = _likeNumberPanel.GetChild(i);
                Text targetText = target.GetComponent<Text>();
                targetText.text = "0";
                _likeNumberDic.Add(target.name, targetText);
                
            }
        }

        private void ClickAndSubsribe(GameObject go)
        {
            OneIDSceneManager.Instance.PlayCommonSound(14);
            GameObject target = go.transform.parent.gameObject;
            SpineManager.instance.DoAnimation(target, "z2", false,
                () => SpineManager.instance.DoAnimation(target, "z1", false));
            int currentNumber = Convert.ToInt32(_likeNumberDic[target.name].text);
            currentNumber++;
            _likeNumberDic[target.name].text = currentNumber.ToString();
        }
    }
}