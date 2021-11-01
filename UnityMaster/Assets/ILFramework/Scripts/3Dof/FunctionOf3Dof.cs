using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using UnityEngine;

namespace ILFramework
{


    public class FunctionOf3Dof : MonoBehaviour
    {
        public static FunctionOf3Dof Instance;

        public delegate void The3DofCallBack();

        public static  The3DofCallBack Current3DofCallBack;

        public static The3DofCallBack OldLuaClickDown;
        public static The3DofCallBack OldLuaClickStay;
        public static The3DofCallBack OldLuaClickUp;
        private bool _isClickOldLuaTriggerButton;
        private bool _isClickOldLuaAppButton;

        public static The3DofCallBack ClickAnyButtonDown;
        public static The3DofCallBack ClickAnyButtonStay;
        public static The3DofCallBack ClickAnyButtonUp;
        public LuaFunction LuaClickAnyButtonDown;
        public LuaFunction LuaClickAnyButtonStay;
        public LuaFunction LuaClickAnyButtonUp;
        private bool _isClickAnyButton = false;
        
        public static The3DofCallBack ClickOneButtonDown;
        public static The3DofCallBack ClickOneButtonStay;
        public static The3DofCallBack ClickOneButtonUp;
        public LuaFunction LuaClickOneButtonDown;
        public LuaFunction LuaClickOneButtonStay;
        public LuaFunction LuaClickOneButtonUp;
        private bool _isClickPadButton = false;
        private bool _isClickTriggerButton = false;
        private bool _isClickAppButton = false;
        private bool _isClickHomeButton = false;
        
        
        public LuaFunction CurrentLua3DofCallBack;

        public LuaFunction CurrentLuaEulerCallBack;

        public LuaFunction CurrentLuaPadPosCallBack;

        public LuaFunction CurrentLua3DofScreenPointCallBack;

        public LuaFunction CurrentLua3DofAccelerateValue;

        /// <summary>
        /// 数据类型
        /// </summary>
        public int Type;
        /// <summary>
        /// 欧拉角X
        /// </summary>
        private float _eulerAngleX = 0f;
        /// <summary>
        /// 欧拉角Y
        /// </summary>
        private float _eulerAngleY = 0f;
        /// <summary>
        /// 欧拉角Z
        /// </summary>
        private float _eulerAngleZ = 0f;
        /// <summary>
        /// 当前数据的编号/次数
        /// </summary>
        private int _times = 0;
        /// <summary>
        /// 背面的扳机是否按下255 表示按下 0表示未按下
        /// </summary>
        private bool _trigger = false;
        /// <summary>
        /// app按键三个点的哪个
        /// </summary>
        private bool _appButton = false;
        /// <summary>
        /// Home 按键圆圈的那个
        /// </summary>
        private bool _homeButton = false;
        /// <summary>
        /// 触摸板按键 最头上的那个
        /// </summary>
        private bool _padButton = false;
        /// <summary>
        /// 0：Idle 1：press 2:event 3:release
        /// Touch pad event 触摸板时间
        /// </summary>
        private int _touchPadEvent = 0;
        /// <summary>
        /// 触摸位置X
        /// </summary>
        private int _touchPadXPos = 0;
        /// <summary>
        /// 触摸位置Y
        /// </summary>
        private int _touchPadYPos = 0;
        //根据pad按键的状态和触摸板的位置信息 计算出下列按键类型
        /// <summary>
        /// 按下了触摸板的左侧
        /// </summary>
        private bool _padBtnLeft = false;
        /// <summary>
        /// 按下了触摸板的右侧
        /// </summary>
        private bool _padBtnRight = false;
        /// <summary>
        /// 按下了触摸板的上侧
        /// </summary>
        private bool _padBtnTop = false;
        /// <summary>
        /// 按下了触摸板的下侧
        /// </summary>
        private bool _padBtnBotton = false;
        /// <summary>
        /// 按下了触摸板的中间
        /// </summary>
        private bool _padBtnCenter = false;
        /// <summary>
        /// 欧拉角Y
        /// </summary>
        public float EulerAngleY => _eulerAngleY;
        /// <summary>
        /// 欧拉角Z
        /// </summary>
        public float EulerAngleZ => _eulerAngleZ;
        /// <summary>
        /// 欧拉角X
        /// </summary>
        public float EulerAngleX => _eulerAngleX;
        /// <summary>
        /// 当前数据的编号/次数
        /// </summary>
        public int Times => _times;
        /// <summary>
        /// 背面的扳机
        /// </summary>
        public bool Trigger => _trigger;
        /// <summary>
        /// App按键（三个点的那个）
        /// </summary>
        public bool AppButton => _appButton;
        /// <summary>
        /// Home按键
        /// </summary>
        public bool HomeButton => _homeButton;
        /// <summary>
        /// 触摸板按键
        /// </summary>
        public bool PadButton => _padButton;
        /// <summary>
        /// 触摸板事件 0：Idle 1：press 2:event 3:release
        /// </summary>
        public int TouchPadEvent => _touchPadEvent;
        /// <summary>
        /// 触摸位置X
        /// </summary>
        public int TouchPadXPos => _touchPadXPos;
        /// <summary>
        /// 触摸位置Y
        /// </summary>
        public int TouchPadYPos => _touchPadYPos;
        /// <summary>
        /// 触摸板左侧按键
        /// </summary>
        public bool PadBtnLeft => _padBtnLeft;
        /// <summary>
        /// 触摸板右侧按键
        /// </summary>
        public bool PadBtnRight => _padBtnRight;
        /// <summary>
        ///  触摸板上边按键
        /// </summary>
        public bool PadBtnTop => _padBtnTop;
        /// <summary>
        /// 触摸板下边按键
        /// </summary>
        public bool PadBtnBotton => _padBtnBotton;
        /// <summary>
        /// 触摸板中间按键
        /// </summary>
        public bool PadBtnCenter => _padBtnCenter;
        /// <summary>
        /// 是否打开遥控器
        /// </summary>
        public bool IsOpened = true;

