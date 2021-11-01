using ILFramework;
using ILFramework.HotClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CourseStampsPart
{
    class tampChild
    {
        private GameObject parent;
        private Finish finish;
        private Score score;
        int index;
        List<int> data;
        Dictionary<int, List<GameObject>> BtnsDict;
        public tampChild(int index)
        {
            this.parent = CourseStampsPart1.curGo.transform.Find("Content").GetChild(index).gameObject;
            parent.gameObject.SetActive(true);
            this.index = index;
            data = new List<int>();
            BtnsDict = new Dictionary<int, List<GameObject>>();
            finish = new Finish();
            score = new Score();
            Init();
        }

        public void Init()
        {
            Transform trans = parent.transform.Find("right/AddClick");
            for (int i = 0; i < trans.childCount; i++)
            {
                trans.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
            trans = parent.transform.Find("left/AddClick");
            for (int i = 0; i < trans.childCount; i++)
            {
                trans.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
        }
        public void AddOnClick()
        {
            Transform trans = parent.transform.Find("right/AddClick");
            for(int i = 0;i<trans.childCount;i++)
            {
                int count = i;
                BtnsDict.Add(i, new List<GameObject>() { trans.GetChild(i).gameObject });
                trans.GetChild(i).GetComponent<Button>().onClick.AddListener(() => BtnOnClick(count));
            }
            trans = parent.transform.Find("left/AddClick");
            for (int i = 0; i < trans.childCount; i++)
            {
                int count = i;
                BtnsDict[i].Add(trans.GetChild(i).gameObject);
                trans.GetChild(i).GetComponent<Button>().onClick.AddListener(() => BtnOnClick(count));
            }
        }
        public void BtnOnClick(int index)
        {
            bool ison = false;
            for(int i = 0;i<data.Count;i++)
            {
                if(data[i] == index)
                {
                    ison = true;
                    break;
                }
            }
            if (!ison)
            {
                int num = Util.Random(1, 4);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, num);
                for (int i = 0; i < BtnsDict[index].Count; i++)
                {
                    BtnsDict[index][i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
                data.Add(index);
                bool isFinish = score.AddScore();
                if (isFinish)
                {
                    finish.ShowLizi();

                }
            }
        }
        public void ShowFinish()
        {
            finish.ShowFinish();
        }
    }
}
