using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;


namespace ILFramework.HotClass
{
    public class Man
    {
        public string HatType;
        public string PlantsType;
        public string ClothesType;
        public string ShoesType;
        public string OrnType;

        public Man(string hatType, string plantsType, string clothesType, string shoesType, string ornType)
        {
            this.HatType = hatType;
            this.PlantsType = plantsType;
            this.ClothesType = clothesType;
            this.ShoesType = shoesType;
            this.OrnType = ornType;
        }
    }

    public class CourseTDG1P1L10Part1
    {
        GameObject curGo;
        Transform Buttom;
        Transform Man;
        GameObject Npc;
        //大裤衩
        GameObject Panties;
        Transform Things;
        Transform ClothingBtns;
        Transform Wardrobe;
        GameObject Cover;
        GameObject CoverMask;
        Button AgianBtn;
        Button ScreenBtn;
        //展示部分
        Man SingleConfigOfMan;
        List<Man> ConfigsOfMan;
        GameObject ShowMan;
        GameObject Model;
        GameObject TouchMask;
        GameObject HindAnim;
        GameObject CoolAnim;
        GameObject CoolIdleAnim;

        //modelIcon部分
        Transform SmallModels;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            Buttom = curTrans.Find("Content/Buttom");
            Man = Buttom.Find("Man");
            Npc = curTrans.Find("Content/NPC").gameObject;
            Panties = Man.Find("Body/Panties").gameObject;
            Things = Buttom.Find("Things");
            ClothingBtns = Things.Find("ClothingBtns");
            Wardrobe = Things.Find("Wardrobe");
            Cover = Things.Find("Cover").gameObject;

            CoverMask = curTrans.Find("Content/CoverMask").gameObject;
            AgianBtn = Buttom.Find("OtherBtns/AgianBtn").GetComponent<Button>();
            ScreenBtn = Buttom.Find("OtherBtns/ScreenBtn").GetComponent<Button>();
            ConfigsOfMan = new List<Man>();
            ShowMan = curTrans.Find("Content/ShowMan").gameObject;
            Model = ShowMan.transform.Find("Model").gameObject;
            TouchMask = ShowMan.transform.Find("TouchMask").gameObject;
            HindAnim = ShowMan.transform.Find("HindAnim").gameObject;
            CoolAnim = ShowMan.transform.Find("CoolAnim").gameObject;
            CoolIdleAnim = ShowMan.transform.Find("CoolIdleAnim").gameObject;
            SmallModels = Buttom.Find("SmallModels");
            InitGame();
        }

