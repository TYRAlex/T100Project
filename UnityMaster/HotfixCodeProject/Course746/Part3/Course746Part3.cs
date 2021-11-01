using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course746Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private BellSprites _directionSprites;

        private GameObject Max;

        private GameObject _mainTarget;

        private GameObject _mask;
        
        // private Transform _dragPanel;
        //
        // private Transform _buttonPanel;

        private Dictionary<string, Transform> _allSceneTransformDic;

        private int _currentFinishIndex = 0;

        bool isPlaying = false;

       
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;
            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            LoadGameProperty();
            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            talkIndex = 1;
            _currentFinishIndex = 0;
            GetTargetComponent<Transform>("UpLeftSpine").gameObject.Hide();
            GetTargetComponent<Transform>("UpRightSpine").gameObject.Hide();
        }

        void LoadGameProperty()
        {
            _mask = curTrans.GetGameObject("Mask");
            _mainTarget = curTrans.GetGameObject("MainTarget");
            _allSceneTransformDic=new Dictionary<string, Transform>();
            _directionSprites = GetTargetComponent<BellSprites>("Sprites");
            mILDrager leftDrager = GetTargetComponent<mILDrager>("Drager1");
            leftDrager.SetDragCallback(null,null,LeftEndDrag);
            mILDrager rightDrager = GetTargetComponent<mILDrager>("Drager2");
            rightDrager.SetDragCallback(null,null,RightEndDrag);

            mILDroper leftDroper = GetTargetComponent<mILDroper>("LeftDrop");
            mILDroper rightDroper = GetTargetComponent<mILDroper>("RightDrop");
            leftDroper.SetDropCallBack(AfterDrop);
            rightDroper.SetDropCallBack(AfterDrop);

            GameObject playButtonGameObject = curTrans.GetGameObject("ClickPanel/PlayButton");
            PointerClickListener.Get(playButtonGameObject).clickDown = StartPlayClickEvent;
            GameObject resetButtonGameObjet = curTrans.GetGameObject("ClickPanel/ResetGameButton");
            PointerClickListener.Get(resetButtonGameObjet).clickDown = ResetGameClickEvent;
        }

        private void ResetGameClickEvent(GameObject go)
        {
            PlaySpine(_mainTarget, "qiao");
            GetTargetComponent<Transform>("UpLeftSpine").gameObject.Hide();
            GetTargetComponent<Transform>("UpRightSpine").gameObject.Hide();
        }

        private void StartPlayClickEvent(GameObject go)
        {
            if (_currentFinishIndex >= 2)
            {
                isPlaying = false;
                _mask.Show();
                Image leftDirectionImage = GetTargetComponent<Image>("UpLeftSpine");
                Image rightDirectionImage = GetTargetComponent<Image>("UpRightSpine");
                if (leftDirectionImage.sprite.name.Equals("a1")&&rightDirectionImage.sprite.name.Equals("a1"))
                {
                    PlaySpine(_mainTarget,"qiao2");
                    PlayVoice(1, () => SoundManager.instance.ShowVoiceBtn(true));
                    //todo... 播放语音：
                }
                else if (leftDirectionImage.sprite.name.Equals("a2")&&rightDirectionImage.sprite.name.Equals("a2"))
                {
                    PlaySpine(_mainTarget, "qiao3",FailAndExcuteNext);
                    PlayVoice(2);
                }
                else if (leftDirectionImage.sprite.name.Equals("a1")&&rightDirectionImage.sprite.name.Equals("a2"))
                {
                    PlaySpine(_mainTarget, "qiao4",FailAndExcuteNext);
                    PlayVoice(3);
                }
                else if (leftDirectionImage.sprite.name.Equals("a2")&&rightDirectionImage.sprite.name.Equals("a1"))
                {
                    PlaySpine(_mainTarget, "qiao5",FailAndExcuteNext);
                    PlayVoice(4);
                }
            }
        }

        private bool AfterDrop(int dragTye, int dropIndex, int dropType)
        {
            
            Image targetImage= GetTargetComponent<Image>(dropType==3?"UpLeftSpine":"UpRightSpine");
            targetImage.gameObject.Show();
            _currentFinishIndex++;
            if (dragTye == 1)
            {
                
                GetTargetComponent<mILDrager>("Drager1").DoReset();
                targetImage.sprite = _directionSprites.sprites[0];
            }
            else if(dragTye==2)
            {
                GetTargetComponent<mILDrager>("Drager2").DoReset();
                targetImage.sprite = _directionSprites.sprites[1];
            }
            //Debug.LogError("Right dragTye:"+dragTye+" dropIndex:"+dropIndex+" dropType:"+dropType);
            return true;
        }

        void FailAndExcuteNext()
        {
            PlaySpine(_mainTarget, "qiao", () =>
            {
                _mask.Hide();
                isPlaying = true;
            });

        }

        private void RightEndDrag(Vector3 pos, int dragType, int dragIndex, bool isMatch)
        {
            // Debug.LogError("RightDrag:"+pos+" :"+dragType+" :"+dragIndex+" :"+isMatch);
            if (!isMatch)
                GetTargetComponent<mILDrager>("Drager2").DoReset();
        }

        private void LeftEndDrag(Vector3 pos, int dragType, int dragIndex, bool isMatch)
        {
            //Debug.LogError("LeftDrag:"+pos+" :"+dragType+" :"+dragIndex+" :"+isMatch);
            if (!isMatch)
                GetTargetComponent<mILDrager>("Drager1").DoReset();
        }


        



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
          
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { Max.SetActive(false); isPlaying = true;   _mask.Hide(); }));
            
            
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
                //todo...画面中间有一个变形出现的云渡桥等等 ，这个动画还不知道在哪
                PlayVoice(5);
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

        }

        private void OnDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            if (!isMatch)
            {

            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);


            }
        }

        void PlayVoice(int index,Action callback=null)
        {
            float timer= SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, index);
            Delay(timer, callback);
        }

        void PlaySpine(GameObject target, string spineName, Action callback = null, bool isLoop = false)
        {
            SpineManager.instance.DoAnimation(target, spineName, isLoop, callback);
        }

        T GetTargetComponent<T>(string sceneObjectName) where T : Component
        {
            Transform target = null;
            if (!_allSceneTransformDic.TryGetValue(sceneObjectName, out target))
            {
                
                target = curTrans.GetTransform(sceneObjectName);
                if (target == null)
                {
                    for (int i = 0; i < curTrans.childCount; i++)
                    {
                        Transform firstLevelTarget = curTrans.GetChild(i);
                        if (firstLevelTarget.childCount > 0)
                        {
                            for (int j = 0; j < firstLevelTarget.childCount; j++)
                            {
                                Transform secondTarget = firstLevelTarget.GetChild(j);
                                if (secondTarget.name.Equals(sceneObjectName))
                                {
                                    target = secondTarget;
                                    break;
                                }
                            }
                        }
                        if(target!=null)
                            break;
                    }
                }

                if (target != null)
                    _allSceneTransformDic.Add(target.name, target);
            }

            if (target == null)
            {
                Debug.LogError("当前的目标物体没找到，请检查名字！"+sceneObjectName);
                return null;
            }

            if (target.GetComponent<T>() != null)
                return target.GetComponent<T>();
            else
            {
                Debug.LogError("目标物体没有对应的组件，请检查！");
                return null;
            }
        }

        void Delay(float timer, Action callback)
        {
            mono.StartCoroutine(DelayIE(timer, callback));
        }

        IEnumerator DelayIE(float timer,Action callback)
        {
            yield return new WaitForSeconds(timer);
            callback?.Invoke();
        }
    }
}
