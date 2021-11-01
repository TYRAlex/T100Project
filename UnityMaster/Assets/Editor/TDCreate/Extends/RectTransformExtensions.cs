using UnityEngine;

namespace TDCreate
{
    public enum AnchorPresetsType
    {
        LeftTop,
        LeftMiddle,
        LeftBottom,
        LeftStretch,

        CenterTop,
        CenterMiddle,
        CenterBottom,
        CenterStretch,

        RightTop,
        RightMiddle,
        RightBottom,
        RightStretch,

        StretchTop,
        StretchMiddle,
        StretchBottom,
        StretchAll,
    }


    public enum PivotPresetsType
    {
        LeftTop,
        LeftMiddle,
        LeftBottom,

        CenterTop,
        CenterMiddle,
        CenterBottom,

        RightTop,
        RightMiddle,
        RightBottom,
    }

    public static class RectTransformExtensions
    {
        public static void SetScale(this GameObject go, Vector2 scale)
        {
            go.GetComponent<RectTransform>().localScale = scale;
        }

        public static void SetAnchorPos(this GameObject go, Vector2 anchorPos)
        {
            go.GetComponent<RectTransform>().anchoredPosition = anchorPos;
        }

        public static void SetAnchor(this RectTransform rect, AnchorPresetsType type)
        {
            var width = rect.rect.width;
            var height = rect.rect.height;
            switch (type)
            {
                case AnchorPresetsType.LeftTop:
                    rect.anchorMin = new Vector2(0f, 1f);
                    rect.anchorMax = new Vector2(0f, 1f);
                    rect.offsetMin = new Vector2(0, -height);
                    rect.offsetMax = new Vector2(width, 0);
                    break;
                case AnchorPresetsType.LeftMiddle:
                    rect.anchorMin = new Vector2(0f, 0.5f);
                    rect.anchorMax = new Vector2(0f, 0.5f);
                    rect.offsetMin = new Vector2(0, -height / 2);
                    rect.offsetMax = new Vector2(width, height / 2);

                    break;
                case AnchorPresetsType.LeftBottom:
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.zero;
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = new Vector2(width, height);
                    break;
                case AnchorPresetsType.LeftStretch:
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = new Vector2(0f, 1f);
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = new Vector2(width, 0);
                    break;
                case AnchorPresetsType.CenterTop:
                    rect.anchorMin = new Vector2(0.5f, 1f);
                    rect.anchorMax = new Vector2(0.5f, 1f);
                    rect.offsetMin = new Vector2(-width / 2, -height);
                    rect.offsetMax = new Vector2(width / 2, 0);
                    break;
                case AnchorPresetsType.CenterMiddle:
                    rect.anchorMin = new Vector2(0.5f, 0.5f);
                    rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.offsetMin = new Vector2(-width / 2, -height / 2);
                    rect.offsetMax = new Vector2(width / 2, height / 2);
                    break;
                case AnchorPresetsType.CenterBottom:
                    rect.anchorMin = new Vector2(0.5f, 0f);
                    rect.anchorMax = new Vector2(0.5f, 0f);
                    rect.offsetMin = new Vector2(-width / 2, 0f);
                    rect.offsetMax = new Vector2(width / 2, height);
                    Debug.LogError("offsetMin" + rect.offsetMin);
                    Debug.LogError("offsetMax" + rect.offsetMax);
                    break;
                case AnchorPresetsType.CenterStretch:
                    rect.anchorMin = new Vector2(0.5f, 0f);
                    rect.anchorMax = new Vector2(0.5f, 1f);
                    rect.offsetMin = new Vector2(-width / 2, 0f);
                    rect.offsetMax = new Vector2(width / 2, 0);
                    break;
                case AnchorPresetsType.RightTop:
                    rect.anchorMin = new Vector2(1f, 1f);
                    rect.anchorMax = new Vector2(1f, 1f);
                    rect.offsetMin = new Vector2(-width, -height);
                    rect.offsetMax = Vector2.zero;
                    break;
                case AnchorPresetsType.RightMiddle:
                    rect.anchorMin = new Vector2(1f, 0.5f);
                    rect.anchorMax = new Vector2(1f, 0.5f);
                    rect.offsetMin = new Vector2(-width, -height / 2);
                    rect.offsetMax = new Vector2(0, height / 2);
                    break;
                case AnchorPresetsType.RightBottom:
                    rect.anchorMin = new Vector2(1f, 0f);
                    rect.anchorMax = new Vector2(1f, 0f);
                    rect.offsetMin = new Vector2(-width, 0);
                    rect.offsetMax = new Vector2(0, height);
                    break;
                case AnchorPresetsType.RightStretch:
                    rect.anchorMin = new Vector2(1f, 0f);
                    rect.anchorMax = new Vector2(1f, 1f);
                    rect.offsetMin = new Vector2(-width, 0);
                    rect.offsetMax = Vector2.zero;
                    break;
                case AnchorPresetsType.StretchTop:
                    rect.anchorMin = new Vector2(0f, 1f);
                    rect.anchorMax = new Vector2(1f, 1f);
                    rect.offsetMin = new Vector2(0, -height);
                    rect.offsetMax = Vector2.zero;
                    break;
                case AnchorPresetsType.StretchMiddle:
                    rect.anchorMin = new Vector2(0f, 0.5f);
                    rect.anchorMax = new Vector2(1f, 0.5f);
                    rect.offsetMin = new Vector2(0, -height / 2);
                    rect.offsetMax = new Vector2(0, height / 2);
                    break;
                case AnchorPresetsType.StretchBottom:
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = new Vector2(1f, 0f);
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = new Vector2(0, height);
                    break;
                case AnchorPresetsType.StretchAll:
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = Vector2.zero;
                    break;
            }
        }
        public static void SetPivot(this RectTransform rect, PivotPresetsType type)
        {
            switch (type)
            {
                case PivotPresetsType.LeftTop:
                    rect.pivot = new Vector2(0, 1);
                    break;
                case PivotPresetsType.LeftMiddle:
                    rect.pivot = new Vector2(0, 0.5f);
                    break;
                case PivotPresetsType.LeftBottom:
                    rect.pivot = Vector2.zero;
                    break;
                case PivotPresetsType.CenterTop:
                    rect.pivot = new Vector2(0.5f, 1);
                    break;
                case PivotPresetsType.CenterMiddle:
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    break;
                case PivotPresetsType.CenterBottom:
                    rect.pivot = new Vector2(0.5f, 0);
                    Debug.LogError("pivot:"+ rect.pivot);
                    break;
                case PivotPresetsType.RightTop:
                    rect.pivot = Vector2.one;
                    break;
                case PivotPresetsType.RightMiddle:
                    rect.pivot = new Vector2(1f, 0.5f);
                    break;
                case PivotPresetsType.RightBottom:
                    rect.pivot = new Vector2(1f, 0);
                    break;
            }
        }
    }
}

