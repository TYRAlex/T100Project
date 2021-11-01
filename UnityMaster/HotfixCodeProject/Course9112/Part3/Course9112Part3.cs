using LuaFramework;
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
    public class Course9112Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject Bg;
        private GameObject _bg2;
        private BellSprites bellTextures;
        private GameObject btnTest;
        private GameObject max;
        private GameObject _maxNew;
        private Vector2 _maxPos1;
        private Vector2 _maxPos2;
        private Vector2 _maxPos3;
        private GameObject _level1;
        private GameObject _room;
        private GameObject _window;
        private GameObject _food;
        private GameObject _iron;
        private GameObject _maxClick;
        private GameObject _clickEvent;
        private GameObject[] _clickLevel1;
        private bool _clicked1;
        private bool _clicked2;
        private bool _clicked3;

        private GameObject _level2;
        private GameObject[] _dragItems;
        private mILDrager[] _dragLevel2;

        private GameObject _level3;

        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;
            _maxPos1 = new Vector2(0, -490);
            _maxPos2 = new Vector2(0, -270);
            _maxPos3 = new Vector2(-465, -345);

            _bg2 = curTrans.GetGameObject("Bg2");
            max = curTrans.GetGameObject("max");
            _level1 = curTrans.GetGameObject("Level1");
            _room = _level1.transform.GetGameObject("Room");
            _window = _level1.transform.GetGameObject("Window");
            _food = _level1.transform.GetGameObject("Food");
            _iron = _level1.transform.GetGameObject("Iron");
            _maxClick = _level1.transform.GetGameObject("MaxClick");
            _clickEvent = curTrans.GetGameObject("ClickEvent");
            _clickLevel1 = new GameObject[_clickEvent.transform.childCount];
            for (int i = 0; i < _clickEvent.transform.childCount; i++)
            {
                _clickLevel1[i] = _clickEvent.transform.GetChild(i).gameObject;
            }

            _level2 = curTrans.GetGameObject("Level2");
            _dragLevel2 = new mILDrager[_level2.transform.childCount];
            _dragItems = new GameObject[_level2.transform.childCount];
            for (int i = 0; i < _level2.transform.childCount; i++)
            {
                _dragItems[i] = _level2.transform.GetChild(i).gameObject;
                _dragLevel2[i] = _level2.transform.GetChild(i).GetComponent<mILDrager>();
            }

            _level3 = curTrans.GetGameObject("Level3");
            _maxNew = _level3.transform.GetGameObject("max2");
            Util.AddBtnClick(btnTest, ReStart);
#if !UNITY_EDITOR
            btnTest.SetActive(false);