        void InitGame()
        {
            //添加btn回调
            AgianBtn.onClick.AddListener(AgianCallback);
            ScreenBtn.onClick.AddListener(ScreenCallback);
            ScreenBtn.gameObject.SetActive(false);
            InitClothingBtns();
            InitClothesBtn();
            Panties.SetActive(true);

            SingleConfigOfMan = new Man("", "", "", "", "");
            ShowMan.SetActive(false);
            Model.SetActive(false);
            TouchMask.SetActive(false);
            HindAnim.SetActive(false);
            CoolAnim.SetActive(false);
            CoolIdleAnim.SetActive(false);
            TouchMask.GetComponent<ILObject3DAction>().OnPointDownLua = CloseShowMan;
            ConfigsOfMan.Clear();
            ResetModelIcons();
            Resuitup();


            CoverMask.SetActive(true);
            SoundManager.instance.BgSoundPart2(SoundManager.SoundType.BGM, 0.5f);
            SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, 0, null, () =>
            {
                CoverMask.SetActive(false);
            });
        }


        void InitClothingBtns()
        {
            for (int i = 0; i < ClothingBtns.childCount; i++)
            {
                var btn = ClothingBtns.GetChild(i).gameObject;
                var pointer = btn.GetComponent<ILObject3DAction>();
                pointer.index = i;
                pointer.OnPointDownLua = PointDownClothingBtn;
                btn.transform.Find("Normal").gameObject.SetActive(true);
                btn.transform.Find("Light").gameObject.SetActive(false);
                btn.transform.Find("Anim").gameObject.SetActive(false);
            }
        }

        void InitClothesBtn()
        {
            for (int i = 0; i < Wardrobe.childCount; i++)
            {
                int countOfChild = Wardrobe.GetChild(i).childCount;
                var curClothes = Wardrobe.GetChild(i);
                List<Transform> clotherArray = new List<Transform>();
                for (int j = 0; j < countOfChild; j++)
                {
                    var clothes = curClothes.GetChild(j);
                    clotherArray.Add(clothes);
                    var action = clothes.GetComponent<ILObject3DAction>();
                    action.index = (i + 1) * 10 + j;
                    action.OnPointDownLua = DownClothes;
                    clothes.Find("Normal").gameObject.SetActive(true);
                    clothes.Find("Light").gameObject.SetActive(false);
                    clothes.Find("Anim").gameObject.SetActive(false);

                }
            }
        }

        #region 每个具体衣服的按钮回调
        GameObject lastSelectClothes = null;

        void SetLastSelectClothes()
        {
            if (lastSelectClothes != null)
            {
                var lastNormal = lastSelectClothes.transform.Find("Normal").gameObject;
                var lastAnim = lastSelectClothes.transform.Find("Anim").gameObject;
                var lastLight = lastSelectClothes.transform.Find("Light").gameObject;
                lastNormal.SetActive(true);
                lastAnim.SetActive(false);
                lastLight.SetActive(false);
            }
        }

        private void DownClothes(int index)
        {
            SetLastSelectClothes();

            int i = (index / 10) - 1;
            int j = index % 10;
            var clothes = Wardrobe.GetChild(i).GetChild(j).gameObject;
            var normal = clothes.transform.Find("Normal").gameObject;
            var anim = clothes.transform.Find("Anim").gameObject;
            var light = clothes.transform.Find("Light").gameObject;
            normal.SetActive(false);
            anim.SetActive(true);
            lastSelectClothes = clothes;

            if (i == 1 || i == 2)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
            }



            //穿上衣服
            string clothesType = Wardrobe.GetChild(i).name;
            string clothesName = Wardrobe.GetChild(i).GetChild(j).name;
            SuitUp(clothesType, clothesName);
            //设置选衣服配置
            SetConfigOfMan(clothesType, clothesName);
            CoverMask.SetActive(true);
            SpineManager.instance.DoAnimation(anim, "UI_" + clothes.name, false, () =>
            {
                CoverMask.SetActive(false);
                anim.SetActive(false);
                normal.SetActive(false);
                light.SetActive(true);
            });
        }
        #endregion

        #region 每个侧边选项卡的回调
        GameObject lastClothingBtn;

        void SetlastClothingBtn()
        {
            if (lastClothingBtn != null)
            {
                var lastNormal = lastClothingBtn.transform.Find("Normal").gameObject;
                var lastAnim = lastClothingBtn.transform.Find("Anim").gameObject;
                var lastLight = lastClothingBtn.transform.Find("Light").gameObject;
                lastNormal.SetActive(true);
                lastAnim.SetActive(false);
                lastLight.SetActive(false);

                SetLastSelectClothes();
            }
        }

        private void PointDownClothingBtn(int index)
        {
            SetlastClothingBtn();

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
            var btn = ClothingBtns.GetChild(index);
            var anim = btn.Find("Anim").gameObject;
            var normal = btn.Find("Normal").gameObject;
            var light = btn.Find("Light").gameObject;
            anim.SetActive(true);
            normal.SetActive(false);
            CoverMask.SetActive(true);
            lastClothingBtn = btn.gameObject;
            SpineManager.instance.DoAnimation(anim, btn.name, false, () =>
            {
                CoverMask.SetActive(false);
                anim.SetActive(false);
                normal.SetActive(false);
                light.SetActive(true);
            });
            //显示不同的衣物组合
            ShowClothes(index);
        }
        #endregion


        //截屏
        private void ScreenCallback()
        {
            Debug.Log("截屏");
            if (ConfigsOfMan.Count >= 8)
            {
                ConfigsOfMan.RemoveAt(0);
            }
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
            ConfigsOfMan.Add(SingleConfigOfMan);


            SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, 1, () =>
            {
                CoverMask.SetActive(true);
            }, () =>
            {
                // CoverMask.SetActive(false);

                // ShowMan.SetActive(true);
                // HindAnim.SetActive(true);
                // CoolAnim.SetActive(true);

                // Model.SetActive(true);
                // SetModelAccordingConfig();

                // SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                // SpineManager.instance.DoAnimation(HindAnim, "animation", false);
                // SpineManager.instance.DoAnimation(CoolAnim, "animation_cool", false, () =>
                // {
                //     HindAnim.SetActive(false);
                //     CoolAnim.SetActive(false);
                //     CoolIdleAnim.SetActive(true);
                //     SpineManager.instance.DoAnimation(CoolIdleAnim, "idle");
                //     TouchMask.SetActive(true);
                // });
            });
          

            ShowMan.SetActive(true);
            HindAnim.SetActive(true);
            CoolAnim.SetActive(true);

            Model.SetActive(true);
            SetModelAccordingConfig();

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
            SpineManager.instance.DoAnimation(HindAnim, "animation", false);
            SpineManager.instance.DoAnimation(CoolAnim, "animation_cool", false, () =>
            {
                
                CoverMask.SetActive(false);
                HindAnim.SetActive(false);
                CoolAnim.SetActive(false);
                CoolIdleAnim.SetActive(true);
                SpineManager.instance.DoAnimation(CoolIdleAnim, "idle");
                TouchMask.SetActive(true);
            });


        }

        // 重玩游戏
        private void AgianCallback()
        {
            Debug.Log("脱光");
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
            Resuitup();
        }

        void ShowClothes(int index)
        {
            Cover.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
            for (int i = 0; i < Wardrobe.childCount; i++)
            {
                Wardrobe.GetChild(i).gameObject.SetActive(false);
            }
            Wardrobe.GetChild(index).gameObject.SetActive(true);
        }

        void SuitUp(string clothesType, string clothesName)
        {
            // Debug.LogError(clothesType + "--------" + clothesName);
            Transform clothesParent = Man.Find(clothesType);
            for (int i = 0; i < clothesParent.childCount; i++)
            {
                clothesParent.GetChild(i).gameObject.SetActive(false);
            }
            //换裤子的时候隐藏大裤衩
            if (clothesType.Equals("plants"))
            {
                Panties.SetActive(false);
            }
            CoverMask.SetActive(true);
            clothesParent.Find(clothesName).gameObject.SetActive(true);

            if(clothesType.Equals("hat"))
            {
                int randomIndex = UnityEngine.Random.Range(6,8);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,randomIndex);
            }

            SpineManager.instance.DoAnimation(Man.gameObject, "animation2", false, () =>
            {
                SpineManager.instance.DoAnimation(Man.gameObject, "animation3", false, () =>
                {
                    CoverMask.SetActive(false);
                });
            });

            // SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 1, () =>
            // {
            //     SpineManager.instance.DoAnimation(Man.gameObject, "animation2");
            // }, () =>
            // {
            //     SpineManager.instance.DoAnimation(Man.gameObject, "animation3", false, () =>
            //     {
            //         CoverMask.SetActive(false);
            //     });
            // });
        }


        //脱光
        void Resuitup()
        {
            for (int i = 1; i < Man.childCount; i++)
            {
                var clothesTypeParent = Man.GetChild(i);
                for (int j = 0; j < clothesTypeParent.childCount; j++)
                {
                    clothesTypeParent.GetChild(j).gameObject.SetActive(false);
                }
            }
            //穿上大裤衩
            Panties.SetActive(true);
            ScreenBtn.gameObject.SetActive(false);

            SingleConfigOfMan = new Man("", "", "", "", "");
        }


        private void SetConfigOfMan(string clothesType, string clothesName)
        {
            switch (clothesType)
            {
                case "hat":
                    SingleConfigOfMan.HatType = clothesType + "," + clothesName;
                    break;
                case "plants":
                    SingleConfigOfMan.PlantsType = clothesType + "," + clothesName;
                    break;
                case "clothes":
                    SingleConfigOfMan.ClothesType = clothesType + "," + clothesName;
                    break;
                case "shoes":
                    SingleConfigOfMan.ShoesType = clothesType + "," + clothesName;
                    break;
                case "orn":
                    SingleConfigOfMan.OrnType = clothesType + "," + clothesName;
                    break;
            }

            if (SingleConfigOfMan.HatType != "" && SingleConfigOfMan.PlantsType != "" &&
            SingleConfigOfMan.ClothesType != "" && SingleConfigOfMan.ShoesType != "" &&
            SingleConfigOfMan.OrnType != "")
            {
                ScreenBtn.gameObject.SetActive(true);
            }
        }

        //根据配置设置model
        private void SetModelAccordingConfig()
        {
            ResetModel();
            string[] hat = SingleConfigOfMan.HatType.Split(',');
            string[] plants = SingleConfigOfMan.PlantsType.Split(',');
            string[] clothes = SingleConfigOfMan.ClothesType.Split(',');
            string[] shoes = SingleConfigOfMan.ShoesType.Split(',');
            string[] orn = SingleConfigOfMan.OrnType.Split(',');

            //分别设置模型各部分
            try
            {
                SetPartOfModel(Model, hat[0], hat[1]);
                SetPartOfModel(Model, plants[0], plants[1]);
                SetPartOfModel(Model, clothes[0], clothes[1]);
                SetPartOfModel(Model, shoes[0], shoes[1]);
                SetPartOfModel(Model, orn[0], orn[1]);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }

        void ResetModel()
        {
            for (int i = 1; i < Model.transform.childCount; i++)
            {
                var clothesTypeParent = Model.transform.GetChild(i);
                for (int j = 0; j < clothesTypeParent.childCount; j++)
                {
                    clothesTypeParent.GetChild(j).gameObject.SetActive(false);
                }
            }
            Model.transform.Find("Body").Find("Panties").gameObject.SetActive(true);
        }
        void SetPartOfModel(GameObject model, string clothesType, string clothesName)
        {
            if (clothesType.Equals("plants"))
            {
                model.transform.Find("Body").Find("Panties").gameObject.SetActive(false);
            }
            model.transform.Find(clothesType).Find(clothesName).gameObject.SetActive(true);
        }

        private void CloseShowMan(int index)
        {
            Model.SetActive(false);
            ShowMan.SetActive(false);
            TouchMask.SetActive(false);
            HindAnim.SetActive(false);
            CoolAnim.SetActive(false);
            CoolIdleAnim.SetActive(false);
            //脱光
            Resuitup();
            //设置小模型的衣服
            ShowSigleModelIcon();
        }

        void ShowSigleModelIcon()
        {
            for (int i = 0; i < ConfigsOfMan.Count; i++)
            {
                var modelBtn = SmallModels.GetChild(i).gameObject;
                modelBtn.SetActive(true);
                modelBtn.GetComponent<ILObject3DAction>().index = i;
                modelBtn.GetComponent<ILObject3DAction>().OnPointDownLua = ClickModelIconCallBack;

                var model = modelBtn.transform.Find("Man").gameObject;
                ResetSingleIModel(model.transform);
                //设置每个model的样子
                var config = ConfigsOfMan[i];
                string[] hat = config.HatType.Split(',');
                string[] plants = config.PlantsType.Split(',');
                string[] clothes = config.ClothesType.Split(',');
                string[] shoes = config.ShoesType.Split(',');
                string[] orn = config.OrnType.Split(',');

                SetPartOfModel(model, hat[0], hat[1]);
                SetPartOfModel(model, plants[0], plants[1]);
                SetPartOfModel(model, clothes[0], clothes[1]);
                SetPartOfModel(model, shoes[0], shoes[1]);
                SetPartOfModel(model, orn[0], orn[1]);
            }
        }

        private void ClickModelIconCallBack(int index)
        {
            Resuitup();
            //获取单个配置
            var config = ConfigsOfMan[index];
            string[] hat = config.HatType.Split(',');
            string[] plants = config.PlantsType.Split(',');
            string[] clothes = config.ClothesType.Split(',');
            string[] shoes = config.ShoesType.Split(',');
            string[] orn = config.OrnType.Split(',');

            // 穿上衣服
            SuitUp(hat[0], hat[1]);
            SuitUp(plants[0], plants[1]);
            SuitUp(clothes[0], clothes[1]);
            SuitUp(shoes[0], shoes[1]);
            SuitUp(orn[0], orn[1]);
            //设置选衣服配置
            SetConfigOfMan(hat[0], hat[1]);
            SetConfigOfMan(plants[0], plants[1]);
            SetConfigOfMan(clothes[0], clothes[1]);
            SetConfigOfMan(shoes[0], shoes[1]);
            SetConfigOfMan(orn[0], orn[1]);
        }

        void ResetSingleIModel(Transform model)
        {
            for (int i = 1; i < model.childCount; i++)
            {
                var clothesType = model.GetChild(i);
                for (int j = 0; j < clothesType.childCount; j++)
                {
                    clothesType.GetChild(j).gameObject.SetActive(false);
                }
            }
        }

        void ResetModelIcons()
        {
            for (int i = 0; i < SmallModels.childCount; i++)
            {
                SmallModels.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
