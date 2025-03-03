//using DynamicPanels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Panels
{
    public class PanelHeader : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private Panel m_panel;
        public Panel Panel { get { return m_panel; } }
        public TextMeshProUGUI panelTitle;

        private Vector2 m_initialTouchPos;
        internal Vector2 InitialTouchPos { get { return m_initialTouchPos; } }
        //public Camera camera;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!PanelManager.instance.OnPanelBeginDrag(m_panel))
            {
                eventData.pointerDrag = null;
                return;
            }
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_panel.RectTransform, eventData.position, Panel.Canvas.worldCamera, out m_initialTouchPos);
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(m_panel.RectTransform, eventData.position, camera, out m_initialTouchPos);
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(m_panel.RectTransform, eventData.position, m_panel.Canvas.Internal.worldCamera, out m_initialTouchPos);

            if (Panel.isDockable)
                PanelManager.instance.AnchorZonesSetActive(true);

            if (Panel.Group == null || Panel.Group.layout == Layout.Unrestricted)
            {
                Panel.RectTransform.SetAsLastSibling();

            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            PanelManager.instance.OnPanelDrag(Panel, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            PanelManager.instance.AnchorZonesSetActive(false);
            PanelManager.instance.OnPanelEndDrag(Panel, eventData);
        }
    }
}
