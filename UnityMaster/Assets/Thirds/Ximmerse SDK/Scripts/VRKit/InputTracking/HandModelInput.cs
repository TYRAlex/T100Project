//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using Ximmerse.VR;

namespace Ximmerse.InputSystem {

	public enum HandModelNode {
		Shoulder,
		Elbow,
		Wrist,
		Pointer,
	}

	[System.Serializable]
	public partial class HandModel:IInputTracking{

		public string name;
		public TrackedControllerInput Controller;
		public HandModelNode defaultNode=HandModelNode.Pointer;
		public ControllerType handedness;

		public HandModel(string name,ControllerType handedness) {
			this.name=name;
			this.handedness=handedness;
		}

		public virtual Vector3 GetHeadOrientation() {
            if (VRContext.currentDevice == null) return Vector3.zero;
			return VRContext.currentDevice.GetRotation()*Vector3.forward;
		}

		public virtual bool IsControllerRecentered() {
			// TODO : 
			return false;
		}

		public virtual bool Exists(int node) {
			return true;
		}

		public virtual Vector3 GetLocalPosition(int node) {
			if(node==-1) {
				node=(int)defaultNode;
			}
			//
			switch((HandModelNode)node) {
				case HandModelNode.Pointer:return pointerPosition;
				case HandModelNode.Wrist:return wristPosition;
				case HandModelNode.Elbow:return elbowPosition;
				case HandModelNode.Shoulder:return shoulderPosition;
			}
			//
			return Vector3.zero;
		}

		public virtual Quaternion GetLocalRotation(int node) {
			if(node==-1) {
				node=(int)defaultNode;
			}
			//
			switch((HandModelNode)node) {
				case HandModelNode.Pointer:return pointerRotation;
				case HandModelNode.Wrist:return wristRotation;
				case HandModelNode.Elbow:return elbowRotation;
				case HandModelNode.Shoulder:return shoulderRotation;
			}
			//
			return Quaternion.identity;
		}
	}

	public class HandModelInput:MonoBehaviour,IInputModule{

		#region Fields

		[SerializeField]protected HandModel[] m_Controllers=new HandModel[2]{
			new HandModel("LeftController",ControllerType.LeftController),
			new HandModel("RightController",ControllerType.RightController),
		};

		#endregion Fields

		#region Messages

		public virtual int InitInput() {
			//if((XDevicePlugin.GetInt(XDevicePlugin.ID_CONTEXT,XDevicePlugin.kField_CtxDeviceVersionInt,0)&0xF000)!=0x1000) {
			//	return -1;
			//}
			// Override ControllerInput in ControllerInputManager.
			if(true){
				//
				ControllerInputManager mgr=ControllerInputManager.instance;
				ControllerInput ci;
				if(mgr!=null) {
					for(int i=0,imax=m_Controllers.Length;i<imax;++i) {
						if(m_Controllers[i].handedness!=ControllerType.None) {
							ci=mgr.GetControllerInput(m_Controllers[i].name);
							if(ci is TrackedControllerInput) {
								m_Controllers[i].Controller=ci as TrackedControllerInput;
								m_Controllers[i].Controller.inputTracking=m_Controllers[i];
								m_Controllers[i].Controller.node=-1;
							}else {
								m_Controllers[i].Controller=new TrackedControllerInput(m_Controllers[i].name,m_Controllers[i],-1);
								mgr.AddControllerInput(m_Controllers[i].name,m_Controllers[i].Controller);
							}
							//
							m_Controllers[i].Start();
						}
					}
				}else {
					return -1;
				}
			}
			//
			return 0;
		}

		public virtual int UpdateInput() {
			for(int i=0,imax=m_Controllers.Length;i<imax;++i) {
				if(m_Controllers[i].handedness!=ControllerType.None) {
					m_Controllers[i].OnControllerUpdate();
				}
			}
			return 0;
		}

		public virtual int ExitInput() {
			for(int i=0,imax=m_Controllers.Length;i<imax;++i) {
				if(m_Controllers[i].handedness!=ControllerType.None) {
					m_Controllers[i].OnDestroy();
				}
			}
			return 0;
		}

		#endregion Messages

	}
}