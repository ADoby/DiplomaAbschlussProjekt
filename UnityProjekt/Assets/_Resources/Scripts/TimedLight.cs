using UnityEngine;
using System.Collections;

public class TimedLight : MonoBehaviour {

    public float time = 1.0f;
    public float timer = 0f;

    public Light TheLight;
    public float MaxIntensity = 2.0f;

    public void Reset()
    {
        timer = time;
        UpdateLight();
    }


    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateLight();
        }
        else if (timer < 0)
        {
            timer = 0;
            UpdateLight();
        }
    }

    private void UpdateLight()
    {
        TheLight.intensity = (timer / time) * MaxIntensity;
    }
}
