using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class TDNLPart1
    {
        private int _talkIndex;
        private MonoBehaviour _mono;
        private GameObject _curGo;
        private GameObject _bell;


        private GameObject _yun;
        private GameObject _tree;
  
        private GameObject _mask;

        private GameObject _wait;
        private CanvasSizeFitter _waitCSF;
        private GameObject _wait2;

        private GameObject _success;
        private GameObject _success3;
        private Text _successNum;

        private GameObject _timeAndScore;
        private Text _numTxt;

        private Image _progress2;
        private GameObject _tipAni;

        private GameObject _caidai;

        private Transform _maps;
        private Transform _chuiZis;
        private int _curSocre;     //当前分数  init 0

        private int _curGameRemainTime;   //游戏剩余时间  init 90

        private float _proportion;   //进度条满值是1， 1÷当前剩余时间时间就是每秒减去多少

        private int _tipTime;   //游戏提示时间 init 10

        private float _intervalTime;  //敌人生成间隔时间

        private float _enenmyWaitTime;   //敌人待机时间
 
        private List<Transform> _createEnemys;
        private List<Transform> _chuiZiPostions;

        private GameObject _enemyPrefab;
        private GameObject _chuiZiPrefab;
        private GameObject _chuiZiAniGo;
       
        private int _lastRandowIndex;

        void Start(object o)
        {
        
            _curGo = (GameObject)o;
            Transform curTrans = _curGo.transform;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            _mono.StopAllCoroutines();

            _bell = curTrans.Find("bell").gameObject;
            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _yun = curTrans.GetGameObject("Bg/RawImage/yun");
            _tree = curTrans.GetGameObject("Bg/RawImage/tree");

            _mask = curTrans.GetGameObject("mask");
            _mask.Show();

            _wait = curTrans.GetGameObject("mask/Wait");

            _waitCSF = _wait.transform.GetCanvasSizeFitter();

            _waitCSF.Action = () => {
                _wait.transform.GetRectTransform().sizeDelta = new Vector2(_waitCSF.CurBackgroundV2.x + 100, _waitCSF.CurBackgroundV2.y + 100);
            };


            _wait.Show();
            _wait2 = curTrans.GetGameObject("mask/Wait/2");

            _success = curTrans.GetGameObject("mask/Success");
            _success.Hide();

            _success3 = curTrans.GetGameObject("mask/Success/3");
            _successNum = curTrans.GetText("mask/Success/SuccessTxt");
                          
            _timeAndScore = curTrans.GetGameObject("TimeAndScore");
            _numTxt = curTrans.GetText("TimeAndScore/ScoreBg/NumTxt");
            _progress2 = curTrans.GetImage("TimeAndScore/TimeBg/Progress1/Progress2");       
           
            _tipAni = curTrans.GetGameObject("TimeAndScore/TimeBg/6");

            _maps = curTrans.GetTransform("Maps");
            _chuiZis = curTrans.GetTransform("ChuiZis");

            _enemyPrefab = curTrans.GetGameObject("Enemy");


            string oneName = curTrans.GetChild(0).gameObject.name;
            Debug.LogError("oneName："+ oneName);
            if (oneName== "ChuiZi")
            {
                _chuiZiPrefab = curTrans.Find("ChuiZi").gameObject;
            }
            else
            {
                for (int i = 0; i < _chuiZis.childCount; i++)
                {
                    var mask = _chuiZis.GetChild(i).GetChild(0);
                    Debug.LogError(mask.childCount);
                    bool is0 = mask.childCount == 0;
                    if (!is0)
                    {
                         Debug.LogError(mask.parent.name);
                        _chuiZiPrefab = mask.GetGameObject("ChuiZi");
                        _chuiZiPrefab.transform.SetParent(_curGo.transform);
                        _chuiZiPrefab.transform.SetAsFirstSibling();
                        break;
                    }
                }
            }

            for (int i = 0; i < _maps.childCount; i++)
            {
                var mask = _maps.GetChild(i).GetChild(0);
                bool is0 = mask.childCount==0;
                if (!is0)
                {                 
                    var enenyItem= mask.GetChild(0).gameObject.GetComponent<EnenyItem>();
                    enenyItem.DestroyEnemy();
                }
            }
       

               
            _chuiZiAniGo = curTrans.GetGameObject("ChuiZi/5");
            _caidai = curTrans.GetGameObject("mask/Success/caidai");
            _caidai.Show();
            InitData();

            GameStart();
        }

        void InitData()
        {
            Debug.LogError("打地鼠初始化...");
            _progress2.fillAmount = 1;
            _curSocre = 0;
            _curGameRemainTime = 90;
            _numTxt.text = _curSocre.ToString();
            _proportion = (float)(Math.Round((decimal)(_progress2.fillAmount / _curGameRemainTime), 3));
            _intervalTime = 3.0f;
            _enenmyWaitTime = 2.5f;
            _tipTime = 10;
            _lastRandowIndex = -1;
           
            _createEnemys = new List<Transform>();
            _chuiZiPostions = new List<Transform>();
        }

        void GameStart()
        {
          

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            _bell.Hide();
            InitEnemys();
            OnClickWait();
        }

        #region 敌人
        private void InitEnemys()
        {
            for (int i = 0; i < _maps.childCount; i++)
            {
                var parent = _maps.GetChild(i).GetTransform("Mask");
                _createEnemys.Add(parent);         
            }

            for (int i = 0; i < _chuiZis.childCount; i++)
            {
                var parent = _chuiZis.GetChild(i).GetTransform("Mask");
                _chuiZiPostions.Add(parent);
            }
        }


        private void PlayChuiZiAni(int index)
        {
           var parent = _chuiZiPostions[index];
               
            _chuiZiPrefab.transform.SetParent(parent);
            _chuiZiPrefab.transform.GetRectTransform().anchoredPosition = new Vector2(0, 150);
            _chuiZiPrefab.Show();

            SpineManager.instance.DoAnimation(_chuiZiAniGo, "chuizi", false, () => {
                _chuiZiPrefab.Hide();
            });
        }

        private IEnumerator CreateEnemy()
        {                     
            while (_curGameRemainTime >= 0)
            {         
                //生成间隔
                yield return new WaitForSeconds(_intervalTime);

                var curIndex = RandowIndex();
                var curCreateParent = _createEnemys[curIndex];           
                var enemy= GameObject.Instantiate(_enemyPrefab, curCreateParent, false);

                var enenyItem = enemy.GetComponent<EnenyItem>();

                bool isOnClick = false;

                enenyItem.SetBeOnClickCallBack(() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                    isOnClick = true;
                    _curSocre++;
                    _numTxt.text = _curSocre.ToString();
                    enenyItem.Empty4.raycastTarget = false;
                    PlayChuiZiAni(curIndex);
                    _mono.StartCoroutine(Delay(0.5f, () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                        enenyItem.BeAttackAni(() => { enenyItem.VanishAni(() => { enenyItem.DestroyEnemy(); }); });
                    }
                    ));
                   
                });

                enemy.transform.GetRectTransform().DOAnchorPosY(-100, 0).
                    OnComplete(()=> { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);                                     });

                enenyItem.AppearAni(() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                    enenyItem.Empty4.raycastTarget = true;
                    enenyItem.TauntAni();
                });

                //停留
               yield return new WaitForSeconds(_enenmyWaitTime);
              
                if (enemy != null && !isOnClick)
                {
                  
                    enenyItem.Empty4.raycastTarget = false;
                    enemy.transform.GetRectTransform().DOAnchorPosY(-330, 0).OnComplete(() => {
                     
                        enenyItem.DestroyEnemy(); });
                }


            }
        }

        private int RandowIndex()
        {
            if (_lastRandowIndex==-1)
            {
                _lastRandowIndex = UnityEngine.Random.Range(0, _createEnemys.Count);
                return _lastRandowIndex;
            }
            else
            {             
                while(true)
                {
                    var curRandowIndex = UnityEngine.Random.Range(0, _createEnemys.Count);
                    if (_lastRandowIndex!= curRandowIndex)
                    {
                        _lastRandowIndex = curRandowIndex;
                         return curRandowIndex;                      
                    }
                }           
            }

        }
  
        #endregion

        #region 添加点击事件

        private void OnClickWait()
        {
           
            PointerClickListener.Get(_mask.gameObject).onClick = OnClickMask;

            void OnClickMask(GameObject go)
            {
                BtnPlaySound();

                SpineManager.instance.DoAnimation(_yun, "yun", true);
                SpineManager.instance.DoAnimation(_tree, "shu", true);

                _wait.Hide();
                _mask.Hide();
                _timeAndScore.Show();
             
                _mono.StartCoroutine(UpDate());
               _mono.StartCoroutine(CreateEnemy());
                PointerClickListener.Get(_mask.gameObject).onClick = null;
            }
        }
        #endregion

        #region 游戏胜利

        private void GameSuccess()
        {
         
            _mask.SetActive(true);
            _success.SetActive(true);
             
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6);
            SpineManager.instance.DoAnimation(_success3, "a2", false, One);
            SpineManager.instance.DoAnimation(_caidai, "animation", false);
            void One()
            {
                SpineManager.instance.DoAnimation(_success3, "a", true, Two);
            }

            void Two()
            {             
                _successNum.text = _curSocre.ToString();             
            }

          

        }

    


        #endregion

        #region 进度条

        private void SetProgress()
        {          
            _progress2.fillAmount -= _proportion;
        }

        private void TipAni()
        {
            SpineManager.instance.DoAnimation(_tipAni, "g", false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);   //提示警报音效          
        }


        #endregion


        IEnumerator UpDate()
        {
            while (_curGameRemainTime>=0)
            {              
                yield return new WaitForSeconds(1f);
                _curGameRemainTime--;
                SetProgress();
                SetIntervalTime(_curGameRemainTime);

                if (_curGameRemainTime<=_tipTime)                
                    TipAni();                 
            }

            if (_curGameRemainTime<0)
            {
                Debug.LogError("游戏结束");
                _tipAni.Hide();
                GameSuccess();
                _mono.StopCoroutine(UpDate());
                _mono.StopCoroutine(CreateEnemy());         
            }
            
        }

        private void SetIntervalTime(int curGameRemainTime)
        {
            if (61<=curGameRemainTime && curGameRemainTime<=90)
            {
                _intervalTime = 3.0f;
                _enenmyWaitTime = 2.5f;
            }
            else if(31<= curGameRemainTime && curGameRemainTime<=60)
            {
                _intervalTime = 2.0f;
                _enenmyWaitTime = 1.5f;
            }
            else if(0 <= curGameRemainTime && curGameRemainTime<=30)
            {
                _intervalTime =1.5f;
                _enenmyWaitTime = 1.5f;
            }
        }


        IEnumerator Delay(float delay,Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
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
            SpineManager.instance.DoAnimation(_bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(_bell, "DAIJI");

            if (method_2 != null)
            {
                method_2();
            }
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);

            switch (_talkIndex)
            {
                case 1:
                    break;             
            }
          
            _talkIndex++;
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




