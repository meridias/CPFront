using Panels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseOutput : MonoBehaviour
{
    public Panel panel;

    public virtual string Input { get; set; }

    public virtual void OnPanelResize(PointerEventData eventData)
    {
        Vector2 pos = eventData.position;

        if (panel.Group != null && panel.Group.position == Direction.Main)
        {//check drag point against outside edge of Main panelGroup and adjust drag Vector2 to keep it inside the lines
            Vector2 localPoint;
            float modX = eventData.position.x;
            float modY = eventData.position.y;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panel.Group.RectTransform, eventData.position, panel.Canvas.worldCamera, out localPoint);
            if (localPoint.x > panel.Group.RectTransform.sizeDelta.x)
            {
                //drag point x is outside the main group to the right
                float xDif = localPoint.x - panel.Group.RectTransform.sizeDelta.x;//this is how far out we are
                modX = modX - xDif;
            }
            else if (localPoint.x < 0f)
            {
                modX = modX - localPoint.x;
            }
            if (localPoint.y < 0f)
            {
                float yDif = -localPoint.y;
                modY = modY + yDif;
            }
            else if (localPoint.y > panel.Group.RectTransform.sizeDelta.y)
            {
                float yDif = localPoint.y - panel.Group.RectTransform.sizeDelta.y;
                modY = modY - yDif;
            }
            pos = new Vector2(modX, modY);
        }

        float draggingX = PanelUtils.GetXDragDelta(panel, pos);// eventData.position);
        float draggingY = PanelUtils.GetYDragDelta(panel, pos);// eventData.position);
                                                               //Debug.Log(draggingY);
        for (int i = 0; i < panel.draggedZones.Count; i++)
        {
            if ((panel.draggedZones[i].direction == Direction.Left || panel.draggedZones[i].direction == Direction.Right) && draggingX == 0f)
                continue;//not actually moving to the left/right so skip
            if ((panel.draggedZones[i].direction == Direction.Top || panel.draggedZones[i].direction == Direction.Bottom) && draggingY == 0f)
                continue;//not actually moving up/down so skip
            if (panel.draggedZones[i].direction == Direction.Left || panel.draggedZones[i].direction == Direction.Right)
            {
                panel.OnResize(panel.draggedZones[i].direction, draggingX);
            }
            else
            {
                panel.OnResize(panel.draggedZones[i].direction, draggingY);
            }
        }

    }


}
