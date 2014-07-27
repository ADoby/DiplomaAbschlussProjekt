using UnityEngine;
using System.Collections;

[System.Serializable]
public class Tab_Control : MonoBehaviour {

    public GameObject[] tabs;

    [SerializeField]
    private int lastTab = 0;

    void OnEnable()
    {
        ActivateTab();
    }

    public void ActivateTab()
    {
        if (lastTab < 0)
            lastTab = 0;
        if (lastTab >= tabs.Length)
            lastTab = tabs.Length - 1;

        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].SetActive(false);
        }
        tabs[lastTab].SetActive(true);
    }

    public void SetTab(int id)
    {
        lastTab = id;
        ActivateTab();
    }
}
