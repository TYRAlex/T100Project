using ILFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CoursePortraitPart1
{
    public class Painting
    {
        public int index;
        public int score;
        public float speed;
        public int isPortrait;
        public bool isEnd;
        public GameObject painting, father;
        public GameObject image, ani;
        public Vector3 aniScale;

        enum PAINTING_SPINE
        {
            Error = 0, Error2 = 1, Error3 = 2, Error4 = 3, Right1 = 4
        }
        string[] paintingSpine;

        public Painting(GameObject painting, int score, float speed)
        {
            string[] str = painting.name.Split('_');
            index = Convert.ToInt32(str[1]) - 1;
            this.painting = painting;
            this.score = score;
            this.speed = speed;
            father = painting.transform.parent.gameObject;
            image = painting.transform.GetChild(0).gameObject;
            ani = painting.transform.GetChild(1).gameObject;
            paintingSpine = new string[] { "erro", "erro2", "erro3", "erro4", "right1" };
            isPortrait = 0;
            isEnd = false;
            painting.SetActive(false);
            image.SetActive(true);
            ani.SetActive(true);
            aniScale = ani.transform.localScale;
            ani.transform.localScale = Vector3.zero;
            Util.AddBtnClick(image, Click);
        }

        public void Move()
        {
            painting.transform.Translate(Vector3.down * speed);
            if (painting.transform.position.y < -400)
            {
                isEnd = true;
                painting.transform.SetParent(father.transform);
                painting.transform.localPosition = Vector3.zero;
            }
        }

        void Click(GameObject image)
        {


            if (index == 4)
            {
                isPortrait = 2;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
            }
            else
            {
                isPortrait = 1;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);
            }
            image.SetActive(false);
            //ani.SetActive(true);
            ani.transform.localScale = aniScale;
            SpineManager.instance.DoAnimation(ani, paintingSpine[index], false, EndClick);            
        }

        void EndClick()
        {
            if (index == 4)
            {
                if (father != null)
                {
                    painting.transform.SetParent(father.transform);
                }                
                image.SetActive(true);
                painting.transform.localPosition = Vector3.zero;
                ani.transform.localScale = Vector3.zero;
                isPortrait = 0;
            }
            else
            {
                //ani.SetActive(false);
                ani.transform.localScale = Vector3.zero;
                image.SetActive(true);
                isPortrait = 0;
                Debug.Log(image.name);
            }
        }
    }
}
