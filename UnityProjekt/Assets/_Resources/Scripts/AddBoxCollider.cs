using UnityEngine;
using System.Collections;

public class AddBoxCollider : MonoBehaviour {

	// Use this for initialization
    void Awake()
    {
        BoxCollider2D[] floors = GetComponentsInChildren<BoxCollider2D>();

        foreach (var item in floors)
        {

            GameObject child = new GameObject("ChildCollider");
            child.transform.parent = item.transform;
            child.transform.localPosition = Vector3.zero;
            BoxCollider boxCollider = child.AddComponent<BoxCollider>();

            boxCollider.size = new Vector3(item.size.x * item.transform.localScale.x, item.size.y * item.transform.localScale.y, 100);
            boxCollider.center = new Vector3(item.center.x, item.center.y, 0);
        }
    }
}