        private Sim3dof _sim3dofObject;

        #region 加速度属性

        private float _lastPosX;
        private float _lastPosY;
        private float _lastPosZ;

        private float _acceX=0f;
        private float _acceY=0f;
        private float _acceZ=0f;


        #endregion


        /// <summary>
        /// 得到在遥控器的触摸板上的位置
        /// </summary>
        /// <returns></returns>
        public Vector2 GetTouchPadPositon()
        {
            // if (!IsOpened)
            //     return Vector2.zero;
            return new Vector2(_touchPadXPos, _touchPadYPos);
        }

        /// <summary>
        /// 得到遥控器的旋转角度的值
        /// </summary>
        /// <returns></returns>
        public Vector3 GetEulerAngle()
        {
            // if (!IsOpened)
            //     return Vector3.zero;

            return new Vector3(_eulerAngleX, _eulerAngleY, _eulerAngleZ);
        }

        public Vector3 GetAccelerateValue()
        {
            _acceX = _eulerAngleX - _lastPosX;
            _acceY = _eulerAngleY - _lastPosY;
            _acceY = _eulerAngleZ - _lastPosZ;
           
            _lastPosX = _eulerAngleX;
            _lastPosY = _eulerAngleY;
            _lastPosZ = _eulerAngleZ;
            return new Vector3(_acceX, _acceY, _acceZ);
        }

        public Vector3 GetScreenPoint()
        {
            float w = 16.5f;//y
            float v = 28f;//x
            var y = _eulerAngleX;
            var x = _eulerAngleY;
            if (_eulerAngleX >= v && _eulerAngleX <= 180) y = v;
            if (_eulerAngleX<= 360 - v && _eulerAngleX> 180) y = 360 - v;
            if (_eulerAngleY <= 360 - w && _eulerAngleY > 180) x = 360 - w;
            if (_eulerAngleY>= w && _eulerAngleY <= 180) x = w;
            _sim3dofObject.origin.transform.rotation = Quaternion.Euler(new Vector3(-x, y, _eulerAngleZ));
            //Debug.Log("3DofRotation:"+new Vector3(-x, y, _eulerAngleZ)+" mousePoint:"+_sim3dofObject.mousePoint+" 遥控器的数值:"+GetEulerAngle());
            return _sim3dofObject.mousePoint;
        }


        // public Text InfoPrint;

        void Awake()
        {
            Instance = this.GetComponent<FunctionOf3Dof>();
            _sim3dofObject = FindObjectOfType<Sim3dof>();
            _lastPosX = _eulerAngleX;
            _lastPosY = _eulerAngleY;
            _lastPosZ = _eulerAngleZ;
        }

        private void Start()
        {
            _isClickAnyButton = false;
            _isClickPadButton = false;
            _isClickTriggerButton = false;
            _isClickAppButton = false;
            _isClickHomeButton = false;
            _isClickOldLuaAppButton = false;
            _isClickOldLuaTriggerButton = false;
        }

        public void StartOldLuaCLickFunc()
        {
            OldLuaClickDown = _sim3dofObject.ClickDownFunc;
            OldLuaClickStay = _sim3dofObject.ClickStayFunc;
            OldLuaClickUp = _sim3dofObject.ClickUpFunc;
        }

        void SetOldLuaCLickEventNull()
        {
            OldLuaClickDown = null;
            OldLuaClickStay = null;
            OldLuaClickUp = null;
        }

