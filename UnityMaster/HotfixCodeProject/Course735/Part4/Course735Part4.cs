using DG.Tweening;
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
    public class Course735Part4
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private List<Vector2> imagepos;
        private List<Transform> image;
        private List<Vector2> posList;
        private Transform Image;
        private mILDrager drag;
        private Vector2 b;
        private Vector2 c;
        private bool canMove;
        private Transform tips;
        private int index1;
        private int count;

        void Start(object o)
        {
            DOTween.KillAll();
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;
            Image = curTrans.Find("image");
            drag = curTrans.Find("dragrect").GetComponent<mILDrager>();
            tips = curTrans.Find("tips");
            canMove = true;
            index1 = 0;
            count = 0;
            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
            AddList();
        }


        void AddList() 
        {
            imagepos = new List<Vector2>();
            posList = new List<Vector2>();
            imagepos.Add(Image.Find("1").GetRectTransform().anchoredPosition);
            imagepos.Add(Image.Find("2").GetRectTransform().anchoredPosition);
            imagepos.Add(Image.Find("3").GetRectTransform().anchoredPosition);
            imagepos.Add(Image.Find("4").GetRectTransform().anchoredPosition);

            posList.Add(Image.Find("1").GetRectTransform().anchoredPosition);
            posList.Add(Image.Find("2").GetRectTransform().anchoredPosition);
            posList.Add(Image.Find("3").GetRectTransform().anchoredPosition);
            posList.Add(Image.Find("4").GetRectTransform().anchoredPosition);

            image = new List<Transform>();
            image.Add(Image.Find("1"));
            image.Add(Image.Find("2"));
            image.Add(Image.Find("3"));
            image.Add(Image.Find("4"));
        }

     

    



        private void GameInit()
        {
            talkIndex = 1;
            b = new Vector2(0, 0);
            c = new Vector2(0, 0);
            count = 0;canMove = false;index1 = 0;
            tips.GetComponent<RawImage>().texture = tips.GetComponent<BellSprites>().texture[0];
            //为解决各种交通问题，推动道路交通向智能化发展，机器人开始得到了越来越广泛的应用。
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            Image.Find("1").GetRectTransform().anchoredPosition = new Vector2(-6, 65);
            Image.Find("2").GetRectTransform().anchoredPosition = new Vector2(433, 96);
            Image.Find("3").GetRectTransform().anchoredPosition = new Vector2(0, 155);
            Image.Find("4").GetRectTransform().anchoredPosition = new Vector2(-438, 96);
            Image.Find("1").SetAsLastSibling();
            Image.Find("3").SetAsFirstSibling();
            Image.Find("4").SetSiblingIndex(1);
            Image.Find("2").SetSiblingIndex(2);
            Image.Find("1").GetComponent<RawImage>().texture = Image.Find("1").GetComponent<BellSprites>().texture[0];
            Image.Find("2").GetComponent<RawImage>().texture = Image.Find("2").GetComponent<BellSprites>().texture[1];
            Image.Find("3").GetComponent<RawImage>().texture = Image.Find("3").GetComponent<BellSprites>().texture[1];
            Image.Find("4").GetComponent<RawImage>().texture = Image.Find("4").GetComponent<BellSprites>().texture[1];
            drag.SetDragCallback(OnBeginDrag,null,OnEndDrag);
        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
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
                //2016年，北京率先尝试应用了机器人交警“小文”，其在北京街头抓拍闯红灯者、引导交通通行，为执法人员提供了一定帮助
                canMove = false;
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 1, null, () => { canMove = true; }));
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
            if (canMove) 
            {
                b = Input.mousePosition;
            }
   
        }

        private void OnDrag(Vector3 pos, int type, int index)
        {

        }
        private void TranlatePos(Transform a, Vector2 b)
        {
           
            a.GetRectTransform().DOAnchorPos(b, 0.5f).OnComplete(()=> 
            {
                CheckLayers(a);
                ShowImage(a);
            
                tips.GetComponent<RawImage>().texture = tips.GetComponent<BellSprites>().texture[index1];
            });
        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            c = Input.mousePosition;

            if (b.x - c.x > 0&&canMove==true)
            {
                
                count++;
                canMove = false;
                index1++;
                index1 = index1 % 4;
                if (count < 4)
                {
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, index1 + 1,null, () => { canMove = true; }));
                }
                else 
                {
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 5, () => {  }));
                }
                //Debug.Log("滑动");
                //int name1 = int.Parse(image[0].name);
                //int name2 = int.Parse(image[1].name);
                //int name3 = int.Parse(image[2].name);
                //int name4 = int.Parse(image[3].name);

                //name1--;name2--;name3--;name4--;
                //if (name1 < 0)
                //{
                //    name1 = 3;
                //}
                //else if (name2 < 0)
                //{
                //    name2 = 3;
                //}
                //else if (name3 < 0)
                //{
                //    name3 = 3;
                //}
                //else 
                //{
                //    name4 = 3;
                //}
                //image[0].name = name1.ToString();
                //image[1].name = name2.ToString();
                //image[2].name = name3.ToString();
                //image[3].name = name4.ToString();
                //TranlatePos(image[0],imagepos[int.Parse(image[0].name)]);
                //TranlatePos(image[1], imagepos[int.Parse(image[1].name)]);
                //TranlatePos(image[2], imagepos[int.Parse(image[2].name)]);
                //TranlatePos(image[3], imagepos[int.Parse(image[3].name)]);

                Rotate(imagepos, 1);
               
                TranlatePos(image[0], imagepos[0]);
                TranlatePos(image[1], imagepos[1]);
                TranlatePos(image[2], imagepos[2]);
                TranlatePos(image[3], imagepos[3]);
            }
        }

        private void CheckLayers(Transform a) 
        {
            if (a.GetRectTransform().anchoredPosition == posList[0])
            {
                a.SetSiblingIndex(3);
            }
            else if (a.GetRectTransform().anchoredPosition == posList[1])
            {
                a.SetSiblingIndex(2);
            }
            else if (a.GetRectTransform().anchoredPosition == posList[2])
            {
                a.SetSiblingIndex(0);
            }
            else 
            {
                a.SetSiblingIndex(1);
            }
        }
        private void ShowImage(Transform a) 
        {
            if (a.GetRectTransform().anchoredPosition == posList[0])
            {
                a.GetComponent<RawImage>().texture = a.GetComponent<BellSprites>().texture[0];
            }
            else 
            {
                a.GetComponent<RawImage>().texture = a.GetComponent<BellSprites>().texture[1];
            }
        }

        public void Rotate(List<Vector2> nums, int k)
        {

         
            k = k % nums.Count;
            for (var i = 0; i < k; i++)
            {
                var previous = nums[nums.Count - 1];
                for (var j = 0; j < nums.Count; j++)
                {
                    var temp = nums[j];
                    nums[j] = previous;
                    previous = temp;
                }
            }
        }
    }
}
