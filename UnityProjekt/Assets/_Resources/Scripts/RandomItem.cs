using UnityEngine;
using System.Collections;

public enum ItemType
{
    Item25,
    Item50,
    Item75,
    Item100
}

public class RandomItem : Drop 
{

    public ItemType itemType;


    public override void TriggerHit(Collider2D other)
    {
        if (other && other.gameObject && other.gameObject.tag == "Player")
        {
            other.gameObject.SendMessage("GenerateItem", itemType, SendMessageOptions.DontRequireReceiver);
        }
    }
}
