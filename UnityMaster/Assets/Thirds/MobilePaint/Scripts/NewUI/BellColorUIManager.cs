// sends picked color to MobilePaint script

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace unitycoder_MobilePaint
{

	public class BellColorUIManager : MonoBehaviour 
	{
		MobilePaint mobilePaint;
		public Button[] colorpickers; // colors are taken from these buttons

		public bool offsetSelected=true; // should we move the pencil when its selected
		public float defaultOffset=-46;
		public float moveOffsetX=-24;
        //颜色按钮父类
        public GameObject colorBtnParent;

        [HideInInspector] public Image currentColorImage;

		void Start()
		{
			mobilePaint = PaintManager.mobilePaint;

			if (mobilePaint==null) Debug.LogError("No MobilePaint assigned at "+transform.name, gameObject);
			if (colorpickers.Length<1) Debug.LogWarning("No colorpickers assigned at "+transform.name, gameObject);

			currentColorImage = GetComponent<Image>();
			if (currentColorImage==null) Debug.LogError("No image component founded at "+transform.name, gameObject);

			// Add event listeners to pencil buttons
			for (int i=0;i<colorpickers.Length;i++)
			{
				var button = colorpickers[i];
				if (button!=null)
				{
					button.onClick.AddListener(delegate {this.SetCurrentColor(button);});
				}
			}
            HideAllColorBtns();
        }

		// some button was clicked, lets take color from it and send to mobilepaint canvas 
 		public void SetCurrentColor(Button button)
		{
            HideAllColorBtns();
            Image img = button.transform.GetChild(0).GetComponent<Image>();
            img.gameObject.SetActive(true);
            button.transform.GetChild(1).gameObject.SetActive(false);

            if (button.name == "Eraser")
            {
                mobilePaint.SetDrawModeEraser();
                return;
            }
            else
            {
                mobilePaint.SetDrawModeBrush();
            }

            Color newColor = img.color;
			currentColorImage.color = newColor; // set current color image

			// send new color
			mobilePaint.SetPaintColor(newColor);

			if (offsetSelected)
			{
				ResetAllOffsets();
				SetButtonOffset(button,moveOffsetX);
			}
        }

        void HideAllColorBtns()
        {
            for (int i = 0; i < colorBtnParent.transform.childCount; i++)
            {
                var child = colorBtnParent.transform.GetChild(i);
                child.GetChild(0).gameObject.SetActive(false);
                child.GetChild(1).gameObject.SetActive(true);
            }

        }

        void ResetAllOffsets()
		{
			for (int i=0;i<colorpickers.Length;i++)
			{
				SetButtonOffset(colorpickers[i],defaultOffset); 
			}
		}


		void SetButtonOffset(Button button,float offsetX)
		{
			RectTransform rectTransform = button.GetComponent<RectTransform>();
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,offsetX,rectTransform.rect.width);
		}

	} // class
} // namespace