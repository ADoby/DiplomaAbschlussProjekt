using UnityEngine;

[ExecuteInEditMode]
public class UIEditorPanel : UIPanel
{

    void Start()
    {
        UpdateChildren();
    }

    void OnGUI()
    {
        UpdateUI();
    }

    public void UpdateChilds()
    {
        UIRect[] newChildren = gameObject.GetComponentsInChildren<UIRect>();

        foreach (var child in newChildren)
        {
            child.UpdateChildren();
            //child.UpdateUI();
        }
    }
}
