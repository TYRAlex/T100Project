using System;
using System.Collections.Generic;
using System.Timers;

namespace FrogePart4
{
    class ConstVariable
    {
        public enum LColor
        {
            None,
            Blue = 2,
            Red = 5,
            Green = 3,
            Yellow = 4,
        }
        static int[] randColor = new int[] { 2,5,3 };
        //static string[] colorSpine = new string[] { "huang", "jv", "lv" };   // 不同颜色青蛙待机动画 荷叶待机动画
        static string[] colorToSpine = new string[] { "1", "2", "3" };   // 青蛙转换颜色动画

        // 开始按钮
        public static string startBtnIdle = "animation";
        public static string startBtnTips = "animation2";

        // 成功动画
        public static string sucessAni = "animation";
        public static string sucessIdle= "animation2";

        // 青蛙动画
        public static string frogConnectIdle = "lv";
        public static string frogDisConnectIdle = "hui";
        public static string frogConnect = "z";
        public static string frogTakeOff = "tiao";
        // 编号动画
        public static string[] numberTake = new string[] { "11","12","13","14" };
        public static string[] numberIdle = new string[] { "d1", "d2", "d3", "d4" };

        // 荷叶不同颜色 大小 待机动画
        static string[] redLeafSpine = new string[] { "hong1","xhong1" };
        static string[] greenLeafSpine = new string[] { "lv1", "xlv1" };
        static string[] blueLeafSpine = new string[] { "lan1", "xlan1" };
        // 荷叶不同颜色 大小 跳跃动画
        static string[] redTakeSpine = new string[] { "hong2", "xhong2" };
        static string[] greenTakeSpine = new string[] { "lv2", "xlv2" };
        static string[] blueTakeSpine = new string[] { "lan2", "xlan2" };

        static float maxRadius = 88;
        static float minRadius = 69;

        public static string leafPoolName = "leafPool";

        static List<int> randNum = new List<int>();
        static Timer timer = null;
        public static int RandDistance(int min, int max)
        {
            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
            int result = UnityEngine.Random.Range(min, max);
            //while (!CheckOut(ref result))
            //{
            //    UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
            //    result = UnityEngine.Random.Range(min, max);
            //}
            randNum.Add(result);
            return result;
        }

        static void RandTimer()
        {
            if (timer == null)
                timer = new Timer();
            timer.Interval = 3000;
            timer.Elapsed += new ElapsedEventHandler(ClearList);
            timer.Start();
        }

        static void ClearList(object sender, ElapsedEventArgs s)
        {
            randNum.Clear();
        }

        static bool CheckOut(ref int num)
        {
            for (int i = 0; i < randNum.Count; i++)
            {
                if (num == randNum[i])
                    return false;
            }
            return true;
        }

        public static LColor RandColor()
        {
            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
            int idx = UnityEngine.Random.Range(0, 3);
            return GetColorByInt(randColor[idx]);
        }

        public static float GetCircleRadius(string aniName)
        {
            string s = aniName.Substring(0, 1);
            return s == "x" ? minRadius : maxRadius;
        }

        public static LColor GetColorByInt(int color)
        {
            switch (color)
            {
                case 2:
                    return LColor.Blue;
                case 5:
                    return LColor.Red;
                case 3:
                    return LColor.Green;
                case 4:
                    return LColor.Yellow;
                default:
                    return LColor.None;
            }
        }

        public static string[] GetSpineByColor(LColor color)
        {
            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
            int num = UnityEngine.Random.Range(0, 2);

            string[] anis = new string[2];

            switch (color)
            {
                case LColor.None:
                    break;
                case LColor.Blue:
                    anis[0] = blueLeafSpine[num];
                    anis[1] = blueTakeSpine[num];
                    break;
                case LColor.Red:
                    anis[0] = redLeafSpine[num];
                    anis[1] = redTakeSpine[num];
                    break;
                case LColor.Green:
                    anis[0] = greenLeafSpine[num];
                    anis[1] = greenTakeSpine[num];
                    break;
                default:
                    break;
            }
            return anis;
        }

        public static string GetColorToSpine(LColor color)
        {
            string str = "";
            switch (color)
            {
                case LColor.None:
                    break;
                case LColor.Blue:
                    str = colorToSpine[1];
                    break;
                case LColor.Red:
                    str = colorToSpine[2];
                    break;
                case LColor.Green:
                    str = colorToSpine[0];
                    break;
                default:
                    break;
            }
            return str;
        }
    }
}
