using UnityEngine;

[ExecuteInEditMode]
public class UIEditorPanel : UIPanel
{

    void Start()
    {
        if(Application.isPlaying)
            UpdateChildren();
    }

    void OnGUI()
    {
        UpdateUI();
    }
}
