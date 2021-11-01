using System.Collections;
using System.Collections.Generic;
using ILFramework;
using UnityEngine;

namespace OneID
{
    public class GetTicketManager : MonoBehaviour
    {
        private Transform _boxexPanel;

        
        void Awake()
        {
            
            
        }

      
        void Start()
        {
            ResetBoxes();

        }

        void ResetBoxes()
        {
            _boxexPanel = this.transform.GetTransform("Boxes");
            for (int i = 0; i < _boxexPanel.childCount; i++)
            {
                GameObject targetBox = _boxexPanel.GetChild(i).gameObject;
                GameObject boxSpine = targetBox.transform.GetGameObject(targetBox.name);
                //print(targetBox.name+ " :"+boxSpine.name);
                SpineManager.instance.DoAnimation(boxSpine, "xz1", false); 
                PointerClickListener.Get(targetBox).clickDown = ClickBoxEvent;
            }
        }

        private void ClickBoxEvent(GameObject go)
        {
            OneIDSceneManager.Instance.PlayCommonSound(11);
            int randomNumber = Random.Range(2, 5);
            GameObject targetSpine = go.transform.GetGameObject(go.name);
            SpineManager.instance.DoAnimation(targetSpine, "xz" + randomNumber, false, () =>
            {
                SpineManager.instance.DoAnimation(targetSpine, "xz" + randomNumber + "a", true);
            });
        }
    }
}