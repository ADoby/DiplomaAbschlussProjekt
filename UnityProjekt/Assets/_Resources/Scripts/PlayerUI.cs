using UnityEngine;
using System.Collections;

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


    public GameObject SkillAndItemMenu;
    public UIButton SkillTabButton;

    public UIRect skillPanel;

    private float currentHealth = 0, wantedHealth = 0;
    private float currentMaxHealth = 0;

    private float currentExp = 0, wantedExp = 0;

    private float currentMoney = 0, wantedMoney = 0;

    public float speed = 2.0f;

	// Use this for initialization
	void Start () {
        SkillAndItemMenu.SetActive(false);

        StartCoroutine(UpdateUI());
	}


    void Update()
    {
        currentExp = Mathf.Lerp(currentExp, wantedExp, Time.deltaTime * speed);
        currentMoney = Mathf.Lerp(currentMoney, wantedMoney, Time.deltaTime * speed);
        currentHealth = Mathf.Lerp(currentHealth, wantedHealth, Time.deltaTime * speed);

        ChangeUI();

        if (InputController.GetClicked(playerControl.PlayerID() + "_SKILLMENU"))
        {
            SkillAndItemMenu.SetActive(!SkillAndItemMenu.activeSelf);
            if (SkillAndItemMenu.activeSelf)
            {
                GameEventHandler.TriggerOnPause();
            }
            else
            {
                GameEventHandler.TriggerOnResume();
            }
        }
    }

    private void ChangeUI()
    {
        level.Text = playerControl.Level.ToString("##0");
        HealthBar.RelativeSize.x = currentHealth / currentMaxHealth;

        HealthText.Text = currentHealth.ToString("###0") + "/" + currentMaxHealth.ToString("###0");

        Money.Text = "Money:" + currentMoney.ToString("#####0");

        Exp.Text = "Experience:" + (currentExp * 100).ToString("##0") + "%";
        ExpBar.RelativeSize.x = currentExp;
    }

    public IEnumerator UpdateUI()
    {
        wantedHealth = playerControl.PlayerClass.CurrentHealth;
        currentMaxHealth = playerControl.PlayerClass.GetAttributeValue(AttributeType.HEALTH);

        wantedMoney = playerControl.Money;

        wantedExp = ((playerControl.CurrentExperience - playerControl.PrevNeededExperience) / (playerControl.NeededExperience - playerControl.PrevNeededExperience));

        SkillTabButton.Text = "Skill/Attribute(" + playerControl.PlayerClass.skillPoints.ToString() + ")";

        if (playerControl.PlayerClass.skillPoints > 0)
            skillPanel.Enabled = true;
        else
            skillPanel.Enabled = false;

        yield return new WaitForSeconds(UpdateTimer);
        StartCoroutine(UpdateUI());
    }
}
