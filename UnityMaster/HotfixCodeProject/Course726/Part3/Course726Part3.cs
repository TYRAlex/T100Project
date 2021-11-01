using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course726Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;

        private MonoScripts _monoScripts;
        
        private Transform _cube;
        private Transform _cube2;

        private Button _connectButton;
        private Button _disconnectButton;
        
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();

            bell = curTrans.Find("bell").gameObject;

            talkIndex = 1;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            _monoScripts = curTrans.GetTargetComponent<MonoScripts>("mono");
            Debug.Log(_monoScripts);
            _monoScripts.FixedUpdateCallBack = TriggerCallBack;
            _cube = curTrans.GetTransform("Cube");
            _cube2 = curTrans.GetTransform("Cube2");
            _connectButton = curTrans.GetTargetComponent<Button>("Connect");
            _connectButton.onClick.RemoveAllListeners();
            _connectButton.onClick.AddListener( ConnectAndroid.Instance.SendConnectData);
            _disconnectButton = curTrans.GetTargetComponent<Button>("DisConnect");
            _disconnectButton.onClick.RemoveAllListeners();
            _disconnectButton.onClick.AddListener(ConnectAndroid.Instance.CloseSubscribe);
            GameStart();
        }

        void GameStart()
        {
           ConnectAndroid.Instance.SendConnectData();
           
               
           FunctionOf3Dof.Current3DofCallBack = TriggerCallBackDelegate;
           
           
        }


        void TriggerCallBack()
        {
            //Debug.Log("Access to the Connect");
            if (FunctionOf3Dof.Instance.Trigger == true)
            {
                Debug.Log("开启开关");
                _cube.gameObject.Hide();
            }
            else
            {
                _cube.gameObject.Show();
            }
            
            _cube2.localPosition = FunctionOf3Dof.Instance.GetTouchPadPositon();
        }

        void TriggerCallBackDelegate()
        {
            Debug.Log("成功连接");
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");

            if (method_2 != null)
            {
                method_2();
            }
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if(talkIndex == 1)
            {

            }
            talkIndex++;
        }

        private void BtnPlaySound() {
            SoundManager.instance.PlayClip(9);
        }

        private void BtnPlaySoundF()
        {
            SoundManager.instance.PlayClip(6);
        }
    }
}
