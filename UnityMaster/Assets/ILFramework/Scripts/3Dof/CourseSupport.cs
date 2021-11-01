//using bell.ai.t100.photoattendance;
//using cn.blockstudio.unityeventbus;
//using LuaFramework;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;

//namespace bell.ai.t100.remotecontrol
//{
//    public class CourseSupport : MonoBehaviour
//    {
//        [SerializeField]
//        CourseManager courseManager;
//        GameManager gameManager;
//        Canvas root;
//        private void Start()
//        {

            
//            DontDestroyOnLoad(this.gameObject);
//            StartCoroutine(ST());
//        }

//        IEnumerator ST()
//        {
//            yield return new WaitForSeconds(2);
//            courseManager = FindObjectOfType<CourseManager>();
//            gameManager = FindObjectOfType<GameManager>();
//            EventBus.instance.register(this);
//        }

//        [SubscriberMain]
//        public void irControllerChange(PhotoAttendanceActiveEvent e)
//        {
//            if (courseManager == null)
//            {
//                return;
//            }

//            if (gameManager == null)
//            {
//                return;
//            }

//            RemoteController.instance.removeAllListener();
//            switch (e.currentState)
//            {
//                case PhotoAttendanceActiveEvent.UIState.ATTDANCE: changeAttendanceState(); break;
//                //case PhotoAttendanceActiveEvent.UIState.COURSE_SCENE: changeCourseState(); break;//暂时屏蔽掉
//                case PhotoAttendanceActiveEvent.UIState.SELECT_SCENE: changeSelectState(); break;
//                default: break;
//            }
//        }


//        void changeCourseState()
//        {
//            //RemoteController.instance.LeftKeyDrop.AddListener(courseManager.Prev);
//            RemoteController.instance.LeftKeyDrop.AddListener(() => { Util.CallMethod("RightUICtrl", "PrevOnClick"); });
//            //RemoteController.instance.RightKeyDrop.AddListener(courseManager.Next);
//            RemoteController.instance.RightKeyDrop.AddListener(() => { Util.CallMethod("RightUICtrl", "NextOnClick"); });
//            //解决瑞林提出的bug id：993
//            RemoteController.instance.EscKeyDrop.AddListener(() => { gameManager.useRightBack = true;
//                Util.CallMethod("RightUICtrl", "BackOnClick");
//                irControllerChange(new PhotoAttendanceActiveEvent(PhotoAttendanceActiveEvent.UIState.SELECT_SCENE));
//            });
//            //RemoteController.instance.DownKeyDrop.AddListener(() => { courseManager.ToggleVoice(); });
//            RemoteController.instance.ConfirmDrop.AddListener(() => { DoRightUIClick("ButtonPlay"); });
//            RemoteController.instance.DownKeyDrop.AddListener(() => { Util.CallMethod("RightUICtrl", "TalkOnClick"); });
//            //RemoteController.instance.UpKeyDrop.AddListener(courseManager.Refresh);
//            RemoteController.instance.UpKeyDrop.AddListener(() => { Util.CallMethod("RightUICtrl", "RefreshOnClick"); });
//        }

//        void changeSelectState()
//        {
//            //RemoteController.instance.EscKeyDrop.AddListener(Application.Quit);//防止直接退出
//        }

//        void changeAttendanceState()
//        {
//            PhotoAttendanceTool tool = FindObjectOfType<PhotoAttendanceTool>();
//            RemoteController.instance.ConfirmDrop.AddListener(tool.takePicture);
//            RemoteController.instance.LeftKeyDrop.AddListener(tool.cancelClick);
//            RemoteController.instance.RightKeyDrop.AddListener(tool.saveClick);
//            RemoteController.instance.UpKeyDrop.AddListener(() => { Util.CallMethod("RightUICtrl", "DoStudentPrev"); });
//            RemoteController.instance.DownKeyDrop.AddListener(() => { Util.CallMethod("RightUICtrl", "DoStudentNext"); });
//        }

//        public void DoRightUIClick(string btnName)
//        {
//            var rightui = GameObject.FindGameObjectWithTag("RightUIPanel");
//            Debug.Log("rightui:" + rightui.name);
//            if (rightui == null) return;
//            var btn = rightui.transform.Find("RightPanelBackground/RightPanel/" + btnName);
//            Debug.Log("rightui btn:" + btn.name);
//            if (root == null) root = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
//            PointerEventData eventData = new PointerEventData(root.GetComponent<EventSystem>());
//            eventData.button = PointerEventData.InputButton.Left;
//            eventData.clickCount = 1;
//            eventData.clickTime = 0.2f;
//            btn.SendMessage("OnPointerClick", eventData, SendMessageOptions.DontRequireReceiver);
//        }
//    }

//}

