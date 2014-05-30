using UnityEngine;

[System.Serializable]
[ExecuteInEditMode]
public class UIEditorPanel : UIPanel
{
    void OnGUI()
    {
        UpdateUI();

#if UNITY_EDITOR
        foreach (Transform child in transform)
        {
            UIRect childRect = child.GetComponent<UIRect>();
            if (childRect)
            {
                childRect.UpdateUI();
            }
        }
#endif
        
    }
}
