using System.Collections;
using LuaInterface;
using UnityEngine;
using Spine;
using Spine.Unity;
using System;
using UnityEngine.UI;
using Spine.Unity.Modules.AttachmentTools;


namespace ILFramework
{
    public class SpineManager : Manager<SpineManager>
    {
        public Spine.AnimationState getAnimationState(GameObject gameobject)
        {
            var skeletonAnimation = gameobject.GetComponent<SkeletonAnimation>();
            var skeletonGraphic = gameobject.GetComponent<SkeletonGraphic>();
            Spine.AnimationState spineAnimationState = null;
            if (skeletonAnimation)
            {
                spineAnimationState = skeletonAnimation.AnimationState;
            }
            if (skeletonGraphic)
            {
                spineAnimationState = skeletonGraphic.AnimationState;
            }
            return spineAnimationState;
        }

        /// <summary>
        /// 获取动画的长度,在不播放动画的时候获取
        /// </summary>
        /// <param name="gameobject"></param>
        /// <param name="animationName"></param>
        /// <returns></returns>
        public float GetAnimationLength(GameObject gameobject, string animationName)
        {
            float tmptime = 0;
            if (gameObject == null) return tmptime;
            Spine.AnimationState spineAnimationState = getAnimationState(gameobject);
            if (spineAnimationState != null)
                tmptime = spineAnimationState.Data.skeletonData.FindAnimation(animationName).duration;
            return tmptime;
        }

        public String GetCurrentAnimationName(GameObject gameobject)
        {
            if (gameObject == null) return null;
            Spine.AnimationState spineAnimationState = getAnimationState(gameobject);
            if (spineAnimationState != null)
                return spineAnimationState.GetCurrent(0).Animation.name;
            return null;
        }

        public float DoAnimation(GameObject gameobject, string animationName, bool isLoop = true, Action callback = null)
        {
            float timeLine = 0;
            if (gameObject == null) return timeLine;
            Spine.AnimationState spineAnimationState = getAnimationState(gameobject);
            if (spineAnimationState != null)
            {
                //StartCoroutine(DoRoutine(spineAnimationState, animationName, isLoop));
                //Debug.LogWarningFormat("打印当前obj:{0}", gameobject.name);

                var skgrap = gameobject.GetComponent<SkeletonGraphic>();
                if (skgrap != null)
                    skgrap.startingAnimation = null;
                var skeleton = gameobject.GetComponent<SkeletonAnimation>();
                if (skeleton != null)
                    skeleton.AnimationName = null;

                var track = spineAnimationState.SetAnimation(0, animationName, isLoop);

                timeLine = spineAnimationState.Data.skeletonData.FindAnimation(animationName).duration;
                track.Complete += (TrackEntry trackEntry) =>
                {
                    callback?.Invoke();
                };
            }
            return timeLine;
        }

        // 兼容1.0
        public float DoAnimationLua(GameObject gameobject, string animationName, bool isLoop = true, LuaFunction luaCallback = null)
        {
            float timeLine = 0;
            if (gameObject == null) return timeLine;
            Spine.AnimationState spineAnimationState = getAnimationState(gameobject);
            if (spineAnimationState != null)
            {
                var skgrap = gameobject.GetComponent<SkeletonGraphic>();
                if (skgrap != null)
                    skgrap.startingAnimation = null;
                var skeleton = gameobject.GetComponent<SkeletonAnimation>();
                if (skeleton != null)
                    skeleton.AnimationName = null;

                var track = spineAnimationState.SetAnimation(0, animationName, isLoop);

                timeLine = spineAnimationState.Data.skeletonData.FindAnimation(animationName).duration;
                track.Complete += (TrackEntry trackEntry) =>
                {
                    if (luaCallback != null) luaCallback.Call<GameObject>(gameobject);
                };
            }
            return timeLine;
        }

        IEnumerator DoRoutine(Spine.AnimationState spineAnimationState, string animationName, bool isLoop)
        {
            spineAnimationState.SetAnimation(0, animationName, isLoop);
            yield return new WaitForSeconds(1f);
        }

        public void ClearTrack(GameObject gameobject, int Track = 0)
        {
            Spine.AnimationState spineAnimationState = getAnimationState(gameobject);
            var skeletonGraphic = gameobject.GetComponent<SkeletonGraphic>();

            if (skeletonGraphic)
            {
                skeletonGraphic.SetAllDirty();
                skeletonGraphic.Skeleton.SetBonesToSetupPose();
            }

            var skeletonAnimation = gameobject.GetComponent<SkeletonAnimation>();

            if (skeletonAnimation)
            {
                skeletonAnimation.skeleton.SetBonesToSetupPose();
            }
            if (spineAnimationState != null)
            {
                spineAnimationState.ClearTrack(Track);
                spineAnimationState.Update(0);
            }
        }

        public void SetFreeze(GameObject gameobject, bool isFreeze)
        {
            if (gameObject == null) return;
            var skeletonGraphic = gameobject.GetComponent<SkeletonGraphic>();
            if (skeletonGraphic)
            {
                skeletonGraphic.freeze = isFreeze;
            }
        }

