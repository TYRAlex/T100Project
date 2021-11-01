using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace bell.ai.t100.remotecontrol
{
    public class RemoteController : MonoBehaviour
    {


        public static RemoteController instance;

        [SerializeField]
        UnityEvent confirmDrop;

        [SerializeField]
        UnityEvent upKeyDrop;

        [SerializeField]
        UnityEvent downKeyDrop;

        [SerializeField]
        UnityEvent leftKeyDrop;

        [SerializeField]
        UnityEvent rightKeyDrop;

        [SerializeField]
        UnityEvent escKeyDrop;

        [SerializeField]
        UnityEvent backKeyDrop;

        object eventLock;

        /// <summary>
        /// 确认键按下监听
        /// </summary>
        public UnityEvent ConfirmDrop
        {
            get
            {
                return confirmDrop;
            }

            set
            {
                confirmDrop = value;
            }
        }

        /// <summary>
        /// 上方向键按下监听
        /// </summary>
        public UnityEvent UpKeyDrop
        {
            get
            {
                return upKeyDrop;
            }

            set
            {
                upKeyDrop = value;
            }
        }


        /// <summary>
        /// 下方向键按下监听
        /// </summary>
        public UnityEvent DownKeyDrop
        {
            get
            {
                return downKeyDrop;
            }

            set
            {
                downKeyDrop = value;
            }
        }


        /// <summary>
        /// 左方向键按下监听
        /// </summary>
        public UnityEvent LeftKeyDrop
        {
            get
            {
                return leftKeyDrop;
            }

            set
            {
                leftKeyDrop = value;
            }
        }


        /// <summary>
        /// 右方向键按下监听
        /// </summary>
        public UnityEvent RightKeyDrop
        {
            get
            {
                return rightKeyDrop;
            }

            set
            {
                rightKeyDrop = value;
            }
        }

        public UnityEvent EscKeyDrop
        {
            get
            {
                return escKeyDrop;
            }

            set
            {
                escKeyDrop = value;
            }
        }

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                instance = this;

            DontDestroyOnLoad(this.gameObject);




        }




        /// <summary>
        /// 删除所有按键监听事件
        /// </summary>
        public void removeAllListener()
        {
            ConfirmDrop.RemoveAllListeners();
            UpKeyDrop.RemoveAllListeners();
            DownKeyDrop.RemoveAllListeners();
            LeftKeyDrop.RemoveAllListeners();
            RightKeyDrop.RemoveAllListeners();
            escKeyDrop.RemoveAllListeners();
            backKeyDrop.RemoveAllListeners();
        }


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) { UpKeyDrop.Invoke(); }
            if (Input.GetKeyDown(KeyCode.DownArrow)) { DownKeyDrop.Invoke(); }
            if (Input.GetKeyDown(KeyCode.LeftArrow)) { LeftKeyDrop.Invoke(); }
            if (Input.GetKeyDown(KeyCode.RightArrow)) { RightKeyDrop.Invoke(); }
            if (Input.GetKeyDown(KeyCode.Joystick1Button0)) { ConfirmDrop.Invoke(); }
            if (Input.GetKeyDown(KeyCode.Escape)) { escKeyDrop.Invoke(); }   
        }

        
    }
}


