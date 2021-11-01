using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Spine.Unity;

namespace ILFramework.HotClass
{

    public enum RoleType
    {
        Bd,
        Xem,
        Child,
        Adult,
    }

    public class TD8962Part1
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;
        private GameObject _mask;
        private GameObject _replaySpine;
        private GameObject _startSpine;
        private GameObject _okSpine;
        private GameObject _successSpine;
        private GameObject _spSpine;
        private GameObject _dDD;
        private GameObject _sDD;
        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;
        private bool _isPlaying;


        private Transform fei;
        private Transform color;
        private Transform xuetiao;
        private Transform gj;
        private Transform BG;
        private Transform bd;
        private Transform bdPos;
        private Transform sp;

        private GameObject dj;
        private GameObject guang;
        private GameObject xem;
        private GameObject guangfei;
        private GameObject gjxg1;
        private GameObject gjxg2;
        private GameObject yan;
        private GameObject qj;
        private GameObject zx;
        private GameObject gjxem;

        private string shootName;
        private GameObject clickObj;
        private List<Material> bdMates;

        private List<Vector2> bdPosition;

        private Dictionary<string, string> ddColor;



        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _mask = curTrans.GetGameObject("mask");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");

            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/sp");


            _dDD = curTrans.GetGameObject("dDD");
            _sDD = curTrans.GetGameObject("sDD");




            fei = curTrans.Find("fei");
            color = curTrans.Find("color");
            xuetiao = curTrans.Find("xuetiao");
            gj = curTrans.Find("gj");
            BG = curTrans.Find("BG");
            bd = curTrans.Find("bd");
            bdPos = curTrans.Find("bdPos");
            _startATKPos= curTrans.Find("startAtkPos");
            sp = curTrans.Find("sp");

            dj = curTrans.GetGameObject("BG/dj");
            qj = curTrans.GetGameObject("qj");
            guang = curTrans.GetGameObject("guang");
            xem = curTrans.GetGameObject("xem");
            guangfei = curTrans.GetGameObject("fei/guangfei");
            gjxg1 = curTrans.GetGameObject("gjxg1");
            gjxg2 = curTrans.GetGameObject("gjxg2");
            yan = curTrans.GetGameObject("yan");
            zx = curTrans.GetGameObject("zx");
            gjxem = curTrans.GetGameObject("gjxem");

            Empty4Raycast[] qbEr4s = color.GetComponentsInChildren<Empty4Raycast>();
            for (int i = 0; i < qbEr4s.Length; i++)
            {
                Util.AddBtnClick(qbEr4s[i].gameObject, QbClick);
            }

            Empty4Raycast[] bdEr4s = bd.GetComponentsInChildren<Empty4Raycast>();
            for (int i = 0; i < bdEr4s.Length; i++)
            {
                Util.AddBtnClick(bdEr4s[i].gameObject, ClickBd);
            }

            bdPosition = new List<Vector2>();
            bdMates = new List<Material>();
            ddColor = new Dictionary<string, string>();
            ddColor.Add("qdA", "djE");
            ddColor.Add("qdB", "djD");
            ddColor.Add("qdC", "djC");
            ddColor.Add("qdD", "djB");
            ddColor.Add("qdE", "djF");
            ddColor.Add("qdF", "djG");












