using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course915Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private Transform _mask;
        

        private Dictionary<string, GameObject> _objectDic;

        private Dictionary<string, GameObject> _lineDic;


        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            ResetAllGameProperty(curTrans);
          
            ReStart();
        }

        void ResetAllGameProperty(Transform curTrans)
        {
            SoundManager.instance.StopAudio();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            _objectDic=new Dictionary<string, GameObject>();
            _lineDic = new Dictionary<string, GameObject>();
            Transform objectsParent = curTrans.GetTransform("Objects");
            for (int i = 0; i < objectsParent.childCount; i++)
            {
                Transform target = objectsParent.GetChild(i);
                _objectDic.Add(target.name, target.gameObject);
                target.gameObject.Hide();
                //Debug.Log("name:" + target.name);
                //SpineManager.instance.DoAnimation(target.gameObject, target.name + "-ANIME", false);
            }

            // Debug.Log(_objectDic["1"]);
            // FindTargetAndPlay("1");
            Transform lineParent = curTrans.GetTransform("LineGroup");
            for (int i = 0; i < lineParent.childCount; i++)
            {
                Transform target = lineParent.GetChild(i);
                _lineDic.Add(target.name, target.gameObject);
                SpineManager.instance.DoAnimation(target.gameObject, "J"+target.name, false);
            }

            Transform clickParent = curTrans.GetTransform("ClickTarget");
            for (int i = 0; i < clickParent.childCount; i++)
            {
                GameObject target = clickParent.GetChild(i).gameObject;
                target.transform.GetChild(0).gameObject.Show();
                PointerClickListener.Get(target).onClick = null;
                PointerClickListener.Get(target).onClick = ClickEvent;

            }

        }

        void ReStart()
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(true);
            Bg = curTrans.Find("Bg").gameObject;
            _mask = curTrans.GetTransform("mask");
            _mask.gameObject.Show();
            _mask.SetAsLastSibling();
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();

        }

        void ClickEvent(GameObject obj)
        {
            string targetName = obj.name;
            if (obj.transform.GetChild(0).gameObject.activeSelf)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                obj.transform.GetChild(0).gameObject.SetActive(false);
                FindTargetAndPlay(targetName);
            }
            

        }

        void FindTargetAndPlay(string name)
        {
            GameObject targetObject = null;
            // if (_objectDic.ContainsKey(name))
            // {
            //     //Debug.Log(_objectDic[name]);
            //     GameObject target = _objectDic[name];
            //     target.Show();
            //     SpineManager.instance.DoAnimation(target, name + "-ANIME", false);
            // }
            // else
            // {
            //     Debug.LogError("找不到相应的物体，请检查！" + name);
            // }

            if(_objectDic.TryGetValue(name, out targetObject))
            {
                
                targetObject.Show();
                SpineManager.instance.DoAnimation(targetObject, name + "-ANIME", false);
            }
            else
            {
                
                Debug.LogError("找不到相应的物体，请检查！" + name);
            }

            if (name == "7")
            {
                return;
            }
            if (name == "5"||name=="6")
            {
                SpineManager.instance.DoAnimation(_lineDic["7"], "J7-ANIME", false);
            }
            else if (name == "Y")
            {
                SpineManager.instance.DoAnimation(_lineDic["5"], "J5-ANIME", false);
            }
            else if (name == "N")
            {
                SpineManager.instance.DoAnimation(_lineDic["6"], "J6-ANIME", false);
            }
            else
            {
                if (_lineDic.TryGetValue(name, out targetObject))
                {

                    SpineManager.instance.DoAnimation(targetObject, "J" + name + "-ANIME", false);
                
                }
                else
                {
                    Debug.LogError("找不到相应的物体，请检查！" + name);
                }
            }
            
            

        }

        void GameStart()
        {
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,null, () =>
            {
                bell.Hide();
                _mask.SetAsFirstSibling();
            }));
        }

        #region BellFuc

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {

            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }

            #endregion
        
    }
}
