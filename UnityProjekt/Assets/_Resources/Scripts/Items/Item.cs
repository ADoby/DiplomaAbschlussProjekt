using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class Item {

    protected string[] colors = { 
                                    "#A0A0A0", "#D0D0D0", "#FFFFFF", "#33FF66", "#00FF00", 
                                    "#FFFF33", "#FF66CC", "#CC3366", "#CC9933", "#FF6600"};

    protected virtual string[] names
    {
        get
        {
            return new string[] { "Item" };
        }
    }

    public int prefixID = 0;

    public string Name = "Item";
    public virtual string Description
    {
        get
        {
            return "<color=" + colors[prefixID] + ">" + Name + "</color>";
        }
    }

    public void GenerateName(string prefix = "", string suffix = "")
    {
        Name = prefix + " " + names[Random.Range(0, names.Length)] + " " + suffix;
    }

    public abstract void UpdateStats(float value);

    //Once when item gets created
    public virtual void Start(PlayerClass playerClass)
    {

    }

    //update every frame for now
    public virtual void Update(PlayerClass playerClass)
    {

    }

    //when item gets deleted
    public virtual void Delete(PlayerClass playerClass)
    {

    }

    public virtual float OnPlayerGetsDamage(PlayerClass playerClass, float damage)
    {
        return damage;
    }
    public virtual void OnPlayerDamaged(PlayerClass playerClass, float damage)
    {

    }
    public virtual float OnPlayerDoesDamage(PlayerClass playerClass, float damage)
    {
        return damage;
    }
    public virtual void OnPlayerDidDamage(PlayerClass playerClass, float damage)
    {

    }
    public virtual void OnPlayerKilledEntity(PlayerClass playerClass)
    {

    }

    public virtual float OnPlayerLethalDamage(PlayerClass playerClass, float damage)
    {
        return damage;
    }
    public virtual void OnPlayerDied(PlayerClass playerClass)
    {

    }
}
