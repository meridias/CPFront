using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Panels
{
    public class AnchorDropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public PanelGroup panelGroup;
        public RectTransform RectTransform { get; private set; }

        private Graphic raycastAnchor;

        private void Awake()
        {
            RectTransform = (RectTransform)transform;
            //for now, this is Image, will be NonDrawingGraphic
            raycastAnchor = GetComponent<Graphic>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetAnchor(PanelGroup group, float size)
        {
            panelGroup = group;

            if (panelGroup.layout == Layout.Down)
            {
                //set object anchor points for auto width
                RectTransform.anchorMin = new Vector2(0f, 1f);
                RectTransform.anchorMax = new Vector2(1f, 1f);
                RectTransform.pivot = new Vector2(0f, 0.5f);
                RectTransform.sizeDelta = new Vector2(0f, size);
            }
            else if (panelGroup.layout == Layout.Right)
            {
                //set object anchor points for auto height
                RectTransform.anchorMin = new Vector2(0f, 0f);
                RectTransform.anchorMax = new Vector2(0f, 1f);
                RectTransform.pivot = new Vector2(0.5f, 1f);
                RectTransform.sizeDelta = new Vector2(size, 0f);
            }

        }

        public void AdjustPos(Vector2 newPos)
        {
            RectTransform.localPosition = newPos;

        }

        public void SetActive(bool value)
        {
            //hoveredPointerId = PanelManager.NON_EXISTING_TOUCH;
            raycastAnchor.raycastTarget = value;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log("entered");
            PanelManager.instance.StartPreviewHover(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("exited");
            PanelManager.instance.StopPreviewHover(this);
        }

    }
}
