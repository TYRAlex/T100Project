using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class Empty4Raycast : MaskableGraphic,IPointerDownHandler, IPointerUpHandler
    {

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnPointerUp(PointerEventData eventData)
        {
        }

        protected Empty4Raycast()
        {
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }
    }
}
