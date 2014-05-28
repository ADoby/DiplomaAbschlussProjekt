using UnityEngine;
using System.Collections.Generic;

public abstract class UIRect : MonoBehaviour
{
    public Rect rect;

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
        UIRect[] childArray = GetComponentsInChildren<UIRect>();
        children.AddRange(childArray);
    }

    #endregion

    abstract public void Draw();

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
