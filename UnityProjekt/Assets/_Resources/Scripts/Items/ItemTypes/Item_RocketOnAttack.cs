using UnityEngine;

    public class Item_RocketOnAttack : Item
    {
        protected override string[] names
        {
            get
            {
                return new string[] { "Glove", "Helmet", "Shield" };
            }
        }

        public override string Description
        {
            get
            {
                return "<color=" + colors[prefixID] + ">" + Name + "</color>\n<color=#226622>Rocket Damage: " + RocketDamage.ToString("##0") + "</color>";
            }
        }
        
        public float RocketDamage = 100f;
        public float RocketDamagePerLevel = 10f;

        public string RocketPoolName = "SelfFindingRocket";

        public float cooldown = 0.5f;
        public float timer = 0f;

        public override void UpdateStats(float value)
        {
            base.UpdateStats(value);
            RocketDamage *= value;
        }

        public override void Start(PlayerClass playerClass)
        {
            base.Start(playerClass);
            RocketDamage += RocketDamagePerLevel * Value * playerClass.playerControl.Level;
        }

        public override void Update(PlayerClass playerClass)
        {
            base.Update(playerClass);
            timer -= Time.deltaTime;
        }

        public override void OnPlayerLevelUp(PlayerClass playerClass)
        {
            base.OnPlayerLevelUp(playerClass);
            RocketDamage += RocketDamagePerLevel * Value;
        }

        public override void OnPlayerDidDamage(PlayerClass playerClass, Damage damage)
        {
            base.OnPlayerDidDamage(playerClass, damage);
            if (timer <= 0)
            {
                timer = cooldown;
                SpawnRocket(playerClass);
            }
        }

        public void RocketSpawned(GameObject go)
        {
            go.GetComponent<Rocket>().Impulse(Vector3.up * 10.0f);
        }

        private void SpawnRocket(PlayerClass playerClass)
        {
            EntitySpawnManager.Spawn(RocketPoolName, playerClass.playerTransform.position + Vector3.up*2f, Quaternion.identity, callBack:RocketSpawned);
        }
    }

