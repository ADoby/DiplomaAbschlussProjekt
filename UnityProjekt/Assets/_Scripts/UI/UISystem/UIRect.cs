using UnityEngine;
using System.Collections.Generic;

#region Enum and Class Definitions

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
    public Vector2 Value = Vector2.zero;

    public UIPosition(float x, float y, bool normalized)
    {
        this.normalized = normalized;
        Value.x = x;
        Value.y = y;
    }

    public void SetPosition(float x, float y)
    {
        Value.x = x;
        Value.y = y;
    }
}
[System.Serializable]
public class UISize
{
    public bool normalized = false;
    public Vector2 Value = Vector2.zero;

    public UISize(float x, float y, bool normalized)
    {
        this.normalized = normalized;
        Value.x = x;
        Value.y = y;
    }

    public void SetSize(float x, float y)
    {
        Value.x = x;
        Value.y = y;
    }
}

#endregion

[System.Serializable]
public class UIRect : MonoBehaviour
{

    #region Public Member
    
    public bool Visible = true;
    public bool ShowBackground = false;
    public UIPosition Position = new UIPosition(0, 0, false);
    public UISize Size = new UISize(0, 0, false);

    public void SetPosition(float x, float y, bool normalized)
    {
        Position.SetPosition(x, y);
        Position.normalized = normalized;
    }
    public void SetSize(float x, float y, bool normalized)
    {
        Size.SetSize(x, y);
        Size.normalized = normalized;
    }

    public HorizontalAnchorPoint HorizontalAnchor;
    public VerticalAnchorPoint VerticalAnchor;

    public HorizontalAnchorPoint HorizontalAlignment;
    public VerticalAnchorPoint VerticalAlignment;

    #endregion

    #region Protected Member

    protected bool uiActive = true;
    protected Rect absoluteRect;

    #endregion

    #region Parent

    protected UIRect parent;

    protected void UpdateParent()
    {
        /*Transform currentTransform = transform;
        do
        {
            currentTransform = currentTransform.parent;
            if (currentTransform == null)
                break;

            parent = currentTransform.GetComponent<UIRect>();
        } while (parent == null);
        */
    }

    #endregion

    #region Children

    protected List<UIRect> children = new List<UIRect>();

    public List<UIRect> GetChildren()
    {
        return children;
    }

    public void AddChild(UIRect newChild)
    {
        children.Add(newChild);
        if (newChild.parent == null)
        {
            newChild.parent = this;
        }
        else
        {
            newChild.parent.RemoveChild(newChild);
            newChild.parent = this;
        }

        if (newChild.transform != null && transform != null)
        {
            newChild.transform.parent = transform;
        }
    }

    public void RemoveChild(UIRect oldChild)
    {
        if (children.Contains(oldChild))
            children.Remove(oldChild);
    }

    public void UpdateChildren()
    {
        children.Clear();
        foreach (Transform child in transform)
        {
            UIRect childRect = child.GetComponent<UIRect>();
            if (childRect)
            {
                AddChild(childRect);
                childRect.UpdateChildren();
            }
        }
    }

    protected void DrawChildren()
    {
        foreach (var child in children)
        {
            child.Draw();
        }
    }

    #endregion


    #region Virtual Functions

    public virtual void DrawMe() 
    { 
        //Override me
    }

    public void Draw()
    {
        if (!uiActive)
        {
            GUI.enabled = false;
        }
            

        if (ShowBackground)
        {
            GUI.Box(absoluteRect, "");
        }

        if (Visible)
        {
            DrawMe();
        }
            

        if (!uiActive)
        {
            GUI.enabled = true;
        }
    }

    #endregion

    #region Public Functions

    public void SetActive(bool newActive, bool recursive)
    {
        uiActive = newActive;
        if (recursive)
        {
            foreach (var child in children)
            {
                child.SetActive(newActive, recursive);
            }
        }

    }

    public void UpdateHierarchy()
    {
        UpdateChildren();
        UpdateParent();

        foreach (var child in children)
        {
            child.UpdateHierarchy();
        }
    }

    public void UpdateUI()
    {
        UpdateRect();
        Draw();

        foreach (var child in children)
        {
            child.UpdateUI();
        }
    }

    #endregion


    #region Protected Functions

    protected void UpdateRect()
    {
        absoluteRect = new Rect(Position.Value.x, Position.Value.y, Size.Value.x, Size.Value.y);
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

    protected void RelativeToAbsolute(float width, float height)
    {
        if(Position.normalized){
            absoluteRect.x *= width;
            absoluteRect.y *= height;
        }
        if (Size.normalized)
        {
            absoluteRect.width *= width;
            absoluteRect.height *= height;
        }
    }

    #endregion
}
