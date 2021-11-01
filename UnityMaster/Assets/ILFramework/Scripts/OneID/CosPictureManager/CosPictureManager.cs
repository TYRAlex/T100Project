using System;
using System.Collections;
using System.Collections.Generic;
using ILFramework;
using UnityEngine;

namespace OneID
{
    public class CosPictureManager : MonoBehaviour
    {
        
        private GameObject _photograph;
        private GameObject _confirm;
        private GameObject _cancel;
        private OpenCamera _openCamera;
        private void Awake()
        {
            _photograph = this.transform.GetGameObject("Photograph");
            _confirm = this.transform.GetGameObject("Confirm");
            _cancel = this.transform.GetGameObject("Cancel");
            _openCamera = this.transform.GetTargetComponent<OpenCamera>("Camera");
            PointerClickListener.Get(_photograph).clickDown = ClickEvent;
            PointerClickListener.Get(_confirm).clickDown = ClickEvent;
            PointerClickListener.Get(_cancel).clickDown = ClickEvent;
        }

        private void OnEnable()
        {
            _photograph.Show();
            ShowConfirmOrCancelButton(false);
        }

        private void ClickEvent(GameObject go)
        {
            Debug.LogError("go:"+go.name);
            if (go.name.Equals("Photograph"))
            {
                OneIDSceneManager.Instance.PlayCommonSound(6);
                TakePhotoAni(()=>
                {
                    _photograph.Hide();
                    _openCamera.PauseTheWebcam();
                    ShowConfirmOrCancelButton(true);
                });
            }
            else if (go.name.Equals("Confirm"))
            {
                OneIDSceneManager.Instance.PlayCommonSound(3);
                SpineManager.instance.DoAnimation(_confirm.transform.GetGameObject("Spine"), "an4", false,
                    () => SpineManager.instance.DoAnimation(_confirm.transform.GetGameObject("Spine"), "an1", false,
                        () =>
                        {
                            ShowConfirmOrCancelButton(false);
                        }));



            }
            else if (go.name.Equals("Cancel"))
            {
                OneIDSceneManager.Instance.PlayCommonSound(3);
                SpineManager.instance.DoAnimation(_cancel.transform.GetGameObject("Spine"), "an6", false,
                    () => SpineManager.instance.DoAnimation(_cancel.transform.GetGameObject("Spine"), "an3", false, () =>
                    {
                        ShowConfirmOrCancelButton(false);
                        _photograph.Show();
                        _openCamera.PlayTheWebcam();
                    }));
                
            }
        }

        void ShowConfirmOrCancelButton(bool isShow)
        {
            _confirm.SetActive(isShow);
            _cancel.SetActive(isShow);
        }
        
        


        public void TakePhotoAni(Action callback)
        {
            SpineManager.instance.DoAnimation(_photograph.transform.GetGameObject("Spine"), "an5", false,
                () => SpineManager.instance.DoAnimation(_photograph.transform.GetGameObject("Spine"), "an2", false, () =>
                {
                    callback?.Invoke();
                }));
        }
    }
}