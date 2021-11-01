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
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }

    public enum E_DevilColor
    {
        PurpleBlue=1,
        RedBlue,
        RedYellow,
        YellowGreen,
        BlueGreen
    }

    public class TD8913Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject dbd;
       

        private Transform anyBtns;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

       

      
       

        private float textSpeed;

        //用于情景对话，需要的自行复制在 Dialogues路径下找对应spine
        private GameObject buDing;
        private Text bdText;
        private GameObject devil;
        private Text devilText;

        private Transform bdStartPos;
        private Transform bdEndPos;
        private Transform devilStartPos;
        private Transform devilEndPos;

        #region 游戏互动环节属性

        private Transform _devilSwitchPanel;

        private Transform _devilLeftNumberPanel;
        private List<GameObject> _numberList;

        private Transform _bulletPanel;
        private Dictionary<string, GameObject> _bulletDic;

        private GameObject _cannon;
        private GameObject _rocker;
        private GameObject _devil;
        private Transform _devilGameStartPos;
        private Transform _devilGameEndPos;
       

        private Tweener _devilMove = null;

        private E_DevilColor _currentDevilColor;

        private Transform _bulletShootPanel;
        private Transform _bullet;
        private string _currentBulletColor;
        private Transform _bulletStartPos;
        private Transform _bulletEndPos;
        private int _currentDevilNumber;

        private Transform _bulletPanelEndPos;

        private string[] _bulletName;
        private Transform[] _mixBulletArray;
        private bool isMixedBullet = false;
        #endregion
      
        void LoadNewGameObject()
        {
            Transform GamePanel = curTrans.GetTransform("GamePanel");
            _devilSwitchPanel = GamePanel.GetTransform("DevilSwitch");
            _devilLeftNumberPanel = GamePanel.GetTransform("Number");
            _numberList=new List<GameObject>();
            for (int i = 0; i < _devilLeftNumberPanel.childCount; i++)
            {
                Transform target = _devilLeftNumberPanel.GetChild(i);
                _numberList.Add(target.gameObject);
            }

            _bulletName = new[]
            {
                "PurpleBlue",
                "RedBlue",
                "RedYellow",
                "YellowGreen",
                "BlueGreen"
            };
            _mixBulletArray = new Transform[2];
            ClearTheMixArray();
            _bulletPanel = GamePanel.GetTransform("BulletPanel");
            _bulletDic=new Dictionary<string, GameObject>();
            for (int i = 0; i < _bulletPanel.childCount; i++)
            {

                Transform target = _bulletPanel.GetChild(i).GetChild(0);
                //Debug.Log(target.name);
                if(target.name.Contains("bg"))
                    continue;
                PointerClickListener.Get(target.GetChild(0).gameObject).onClick = ClickAndSelectCurrentBullet;
                _bulletDic.Add(target.name, target.gameObject);
            }

            _cannon = GamePanel.GetGameObject("Cannon");
            _rocker = GamePanel.GetGameObject("Rocker");
            PointerClickListener.Get(_rocker.transform.GetChild(0).gameObject).onClick = ClickAndShoot;
            _devil = GamePanel.GetGameObject("Devil");
            _devilGameStartPos = GamePanel.GetTransform("DevilStartPos");
            _devilGameEndPos = GamePanel.GetTransform("DevilEndPos");
            
            
            
            _bulletShootPanel = GamePanel.GetTransform("BulletShootPanel");
            _bullet = _bulletShootPanel.GetTransform("Bullet");
            _bulletStartPos = _bulletShootPanel.GetTransform("BulletStartPos");
            _bulletEndPos = _bulletShootPanel.GetTransform("BulletEndPos");
            _bulletPanelEndPos = GamePanel.GetTransform("BulletEndPos");
        }

       

        private void GameInit()
        {
            isMixedBullet = false;
            isPlaying = false;
            talkIndex = 1;
            textSpeed =0.5f;
            _currentDevilColor = E_DevilColor.BlueGreen;
            _currentBulletColor = "Green";
            _currentDevilNumber = 5;
            SpineManager.instance.DoAnimation(_cannon, "pao", false);
            _devil.transform.position = _devilGameStartPos.position;
            _devil.Hide();
            ShowDevilNumberLeft();
            ShowAllBulletImage();
            //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            ShowThePlayPanel();
            //Debug.LogError("hhhhh"+isPlaying);
        }
        //胜利动画名字
       
        private string sz;
        bool isPlaying = false;
       
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            LoadAllGameGameObject();
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            successSpine.Hide();
            caidaiSpine.Hide();
            anyBtns.gameObject.Show();
            anyBtns.GetChild(1).gameObject.Hide();
            mask.Show();
            mask.transform.SetAsLastSibling();
            GameObject bf = anyBtns.GetChild(0).gameObject;
            bf.Show();
            bf.name= getBtnName(BtnEnum.bf, 0);
            GameInit();
            //GameStart();
        }

        void LoadAllGameGameObject()
        {
            LoadOriginalGameObject();
            LoadNewGameObject();
        }
        
        void LoadOriginalGameObject()
        {
              
            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(false);

            buDing = curTrans.Find("mask/buDing").gameObject;
            bdText = buDing.transform.GetChild(0).GetComponent<Text>();
            bdStartPos = curTrans.Find("mask/bdStartPos");
            buDing.transform.position = bdStartPos.position;
            bdEndPos = curTrans.Find("mask/bdEndPos");

            devil = curTrans.Find("mask/devil").gameObject;
            devilText = devil.transform.GetChild(0).GetComponent<Text>();
            devilStartPos = curTrans.Find("mask/devilStartPos");
            devil.transform.position = devilStartPos.position;
            devilEndPos = curTrans.Find("mask/devilEndPos");

            bd = curTrans.Find("BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.GetGameObject("DBD");
            dbd.Hide();
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);


            anyBtns = curTrans.Find("mask/Btns");
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
            }
            anyBtns.gameObject.SetActive(false);

            sz = "6-12-z";

           
        }




       


        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string getBtnName(BtnEnum btnEnum, int index)
        {
            string result = string.Empty;
            switch (btnEnum)
            {
                case BtnEnum.bf:
                    result = "bf";
                    break;
                case BtnEnum.fh:
                    result = "fh";
                    break;
                case BtnEnum.ok:
                    result = "ok";
                    break;
                default:
                    break;
            }
            SpineManager.instance.DoAnimation(anyBtns.GetChild(index).gameObject, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    
                    GameStart();
                }
                else if(obj.name == "fh")
                {
                    
                    
                    mask.Hide();
                    anyBtns.GetChild(1).gameObject.Hide();
                    GameInit();
                    isPlaying = true;
                    Debug.Log("启动二次");
                    _devil.Show();
                    DevilMove();
                }
                else if(obj.name=="ok")
                {
                    anyBtns.GetChild(0).gameObject.Hide();
                    GameFinished();
                }
                SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); });
            });
        }

        void GameFinished()
        {
            SoundManager.instance.Stop("bgm");
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4);
            dbd.Show();
            dbd.transform.SetAsLastSibling();
            mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 5));

        }

       

        void GameStart()
        {
            // buDing.transform.DOMove(bdEndPos.position,1f).OnComplete(()=> {/*正义的一方对话结束 devil开始动画*/
            //
            //     devil.transform.DOMove(devilEndPos.position, 1f).OnComplete(() => {/*对话*/ });
            // });
            
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            mask.Show();
             bd.Show();
             bd.transform.SetAsLastSibling();
             mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 6, null,
                 () => SoundManager.instance.ShowVoiceBtn(true)));
             // bd.Hide();
             // isPlaying = true;
             // DevilMove();
        }
        
        void HideTargetBulletImage(string targetName)
        {
           
            foreach (GameObject bullet in _bulletDic.Values)
            {
                if (bullet.name == targetName)
                {
                    //Debug.Log("111");
                    bullet.Hide();
                }
                // else
                // {
                //     Debug.Log("222");
                //     bullet.Show();
                // }
            }
        }

        void ShowAllBulletImage()
        {
            foreach (GameObject image in _bulletDic.Values)
            {
                image.Show();
                image.transform.position = image.transform.parent.GetChild(1).position;
            }
        }

        void ClickAndSelectCurrentBullet(GameObject obj)
        {
            if(!isPlaying|| isMixedBullet)
                return;
            //Debug.Log("ssss");
           
            _currentBulletColor = obj.name;
           
            string spineName = null;
            int index = AddBulletNameToTheMix(obj.transform);
            if (index == 0)
            {
                obj.transform.parent.DOMove(_bulletPanelEndPos.position, 0.5f).OnComplete(() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                    HideTargetBulletImage(_currentBulletColor);
                    var parent = obj.transform.parent;
                    parent.gameObject.Hide();
                
                    switch (_currentBulletColor)
                    {
                        case "Green":
                            spineName = "fr-2";
                            break;
                        case "Red":
                            spineName = "fr-3";
                            break;
                        case "Purple":
                            spineName = "fr-1";
                            break;
                        case "Yellow":
                            spineName = "fr-11";
                            break;
                        case "Pink":
                            spineName = "fr-9";
                            break;
                        case "Blue":
                            spineName = "fr-10";
                            break;
                    }


                    SpineManager.instance.DoAnimation(_cannon, spineName, false);



                });
            }
            else if (index == 1)
            {
                obj.transform.parent.DOMove(_bulletPanelEndPos.position, 0.5f).OnComplete(() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                    HideTargetBulletImage(_currentBulletColor);
                    var parent = obj.transform.parent;
                    parent.gameObject.Hide();
                    
                    isMixedBullet = true;
                    bool isRight = GetMixBulletName();
                    if (isRight)
                    {
                        switch (_currentBulletColor)
                        {
                            case "PurpleBlue":
                                spineName = "fr-6";
                                break;
                            case "RedBlue":
                                spineName = "fr-5";
                                break;
                            case "RedYellow":
                                spineName = "fr-7";
                                break;
                            case "YellowGreen":
                                spineName = "fr-8";
                                break;
                            case "BlueGreen":
                                spineName = "fr-4";
                                break;
                        }

                        //Debug.Log("1:"+_mixBulletArray[0].name+" 2:"+_mixBulletArray[1].name+" 当前颜色："+_currentBulletColor+" SpineName:"+spineName);
                        SpineManager.instance.DoAnimation(_cannon, spineName, false);

                    }
                    else
                    {
                        Tweener tw = null;
                        SpineManager.instance.DoAnimation(_cannon, "pao", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                        for (int i = 0; i < _mixBulletArray.Length; i++)
                        {
                            Transform target = _mixBulletArray[i].parent;
                            target.gameObject.Show();
                            Transform endPos = target.parent.GetChild(1);
                            tw = target.DOMove(endPos.position, 0.5f);
                        }

                        tw.OnComplete(() => isMixedBullet = false);
                    }

                    ClearTheMixArray();
                });
            }
            else
            {
                Debug.LogError("方法错误，请检查！");
            }






        }
        
        int AddBulletNameToTheMix(Transform target)
        {
            int addIndex = 2;
            for (int i = 0; i < _mixBulletArray.Length; i++)
            {
                
                if (_mixBulletArray[i] == null)
                {
                    _mixBulletArray[i] = target;
                    addIndex = i;
                    Debug.Log("i:" + i);
                    break;
                }
            }

            return addIndex;
        }

        void ClearTheMixArray()
        {
            for (int i = 0; i < _mixBulletArray.Length; i++)
            {
                _mixBulletArray[i]=null;
            }
        }

        bool GetMixBulletName()
        {
            
            for (int i = 0; i < _bulletName.Length; i++)
            {
                string target = _bulletName[i];
                if (target.Contains(_mixBulletArray[0].name) && target.Contains(_mixBulletArray[1].name))
                {
                    
                    _currentBulletColor= target;
                    return true;
                }
            }
            
            //Debug.LogError("输入的名字为：0:" + _mixBulletArray[0] + " 1:" + _mixBulletArray[1]);
            return false;
        }

        private bool _isShoot = false;
        void ClickAndShoot(GameObject obj)
        {
            if(!isPlaying||_isShoot)
                return;
            _isShoot = true;
            SpineManager.instance.DoAnimation(_rocker, "ok", false,
                () => SpineManager.instance.DoAnimation(_rocker, "ok2", false, () =>
                {
                    _isShoot = false;
                }));
            isMixedBullet = false;
            ShowAllBulletImage();
            CreatBulletAndShoot();
        }
        
        

        void DevilSwitch(bool isAlive)
        {
            GameObject red = _devilSwitchPanel.GetGameObject("Red");
            GameObject gray = _devilSwitchPanel.GetGameObject("Gray");
            if (isAlive)
            {
                red.Show();
                gray.Hide();
            }
            else
            {
                gray.Show();
                red.Hide();
            }
        }

        void ShowNumber(int number)
        {
            for (int i = 0; i < _numberList.Count; i++)
            {
                GameObject target = _numberList[i].gameObject;
                if (target.name.Equals(number.ToString()))
                {
                    target.Show();
                }
                else
                {
                    target.Hide();
                }
            }
        }

       

        void ShowDevilNumberLeft()
        {
            if (_currentDevilNumber > 0)
            {
                DevilSwitch(true);
                ShowNumber(_currentDevilNumber);
            }
            else
            {
                DevilSwitch(false);
                ShowNumber(0);
            }
        }

        void ShowAndChangeDevilColor()
        {
            //_devil.Show();
            int randomNumber = Random.Range(1, 6);
            E_DevilColor target = (E_DevilColor) randomNumber;
            _currentDevilColor = target;
          
            SpineManager.instance.DoAnimation(_devil, "xem-" + randomNumber, true);
        }

        void DevilMove()
        {
            if(!isPlaying)
                return;
            //Debug.Log("sssss"+isPlaying);
            ShowAndChangeDevilColor();
            _devilMove?.Kill();
            _devil.transform.localPosition = _devilGameStartPos.transform.localPosition;
            _devilMove = _devil.transform.DOLocalMove(_devilGameEndPos.localPosition, 10f);
            _devilMove.SetEase(Ease.Linear);
            mono.StartCoroutine(DevilMoveUpdate());
            // _devilMove.OnUpdate(() =>
            // {
            //     if (!isPlaying)
            //     {
            //        
            //         _devilMove.Kill();
            //         _devil.transform.localPosition = _devilGameStartPos.localPosition;
            //     }
            //     Debug.Log("-------------"+isPlaying);
            //     if (isPlaying&&Vector3.Distance(_devil.transform.localPosition, _devilGameEndPos.localPosition) < 5)
            //     {
            //         //todo...  小恶魔音效：哈哈哈
            //         Debug.Log("::::::"+isPlaying);
            //         DevilMove();
            //     }
            // });
        }

        IEnumerator DevilMoveUpdate()
        {
            while (isPlaying)
            {
                if (!isPlaying)
                {
                   
                    _devilMove.Kill();
                    _devil.transform.localPosition = _devilGameStartPos.localPosition;
                }
                //Debug.Log("-------------"+isPlaying);
                if (isPlaying&&Vector3.Distance(_devil.transform.localPosition, _devilGameEndPos.localPosition) < 5)
                {
                    //todo...  小恶魔音效：哈哈哈
                    //Debug.Log("::::::"+isPlaying);
                    DevilMove();
                }
                yield return new WaitForFixedUpdate();
            }
        }

        private Tweener _bulletMove;

        void ChangeBulletColor()
        {
            Transform parent = _bullet.GetTransform("Color");
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform target = parent.GetChild(i);
                if (target.name.Equals(_currentBulletColor))
                {
                    target.gameObject.Show();
                }
                else
                {
                    target.gameObject.Hide();
                }
            }
        }

        string GetCannonShootBulletSpineName()
        {
            string cannonSpineName = string.Empty;
            switch (_currentBulletColor)
            {
                case "PurpleBlue":
                    cannonSpineName = "c-c2";
                    break;
                case "RedBlue":
                    cannonSpineName = "c-b2";
                    break;
                case "RedYellow":
                    cannonSpineName = "c-d2";
                    break;
                case "YellowGreen":
                    cannonSpineName = "c-e2";
                    break;
                case "BlueGreen":
                    cannonSpineName = "c-a2";
                    break;
            }

            //_currentBulletColor = "";
            return cannonSpineName;
        }

        void CreatBulletAndShoot()
        {
            _bullet.gameObject.Show();
            ChangeBulletColor();
            string cannonCurrentName = SpineManager.instance.GetCurrentAnimationName(_cannon);
            if(cannonCurrentName=="pao")
                return;
            string cannonSpineName = GetCannonShootBulletSpineName();
            
            if (cannonSpineName != null&&cannonSpineName.Length>3)
            {
                cannonSpineName = cannonSpineName.Remove(3);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                SpineManager.instance.DoAnimation(_cannon, cannonSpineName, false, () =>
                {
                    SpineManager.instance.DoAnimation(_cannon, "pao", false);
                });
                mono.StartCoroutine(WaitTimerAndExcuteNext(0.5f, () =>
                {
                    _bullet.transform.localPosition = _bulletStartPos.localPosition;
                    _bulletMove = _bullet.DOLocalMove(_bulletEndPos.localPosition, 1f)
                        .OnComplete(() => _bullet.gameObject.Hide());
                    _bulletMove.SetEase(Ease.Linear);
                    _bulletMove.OnUpdate(() =>
                    {
                    
                        if (Vector3.Distance(_bullet.position, _devil.transform.position) < 20f)
                        {
                            _bulletMove.Pause();
                            Hit();
                        }
                    });
                }));
               
            }

            
            
          
        }

        void Hit()
        {
            string currentDevilColor = _currentDevilColor.ToString();
            _bullet.transform.localPosition = _bulletStartPos.localPosition;
           
           
            if (currentDevilColor.Contains(_currentBulletColor))
            {
               
                HitTheDevilRight();
            }
            else
            {
              
                HideTheDevilWrong();
            }
        }

        IEnumerator WaitTimerAndExcuteNext(float timer,Action callback)
        {
            yield return new WaitForSeconds(timer);
            callback?.Invoke();
        }


        void HitTheDevilRight()
        {
            //todo... 小恶魔音效： 啊~
            float timer= SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
            _currentDevilNumber--;
            ShowDevilNumberLeft();
            if (_currentDevilNumber > 0)
            {
                SpineManager.instance.DoAnimation(_devil, "xem-s", false);
                mono.StartCoroutine(WaitTimerAndExcuteNext(timer,
                    () =>
                    {
                        //SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                        BtnPlaySoundSuccess();
                        
                        DevilMove();
                    }));
            }
            else
            {
                SpineManager.instance.DoAnimation(_devil, "xem-s", false);
                mono.StartCoroutine(WaitTimerAndExcuteNext(timer,
                    () =>
                    {
                        WinTheGame();
                   
                    }));
                
            }

            
        }
        void WinTheGame()
        {
            //todo...  小恶魔语音： 啊，你们给我等着
            isPlaying = false;
            float timer= SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4);
            mono.StartCoroutine(WaitTimerAndExcuteNext(timer, () =>
            {
                
                playSuccessSpine();
            }));
           
        }
        

        void HideTheDevilWrong()
        {
            //SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
            BtnPlaySoundFail();
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
            string spineName= SpineManager.instance.GetCurrentAnimationName(_devil);
            string[] split = spineName.Split('-');
            string targetSpineName1 = split[0] + "-x-" + split[1];
            string targetSpineName2 = split[0] + "-z-" + split[1];
            SpineManager.instance.DoAnimation(_devil, targetSpineName1, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(_devil, targetSpineName2, true, () =>
                    {
                        SpineManager.instance.DoAnimation(_devil, spineName, true, () =>
                        {
                            if (_devilMove != null && isPlaying&&_devilMove.IsPlaying())
                            {
                                _devilMove.Play();
                            }
                        });
                    });
                   
                });
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
        IEnumerator SpeckerCoroutine(GameObject target, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(target, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(target, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(target, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if(talkIndex==1)
            {
                mono.StartCoroutine(SpeckerCoroutine(bd,SoundManager.SoundType.VOICE, 0, null, () =>
                {
                    mask.Hide();
                    _devil.Show();
                    bd.Hide();
                    isPlaying = true;
                    //Debug.Log("启动一次");
                    DevilMove();
                
                }));
            }
            if (talkIndex == 2)
            {
                bd.SetActive(false);
                mask.SetActive(false);
            }
            talkIndex++;
        }
        /// <summary>
        /// 播放成功动画
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            mask.SetActive(true);
            successSpine.SetActive(true);
            caidaiSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SpineManager.instance.DoAnimation(successSpine, sz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, sz + "2", false,
                () =>
                {
                    anyBtns.gameObject.SetActive(true);
                    GameObject fh = anyBtns.GetChild(0).gameObject;
                    GameObject ok = anyBtns.GetChild(1).gameObject;
                    
                    fh.Show();
                    ok.Show();
                    fh.name = getBtnName(BtnEnum.fh,0);
                    ok.name = getBtnName(BtnEnum.ok, 1);
                    
                    caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                });
                });
        }

        void ShowThePlayPanel()
        {
            
            
            
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
        }


       

       

        #region 对话框方法

        void ShowDialogue(string str, Text text, Action callBack = null)
        {
            mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        }

        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            int i = 0;
            str = str.Replace(" ", "\u00A0");  //空格非换行
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(textSpeed);
                text.text += str[i];
                i++;
            }
            callBack?.Invoke();
            yield break;
        }

        #endregion

       

    }
}
