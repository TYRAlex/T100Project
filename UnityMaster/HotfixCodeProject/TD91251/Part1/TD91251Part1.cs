using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Spine.Unity;
using UnityEngine.EventSystems;

namespace ILFramework.HotClass
{

    public enum RoleType
    {
        Bd,
        Xem,
        Child,
        Adult,
    }

    public class TD91251Part1
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

        private GameObject _dBD;
        private GameObject _dXem;
        private GameObject _sBD;


        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        private bool _isPlaying;


        private Mesh mesh;

        private GameObject Box;
        private GameObject Box2;
        private GameObject downpos;
        private GameObject downpos2;
        private GameObject dfc;
        private GameObject water;
        private int number;
        private GameObject tempobj;
        private GameObject Drop;
        private bool _cannext;
        private bool _candown;
        private GameObject bg;
        private GameObject bg2;
        private GameObject next;
        private Coroutine coroutine1;
        private Coroutine coroutine2;
        private GameObject hua;
        private GameObject show;
        private List<int> list1;
        private List<int> list2;
        private List<int> list3;
        private int jugleNumber;
        private EventDispatcher eventDispatcher;
        private mILDrager _mil;
        private float timenumber;
        private float tiaonumber;
        private GameObject tiao;
        private GameObject xem;
        private GameObject huo;
        private GameObject show1;
        private GameObject chong;
        private GameObject smz;
        private int xemsmz;
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

