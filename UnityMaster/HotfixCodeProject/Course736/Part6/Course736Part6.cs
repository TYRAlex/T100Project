using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course736Part6
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;
        private GameObject level1;
        private GameObject level2;
        private GameObject Box;
        private GameObject Drop1;
        private GameObject Drop2;
        private GameObject Drag;
        private GameObject DragPos;
        private GameObject mask;
        private GameObject tj;
        private bool _canTj;
        private GameObject[] temp;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;
            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            level1 = curTrans.Find("1").gameObject;
            level2 = curTrans.Find("2").gameObject;
            Box = curTrans.Find("Box").gameObject;
            Drop1 = Box.transform.Find("0").gameObject;
            Drop2 = Box.transform.Find("1").gameObject;
            Drag = Box.transform.Find("Drag").gameObject;
            DragPos = Box.transform.Find("DragPos").gameObject;
            mask = curTrans.Find("mask").gameObject;
            temp = new GameObject[6];
            tj = Box.transform.Find("tj").gameObject;
            Util.AddBtnClick(tj,TJ);
            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            level1.SetActive(true);
            level2.SetActive(false);
            Box.SetActive(false);
            mask.SetActive(false);
            while(Drop1.transform.childCount>0)
            {
                Drop1.transform.GetChild(0).SetParent(Drag.transform);
            }
            while (Drop2.transform.childCount > 0)
            {
                Drop2.transform.GetChild(0).SetParent(Drag.transform);
            }
            //for (int i = 0; i < Drop1.transform.childCount; i++)
            //{
            //    Drop1.transform.GetChild(0).SetParent(Drag.transform);
            //}
            //for (int i = 0; i < Drop2.transform.childCount; i++)
            //{
            //    Drop2.transform.GetChild(0).SetParent(Drag.transform);
            //}
            for (int i = 0; i < Drag.transform.childCount; i++)
            {
                Drag.transform.GetChild(i).GetComponent<mILDrager>().isActived = true;
                Drag.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
                Drag.transform.GetChild(i).position = DragPos.transform.Find(Drag.transform.GetChild(i).name).position;
                Drag.transform.GetChild(i).GetComponent<mILDrager>().SetDragCallback(OnBeginDrag,null,OnEndDrag,null);
            }
            for (int i = 0; i < 6; i++)
            {
                temp[i] = Drag.transform.Find(i.ToString()).gameObject;
                temp[i].transform.localScale = new Vector2(1f, 1f);
            }
            tj.transform.SetAsLastSibling();
            Drag.transform.SetAsLastSibling();
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => {  isPlaying = false;SoundManager.instance.ShowVoiceBtn(true); }));

        }

        private void TJ(GameObject obj)
        {
            if (!_canTj)
                return;
            BtnPlaySound();
            mask.SetActive(true);
            _canTj = false;
            for (int i = 0; i < 3; i++)
            {
                Jugle(Drop1.transform.GetChild(i).gameObject);
                Jugle(Drop2.transform.GetChild(i).gameObject);
            }
            Speak(3);
        }

        private void Jugle(GameObject obj)
        {
            obj.transform.GetChild(0).gameObject.SetActive(true);
            if(obj.GetComponent<mILDrager>().index.ToString()==obj.transform.parent.name)
            {
                obj.transform.GetChild(0).GetComponent<Image>().sprite = Drag.GetComponent<BellSprites>().sprites[0];
            }
            else
            {
                obj.transform.GetChild(0).GetComponent<Image>().sprite = Drag.GetComponent<BellSprites>().sprites[1];
            }
        }

        private void Speak(int index, Action callback = null)
        {
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, index, null, callback));
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
                level2.SetActive(true);
                level1.SetActive(false); 
                Speak(1, () => { SoundManager.instance.ShowVoiceBtn(true); });
            }
            if(talkIndex ==2)
            {
                Max.SetActive(false);
                mask.SetActive(true);
                level2.SetActive(false);
                Box.SetActive(true);
                Speak(2, () => { mask.SetActive(false); });
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
            temp[type].transform.SetParent(Drag.transform);
            Drag.transform.SetAsLastSibling();
            temp[type].transform.SetAsLastSibling();
            temp[type].transform.localScale = new Vector2(1.3f,1.3f);
        }

        private void OnDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            temp[type].transform.localScale = new Vector2(1f, 1f);
            if (isMatch)
            {
                if (temp[type].transform.localPosition.x<0&&Drop1.transform.childCount<3)
                {
                    temp[type].transform.SetParent(Drop1.transform);
                }
                else if (temp[type].transform.localPosition.x > 0 && Drop2.transform.childCount < 3)
                {
                    temp[type].transform.SetParent(Drop2.transform);
                }
                else
                {
                    temp[type].transform.position = DragPos.transform.Find(type.ToString()).position;
                }
                //ClearActive();
                if(Drag.transform.childCount<1)
                {
                    _canTj = true;
                }
            }
            else
            {
                
                temp[type].transform.SetParent(Drag.transform);
                temp[type].transform.position = DragPos.transform.Find(type.ToString()).position;
                _canTj = false;
            }
        }

        private void ClearActive()
        {
            for (int i = 0; i < Drop1.transform.childCount; i++)
            {
                Drop1.transform.GetChild(i).GetComponent<mILDrager>().isActived = false;
            }
            for (int i = 0; i < Drop2.transform.childCount; i++)
            {
                Drop2.transform.GetChild(i).GetComponent<mILDrager>().isActived = false;
            }
        }
    }
}
