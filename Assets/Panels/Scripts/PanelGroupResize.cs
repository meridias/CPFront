using DynamicPanels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Panels
{
    public class PanelGroupResize : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler//, IPointerEnterHandler, IPointerExitHandler
    {
        public RectTransform RectTransform { get; private set; }

        [HideInInspector]
        public PanelGroup panelGroup;
        [HideInInspector]
        public Direction direction;

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            panelGroup = GetComponentInParent<PanelGroup>();

            switch (gameObject.name)
            {
                case "ResizeTop":
                    direction = Direction.Top;
                    break;
                case "ResizeLeft":
                    direction = Direction.Left;
                    break;
                case "ResizeRight":
                    direction = Direction.Right;
                    break;
                case "ResizeBottom":
                    direction = Direction.Bottom;
                    break;
            }
            panelGroup.resizeZones.Add(this);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Initialize(PanelGroup group)//, Direction direction)
        {
            panelGroup = group;
            group.resizeZones.Add(this);
            //this.direction = direction;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //panelGroup.Canvas.draggedGroups.Clear();
            panelGroup.draggedZones.Clear();

            if (!Keyboard.current.shiftKey.IsPressed())
                return;
            //panelGroup.Canvas.draggingGroupResizeZones.Clear();

            panelGroup.draggedZones.AddRange(panelGroup.ClickedResizeZones(eventData.position));
            //List<PanelGroupResize> clickedZones = panelGroup.ClickedResizeZones(eventData.position);
            // Debug.Log(clickedZones.Count);
            // for (int i = 0; i < clickedZones.Count; i++)
            // {
            //   panelGroup.AddGroupToDrag(clickedZones[i].direction);
            // }
            //Debug.Log(panelGroup.draggedZones.Count);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            //Debug.Log("now dragging");
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!Keyboard.current.shiftKey.IsPressed())
                return;

            //Debug.Log(eventData.position);
            float draggingX = PanelUtils.GetXDragDelta(panelGroup, eventData.position);//  0f;
            float draggingY = PanelUtils.GetYDragDelta(panelGroup, eventData.position);//  0f;
            //Debug.Log($"{draggingX}, {draggingY}");

            //adjust drag delta against any min Sizes we hit up against
//            draggingX = PanelUtils.AdjustXDragDelta(panelGroup, draggingX);
//            draggingY = PanelUtils.AdjustYDragDelta(panelGroup, draggingY);

            //Vector2 dragDeltas = new Vector2(draggingX, draggingY);
            //Debug.Log(dragDeltas);
            for (int i = 0; i < panelGroup.draggedZones.Count; i++)
            {
                //if either the drag x or drag y is 0, skip it
                if ((panelGroup.draggedZones[i].direction == Direction.Left || panelGroup.draggedZones[i].direction == Direction.Right) && draggingX == 0f)
                    continue;
                if ((panelGroup.draggedZones[i].direction == Direction.Top || panelGroup.draggedZones[i].direction == Direction.Bottom) && draggingY == 0f)
                    continue;
                //only drag if we can
                if (panelGroup.draggedZones[i].direction == Direction.Left || panelGroup.draggedZones[i].direction == Direction.Right)
                {
                    panelGroup.OnResize(panelGroup.draggedZones[i].direction, draggingX);
                } else
                {
                    panelGroup.OnResize(panelGroup.draggedZones[i].direction, draggingY);
                }
            }

  /*          for (int j = 0; j < panelGroup.Canvas.draggedGroups.Count; j++)
            {
                if ((panelGroup.Canvas.draggedGroups[j].direction == Direction.Left || panelGroup.Canvas.draggedGroups[j].direction == Direction.Right) &&
                    dragDeltas.x == 0f)
                    continue;
                if ((panelGroup.Canvas.draggedGroups[j].direction == Direction.Top || panelGroup.Canvas.draggedGroups[j].direction == Direction.Bottom) &&
                    dragDeltas.y == 0f)
                    continue;
                PanelGroup groupBeingDragged = panelGroup.Canvas.draggedGroups[j].group;
                groupBeingDragged.OnResize(panelGroup.Canvas.draggedGroups[j].direction, dragDeltas);
            }*/
        }

        public void OnEndDrag(PointerEventData eventData)
        {

        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //panelGroup.Canvas.draggedGroups.Clear();
            panelGroup.draggedZones.Clear();
            //         for (int i = 0; i < panelGroup.Canvas.draggedGroups.Count; i++)
            //         {
            //             panelGroup.Canvas.draggedGroups[i].group.beingDragged = false;
            //         }
            //panelGroup.Canvas.draggedGroups.Clear();
        }

  /*      public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("entered");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("exited");
        }*/

    }
}