            _dXem = curTrans.GetGameObject("dXem");
            _dBD = curTrans.GetGameObject("dBD");
            _sBD = curTrans.GetGameObject("sBD");
            smz = curTrans.Find("smz").gameObject;
            xem = curTrans.Find("xem").gameObject;
            dfc = curTrans.Find("dfc").gameObject;
            Box = curTrans.Find("Box").gameObject;
            Box2 = curTrans.Find("Box2").gameObject;
            water = curTrans.Find("water").gameObject;
            downpos = curTrans.Find("downpos").gameObject;
            downpos2 = curTrans.Find("downpos2").gameObject;
            list1 = new List<int>();
            list2 = new List<int>();
            list3 = new List<int>();
            show1 = curTrans.Find("show1").gameObject;
            chong = curTrans.Find("chong").gameObject;
            bg = curTrans.Find("BG").Find("bg").gameObject;
            bg2 = curTrans.Find("BG").Find("bg2").gameObject;
            next = curTrans.Find("next").gameObject;
            hua = curTrans.Find("hua").gameObject;
            tiao = hua.transform.GetChild(2).gameObject;
            huo = hua.transform.GetChild(3).gameObject;
            eventDispatcher = hua.GetComponent<EventDispatcher>();
            _mil = hua.GetComponent<mILDrager>();
            show = curTrans.Find("show").gameObject;
            for (int i = 0; i < Box.transform.childCount; i++)
            {
                Box.transform.GetChild(i).GetComponent<mILDrager>().SetDragCallback(BeginDrag, null, EndDrag, null);
            }
            mesh = new Mesh();
            mesh.name = "six";
            Vector3[] sixvec = new Vector3[7];
            sixvec[0].x = 0;
            sixvec[0].y = -50;
            sixvec[1].x = 44;
            sixvec[1].y = -25;
            sixvec[2].x = 44;
            sixvec[2].y = 25;
            sixvec[3].x = 0;
            sixvec[3].y = 50;
            sixvec[4].x = -44;
            sixvec[4].y = 25;
            sixvec[5].x = -44;
            sixvec[5].y = -25;
            sixvec[6].x = 0;
            sixvec[6].y = 0;
            mesh.vertices = sixvec;
            var triangles = new List<int>() {
            6, 1, 0,
            6, 0, 5,
            6, 5, 4,
            6, 4, 3,
            6, 3, 2,
            6, 2, 1
        };
            mesh.triangles = triangles.ToArray();
            Drop = dfc.transform.Find("Drop").gameObject;
            for (int i = 0; i < Drop.transform.childCount; i++)
            {
                Drop.transform.GetChild(i).GetComponent<MeshFilter>().mesh = mesh;
                Drop.transform.GetChild(i).GetComponent<MeshCollider>().sharedMesh = mesh;
            }
            Util.AddBtnClick(next, Next);
            GameInit();
            GameStart();
        }

        void InitData()
        {
            _isPlaying = true;

            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };

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
            _dBD.Hide();
            _sBD.Hide();
            _dXem.Hide();
            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);

            xemsmz = 3;
            number = 0;
            jugleNumber = 0;
            tiaonumber = 1;
            timenumber = 1;
            _cannext = false;
            _mil.canMove = false;
            next.SetActive(false);
            dfc.SetActive(true);
            dfc.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            Drop.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            bg.transform.GetChild(0).gameObject.SetActive(true);
            bg.GetComponent<RawImage>().color = new Color(1, 1, 1, 1);
            bg2.GetComponent<RawImage>().color = new Color(1, 1, 1, 0);
            bg2.transform.GetChild(0).gameObject.SetActive(false);
            GameobjectClear();
            GameobjectClear2();
            hua.SetActive(false);
            hua.transform.GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
            hua.transform.GetChild(1).GetComponent<SkeletonGraphic>().Initialize(true);
            hua.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -180);   
            show.SetActive(false);
            show.GetComponent<SkeletonGraphic>().Initialize(true);
            show.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
            tiao.GetComponent<Image>().sprite = tiao.GetComponent<BellSprites>().sprites[0];
            tiao.GetComponent<Image>().fillAmount = 0;
            huo.SetActive(false);
            for (int i = 0; i < Drop.transform.childCount; i++)
            {
                Drop.transform.GetChild(i).GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
                Drop.transform.GetChild(i).GetComponent<mILDroper>().index = 0;
                SpineManager.instance.DoAnimation(Drop.transform.GetChild(i).GetChild(0).gameObject, "kong", false);
            }
            for (int i = 0; i < chong.transform.childCount; i++)
            {
                chong.transform.GetChild(i).GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(chong.transform.GetChild(i).gameObject, "kong", false);
            }
            show1.SetActive(false);
            chong.SetActive(false);
            smz.SetActive(false);
            for (int i = 0; i < smz.transform.childCount; i++)
            {
                smz.transform.GetChild(i).gameObject.SetActive(true);
                smz.transform.GetChild(i).GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
                smz.transform.GetChild(i).GetComponent<Image>().sprite = smz.GetComponent<BellSprites>().sprites[0];
            }
            list1.Clear();
            list2.Clear();
            list3.Clear();
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
                        //_sBD.SetActive(true);
                        _dXem.Show();
                        _mask.SetActive(true);
                        BellSpeck(_dXem, 0, null, ShowVoiceBtn, RoleType.Xem, SoundManager.SoundType.VOICE);
                

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
                    _sBD.Show(); _dXem.Hide();
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, _sBD, null,
                       () =>
                       {
                           _sBD.SetActive(false);
                           _mask.SetActive(false);
                           StartGame();
                       }
                       ));
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();
            coroutine1 = _mono.StartCoroutine(CreateAndDown());
            //_cannext = true;
            //next.SetActive(true);
        }

        IEnumerator CreateAndDown()
        {
            while (true)
            {
                Create();
                yield return new WaitForSeconds(1.5f);
            }
        }
        private void Create()
        {
            int a = Random.Range(0,11);
            int b = Random.Range(0, 5);
            GameObject temp = GameObject.Instantiate(Box.transform.GetChild(a).gameObject, downpos.transform.GetChild(b).position, Quaternion.identity);
            temp.SetActive(true);
            temp.GetComponent<mILDrager>().DragRect = dfc.GetComponent<RectTransform>();
            temp.GetComponent<mILDrager>().SetDragCallback(BeginDrag, null, EndDrag, null);
            temp.transform.SetParent(dfc.transform);
            temp.name = Box.transform.GetChild(a).gameObject.name + number;
            temp.GetComponent<mILDrager>().index = number;
            number++;
            temp.transform.SetAsFirstSibling();
            temp.transform.localScale = new Vector3(1, 1, 1);
            move(temp);
        }

        private void move(GameObject obj)
        {
            obj.GetComponent<RectTransform>().DOAnchorPosY(-500f, 4.5f).SetEase(Ease.Linear).SetId(obj.GetComponent<mILDrager>().index).OnComplete(() => { GameObject.Destroy(obj); });
        }

        private void BeginDrag(Vector3 vector3, int a, int b)
        {
            PlaySound(0);
            string temp = string.Empty;
            switch (a)
            {
                case 0:
                    temp = "dsj";
                    break;
                case 1:
                    temp = "heng3";
                    break;
                case 2:
                    temp = "heng2";
                    break;
                case 3:
                    temp = "dan";
                    break;
                case 4:
                    temp = "youxie2";
                    break;
                case 5:
                    temp = "zuoxie2";
                    break;
                case 6:
                    temp = "zsj";
                    break;
                case 7:
                    temp = "zuoxie3";
                    break;
                case 8:
                    temp = "youxie3";
                    break;
            }
            DOTween.Pause(b);
            dfc.transform.Find(temp + b).SetAsLastSibling();
            tempobj = dfc.transform.Find(temp + b).gameObject;
        }

        private void EndDrag(Vector3 vector3, int a, int b, bool c)
        {
            ray(tempobj);
        }

        private void ray(GameObject obj)
        {
            bool[] tempbool = new bool[obj.transform.childCount];
            GameObject[] tempgame = new GameObject[obj.transform.childCount];
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Vector3 vector3 = Camera.main.WorldToScreenPoint(obj.transform.GetChild(i).position);
                Ray ray = Camera.main.ScreenPointToRay(vector3);
                RaycastHit raycastHit;
                if (Physics.Raycast(ray, out raycastHit))
                {
                    tempgame[i] = raycastHit.collider.gameObject;
                    if (raycastHit.collider.gameObject.GetComponent<mILDroper>().index == 0)
                    {
                        tempbool[i] = true;
                    }
                }

            }
            GameObject.Destroy(tempobj);
            for (int i = 0; i < tempbool.Length; i++)
            {
                if (!tempbool[i])
                {
                    PlaySound(1);
                    return;
                }
            }
            PlaySound(8);
            for (int t = 0; t < tempgame.Length; t++)
            { 
                SpineManager.instance.DoAnimation(tempgame[t].transform.GetChild(0).gameObject, "fw7", false);
                tempgame[t].GetComponent<mILDroper>().index = 1;
                Jugle();
            }

        }

        private void Jugle()
        {
            for (int i = 0; i < Drop.transform.childCount; i++)
            {
                if (Drop.transform.GetChild(i).GetComponent<mILDroper>().index == 0)
                    return;
            }
            StopCoroutines(coroutine1);
            GameobjectClear();
            PlaySound(3);
            show1.SetActive(true);
            _mono.StartCoroutine(Wait(3f,()=> {
                show1.SetActive(false);
                next.SetActive(true);
                _cannext = true;
            }));
        }

        private void Next(GameObject obj)
        {
            if (!_cannext)
                return;
            PlaySound(2);
            StopCoroutines(coroutine1);
            GameobjectClear();
            _cannext = false;
            SpineManager.instance.DoAnimation(obj, "an2", false,
                () =>
                {
                    SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
                    PlayBgm(1);
                    bg.GetComponent<RawImage>().DOColor(new Color(1, 1, 1, 0), 2f);
                    bg.transform.GetChild(0).gameObject.SetActive(false);
                    bg2.GetComponent<RawImage>().DOColor(new Color(1, 1, 1, 1), 2f);
                    bg2.transform.GetChild(0).gameObject.SetActive(true);
                    dfc.transform.Find("fc").GetComponent<RectTransform>().DOAnchorPosY(-500f, 2f);
                    Drop.GetComponent<RectTransform>().DOAnchorPosY(-500f, 2f);
                    hua.SetActive(true);
                    eventDispatcher.TriggerEnter2D += Touch;
                    SpineManager.instance.DoAnimation(hua.transform.GetChild(0).gameObject, "hua1", true);
                    hua.GetComponent<RectTransform>().DOAnchorPosY(182f, 2f).OnComplete(() =>
                    {
                        chong.SetActive(true);
                        _mono.StartCoroutine(ChongShake());
                        smz.SetActive(true);
                        show.SetActive(true); SpineManager.instance.DoAnimation(show, "d1", false);
                        _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE,2,_sBD,null,
                            () => 
                            {
                                show.GetComponent<SkeletonGraphic>().DOColor(new Color(1, 1, 1, 0), 1.5f).OnComplete(() =>
                                {
                                    _mil.canMove = true;
                                    show.SetActive(false);
                                    show.GetComponent<SkeletonGraphic>().Initialize(true);
                                    _candown = true;
                                    PlaySound(7, true);
                                    coroutine2 = _mono.StartCoroutine(CreateWaterAndDown());
                                }
                            );
                            }
                            ));
                        //_mono.StartCoroutine(Wait(2f, () =>
                        // {
                        //     show.GetComponent<SkeletonGraphic>().DOColor(new Color(1, 1, 1, 0), 1.5f).OnComplete(()=>
                        //     {
                        //         _mil.canMove = true;
                        //         show.SetActive(false);
                        //         show.GetComponent<SkeletonGraphic>().Initialize(true);
                        //         _candown = true;
                        //         coroutine2 = _mono.StartCoroutine(CreateWaterAndDown());
                        //     }
                        //     );
                        // }
                        //));
                    });
                    next.SetActive(false);
                }
                );

        }

        private void Touch(Collider2D other, int time)
        {
            if(other.name==jugleNumber.ToString())
            {
                PlaySound(4);
                _candown = false;
                _mil.canMove = false;
                other.gameObject.SetActive(false);  
                DOTween.PauseAll();
                float temptiao = jugleNumber < 4 ? 4 : (jugleNumber > 8 ? 6 : 5);
                tiao.GetComponent<Image>().fillAmount = tiaonumber/ temptiao;
                tiaonumber++;
                jugleNumber++;
                SpineManager.instance.DoAnimation(hua.transform.GetChild(1).gameObject,"star",false,
                    () => 
                    {
                        DOTween.PlayAll();_mil.canMove = true;_candown = true;
                        JugleChange();
                    }
                    );
            }
            else
            {
                PlaySound(5);
                string temp = jugleNumber < 4 ? "hua7" : (jugleNumber > 8 ? "hua9" : "hua8");
                string temp1 = jugleNumber < 4 ? "hua1" : (jugleNumber > 8 ? "hua3" : "hua2");
                SpineManager.instance.DoAnimation(hua.transform.GetChild(0).gameObject, temp, false,
                    () => { SpineManager.instance.DoAnimation(hua.transform.GetChild(0).gameObject, temp1, true); }
                    );
            }
        }

        private void JugleChange()
        {
            if(jugleNumber==4)
            {
                _candown = false;
                _mil.canMove = false;              
                for (int i = 0; i < water.transform.childCount; i++)
                {
                    GameObject.Destroy(water.transform.GetChild(i).gameObject);
                }
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                Shoot();
                _mono.StartCoroutine(Wait(1.7f,
                    () => 
                    {
                        string temp1 = jugleNumber < 4 ? "hua1" : (jugleNumber > 8 ? "hua3" : "hua2");
                        SpineManager.instance.DoAnimation(hua.transform.GetChild(0).gameObject, temp1, true);
                        tiao.GetComponent<Image>().sprite = tiao.GetComponent<BellSprites>().sprites[1];
                        tiao.GetComponent<Image>().fillAmount = 0;
                        tiaonumber = 1;
                        hua.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 182);
                        show.SetActive(true);
                        show.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        SpineManager.instance.DoAnimation(show, "d2", false);
                        _mono.StartCoroutine(Wait(2f, () =>
                        {
                            show.GetComponent<SkeletonGraphic>().DOColor(new Color(1, 1, 1, 0), 1.5f).OnComplete(() =>
                            {
                                PlaySound(7, true);
                                _mil.canMove = true;
                                show.SetActive(false);
                                show.GetComponent<SkeletonGraphic>().Initialize(true);
                                _candown = true;
                                timenumber = 0.8f;
                            }
                            );
                        }
                        ));
                    }
                    ));
            }
            if(jugleNumber==9)
            { 
                _candown = false;
                _mil.canMove = false;
                GameobjectClear2();
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                Shoot();
                _mono.StartCoroutine(Wait(1.7f,
                    () => 
                    {
                        string temp1 = jugleNumber < 4 ? "hua1" : (jugleNumber > 8 ? "hua3" : "hua2");
                        SpineManager.instance.DoAnimation(hua.transform.GetChild(0).gameObject, temp1, true);
                        tiao.GetComponent<Image>().sprite = tiao.GetComponent<BellSprites>().sprites[2];
                        tiao.GetComponent<Image>().fillAmount = 0;
                        tiaonumber = 1;
                        hua.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 182);
                        show.SetActive(true);
                        show.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        SpineManager.instance.DoAnimation(show, "d3", false);
                        _mono.StartCoroutine(Wait(2f, () =>
                        {
                            show.GetComponent<SkeletonGraphic>().DOColor(new Color(1, 1, 1, 0), 1.5f).OnComplete(() =>
                            {
                                PlaySound(7, true);
                                _mil.canMove = true;
                                show.SetActive(false);
                                show.GetComponent<SkeletonGraphic>().Initialize(true);
                                _candown = true;
                                timenumber = 0.5f;
                            }
                            );
                        }
                        ));
                    }
                    ));
            }
            if(jugleNumber==15)
            {
                _mono.StopCoroutine(coroutine2);
                _mil.canMove = false;
                GameobjectClear2();
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                Shoot();
                _mono.StartCoroutine(Wait(2.5f,
                    () =>
                    {
                        GameSuccess();
                    }
                    ));
            }
        }

        private void Shoot()
        {
            PlaySound(6);
            huo.SetActive(true);
            huo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 55);
            Vector2 a1 = xem.transform.GetChild(0).position - huo.transform.position;
            float angle = Vector2.Angle(Vector2.right,a1);
            huo.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0,0,angle-90f));
            huo.transform.DOMove(xem.transform.GetChild(0).position,0.5f).SetEase(Ease.Linear).OnComplete(()=> {
                smz.transform.GetChild(xemsmz - 1).GetComponent<Image>().sprite = smz.GetComponent<BellSprites>().sprites[1];
                SpineManager.instance.DoAnimation(smz.transform.GetChild(xemsmz - 1).GetChild(0).gameObject,"sc-boom",false,
                    () => { smz.transform.GetChild(xemsmz - 1).gameObject.SetActive(false);xemsmz --; }
                    );
                huo.SetActive(false);
                SpineManager.instance.DoAnimation(xem,"xem-y",false,
                () => { SpineManager.instance.DoAnimation(xem,"xem1",true); }
                ); });
        }
        IEnumerator CreateWaterAndDown()
        {
            while (true)
            {
                if (_candown)
                {
                    CreateWater();
                }
                yield return new WaitForSeconds(timenumber);
            }
        }
        private void CreateWater()
        {
            List<int> temp = JugleList();
            int a = temp[Random.Range(0, temp.Count)];
            temp.Remove(a);
            Vector2 vector2 = downpos2.transform.GetChild(Random.Range(0, 7)).position;
            GameObject b = GameObject.Instantiate(Box2.transform.GetChild(a).gameObject,vector2,Quaternion.identity,water.transform);
            b.name = a.ToString();
            MoveWater(b);
        }
        private void MoveWater(GameObject obj)
        {
            obj.GetComponent<RectTransform>().DOAnchorPosY(-620f, 2.5f).SetEase(Ease.InCubic).OnComplete(() => { GameObject.Destroy(obj); });
        }
        private List<int> JugleList()
        {
            if (jugleNumber < 4)
            {
                if (list1.Count == 0)
                {
                    list1 = new List<int>() { 0, 1, 2, 3, 4, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 0, 1, 2, 3, 0, 1, 2, 3, 0, 1, 2, 3 };
                }
                return list1;
            }
            else if (jugleNumber > 8)
            {
                if (list3.Count == 0)
                {
                    list3 = new List<int>() { 0, 1, 2, 3, 4, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 9, 10, 11, 12, 13, 14 };
                }
                return list3;
            }
            else
            {
                if (list2.Count == 0)
                {
                    list2 = new List<int>() { 0, 1, 2, 3, 4, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 4, 5, 6, 7, 8, 4, 5, 6, 7, 8 };
                }
                return list2;
            }
        }

        IEnumerator Wait(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }
        private void GameobjectClear()
        {
            for (int i = 0; i < dfc.transform.childCount; i++)
            {
                if (dfc.transform.GetChild(i).name != "fc" && dfc.transform.GetChild(i).name != "Drop"&& dfc.transform.GetChild(i).name != "shu")
                {
                    GameObject.Destroy(dfc.transform.GetChild(i).gameObject);
                }
            }
        }
        private void GameobjectClear2()
        {
            for (int i = 0; i < water.transform.childCount; i++)
            {
                GameObject.Destroy(water.transform.GetChild(i).gameObject);
            }
        }

        IEnumerator ChongShake()
        {
            for (int i = 0; i < chong.transform.childCount; i++)
            {
                SpineManager.instance.DoAnimation(chong.transform.GetChild(i).gameObject, chong.transform.GetChild(i).gameObject.name,true);
                yield return new WaitForSeconds(0.3f);
            }
            yield break;
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
                        //PlayCommonBgm(8); //ToDo...改BmgIndex
                        SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
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
                    PlayCommonBgm(4);
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () =>
                    {
                        _replaySpine.Hide();
                        _dBD.Show();
                        _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, _dBD, null, null, RoleType.Bd));
                        //ToDo...
                        //显示Middle角色并且说话  _dBD.Show(); BellSpeck(_dBD,0);						
                       
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

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Bd, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Bd, float len = 0)
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
