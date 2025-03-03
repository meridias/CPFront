using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Panels
{
    public class PanelsCanvas : MonoBehaviour
    {
        //private PanelsCanvas canvas;
        public Camera worldCamera;

        internal class InternalSettings
        {
            //private readonly PanelsCanvas canvas;
     //       public readonly Camera worldCamera;

            public InternalSettings(PanelsCanvas canvas)
            {
                //this.canvas = canvas;

#if UNITY_EDITOR
                if (!canvas.UnityCanvas) // is null while inspecting this component in edit mode
                    return;
#endif

     //           if (canvas.UnityCanvas.renderMode == RenderMode.ScreenSpaceOverlay ||
     //               (canvas.UnityCanvas.renderMode == RenderMode.ScreenSpaceCamera && !canvas.UnityCanvas.worldCamera))
     //               worldCamera = null;
     //           else
     //               worldCamera = canvas.UnityCanvas.worldCamera ? canvas.UnityCanvas.worldCamera : Camera.main;
            }

            public void OnApplicationQuit()
            {
#if UNITY_2018_1_OR_NEWER
   //             canvas.OnApplicationQuitting();
#else
	//			canvas.OnApplicationQuit();
#endif
            }
        }

#if UNITY_EDITOR
        private InternalSettings m_internal;
        internal InternalSettings Internal
        {
            get
            {
                if (m_internal == null)
                    m_internal = new InternalSettings(this);

                return m_internal;
            }
        }
#else
        internal InternalSettings Internal { get; private set; }
#endif

        public List<PanelGroup> panelGroups = new List<PanelGroup>();
        //public List<PanelGroupResize> draggingGroupResizeZones = new List<PanelGroupResize>();
        //public List<DraggedGroups> draggedGroups = new List<DraggedGroups>();
        [HideInInspector]
        public bool isStartDone = false;

        public RectTransform RectTransform { get; private set; }
        public RectTransform panelGroupsRect;
        public Canvas UnityCanvas { get; private set; }
        //public PanelGroup RootPanelGroup { get; private set; }

        private bool isQuitting = false;

        private void Awake()
        {
            RectTransform = (RectTransform)transform;

            UnityCanvas = GetComponentInParent<Canvas>();

            if (UnityCanvas.renderMode == RenderMode.ScreenSpaceOverlay ||
    (UnityCanvas.renderMode == RenderMode.ScreenSpaceCamera && !UnityCanvas.worldCamera))
                worldCamera = null;
            else
                worldCamera = UnityCanvas.worldCamera ? UnityCanvas.worldCamera : Camera.main;

            //          UnityCanvas = GetComponentInParent<Canvas>();
            //RootPanelGroup = new PanelGroup(this);//, Direction.Right);
            //Debug.Log(PanelManager.instance != null);
            PanelManager.instance.RegisterCanvas(this);
        }

        // Start is called before the first frame update
        void Start()
        {

            isStartDone = true;
        }

#if UNITY_2018_1_OR_NEWER
        private void OnApplicationQuitting()
#else
		private void OnApplicationQuit()
#endif
        {
            isQuitting = true;
        }

        // Update is called once per frame
        void Update()
        {

        }

        [Serializable]
        public class DraggedGroups
        {
            public PanelGroup group;
            public Direction direction;
        }
    }
}
