using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class Item {

    [SerializeField]
    protected string[] colors = { 
                                    "#A0A0A0", "#D0D0D0", "#FFFFFF", "#33FF66", "#00FF00", 
                                    "#FFFF33", "#FF66CC", "#CC3366", "#CC9933", "#FF6600"};

    [SerializeField]
    protected virtual string[] names
    {
        get
        {
            return new string[] { "Item" };
        }
    }

    public int prefixID = 0;

    public float Value = 1f;

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

    public virtual void UpdateStats(float value)
    {
        Value = value;
    }

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

    public virtual void OnPlayerGetsDamage(PlayerClass playerClass, ref Damage damage)
    {

    }
    public virtual void OnPlayerDamaged(PlayerClass playerClass, Damage damage)
    {

    }
    public virtual void OnPlayerDoesDamage(PlayerClass playerClass, ref Damage damage)
    {

    }
    public virtual void OnPlayerDidDamage(PlayerClass playerClass, Damage damage)
    {

    }
    public virtual void OnPlayerKilledEntity(PlayerClass playerClass)
    {

    }

    public virtual void OnPlayerLethalDamage(PlayerClass playerClass, ref Damage damage)
    {

    }
    public virtual void OnPlayerDied(PlayerClass playerClass)
    {

    }

    public virtual void OnPlayerLevelUp(PlayerClass playerClass)
    {

    }
}
