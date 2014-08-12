using UnityEngine;
using System.Collections;

public class GUIDing : MonoBehaviour {

    public GameObject ingameMenu;

	// Update is called once per frame
	void Update () {
        if (InputController.GetClicked("ESCAPE"))
        {
            if (GameManager.GamePaused && !ingameMenu.activeSelf)
                return;

            if (!GameManager.GamePaused)
            {
                ingameMenu.SetActive(true);
                GameEventHandler.TriggerOnPause();
            }
            else
            {
                ingameMenu.SetActive(false);
                GameEventHandler.TriggerOnResume();
            }
        }
	}
}
