using UnityEngine;
using System.Collections;

[System.Serializable]
public class UIItemPickup : MonoBehaviour
{

    public string poolName = "UIItemPickup";

    public string text = "";

    public float YMovement = 1.0f;

    public float timing = 2.0f;
    public float timer = 0f;

    public GUIStyle labelStyle;

    public bool initiated = false;

    public float diffY = 0f;

    void Update()
    {
        if(timer > 0)
            timer -= Time.deltaTime;

        diffY += Time.deltaTime * YMovement;
        if (timer < 0) 
        {
            timer = 0;
            GameObjectPool.Instance.Despawn(poolName, gameObject);
        }

        if (initiated)
        {
            labelStyle.normal.textColor = new Color(1f,1f,1f,timer/timing);
            
        }
    }

    public void Reset()
    {
        timer = timing;
        diffY = 0;
    }

    void OnGUI()
    {
        if (!initiated)
        {
            labelStyle = new GUIStyle(GUI.skin.label);
            initiated = true;
            labelStyle.alignment = TextAnchor.MiddleCenter;
        }
            

        for (int i = 0; i < GameManager.Instance.GetCameras().Length; i++)
        {
            if (GameManager.Instance.GetCameras()[i] != null)
            {
                Vector2 pos = GameManager.Instance.GetCameras()[i].camera.WorldToScreenPoint(transform.position);
                pos.y = Screen.height - pos.y;
                GUI.Label(new Rect(pos.x, pos.y - 20 * diffY, 200, 60), text);
            }
        }
    }
}