        public void ResetAllDelegate()
        {
            Current3DofCallBack = null;
            CurrentLua3DofCallBack = null;
            CurrentLuaEulerCallBack = null;
            CurrentLuaPadPosCallBack = null;
            CurrentLua3DofScreenPointCallBack = null;
            CurrentLua3DofAccelerateValue = null;
            ClickAnyButtonStay = null;
            ClickAnyButtonDown = null;
            ClickAnyButtonUp = null;
            ClickOneButtonDown = null;
            ClickOneButtonStay = null;
            ClickOneButtonUp = null;
            LuaClickAnyButtonDown = null;
            LuaClickAnyButtonStay = null;
            LuaClickAnyButtonUp = null;
            LuaClickOneButtonDown = null;
            LuaClickOneButtonStay = null;
            LuaClickOneButtonUp = null;
            SetOldLuaCLickEventNull();
        }

        


        public void TransInfoToButton(int[] data)
        {
            // string msg = "";
            // for (int i = 0; i < data.Length; i++)
            // {
            //     msg += (data[i] & 0xFF).ToString();
            // }
            if (!IsOpened)
                return;

            if (data.Length != 20)
            {
                return;
            }

            this.Type = data[0] & 0xFF;
            if (Type != 0x01)
            {
                print("_3DOFDataBase, 数据类型不对，type应该是0x01");
                return;
            }
            
            //欧拉角
            this._eulerAngleX = (float) (data[2] * 0x10000 + (data[3] & 0xFF) * 0x100 + (data[4] & 0xFF)) / 10000;
            this._eulerAngleY = (float) (data[5] * 0x10000 + (data[6] & 0xFF) * 0x100 + (data[7] & 0xFF)) / 10000;
            this._eulerAngleZ = (float) (data[8] * 0x10000 + (data[9] & 0xFF) * 0x100 + (data[10] & 0xFF)) / 10000;
            //当前数据的编号/次数
            this._times = data[11] * 0x10000 + (data[12] & 0xFF) * 0x100 + (data[13] & 0xFF);
            //背面的扳机是否按下 255  表示按下  0表示未按下
            this._trigger = data[14] != 0;

            //按键
            //app按键三个点的那个
            this._appButton = (data[15] & 0x10) != 0;
            //home按键圆圈的那个
            this._homeButton = (data[15] & 0x08) != 0;
            //触摸板按键 最头上的那个
            this._padButton = (data[15] & 0x04) != 0;

            //touchPad  event  触摸板事件
            // 0:Idle  1:press 2:event 3:release
            this._touchPadEvent = data[16] & 0xC0 / 0x40;
            this._touchPadXPos = (data[16] & 0x0F) * 0x100 + (data[17] & 0xFF);
            this._touchPadYPos = (data[18] & 0x0F) * 0x100 + (data[19] & 0xFF);

            //如果触摸板被按下  则根据位置计算是按下状态
            if (PadButton)
            {
                if (TouchPadXPos < 200)
                {
                    this._padBtnLeft = true;
                }
                else if (TouchPadXPos > 800)
                {
                    this._padBtnRight = true;
                }
                else if (TouchPadYPos < 200)
                {
                    this._padBtnTop = true;
                }
                else if (TouchPadYPos > 800)
                {
                    this._padBtnBotton = true;
                }
                else if (TouchPadXPos > 400 && TouchPadXPos < 600 & TouchPadYPos > 400 && TouchPadYPos < 600)
                {
                    this._padBtnCenter = true;
                }
            }
            else
            {
                _padBtnLeft = false;
                _padBtnRight = false;
                _padBtnTop = false;
                _padBtnBotton = false;
                _padBtnCenter = false; 
            }

            Current3DofCallBack?.Invoke();
            if (CurrentLua3DofCallBack != null)
                CurrentLua3DofCallBack.Call();

            if (CurrentLuaEulerCallBack != null)
                CurrentLuaEulerCallBack.Call(EulerAngleX, EulerAngleY, EulerAngleZ);

            if (CurrentLuaPadPosCallBack != null)
                CurrentLuaPadPosCallBack.Call(_touchPadXPos, _touchPadYPos);

            if(CurrentLua3DofScreenPointCallBack!=null)
                CurrentLua3DofScreenPointCallBack.Call(GetScreenPoint());
            
            if (CurrentLua3DofAccelerateValue != null)
            {
                _acceX = _eulerAngleX - _lastPosX;
                _acceY = _eulerAngleY - _lastPosY;
                _acceY = _eulerAngleZ - _lastPosZ;
                CurrentLua3DofAccelerateValue.Call(_acceX, _acceY, _acceZ);
                _lastPosX = _eulerAngleX;
                _lastPosY = _eulerAngleY;
                _lastPosZ = _eulerAngleZ;
            }
            
            if (!_isClickAnyButton&&(_padButton || _trigger || _appButton || _homeButton))
            {
                _isClickAnyButton = true;
                
                ClickAnyButtonDown?.Invoke();
                if(LuaClickAnyButtonDown!=null) LuaClickAnyButtonDown.Call();
            }
            else if(_isClickAnyButton&&(_padButton || _trigger || _appButton || _homeButton))
            {
                ClickAnyButtonStay?.Invoke();
                if(LuaClickAnyButtonStay!=null) LuaClickAnyButtonStay.Call();
            }
            else if (_isClickAnyButton && (!_padButton || !_trigger || !_appButton || !_homeButton))
            {
                _isClickAnyButton = false;
                ClickAnyButtonUp?.Invoke();
                if (LuaClickAnyButtonUp != null) LuaClickAnyButtonUp.Call();
            }

            if (!_isClickAppButton && _appButton || (!_isClickHomeButton && _homeButton) ||
                (!_isClickPadButton && _padButton) || (!_isClickTriggerButton && _trigger))
            {
                if (_appButton)
                {
                    _isClickAppButton = true;
                }
                else if (_homeButton)
                {
                    _isClickHomeButton = true;
                }
                else if (_isClickPadButton)
                {
                    _isClickPadButton = true;
                }
                else if (_trigger)
                {
                    _isClickTriggerButton = true;
                }
                ClickOneButtonDown?.Invoke();
                if(LuaClickOneButtonDown!=null)
                    LuaClickOneButtonDown.Call();
            }
            else if (_isClickAppButton && _appButton || (_isClickHomeButton && _homeButton) ||
                     (_isClickPadButton && _padButton) || (_isClickTriggerButton && _trigger))
            {
                ClickOneButtonStay?.Invoke();
                if(LuaClickOneButtonStay!=null)
                    LuaClickOneButtonStay.Call();
            }
            else if (_isClickAppButton && !_appButton || (_isClickHomeButton && !_homeButton) ||
                     (_isClickPadButton && !_padButton) || (_isClickTriggerButton && !_trigger))
            {
                if (!_appButton)
                {
                    _isClickAppButton = false;
                }
                if (!_homeButton)
                {
                    _isClickHomeButton = false;
                }
                if (!_isClickPadButton)
                {
                    _isClickPadButton = false;
                }
                if (!_trigger)
                {
                    _isClickTriggerButton = false;
                }
                ClickOneButtonUp?.Invoke();
                if(LuaClickOneButtonUp!=null)
                    LuaClickOneButtonUp.Call();
            }
            
            
            if (!_isClickOldLuaAppButton && _appButton ||(!_isClickOldLuaTriggerButton && _trigger))
            {
                if (_appButton)
                {
                    _isClickOldLuaAppButton = true;
                }
                else if (_trigger)
                {
                    _isClickOldLuaTriggerButton = true;
                }
                OldLuaClickDown?.Invoke();
            }
            else if (_isClickOldLuaAppButton && _appButton || (_isClickOldLuaTriggerButton && _trigger))
            {
                OldLuaClickStay?.Invoke();
            }
            else if ((_isClickOldLuaAppButton && !_appButton) || (_isClickOldLuaTriggerButton && !_trigger))
            {
                if (!_appButton)
                {
                    _isClickOldLuaAppButton = false;
                }
                if (!_trigger)
                {
                    _isClickOldLuaTriggerButton = false;
                }
                OldLuaClickUp?.Invoke();
            }
            
            

            // InfoPrint.text =
            //     string.Format(
            //         "Type:{0} EulerAngleX:{1} EulerAngleY:{2} EulerAngleZ:{3} Times:{4} Trigger:{5} AppButton:{6} HomeButton:{7} PadButton{8} TouchPadEvent{9} TouchPadXPos{10} TouchPadYPos{11} PadBtnLeft{12} PadBtnRight{13} PadBtnTop{14} PadBtnBottom{15} PadBtnCenter{16} ",
            //         Type, this.EulerAngleX, this.EulerAngleY, this.EulerAngleZ, Times, Trigger, AppButton, HomeButton,
            //         PadButton, TouchPadEvent, TouchPadXPos, TouchPadYPos, PadBtnLeft, PadBtnRight, PadBtnTop,
            //         PadBtnBottom, PadBtnCenter);
            // Debug.Log("欧拉角 X:" + this._eulerAngleX + " Y:" + this._eulerAngleY + " Z:" + this._eulerAngleZ + " 数据编号：" + Times +
            //       " 背面的扳机" + Trigger + " App按键" + AppButton+" Home按键"+HomeButton+" 触摸板"+PadButton+" ");
        }
    }
}