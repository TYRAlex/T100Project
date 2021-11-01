using ILFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CourseTerracesPart
{
    class terracesBtn:baseBtn
    {
        private GameObject go;
        private GameObject light;
        public terracesBtn(GameObject go, Sprite sprite) : base(go,sprite)
        {
            this.curName = go.name;
            this.go = go;
            light = go.transform.GetChild(0).gameObject;
            this.name = (int.Parse(go.name) - 100).ToString();
            this.obj.index = int.Parse(go.name);
        }

        public override void BtnOnclick(Action action)
        {
            Vector3 v = go.transform.localPosition;
            go.transform.localPosition = new Vector3(v.x, v.y, -10);
            light.SetActive(true);
        }
        public void HideLight()
        {
            Vector3 v = go.transform.localPosition;
            go.transform.localPosition = new Vector3(v.x, v.y, 0);
            light.SetActive(false);
        }
        public override void ChooseImage(Sprite _sprite)
        {
            go.GetComponent<SpriteRenderer>().sprite = _sprite;
        }
    }
}
