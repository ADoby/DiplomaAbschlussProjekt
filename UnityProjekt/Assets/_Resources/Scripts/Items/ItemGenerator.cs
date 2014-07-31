using UnityEngine;
using System.Collections;

public class ItemGenerator {
    
    private static string[] prefix = { "Boring", "Broken", "Comon", "Normal", "Good", "Better", "Nice", "Rare", "Super", "Godly" };

    private static string[] suffix = { "of Blubbering", "of Things", "of other Things" };

    private static System.Type[] itemTypes = { 
                                                 typeof(Item_DamageAmplifier), 
                                                 typeof(Item_AbsDamageReduction), 
                                                 typeof(Item_RelDamageReduction), 
                                                 typeof(Item_Vampire), 
                                                 typeof(Item_AbsHealthBonus), 
                                                 typeof(Item_RelHealthBonus), 
                                                 typeof(Item_GoldPerSecond), 
                                                 typeof(Item_RelMovementBonus), 
                                                 typeof(Item_RocketOnAttack) };

    public static Item GenerateItem(int value)
    {
        value = Mathf.Clamp(value, 0, prefix.Length - 1);

        System.Type itemType = itemTypes[(int)Random.Range(0, itemTypes.Length)];

        Item item = (Item)System.Activator.CreateInstance(itemType);
        item.prefixID = value;
        item.GenerateName(prefix[(value)], suffix[Random.Range(0, suffix.Length)]);
        item.UpdateStats((float)(value+1)/10f);

        return item;
    }

}