            GameInit();
            GameStart();
        }

        void InitData()
        {
            grade1 = 0;
            grade2 = 0;
            grade3 = 0;
            level = 0;
            canAtk = false;
            _isPlaying = true;
            canBdClick = false;
            feiIndex = 0;
            feiSwitch(feiIndex);
            HP = 3;
            HPSwitch(HP);
            shootName = null;
            curBd = null;
            gun = null;
            curSp = null;
            curDDColor = null;
            qbClickTime = 0;
            _canClickQb = false;
            BG.GetChild(3).GetComponent<RawImage>().color= new Color(255, 255, 255, 0);
            BG.GetChild(4).GetComponent<RawImage>().color=new Color(255, 255, 255, 0);

            for (int i = 0; i < 2; i++)
            {
                BG.GetChild(i).gameObject.SetActive(true);
            }
            for (int i = 2; i < 5; i++)
            {
                BG.GetChild(i).gameObject.SetActive(false);
            }


            for (int i = 0; i < color.childCount; i++)
            {
                SpineManager.instance.DoAnimation(color.GetChild(i).gameObject, color.GetChild(i).gameObject.name, false);
                InitSpine(color.GetChild(i).GetChild(0), "");
            }
            for (int i = 0; i < bd.childCount; i++)
            {
                SpineManager.instance.DoAnimation(bd.GetChild(i).gameObject, "kong", false);
                bdMates.Add(bd.GetChild(i).GetComponent<SkeletonGraphic>().material);
            }

            SpineManager.instance.DoAnimation(gjxg2, "kong", false);
            SpineManager.instance.DoAnimation(gjxg1, "kong", false);
            SpineManager.instance.DoAnimation(qj, "qj", true);
            xem.GetComponent<SkeletonGraphic>().Initialize(true);
            gjxem.GetComponent<SkeletonGraphic>().Initialize(true);
            PlaySpine(xem.gameObject, "xem1", null, true);
            //PlaySpine(xem.gameObject, "kong", () => { 
               
            //});


            InitSpine(dj.transform, "djA");
            InitSpine(yan.transform, "");
            InitSpine(guang.transform, "");
            InitSpine(zx.transform, "");
            InitSpine(gjxem.transform, "");
            for (int i = 0; i < gj.childCount; i++)
            {
                gj.GetChild(i).gameObject.SetActive(false);

            }

            for (int i = 0; i < bdPos.childCount; i++)
            {
                bdPosition.Add(bdPos.GetChild(i).position);

            }

            for (int i = 0; i < sp.childCount; i++)
            {
                SpineManager.instance.DoAnimation(sp.GetChild(i).GetChild(0).gameObject,"kong",false);
            }

            Empty4Raycast[] bdEr4s = bd.GetComponentsInChildren<Empty4Raycast>();
            for (int i = 0; i < bdEr4s.Length; i++)
            {
                bdEr4s[i].enabled = false;
            }

            Empty4Raycast[] qdEr4s = color.GetComponentsInChildren<Empty4Raycast>();
            for (int i = 0; i < qdEr4s.Length; i++)
            {
                qdEr4s[i].enabled = false;
            }

            InitSpine(guangfei.transform,"");
            BG.GetChild(3).gameObject.SetActive(false);
            BG.GetChild(4).gameObject.SetActive(false);

          //  xem.GetComponent<SkeletonGraphic>().material.SetColor("_Color", new Color(1, 1, 1,1));




            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };

        }
        //Spine初始化
        void InitSpine(Transform _tra, string animation)
        {
            SkeletonGraphic _ske = _tra.GetComponent<SkeletonGraphic>();
            _ske.startingAnimation = animation;
            _ske.Initialize(true);
        }

        void GameInit()
        {
            InitData();

            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio();
            StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide();



            _dDD.Hide();



            _sDD.Hide();







            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);

        }



        void GameStart()
        {
            _mask.Show(); _startSpine.Show();

            PlaySpine(_startSpine, "bf2", () =>
            {
                AddEvent(_startSpine, (go) =>
                {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () =>
                    {
                        PlayBgm(0);
                        _startSpine.Hide();                    
                        _sDD.Show();
                        BellSpeck(_sDD, 0, null, ShowVoiceBtn);                    
                    });
                });
            });
        }


        void TalkClick()
        {
            HideVoiceBtn();
            PlayOnClickSound();
            switch (_talkIndex)
            {
                case 1:
                    BellSpeck(_sDD, 1, null, () => { _sDD.Hide(); _mask.Hide(); StartGame(); });
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑

        #region 肺图片切换
        int feiIndex;
        void feiSwitch(int feiIndex) {
            for (int i = 0; i < 4; i++)
            {
                if (i == feiIndex)
                {

                    fei.GetChild(i).gameObject.SetActive(true);
                    ColorDisPlay(fei.GetChild(i).GetComponent<Image>(), true, null, 2f);

                }

                else {
                    Delay(2, () => { fei.GetChild(i).gameObject.SetActive(false); });
                    
                   // ColorDisPlay(fei.GetChild(i).GetComponent<Image>(), false, null, 2f);
                }
                    
            }
        }
        //物体渐变显示或者消失
        void ColorDisPlay(Image raw, bool isShow = true, Action method = null, float _time = 0.5f)
        {
            if (isShow)
            {
                raw.color = new Color(255, 255, 255, 0);
                raw.gameObject.SetActive(true);
                raw.DOColor(Color.white, _time).SetEase(Ease.OutSine).OnComplete(() => method?.Invoke());
            }
            else
            {
                raw.color = Color.white;
                raw.DOColor(new Color(255, 255, 255, 0), _time).SetEase(Ease.OutSine).OnComplete(() =>
                {
                    raw.gameObject.SetActive(false);
                    method?.Invoke();
                });
            }
        }

        #endregion
        void ColorDisPlay(RawImage raw, bool isShow = true, Action method = null, float _time = 0.5f)
        {
            if (isShow)
            {
                raw.color = new Color(255, 255, 255, 0);
                raw.gameObject.SetActive(true);
                raw.DOColor(Color.white, _time).SetEase(Ease.OutSine).OnComplete(() => method?.Invoke());
            }
            else
            {
                raw.color = Color.white;
                raw.DOColor(new Color(255, 255, 255, 0), _time).SetEase(Ease.OutSine).OnComplete(() =>
                {
                    raw.gameObject.SetActive(false);
                    method?.Invoke();
                });
            }
        }
        #region 小恶魔HP
        int HP;
        #region 攻击小恶魔
        void GjXem(int bilibili) {
            //切关卡前颜色重置
            for (int i = 0; i < color.childCount; i++)
            {
                if (shootName == color.GetChild(i).gameObject.name)
                {
                    SpineManager.instance.DoAnimation(color.GetChild(i).GetChild(0).gameObject, shootName + 3, false);
                    qbClickTime = 0;
                }
            }


            gjxg2.transform.DOMove(xuetiao.GetChild(0+bilibili).position, 0);
            xuetiao.GetChild(3+bilibili).gameObject.SetActive(true);
            xuetiao.GetChild(0+bilibili).gameObject.SetActive(false);
           
            SpineManager.instance.DoAnimation(dj, "djA2", false,()=> {
                SpineManager.instance.DoAnimation(dj, "djA", true);

            });

            if (bilibili!=2)
            {
                PlayVoice(2);
            }
            else if(bilibili==2)
            {
                PlayVoice(0);
            }

            SpineManager.instance.DoAnimation(gjxem,"gj-xem",false,()=> {
                gjxem.GetComponent<SkeletonGraphic>().Initialize(true);
                feiSwitch(1+bilibili);
                SpineManager.instance.DoAnimation(guangfei, "guang-f", false);
                SpineManager.instance.DoAnimation(gjxg2, "gjxg2", false, () => {
                    xuetiao.GetChild(3 + bilibili).gameObject.SetActive(false);
                    InitSpine(gjxg2.transform, "");
                });
                if (bilibili == 2)
                {

                    SpineManager.instance.DoAnimation(guang, "guang", true);
                    BG.GetChild(3).gameObject.SetActive(true);
                    BG.GetChild(4).gameObject.SetActive(true);
                    ColorDisPlay(BG.GetChild(3).GetComponent<RawImage>(),true,null,2.5f);
                    ColorDisPlay(BG.GetChild(4).GetComponent<RawImage>(),true,null,2.5f);
                    gjxg1.GetComponent<SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(gjxg1, "gjxg1", false);
                    SpineManager.instance.DoAnimation(dj, "djA3", true);
                   SpineManager.instance.DoAnimation(xem, "xem4", false,()=> { SpineManager.instance.DoAnimation(xem, "xem5", true); });
                   // _mono.StartCoroutine(ChangeSpineAlpha(xem.GetComponent<SkeletonGraphic>().material,0,1.5f));
                    Delay(7, () => { GameSuccess(); });

                }
                else {
                   
                    
                   // SpineManager.instance.DoAnimation(gjxg1, "gjxg1", false);
                    PlaySpine(gjxg1, "gjxg1", () => {
                      
                      //  PlaySpine(gjxg1, "kong");
                        gjxg1.GetComponent<SkeletonGraphic>().Initialize(true);
                    });
                    
                    SpineManager.instance.DoAnimation(xem, "xem3", false, () => { SpineManager.instance.DoAnimation(xem, "xem1", true); });
                }

              

                
            });
        }
        #endregion
        void HPSwitch(int HP)
        {
            switch (HP)
            {
                case 3:
                    for (int i = 0; i < 3; i++)
                        xuetiao.GetChild(i).gameObject.SetActive(true);
                    break;
                case 2:                   
                    GjXem(0);
                    break;
                case 1:
                    GjXem(1);
                    break;
                case 0:
                    GjXem(2);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 颜色点击
        int qbClickTime;
        bool _canClickQb;
        string curDDColor;

        void QbClick(GameObject obj)
        {
            if (!_canClickQb) return;

            PlayVoice(4);

            GameObject obj1 = obj.transform.parent.GetChild(0).gameObject;

            if (shootName == obj.transform.parent.name)
            {
                SpineManager.instance.DoAnimation(dj, "djA", true);

                //光消失
                SpineManager.instance.DoAnimation(obj1, obj1.name + 3, false);

                InitSpine(zx.transform, "");

                clickObj = null;
                shootName = null;
                canBdClick = false;
            }
            else
            {
                shootName = obj.transform.parent.name;
                
                if(clickObj) SpineManager.instance.DoAnimation(clickObj, clickObj.name + 3, false);

                foreach (string item in ddColor.Keys)
                {
                    if (item == obj.transform.parent.name)
                    {
                        //丁丁
                        SpineManager.instance.DoAnimation(dj, ddColor[item], true);
                        curDDColor = ddColor[item];
                    }
                }

                clickObj = obj1;
                canBdClick = true;
                SpineManager.instance.DoAnimation(obj1, obj1.name + 2, false);

            }
        }
        #endregion

        #region 油污随机选择

        #region 随机链表
        void GetRandomList<T>(ref List<T> list)
        {
            int i, r = list.Count - 1;
            System.Random rand = new System.Random();
            for (i = 0; i < list.Count; i++)
            {
                int n = rand.Next(0, r);
                T tem = list[i];
                list[i] = list[n];
                list[n] = tem;
                r--;
            }
        }
        #endregion

        #endregion

        #region 透明度
        IEnumerator ChangeSpineAlpha(Material material, float aimAlpha, float time, Action method = null)
        {
            float i = 0;
            float curAlpha = material.GetColor("_Color").a;
            float deltaAlpha = aimAlpha - curAlpha;

            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            while (i <= time)
            {
                material.SetColor("_Color", new Color(1, 1, 1, curAlpha + deltaAlpha * i / time));

                yield return wait;
                i += Time.fixedDeltaTime;
            }

            method?.Invoke();
        }
        #endregion

        #region 出现油污
        //油污渐变
        void BdChange(int bdCount) {
            List<Vector2> bdCurList = new List<Vector2>();
            for (int i = 0; i < bdCount; i++)
            {
                bdCurList.Add(bdPosition[i]);
            }
            GetRandomList(ref bdCurList);
            for (int i = 0; i < bdCount; i++)
            {
                bdMates[i].SetColor("_Color", new Color(1, 1, 1, 0));
                bd.GetChild(i).DOMove(bdCurList[i], 0);
                _mono.StartCoroutine(ChangeSpineAlpha(bdMates[i], 1, 0.6f));

                SpineManager.instance.DoAnimation(bd.GetChild(i).gameObject, bd.GetChild(i).gameObject.name, true);
            }
            Delay(0.6f, () => {
                _canClickQb = true;
                for (int i = 0; i < bdCount; i++)
                    bd.GetChild(i).GetChild(0).GetComponent<Empty4Raycast>().enabled = true;

                Empty4Raycast[] qdEr4s = color.GetComponentsInChildren<Empty4Raycast>();
                for (int i = 0; i < qdEr4s.Length; i++)
                {
                    qdEr4s[i].enabled = true;
                }

            });

        }
        void BDInstanse(int bdCount) {
            _canClickQb = false;
            
            SpineManager.instance.DoAnimation(xem, "kong2", false, () => { });
            PlayVoice(1);
            SpineManager.instance.DoAnimation(xem, "xem2", false, () => {

                switch (bdCount)
                {
                    case 6:
                        SpineManager.instance.DoAnimation(xem, "xem3a", false, () => {
                            SpineManager.instance.DoAnimation(xem, "xem1", true);
                            yan.GetComponent<SkeletonGraphic>().Initialize(true);
                            SpineManager.instance.DoAnimation(yan, "yanA", false, () => {
                                InitSpine(guangfei.transform, "");                              
                            });
                            Delay(7f,()=> { BdChange(bdCount); });
                            
                        });
                        break;
                    case 7:
                        SpineManager.instance.DoAnimation(xem, "xem3b", false, () => {
                            SpineManager.instance.DoAnimation(xem, "xem1", true);
                            yan.GetComponent<SkeletonGraphic>().Initialize(true);
                            SpineManager.instance.DoAnimation(yan, "yanA", false, () => {
                                InitSpine(guangfei.transform, "");                               
                            });
                            Delay(7f, () => { BdChange(bdCount); });
                        });
                        break;
                    case 8:
                        SpineManager.instance.DoAnimation(xem, "xem3c", false, () => {
                            SpineManager.instance.DoAnimation(xem, "xem1", true);
                            yan.GetComponent<SkeletonGraphic>().Initialize(true);
                            SpineManager.instance.DoAnimation(yan, "yanA", false, () => {
                                InitSpine(guangfei.transform, "");                               
                            });
                            Delay(7f, () => { BdChange(bdCount); });
                        });
                        break;
                    default:
                        break;
                }



            });









        }

        #endregion

        #region 油污点击
        bool canBdClick;
        GameObject curBd;

        int grade1;
        int grade2;
        int grade3;
        int level;
        void ClickBd(GameObject obj) {
            if (!canBdClick)
                return;
            PlayVoice(3);
            canBdClick = false;
            curBd = obj.transform.parent.gameObject;
            zx.transform.DOMove(obj.transform.position, 0f);
            SpineManager.instance.DoAnimation(zx, "zx", true);
            DDAtk();

          
               

        }
        #endregion

        #region 关卡切换
        void LevelSwitch() {
           
            if (grade1 == 6)
            {
                grade1 = 0;
                Debug.Log("进6");
                HPSwitch(2);

                Delay(2,()=> { BDInstanse(7); });
                 
                level=1;
            }
            else if (grade2 == 7)
            {
                grade2 = 0;
                Debug.Log("进7");
                HPSwitch(1);
                Delay(2, () => { BDInstanse(8); });
               
                level=2;
            }
            else if (grade3 == 8)
            {
                grade3 = 0;
                Debug.Log("进8");
                HPSwitch(0);

            }

        }

        #endregion

        #region 子弹发射
        //判断角度
        private float GetAngle(Vector3 startPos, Vector3 endPos)
        {
            Vector3 dir = endPos - startPos;
            float angle = Vector3.Angle(Vector3.right, dir);
            Vector3 cross = Vector3.Cross(Vector3.right, dir);
            float dirF = cross.z > 0 ? 1 : -1;
            angle = angle * dirF;
            return angle;
        }
        GameObject gun;
        GameObject curSp;
        void Shoot(){
            
            for (int i = 0; i < gj.childCount; i++)
            {              
                if (shootName == gj.GetChild(i).name) 
                    gun = gj.GetChild(i).gameObject;                                                          
            }
            for (int i = 0; i < sp.childCount; i++)
            {
                if (shootName == sp.GetChild(i).name) {
                    curSp = sp.GetChild(i).GetChild(0).gameObject;
                    curSp.transform.DOMove(new Vector2(curBd.transform.position.x-25, curBd.transform.position.y+50) , 0);
                }
                    
            }
            gun.transform.DOMove(_startATKPos.position, 0);
            gun.transform.rotation = Quaternion.Euler(0, 0, -30 + GetAngle(_startATKPos.position, new Vector2(curBd.transform.position.x-25, curBd.transform.position.y + 50)));
/*            gun.transform.Rotate(0, 0, -30);
            gun.transform.Rotate(0, 0, -30 + GetAngle(_startATKPos.position, curBd.transform.position));*/
            gun.SetActive(true);
            gun.transform.DOMove(new Vector2(curBd.transform.position.x-25, curBd.transform.position.y+50), 0.5f).SetEase(Ease.OutCirc).OnComplete(() => {
                gun.SetActive(false);
               // gun.transform.Rotate(0, 0, -30);
                SpineManager.instance.DoAnimation(curSp,curSp.name,false,()=> {
                    InitSpine(zx.transform, "");

                });
            });
        }
        #endregion

        #region dd攻击
        Transform _startATKPos;
        Vector2 _endATKPos;
        bool canAtk;
        void DDAtk() {
           float time1 = SpineManager.instance.DoAnimation(dj, curDDColor + 2, false);

          //  PlayVoice(2);
            Shoot();
            //击中事件
            Delay(time1,()=> {
                for (int i = 0; i < sp.childCount; i++)
                {
                    if (curBd.name == curSp.transform.GetChild(0).name)
                    {
                        SpineManager.instance.DoAnimation(dj, curDDColor , true);
                        Delay(0f, () => {
                         
                            _mono.StartCoroutine(ChangeSpineAlpha(curBd.GetComponent<SkeletonGraphic>().material,0,0.5f));
                            SpineManager.instance.DoAnimation(curBd, curBd.name + 3, false, () =>
                            {
                                SpineManager.instance.DoAnimation(curBd, "kong", false);
                                curBd.transform.GetChild(0).GetComponent<Empty4Raycast>().enabled = false;
                                switch (level)
                                {
                                    case 0:
                                        grade1++;
                                        Debug.Log("grade1:        " + grade1);
                                        Debug.Log("level:        " + level);

                                        Delay(1.5f,()=> { LevelSwitch(); });
                                       
                                        break;
                                    case 1:
                                        grade2++;
                                        Debug.Log("grade2:        " + grade2);
                                        Debug.Log("level:        " + level);
                                        Delay(1.5f, () => { LevelSwitch(); });
                                        break;
                                    case 2:
                                        grade3++;
                                        Debug.Log("grade3:        " + grade3);
                                        Debug.Log("level:        " + level);
                                        Delay(1.5f, () => { LevelSwitch(); });
                                        break;
                                    default:
                                        break;
                                }

                                canBdClick = true;

                            });
                        });


                    }
                    else
                    {

                        SpineManager.instance.DoAnimation(dj, curDDColor, true);
                        SpineManager.instance.DoAnimation(curBd, curBd.name + 2, false, () => {
                            SpineManager.instance.DoAnimation(curBd, curBd.name, true);
                            canBdClick = true;
                        });



                    }

                }

            });
          
            
        
        }
        #endregion















        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();

            BDInstanse(6);
           //  HPSwitch(0);

        }






        /// <summary>
        /// 游戏重玩和Ok界面
        /// </summary>
        private void GameReplayAndOk()
        {
            _mask.Show();
            _replaySpine.Show();
            _okSpine.Show();
            _successSpine.Hide();
            PlaySpine(_replaySpine, "fh2", () =>
            {
                AddEvent(_replaySpine, (go) =>
                {
                    PlayOnClickSound();
                    RemoveEvent(_replaySpine);
                    RemoveEvent(_okSpine);
                    var time = PlaySpine(_replaySpine, "fh");
                    Delay(time, () =>
                    {
                        _okSpine.Hide();
                        PlayBgm(0);
                        GameInit();
                        //ToDo...						
                        StartGame();
                    });
                });
            });

            PlaySpine(_okSpine, "ok2", () =>
            {
                AddEvent(_okSpine, (go) =>
                {
                    PlayOnClickSound();
                   
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () =>
                    {
                        _replaySpine.Hide();
                        _dDD.Show();
                        BellSpeck(_dDD, 2);
                    			

                    });
                });
            });

        }

        /// <summary>
        /// 游戏成功界面
        /// </summary>
        private void GameSuccess()
        {
            _mask.Show();
            _successSpine.Show();
            PlayCommonSound(3);
            PlaySpine(_successSpine, "6-12-z", () => { PlaySpine(_successSpine, "6-12-z2"); });
            PlaySpine(_spSpine, "kong", () => { PlaySpine(_spSpine, _spSpine.name); });
            Delay(4.0f, GameReplayAndOk);
        }




        #endregion

        #region 常用函数

        #region 语音按钮

        private void ShowVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(true);
        }

        private void HideVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(false);
        }

        #endregion

        #region 隐藏和显示

        private void HideAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Hide();
        }

        private void HideChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Hide();
            callBack?.Invoke(go);
        }

        private void ShowChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Show();
            callBack?.Invoke(go);
        }
        private void ShowAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Show();
        }

        #endregion

        #region 拖拽相关

        /// <summary>
        /// 设置Drager回调
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="dragStart"></param>
        /// <param name="draging"></param>
        /// <param name="dragEnd"></param>
        /// <param name="onClick"></param>
        /// <returns></returns>
        private List<mILDrager> SetDragerCallBack(Transform parent, Action<Vector3, int, int> dragStart = null, Action<Vector3, int, int> draging = null, Action<Vector3, int, int, bool> dragEnd = null, Action<int> onClick = null)
        {
            var temp = new List<mILDrager>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var drager = parent.GetChild(i).GetComponent<mILDrager>();
                temp.Add(drager);
                drager.SetDragCallback(dragStart, draging, dragEnd, onClick);
            }

            return temp;
        }

        /// <summary>
        /// 设置Droper回调(失败)
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="failCallBack"></param>
        /// <returns></returns>
        private List<mILDroper> SetDroperCallBack(Transform parent, Action<int> failCallBack = null)
        {
            var temp = new List<mILDroper>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var droper = parent.GetChild(i).GetComponent<mILDroper>();
                temp.Add(droper);
                droper.SetDropCallBack(null, null, failCallBack);
            }
            return temp;
        }


        #endregion

        #region Spine相关

        private void InitSpines(Transform parent, bool isKong = true, Action initCallBack = null)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                var isNullSpine = child.GetComponent<SkeletonGraphic>() == null;
                if (isNullSpine)
                    continue;
                if (isKong)
                    PlaySpine(child, "kong", () => { PlaySpine(child, child.name); });
                else
                    PlaySpine(child, child.name);
            }
            initCallBack?.Invoke();
        }

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }

        private GameObject FindGo(Transform parent, string goName)
        {
            return parent.Find(goName).gameObject;
        }

        #endregion

        #region 音频相关

        private float PlayFailSound()
        {
            PlayCommonSound(5);

            var index = Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        private float PlaySuccessSound()
        {
            PlayCommonSound(4);
            var index = Random.Range(0, _succeedSoundIds.Count);
            var id = _succeedSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        private void PlayOnClickSound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private float PlayBgm(int index, bool isLoop = true)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, index, isLoop);
            return time;
        }

        private float PlayVoice(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, index, isLoop);
            return time;
        }

        private float PlaySound(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index, isLoop);
            return time;
        }

        private float PlayCommonBgm(int index, bool isLoop = true)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, index, isLoop);
            return time;
        }

        private float PlayCommonVoice(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, index, isLoop);
            return time;
        }

        private float PlayCommonSound(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, index, isLoop);
            return time;
        }

        private void StopAllAudio()
        {
            SoundManager.instance.StopAudio();
        }

        private void StopAudio(SoundManager.SoundType type)
        {
            SoundManager.instance.StopAudio(type);
        }

        private void StopAudio(string audioName)
        {
            SoundManager.instance.Stop(audioName);
        }

        #endregion

        #region 延时相关

        private void Delay(float delay, Action callBack)
        {
            _mono.StartCoroutine(IEDelay(delay, callBack));
        }

        private void UpDate(bool isStart, float delay, Action callBack)
        {
            _mono.StartCoroutine(IEUpdate(isStart, delay, callBack));
        }

        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        IEnumerator IEUpdate(bool isStart, float delay, Action callBack)
        {
            while (isStart)
            {
                yield return new WaitForSeconds(delay);
                callBack?.Invoke();
            }
        }

        #endregion

        #region 停止协程

        private void StopAllCoroutines()
        {
            _mono.StopAllCoroutines();
        }

        private void StopCoroutines(string methodName)
        {
            _mono.StopCoroutine(methodName);
        }

        private void StopCoroutines(IEnumerator routine)
        {
            _mono.StopCoroutine(routine);
        }

        private void StopCoroutines(Coroutine routine)
        {
            _mono.StopCoroutine(routine);
        }

        #endregion

        #region Bell讲话

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Adult, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Adult, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

            switch (roleType)
            {
                case RoleType.Bd:
                    daiJi = "bd-daiji"; speak = "bd-speak";
                    break;
                case RoleType.Xem:
                    daiJi = "daiji"; speak = "speak";
                    break;
                case RoleType.Child:
                    daiJi = "animation"; speak = "animation2";
                    break;
                case RoleType.Adult:
                    daiJi = "daiji"; speak = "speak";
                    break;
            }

            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(go, daiJi);
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(go, speak);

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(go, daiJi);
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        #endregion

        #region 监听相关

        private void AddEvents(Transform parent, PointerClickListener.VoidDelegate callBack)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                RemoveEvent(child);
                AddEvent(child, callBack);
            }
        }

        private void AddEvent(GameObject go, PointerClickListener.VoidDelegate callBack)
        {
            PointerClickListener.Get(go).onClick = g => { callBack?.Invoke(g); };
        }

        private void RemoveEvent(GameObject go)
        {
            PointerClickListener.Get(go).onClick = null;
        }
        #endregion

        #region 修改Rect相关

        private void SetPos(RectTransform rect, Vector2 pos)
        {
            rect.anchoredPosition = pos;
        }

        private void SetScale(RectTransform rect, Vector3 v3)
        {
            rect.localScale = v3;
        }

        private void SetMove(RectTransform rect, Vector2 v2, float duration, Action callBack = null)
        {
            rect.DOAnchorPos(v2, duration).OnComplete(() => { callBack?.Invoke(); });
        }

        private void SetMoveAncPosX(RectTransform rect, float value, float duration, Action callBack1 = null, Action callBack2 = null)
        {
            callBack1?.Invoke();
            value = rect.anchoredPosition.x + value;
            rect.DOAnchorPosX(value, duration).OnComplete(() => { callBack2?.Invoke(); });
        }


        #endregion

        #region 打字机
        void ShowDialogue(string str, Text text, Action callBack = null)
        {
            _mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        }

        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            int i = 0;
            str = str.Replace(" ", "\u00A0");  //空格非换行        
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(0.1f);
                text.text += str[i];
                i++;
            }
            callBack?.Invoke();
            yield break;
        }
        #endregion

        #endregion

    }
}
