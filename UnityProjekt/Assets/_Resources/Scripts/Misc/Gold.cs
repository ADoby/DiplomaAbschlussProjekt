using UnityEngine;
using System.Collections;

public class Gold : Drop {

    public int amount = 10;
    public float amountPerDifficulty = 1;

    public override void TriggerHit(Collider2D other)
    {
        if (other && other.gameObject && other.gameObject.tag == "Player")
        {
            other.gameObject.SendMessage("AddMoney", amount + (int)(amountPerDifficulty * GameManager.Instance.CurrentDifficulty), SendMessageOptions.DontRequireReceiver);
        }
    }
}