        public void PlayAnimationState(SkeletonAnimation ske, string name, string time = "0|0")
        {
            DoAnimation(ske.gameObject, name);
            PlayAnimationDuring(ske.gameObject, name, time);
        }
        public void PlayAnimationState(SkeletonGraphic ske, string name, string time = "0|0")
        {
            DoAnimation(ske.gameObject, name);
            PlayAnimationDuring(ske.gameObject, name, time);
        }

        public void PlayAnimationDuring(GameObject gameobject, string animationName, string time, bool reverse = false)
        {
            string[] times = time.Split('|');
            float startTime = Convert.ToSingle(times[0]);
            float endTime = Convert.ToSingle(times[1]);
            if (gameObject == null) return;
            Spine.AnimationState spineAnimationState = getAnimationState(gameobject);
            if (spineAnimationState != null)
            {
                spineAnimationState.SetAnimation(0, animationName, false);
                var track = spineAnimationState.GetCurrent(0);
                track.AnimationStart = startTime;
                track.AnimationEnd = endTime;
                track.TimeScale = reverse ? -1 : 1;
                //spineAnimationState.SetAnimation(0, animationName, false);
            }
        }

        public void SetTimeScale(GameObject go, float time)
        {
            if (go != null)
            {
                SkeletonGraphic sk = go.GetComponent<SkeletonGraphic>();
                if (sk != null)
                    sk.timeScale = time;
            }
        }

        public void CreateRegionAttachmentByTexture(SkeletonAnimation _skeletonGraphic, string slotName, Sprite sprite, Shader curShader)
        {
            Spine.Slot slot = _skeletonGraphic.Skeleton.FindSlot(slotName);
            Spine.RegionAttachment newWeapon = sprite.ToRegionAttachmentPMAClone(curShader);
            slot.Attachment = newWeapon;
        }
        public void HideSpineTexture(SkeletonAnimation _skeletonGraphic, string slotName)
        {
            Spine.Slot slot = _skeletonGraphic.Skeleton.FindSlot(slotName);
            Spine.RegionAttachment attachment = slot.Attachment as Spine.RegionAttachment;
            if(attachment != null)
            {
                attachment.A = 0;
                slot.Attachment = attachment;
            }
        }
        public void HideSpineTexture(SkeletonGraphic _skeletonGraphic, string slotName)
        {
            Spine.Slot slot = _skeletonGraphic.Skeleton.FindSlot(slotName);
            Spine.RegionAttachment attachment = slot.Attachment as Spine.RegionAttachment;
            if (attachment != null)
            {
                attachment.A = 0;
                slot.Attachment = attachment;
            }
        }
        public void ShowSpineTexture(SkeletonAnimation _skeletonGraphic, string slotName)
        {
            Spine.Slot slot = _skeletonGraphic.Skeleton.FindSlot(slotName);
            Spine.RegionAttachment attachment = slot.Attachment as Spine.RegionAttachment;
            attachment.A = 1;
            slot.Attachment = attachment;
        }
        public void CreateRegionAttachmentByTexture(AtlasAsset atlasAsset, string spriteName, GameObject curGo, GameObject target, Shader curShader)
        {
            Spine.RegionAttachment newHand = atlasAsset.GetAtlas().FindRegion(spriteName).ToRegionAttachment(spriteName);
            Material mat = new Material(curShader);
            mat.mainTexture = newHand.GetMaterial().mainTexture;
            (newHand.RendererObject as Spine.AtlasRegion).page.rendererObject = mat;
        }
        public void DressUp(Spine.Skeleton skeleton, Spine.Skin skin, Sprite sprite, string slotName, string attachmentName, Shader shader)
        {
            Spine.RegionAttachment newWeapon = sprite.ToRegionAttachmentPMAClone(shader);
            newWeapon.SetScale(1.0f, 1.0f);
            newWeapon.UpdateOffset();
            int weaponSlotIndex = skeleton.FindSlotIndex(slotName);
            skin.AddAttachment(weaponSlotIndex, attachmentName, newWeapon);

            skeleton.SetSkin(skin);
            skeleton.SetToSetupPose();

            Resources.UnloadUnusedAssets();
        }
        public void DressUp(Spine.Skeleton skeleton, Spine.Skin skin, AtlasAsset atlasAsset, string spriteName, string attachmentName, string slotName)
        {
            Spine.RegionAttachment newHand = atlasAsset.GetAtlas().FindRegion(spriteName).ToRegionAttachment(spriteName);
            newHand.SetPositionOffset(Vector3.zero);
            newHand.Rotation = 0;
            newHand.UpdateOffset();
            int handSlotIndex = skeleton.FindSlotIndex(slotName);
            skin.AddAttachment(handSlotIndex, attachmentName, newHand);

            skeleton.SetSkin(skin);
            skeleton.SetToSetupPose();

            Resources.UnloadUnusedAssets();
        }
        public void Refresh(ref Material material, Shader curshader)
        {
            Material mat = new Material(curshader);
            mat.mainTexture = material.mainTexture;
            material = mat;
        }
    }
}
