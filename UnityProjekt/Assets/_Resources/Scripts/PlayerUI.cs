using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class ValueChances
{
    public int value;
    public float weight;
}

[System.Serializable]
public class PlayerUI : MonoBehaviour {

    public PlayerController playerControl;

    public float UpdateTimer = 0.5f;

    

    public UIEditorPanel panel;

    public UIText level;
    public UIRect HealthBar;
    public UIText HealthText;
    public UIText Money;

    public UIText Exp;
    public UIRect ExpBar;


    public Tab_Control SkillAndItemMenu;
    public UIButton SkillTabButton;

    public UIRect skillPanel;

    public UIText CheckPointText;
    [SerializeField]
    private float currentHealth = 0, wantedHealth = 0;
    [SerializeField]
    private float currentMaxHealth = 0;

    [SerializeField]
    private float currentExp = 0, wantedExp = 0;
    [SerializeField]
    private float currentMoney = 0, wantedMoney = 0;

    public float speed = 2.0f;

    public UIText skillCD1, skillCD2, skillCD3, skillCD4;

    public string UIItemPoolName = "UIItem";

    public GameObject UIItemHolder;

    public int MoneyPerItem25 = 300, MoneyPerItem50 = 800, MoneyPerItem75 = 1500, MoneyPerItem100 = 3000;

    public ValueChances[] Chances25;
    public ValueChances[] Chances50;
    public ValueChances[] Chances75;
    public ValueChances[] Chances100;

    public UIButton Button25, Button50, Button75, Button100;

	// Use this for initialization
	void Start () {
        SkillAndItemMenu.GetComponent<UIRect>().Visible = showMenu;

        Button25.OnButtonClicked += GenerateItems25;
        Button50.OnButtonClicked += GenerateItems50;
        Button75.OnButtonClicked += GenerateItems75;
        Button100.OnButtonClicked += GenerateItems100;

        Button25.Text = String.Format("Buy Item for {0}", MoneyPerItem25);
        Button50.Text = String.Format("Buy Item for {0}", MoneyPerItem50);
        Button75.Text = String.Format("Buy Item for {0}", MoneyPerItem75);
        Button100.Text = String.Format("Buy Item for {0}", MoneyPerItem100);
	}

    public void GenerateItems25(UIRect rect)
    {
        if (!SkillsAndItemsUI.Visible)
            return;
        if (playerControl.Money < MoneyPerItem25)
            return;

        playerControl.Money -= MoneyPerItem25;
        GenerateItem(Chances25);
    }
    public void GenerateItems50(UIRect rect)
    {
        if (!SkillsAndItemsUI.Visible)
            return;
        if (playerControl.Money < MoneyPerItem50)
            return;

        playerControl.Money -= MoneyPerItem50;
        GenerateItem(Chances50);
    }
    public void GenerateItems75(UIRect rect)
    {
        if (!SkillsAndItemsUI.Visible)
            return;
        if (playerControl.Money < MoneyPerItem75)
            return;

        playerControl.Money -= MoneyPerItem75;
        GenerateItem(Chances75);
    }
    public void GenerateItems100(UIRect rect)
    {
        if (!SkillsAndItemsUI.Visible)
            return;
        if (playerControl.Money < MoneyPerItem100)
            return;

        playerControl.Money -= MoneyPerItem100;
        GenerateItem(Chances100);
    }

    public UIRect SkillsAndItemsUI;

    public void GenerateItem(ValueChances[] values)
    {
        var rnd = UnityEngine.Random.value;
        for (int i = 0; i < values.Length; i++)
        {
            if (rnd < values[i].weight)
            {
                AddItem(values[i].value);
                return;
            }
            rnd -= values[i].weight;
        }
    }

    public void AddItem(int value)
    {
        Item newItem = ItemGenerator.GenerateItem(value);

        playerControl.PlayerClass.AddItem(newItem);

        GameObject go = EntitySpawnManager.InstantSpawn(UIItemPoolName, Vector3.zero, Quaternion.identity, countEntity:false);
        go.transform.parent = UIItemHolder.transform;

        

        float y = (int)((playerControl.PlayerClass.items.Count-1) / 4) * 0.05f;
        float x = (int)((playerControl.PlayerClass.items.Count-1) % 4) * 0.25f;
        go.GetComponent<UIButton>().Text = newItem.Description;
        go.GetComponent<UIButton>().RelativePosition.x = x;
        go.GetComponent<UIButton>().RelativePosition.y = y;

        UIItemHolder.GetComponent<UIRect>().AddChild(go.GetComponent<UIButton>());
        UIItemHolder.GetComponent<UIRect>().UpdateChildren();

        if (UIItemHolder.GetComponent<UIScrollPanel>())
        {
            UIItemHolder.GetComponent<UIScrollPanel>().RelHeight = (y + 0.05f);
        }
    }

