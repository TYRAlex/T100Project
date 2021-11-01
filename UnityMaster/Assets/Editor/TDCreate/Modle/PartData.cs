using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDCreate
{
    [Serializable]
    public class PartData
    {
        [PropertyOrder(0)]
        [OnValueChanged("ChangePartType")]
        public PartType PartType;

        [HideInInspector] public string CourseName;
        [HideInInspector] public string PathName;




        public BassPart BassPart;

        public void ChangePartType()
        {
            switch (PartType)
            {
                case PartType.Null:
                    break;
                case PartType.KnowPartType:
                    BassPart = new KnowPart();
                    break;
                case PartType.CardPartType:
                    BassPart = new CardPart();
                    break;
                case PartType.SpritePartType:
                    BassPart = new SpritePart();
                    break;
                case PartType.GamePartType:
                    BassPart = new GamePart();
                    break;
                case PartType.SumUpPartType:
                    BassPart = new SumUpPart();
                    break;
            }
            BassPart.CourseName = CourseName;
            BassPart.PathName = PathName;
        }
    }
}
   
