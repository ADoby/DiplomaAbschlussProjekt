using UnityEngine;
using System.Collections.Generic;

public enum HorizontalAnchorPoint
{
    LEFT,
    CENTER,
    RIGHT
}

public enum VerticalAnchorPoint
{
    TOP,
    CENTER,
    BOTTOM
}

[System.Serializable]
public class UIPosition
{
    public bool normalized = false;
    public Vector2 Value;
}
[System.Serializable]
public class UISize
{
    public bool normalized = false;
    public Vector2 Value;
}

public abstract class UIRect : MonoBehaviour
{
    public UIPosition position;
    public UISize size;

    public HorizontalAnchorPoint HorizontalAnchor;
    public VerticalAnchorPoint VerticalAnchor;

    public HorizontalAnchorPoint HorizontalAlignment;
    public VerticalAnchorPoint VerticalAlignment;

    protected Rect absoluteRect;

    #region Parent

    protected UIRect parent;

    protected void UpdateParent()
    {
        Transform currentTransform = transform;
        do
        {
            currentTransform = currentTransform.parent;
            if (currentTransform == null)
                break;

            parent = currentTransform.GetComponent<UIRect>();
        } while (parent == null);
    }

    #endregion

    #region Children

    protected List<UIRect> children = new List<UIRect>();

    public void UpdateChildren()
    {
        children.Clear();
        foreach (Transform child in transform)
        {
            UIRect childRect = child.GetComponent<UIRect>();
            if (childRect)
            {
                children.Add(childRect);
                childRect.UpdateChildren();
            }
        }
    }

    #endregion

    public void UpdateUI()
    {
        UpdateChildren();
        UpdateParent();
        UpdateAnchor();
        UpdateRect();

        foreach (var child in children)
        {
            child.UpdateUI();
        }
    }

    abstract public void Draw();

    protected void UpdateRect()
    {
        absoluteRect = new Rect(position.Value.x, position.Value.y, size.Value.x, size.Value.y);
        if (parent == null)
        {
            RelativeToAbsolute(Screen.width, Screen.height);
        }
        else
        {
            RelativeToAbsolute(parent.absoluteRect.width, parent.absoluteRect.height);

            absoluteRect.x += parent.absoluteRect.x;
            absoluteRect.y += parent.absoluteRect.y;

            switch (HorizontalAnchor)
            {
                case HorizontalAnchorPoint.LEFT:
                    break;
                case HorizontalAnchorPoint.CENTER:
                    absoluteRect.x += parent.absoluteRect.width * 0.5f;
                    break;
                case HorizontalAnchorPoint.RIGHT:
                    absoluteRect.x += parent.absoluteRect.width;
                    break;
                default:
                    break;
            }

            switch (VerticalAnchor)
            {
                case VerticalAnchorPoint.TOP:
                    break;
                case VerticalAnchorPoint.CENTER:
                    absoluteRect.y += parent.absoluteRect.height * 0.5f;
                    break;
                case VerticalAnchorPoint.BOTTOM:
                    absoluteRect.y += parent.absoluteRect.height;
                    break;
                default:
                    break;
            }

            switch (HorizontalAlignment)
            {
                case HorizontalAnchorPoint.LEFT:
                    break;
                case HorizontalAnchorPoint.CENTER:
                    absoluteRect.x -= absoluteRect.width * 0.5f;
                    break;
                case HorizontalAnchorPoint.RIGHT:
                    absoluteRect.x -= absoluteRect.width;
                    break;
                default:
                    break;
            }

            switch (VerticalAlignment)
            {
                case VerticalAnchorPoint.TOP:
                    break;
                case VerticalAnchorPoint.CENTER:
                    absoluteRect.y -= absoluteRect.height * 0.5f;
                    break;
                case VerticalAnchorPoint.BOTTOM:
                    absoluteRect.y -= absoluteRect.height;
                    break;
                default:
                    break;
            }
        }
    }

    private void RelativeToAbsolute(float width, float height)
    {
        if(position.normalized){
            absoluteRect.x *= width;
            absoluteRect.y *= height;
        }
        if (size.normalized)
        {
            absoluteRect.width *= width;
            absoluteRect.height *= height;
        }
    }

    protected void UpdateAnchor()
    {

    }

    protected void DrawChildren()
    {
        foreach (var child in children)
        {
            child.Draw();
        }
    }
}