    public bool showMenu = false;

    void Update()
    {
        if (playerControl.playerUI == null)
            playerControl.playerUI = this;

        if (InputController.GetClicked(String.Format("{0}_SKILLMENU", playerControl.PlayerID())))
        {
            if (GameManager.GamePaused && !showMenu)
                return;

            showMenu = !showMenu;
            SkillAndItemMenu.GetComponent<UIRect>().Visible = showMenu;
            if (showMenu)
            {
                GameEventHandler.TriggerOnPause();
                SkillAndItemMenu.ActivateTab();
            }
            else
            {
                GameEventHandler.TriggerOnResume();
            }
        }

        if (GameManager.GamePaused)
        {
            return;
        }

        UpdateUI();

        currentExp = Mathf.Lerp(currentExp, wantedExp, Time.deltaTime * speed);
        currentMoney = Mathf.Lerp(currentMoney, wantedMoney, Time.deltaTime * speed);
        currentHealth = Mathf.Lerp(currentHealth, wantedHealth, Time.deltaTime * speed);

        ChangeUI();
    }

    private void ChangeUI()
    {
        level.Text = String.Format("{0:##0}", playerControl.Level);
        HealthBar.RelativeSize.x = currentHealth / currentMaxHealth;

        HealthText.Text = String.Format("{0:###0}/{1:###0}", currentHealth, currentMaxHealth);

        Money.Text = String.Format("Money:{0:#####0}", currentMoney);

        Exp.Text = String.Format("Experience:{0:##0%}", currentExp);
        ExpBar.RelativeSize.x = currentExp;

        skillCD1.Text = String.Format("{0:#0.0}", playerControl.PlayerClass.playerSkills[0].Cooldown);
        skillCD2.Text = String.Format("{0:#0.0}", playerControl.PlayerClass.playerSkills[1].Cooldown);
        skillCD3.Text = String.Format("{0:#0.0}", playerControl.PlayerClass.playerSkills[2].Cooldown);
        skillCD4.Text = String.Format("{0:#0.0}", playerControl.PlayerClass.playerSkills[3].Cooldown);

        CheckPointText.Text = String.Format("Checkpoint:{0:##0%}", playerControl.ProcentageCheckpointTimer);

        

        Button25.Enabled = false;
        Button50.Enabled = false;
        Button75.Enabled = false;
        Button100.Enabled = false;

        if (playerControl.Money >= MoneyPerItem25)
            Button25.Enabled = true;
        if (playerControl.Money >= MoneyPerItem50)
            Button50.Enabled = true;
        if (playerControl.Money >= MoneyPerItem75)
            Button75.Enabled = true;
        if (playerControl.Money >= MoneyPerItem100)
            Button100.Enabled = true;
    }

    public void UpdateUI()
    {
        wantedHealth = playerControl.PlayerClass.CurrentHealth;
        currentMaxHealth = playerControl.PlayerClass.GetAttributeValue(AttributeType.HEALTH);

        wantedMoney = playerControl.Money;

        wantedExp = ((playerControl.CurrentExperience - playerControl.PrevNeededExperience) / (playerControl.NeededExperience - playerControl.PrevNeededExperience));
        wantedExp = Mathf.Clamp(wantedExp, 0f, 1f);

        UpdateSkillPoints();

        if (playerControl.PlayerClass.skillPoints > 0)
            skillPanel.Enabled = true;
        else
            skillPanel.Enabled = false;
    }

    public void UpdateSkillPoints()
    {
        SkillTabButton.Text = String.Format("Skill/Attribute({0})", playerControl.PlayerClass.skillPoints);
    }

    public void UpdateAfterSkillPointUP()
    {
        UpdateUI();
        currentMoney = wantedMoney;
        ChangeUI();
    }
}
