using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace TDCreate {

    public static class AddComponentExtends
    {
        public static void AddSkeletonGraphic(this GameObject go, string path, string startingAniName = "None", bool startingLoop = false, bool raycastTarget = false)
        {
            go.AddComponent<SkeletonGraphic>();
            var sG = go.GetComponent<SkeletonGraphic>();
            var sDataAsset = EditorResourcesManager.LoadSkeletonDataAsset(path);
            sG.skeletonDataAsset = sDataAsset;
            sG.MeshGenerator.settings.pmaVertexColors = false;

            if (startingAniName != "None")
                sG.startingAnimation = startingAniName;

            sG.startingLoop = startingLoop;
            sG.raycastTarget = raycastTarget;
        }

        public static void AddSkeletonGraphic(this GameObject go, SkeletonDataAsset skeletonData, string startingAniName = "None", bool startingLoop = false, bool raycastTarget = false)
        {
            go.AddComponent<SkeletonGraphic>();
            var sG = go.GetComponent<SkeletonGraphic>();

            sG.skeletonDataAsset = skeletonData;
            sG.MeshGenerator.settings.pmaVertexColors = false;

            if (startingAniName != "None")
                sG.startingAnimation = startingAniName;

            sG.startingLoop = startingLoop;
            sG.raycastTarget = raycastTarget;
        }


        public static void AddContentSizeFitter(this GameObject go, ContentSizeFitter.FitMode horizontalFit = ContentSizeFitter.FitMode.Unconstrained,
            ContentSizeFitter.FitMode verticalFit = ContentSizeFitter.FitMode.PreferredSize)
        {
            go.AddComponent<ContentSizeFitter>();
            go.GetComponent<ContentSizeFitter>().horizontalFit = horizontalFit;
            go.GetComponent<ContentSizeFitter>().verticalFit = verticalFit;
        }


        public static void AddText(this GameObject go, int fontSize = 36, string path = Config.DialoguesFontPath,
            TextAnchor alignment = TextAnchor.UpperLeft,
            float lineSpacing = 1.2f, string hex = "323232"
            )
        {
            go.AddComponent<Text>();
            go.GetComponent<Text>().fontSize = fontSize;
            go.GetComponent<Text>().font = EditorResourcesManager.LoadFontAsset(path);
            go.GetComponent<Text>().alignment = alignment;
            go.GetComponent<Text>().lineSpacing = lineSpacing;
            go.GetComponent<Text>().raycastTarget = false;
            go.GetComponent<Text>().color = ColorExtensions.HexToColor(hex);
        }

        public static void AddGridLayoutGroup(this GameObject go, Vector2 cellSize, Vector2 spacing, RectOffset padding,
            GridLayoutGroup.Corner startCorner = GridLayoutGroup.Corner.UpperLeft,
            GridLayoutGroup.Axis startAxis = GridLayoutGroup.Axis.Vertical,
            TextAnchor childAlignment = TextAnchor.UpperLeft,
            GridLayoutGroup.Constraint constraint = GridLayoutGroup.Constraint.Flexible)
        {
            go.AddComponent<GridLayoutGroup>();
            var group = go.GetComponent<GridLayoutGroup>();
            group.padding = padding;
            group.cellSize = cellSize;
            group.spacing = spacing;
            group.startCorner = startCorner;
            group.startAxis = startAxis;
            group.childAlignment = childAlignment;
            group.constraint = constraint;
        }

        public static void AddEmpty4Ray(this GameObject go, bool ray = true)
        {
            go.AddComponent<Empty4Raycast>();
            go.GetComponent<Empty4Raycast>().raycastTarget = ray;
        }

        public static void AddRawImage(this GameObject go, Sprite sprite = null, bool ray = false)
        {
            go.AddComponent<RawImage>();
            go.GetComponent<RawImage>().raycastTarget = ray;

            if (sprite != null)
                go.GetComponent<RawImage>().texture = sprite.texture;
        }
  

        public static void AddImage(this GameObject go, Color color, bool ray = false)
        {
            go.AddComponent<Image>();
            go.GetComponent<Image>().raycastTarget = ray;
            go.GetComponent<Image>().color = color;
        }

        public static void AddImage(this GameObject go, bool ray = false, string hex = "")
        {
            go.AddComponent<Image>();
            go.GetComponent<Image>().raycastTarget = ray;

            if (hex != "")
            {
                go.GetComponent<Image>().color = ColorExtensions.HexToColor(hex);
            }
        }



        public static void AddMask(this GameObject go, bool showMask = false)
        {
            go.AddComponent<Mask>();
            go.GetComponent<Mask>().showMaskGraphic = showMask;
        }

        public static void AddRectTransform(this GameObject go, Vector2 sizeDelta, PivotPresetsType pivotPresetsType = PivotPresetsType.CenterMiddle, AnchorPresetsType anchorPresetsType = AnchorPresetsType.CenterMiddle)
        {
            go.AddComponent<RectTransform>();
            go.GetComponent<RectTransform>().sizeDelta = sizeDelta;
            go.GetComponent<RectTransform>().SetPivot(pivotPresetsType);
            go.GetComponent<RectTransform>().SetAnchor(anchorPresetsType);
        }


        public static void AddCanvas(this GameObject go)
        {
            go.AddComponent<Canvas>();
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;
            canvas.sortingOrder = 0;
            canvas.targetDisplay = 0;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
        }

        public static void AddCanvasScaler(this GameObject go)
        {
            go.AddComponent<CanvasScaler>();
            var canvasScaler = go.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.matchWidthOrHeight = 0;
            canvasScaler.referencePixelsPerUnit = 100;
        }

        public static void AddGraphicRaycaster(this GameObject go)
        {
            go.AddComponent<GraphicRaycaster>();
            var graphicRaycaster = go.GetComponent<GraphicRaycaster>();
            graphicRaycaster.ignoreReversedGraphics = true;
            graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;

        }
    }
}

   



