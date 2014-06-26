using UnityEngine;
using System.Collections;

public class AddBoxCollider : MonoBehaviour
{

    public string WantedLayer = "Level";

	// Use this for initialization
    void Start()
    {
        BoxCollider2D[] floors = GetComponentsInChildren<BoxCollider2D>();

        foreach (var item in floors)
        {

            GameObject child = new GameObject("ChildCollider");
            child.layer = LayerMask.NameToLayer(WantedLayer);
            child.transform.parent = item.transform;

            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
            

            BoxCollider boxCollider = child.AddComponent<BoxCollider>();

            boxCollider.size = new Vector3(item.size.x * item.transform.localScale.x, item.size.y * item.transform.localScale.y, 100);
            boxCollider.center = new Vector3(item.center.x, item.center.y, 0);
        }
    }
}