#endif
            btnTest.SetActive(false);
            ReStart(btnTest);
        }

        void ReStart(GameObject obj)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            max.SetActive(true);
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Util.AddBtnClick(_maxClick, null);
            max.GetComponent<RectTransform>().anchoredPosition = _maxPos1;
            max.GetComponent<RectTransform>().localScale = new Vector3(0.7f, 0.7f, 0);

            _level1.Show();
            _room.Show();
            _window.Show();
            _food.Show();
            _iron.Show();
            _room.GetComponent<RectTransform>().anchoredPosition = new Vector2(3000, 3000);
            _window.GetComponent<RectTransform>().anchoredPosition = new Vector2(3000, 3000);
            _food.GetComponent<RectTransform>().anchoredPosition = new Vector2(3000, 3000);
            _iron.GetComponent<RectTransform>().anchoredPosition = new Vector2(3000, 3000);
            SpineManager.instance.DoAnimation(max, "daiji");
            SpineManager.instance.DoAnimation(_room, "qiang");
            SpineManager.instance.DoAnimation(_window, "c1");
            SpineManager.instance.DoAnimation(_food, "shiwu");
            SpineManager.instance.DoAnimation(_iron, "tieban1");
            _bg2.Hide();
            _maxClick.Hide();
            _clickEvent.Show();
            foreach (var child in _clickLevel1)
            {
                Util.AddBtnClick(child, null);
                child.Show();
            }
            foreach (var item in _dragItems)
            {
                item.Show();
            }
            InitDrag();

            _level2.Hide();
            _level3.Hide();
            _clickEvent.Hide();

            _clicked1 = false;
            _clicked2 = false;
            _clicked3 = false;
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();

        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 2, true);
            //如果人类受到大量核辐射，人体会产生不良反应，严重时甚至会死亡。万一遇到核泄漏时，该如何做好防护工作呢？
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null,
            () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }, 0));
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(max, "daijishuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(max, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        //暂停协程
        IEnumerator WaitCoroutine(float len, Action method_1 = null)
        {
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
        }

        //最后环节协程
        IEnumerator LastCoroutine()
        {
            yield return new WaitForSeconds(2.0f);
            max.Hide();
            _level2.Hide();
            _level3.Show();
            SpineManager.instance.DoAnimation(_maxNew, "daiji");
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
            //如果家里没有专门的核辐射防护服，外出疏散时可穿上长袖外套、长裤、口罩、帽子、手套等防止皮肤外露的衣物，有助于减少体表放射性污染。
            float len = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6, false);
            yield return new WaitForSeconds(5.5f);
            SpineManager.instance.DoAnimation(_maxNew, "ca");
            yield return new WaitForSeconds(1.0f);
            SpineManager.instance.DoAnimation(_maxNew, "ca2");
            yield return new WaitForSeconds(1.0f);
            SpineManager.instance.DoAnimation(_maxNew, "ca3");
            yield return new WaitForSeconds(1.0f);
            SpineManager.instance.DoAnimation(_maxNew, "ca4");
            yield return new WaitForSeconds(1.0f);
            SpineManager.instance.DoAnimation(_maxNew, "ca5");
            yield return new WaitForSeconds(len - 9.5f);
            SoundManager.instance.ShowVoiceBtn(true);
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                max.GetComponent<RectTransform>().anchoredPosition = _maxPos2;
                max.GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 0);
                _room.GetComponent<RectTransform>().anchoredPosition = new Vector2(-910, -490);
                _window.GetComponent<RectTransform>().anchoredPosition = new Vector2(-910, -490);
                _food.GetComponent<RectTransform>().anchoredPosition = new Vector2(-910, -490);
                _iron.GetComponent<RectTransform>().anchoredPosition = new Vector2(-910, -490);
                //点击房间内的物品，了解在家时要如何做好核辐射防护吧!
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null,
                () =>
                {
                    _clickEvent.Show();
                    foreach (var child in _clickLevel1)
                    {
                        Util.AddBtnClick(child, ClickEvent);
                    }
                }, 0));
            }
            if (talkIndex == 2)
            {
                max.Show();
                _level3.Hide();
                _bg2.Hide();
                max.GetComponent<RectTransform>().anchoredPosition = _maxPos1;
                max.GetComponent<RectTransform>().localScale = new Vector3(0.7f, 0.7f, 0);
                //同学们，你们还知道哪些核泄漏时的对应措施呢？期待你们的分享~
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7, null, null, 0));
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

        #region 点击相关
        //点击事件
        void ClickEvent(GameObject obj)
        {
            int index = Convert.ToInt32(obj.name);
            _clickEvent.Hide();
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, index + 1,
            () =>
            {
                if (index == 1)
                {
                    _clicked1 = true;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                    SpineManager.instance.DoAnimation(_window, "c2", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_window, "c3");
                    });
                }
                if (index == 2)
                {
                    _clicked2 = true;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                    SpineManager.instance.DoAnimation(_food, "shiwu2", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_food, "shiwu3");
                    });
                }
                if (index == 3)
                {
                    _clicked3 = true;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                    SpineManager.instance.DoAnimation(_iron, "tieban2", false,
                    () =>
                    {
                        _iron.Hide();
                        SpineManager.instance.DoAnimation(_room, "qiang2", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_room, "qiang3", false);
                            _food.Show();
                            _clickEvent.Show();
                            _clickLevel1[index - 1].Hide();
                            JudgeMaxCanClick();
                        });
                    });
                }
            },
            () =>
            {
                if (index != 3)
                {
                    _clickEvent.Show();
                    _clickLevel1[index - 1].Hide();
                    JudgeMaxCanClick();
                }
            }, 0));
        }

        //判断Max能否点击
        void JudgeMaxCanClick()
        {
            if (_clicked1 && _clicked2 && _clicked3)
            {
                _maxClick.Show();
                Util.AddBtnClick(_maxClick, ClickMax);
            }
        }

        //Max点击事件
        private void ClickMax(GameObject obj)
        {
            _maxClick.Hide();
            _bg2.Show();
            _clickEvent.Hide();
            _level1.Hide();
            _level2.Show();
            max.GetComponent<RectTransform>().anchoredPosition = _maxPos3;
            foreach (var item in _dragItems)
            {
                SpineManager.instance.DoAnimation(item, item.name);
            }
            //当外出撤离时，要穿戴防护衣物，请大家更换合适的衣物。
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, null,
            () =>
            {
                AddDragEvent();
            }, 0));
        }

        #endregion

        #region 拖拽事件

        //初始化拖拽事件
        void InitDrag()
        {
            foreach (var drager in _dragLevel2)
            {
                drager.DoReset();
                drager.canMove = false;
                drager.SetDragCallback(null, null, null, null);
            }
        }

        //添加拖拽事件
        void AddDragEvent()
        {
            foreach (var drager in _dragLevel2)
            {
                drager.canMove = true;
                drager.SetDragCallback(null, DragingEvent, EndDragEvent, null);
            }
        }
        
        //拖拽中事件
        private void DragingEvent(Vector3 dragPos, int dragType, int dragIndex)
        {
            string itemName;
            if (dragIndex == 0)
                itemName = "a";
            else if (dragIndex == 1)
                itemName = "b";
            else if (dragIndex == 2)
                itemName = "c";
            else if (dragIndex == 3)
                itemName = "d";
            else
                itemName = "e";
            SpineManager.instance.DoAnimation(_dragItems[dragIndex], itemName + "2");
        }

        //拖拽结束事件
        private void EndDragEvent(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {
            string itemName;
            if (dragIndex == 0)
                itemName = "a";
            else if (dragIndex == 1)
                itemName = "b";
            else if (dragIndex == 2)
                itemName = "c";
            else if (dragIndex == 3)
                itemName = "d";
            else
                itemName = "e";
            SpineManager.instance.DoAnimation(_dragItems[dragIndex], itemName);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
            if (dragBool)
            {
                _dragItems[dragIndex].Hide();
                MaxAni();
            }
            else
                _dragLevel2[dragIndex].DoReset();
        }

        //判断max的着装状态
        void MaxAni()
        {
            //只着装一件
            if (!_dragItems[0].activeSelf)
                SpineManager.instance.DoAnimation(max, "daijia2");
            if (!_dragItems[1].activeSelf)
                SpineManager.instance.DoAnimation(max, "daijia4");
            if (!_dragItems[2].activeSelf)
                SpineManager.instance.DoAnimation(max, "daijia1");
            if (!_dragItems[3].activeSelf)
                SpineManager.instance.DoAnimation(max, "daijia3");
            
            //着装两件
            if (!_dragItems[0].activeSelf && !_dragItems[1].activeSelf)
                SpineManager.instance.DoAnimation(max, "daijib5");
            if (!_dragItems[0].activeSelf && !_dragItems[2].activeSelf)
                SpineManager.instance.DoAnimation(max, "daijib1");
            if (!_dragItems[0].activeSelf && !_dragItems[3].activeSelf)
                SpineManager.instance.DoAnimation(max, "daijib6");
            if (!_dragItems[1].activeSelf && !_dragItems[2].activeSelf)
                SpineManager.instance.DoAnimation(max, "daijib3");
            if (!_dragItems[1].activeSelf && !_dragItems[3].activeSelf)
                SpineManager.instance.DoAnimation(max, "daijib4");
            if (!_dragItems[2].activeSelf && !_dragItems[3].activeSelf)
                SpineManager.instance.DoAnimation(max, "daijib2");

            //着装三件
            if (!_dragItems[0].activeSelf && !_dragItems[1].activeSelf && !_dragItems[2].activeSelf)
                SpineManager.instance.DoAnimation(max, "daijic3");
            if (!_dragItems[0].activeSelf && !_dragItems[1].activeSelf && !_dragItems[3].activeSelf)
                SpineManager.instance.DoAnimation(max, "daijic4");
            if (!_dragItems[0].activeSelf && !_dragItems[2].activeSelf && !_dragItems[3].activeSelf)
                SpineManager.instance.DoAnimation(max, "daijic1");
            if (!_dragItems[1].activeSelf && !_dragItems[2].activeSelf && !_dragItems[3].activeSelf)
                SpineManager.instance.DoAnimation(max, "daijic2");

            //着装四件
            if (!_dragItems[0].activeSelf && !_dragItems[1].activeSelf && !_dragItems[2].activeSelf && !_dragItems[3].activeSelf)
            {
                SpineManager.instance.DoAnimation(max, "daijiz");
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                mono.StartCoroutine(LastCoroutine());
            }
        }
        #endregion
    }
}
