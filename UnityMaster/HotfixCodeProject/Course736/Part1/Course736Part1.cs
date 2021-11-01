using System;
using System.Collections;
using UnityEngine;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course736Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject ttj;
        private bool _canClick;
        private GameObject Box;
        private GameObject Show;
        private GameObject Drag;
        private GameObject DragPos;
        private GameObject mymask;
        void Start(object o)
        {
            Input.multiTouchEnabled = false;
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;

            ttj = curTrans.Find("ttj").gameObject;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            Show = curTrans.Find("show").gameObject;
            Box = curTrans.Find("Box").gameObject;
            Drag = Box.transform.Find("Drag").gameObject;
            DragPos = Box.transform.Find("Dragpos").gameObject;
            Util.AddBtnClick(curTrans.Find("anniu").gameObject, Click) ;
            mymask = curTrans.Find("mymask").gameObject;
            
            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            talkIndex = 1;
            ttj.GetComponent<SkeletonGraphic>().Initialize(true);
            Box.transform.GetChild(0).gameObject.SetActive(true);
            Box.SetActive(false);
            mymask.SetActive(false);
            for (int i = 0; i < 4; i++)
            {
                Show.transform.GetChild(i).gameObject.SetActive(false);
                Drag.transform.GetChild(i).GetComponent<mILDrager>().SetDragCallback(OnBeginDrag,null,OnEndDrag,null);
                Drag.transform.GetChild(i).position = DragPos.transform.Find(Drag.transform.GetChild(i).name).position;
            }
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { Max.SetActive(false); _canClick = true; }));
            SpineManager.instance.DoAnimation(ttj,"a1",false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
        }


        private void Click(GameObject obj)
        {
            if (!_canClick)
                return;
            BtnPlaySound();
            _canClick = false;
            SpineManager.instance.DoAnimation(ttj,"b1",true);
            Speak(1, () => { SoundManager.instance.ShowVoiceBtn(true); });
        }

        private void Speak(int index,Action callback =null)
        {
            mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,index,null,callback));
        }

        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = Max;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "DAIJIshuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }



        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mymask.SetActive(true);
                Box.SetActive(true);
                ttj.GetComponent<SkeletonGraphic>().Initialize(true);
                Speak(2,()=> { mymask.SetActive(false); });
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private bool OnAfter(int dragType, int index, int dropType)
        {

            if (dragType == dropType)
            {

            }
            return dragType == dropType;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {
            Drag.transform.Find(index.ToString()).SetAsLastSibling();
        }

        private void OnDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            Drag.transform.Find(index.ToString()).position = DragPos.transform.Find(index.ToString()).position;
            if (isMatch)
            {
                mymask.SetActive(true);
                Box.transform.GetChild(0).gameObject.SetActive(false);
                Show.transform.GetChild(index).gameObject.SetActive(true);
                Show.transform.GetChild(index).GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
                Show.transform.GetChild(index).GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(Show.transform.GetChild(index).gameObject, Show.transform.GetChild(index).gameObject.name,true);
                if(index!=1)
                {
                    if(index==3)
                    { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3); }
                    else
                    { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2); }
                    Speak(index+3,()=> { Show.transform.GetChild(index).gameObject.SetActive(false); mymask.SetActive(false); Box.transform.GetChild(0).gameObject.SetActive(true); });
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                    Speak(index + 3);
                }
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);


            }
        }
    }
}
