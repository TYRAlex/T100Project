using ILFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CoursePortraitPart1
{
    public class AddScore
    {
        public Vector3 pos;
        GameObject addScore;

        public AddScore()
        {

        }

        public AddScore(Vector3 vec, GameObject addScore)
        {
            this.pos = vec;
            this.addScore = addScore;
            AddAnimation(pos);
        }

        void AddAnimation(Vector3 pos)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            addScore.SetActive(true);
            addScore.transform.position = pos;
        }       
    }
}
