using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using ModdingUtils.RoundsEffects;
using ModdingUtils.MonoBehaviours;
using HarmonyLib;

namespace CCC.Cards
{
    class FireShieldCard : CustomCard
    {

        /*
         *  Blocking ignites players in a radius around you.
         */

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            FireShield_Mono fireShieldMono = player.gameObject.GetOrAddComponent<FireShield_Mono>();
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Fire Shield";
        }

        protected override string GetDescription()
        {
            return "Blocking Ignites Players in a small radius around you.";
        }

        protected override GameObject GetCardArt()
        {
            return null;
        }

        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Block Cooldown",
                    amount = "75%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                }
            };
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }

        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.MagicPink;
        }

        public override string GetModName()
        {
            return CCC.ModInitials;
        }
    }

    [DisallowMultipleComponent]
    public class FireShield_Mono : MonoBehaviour
    {
        private CharacterData data;
        private CharacterStatModifiers stats;
        private Player player;
        private WeaponHandler weaponHandler;
        private Gun gun;
        private Block block;

        private readonly float maxDistance = 3.0f;

        private void Start()
        {
            this.data = base.GetComponentInParent<CharacterData>();
            this.player = this.data.player;
            this.stats = this.data.stats;
            this.weaponHandler = this.data.weaponHandler;
            this.gun = this.weaponHandler.gun;
            this.block = this.data.block;

            block.BlockAction += OnBlockFireShield;
        }

        private void OnBlockFireShield(BlockTrigger.BlockTriggerType type)
        {
            List<Player> otherPlayers = PlayerManager.instance.players.Where(player => (player.playerID != this.player.playerID)).ToList();
            foreach (Player otherPlayer in otherPlayers)
            {
                Vector2 displacement = (otherPlayer.transform.position - this.player.transform.position);
                if ((displacement.magnitude < maxDistance) && PlayerManager.instance.CanSeePlayer(this.player.transform.position, otherPlayer).canSee)
                {
                    // Ignite otherPlayer
                    otherPlayer.gameObject.AddComponent<FireShield_Effect>();
                }
            }
        }

        private void ResetFeatherFall()
        {
        }

        private void OnDestroy()
        {
            this.block.BlockAction -= OnBlockFireShield;
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }

    public class FireShield_Effect : ReversibleEffect 
    {
        private float startTime;
        private readonly float timeOnFire = 2.0f;
        private float count = 0.0f;
        private float increment = 0.1f;

        private float fireMult = 0.075f;

        private CharacterData data;
        private HealthHandler heat;

        private readonly Color colour = new Color(1f, 0.3f, 0f, 1f);
        private ReversibleColorEffect colourEffect;

        public override void OnStart()
        {
            this.characterStatModifiersModifier.movementSpeed_mult *= 0.65f;
            this.heat = this.player.transform.GetComponent<HealthHandler>();
            this.data = this.player.transform.GetComponent<CharacterData>();

            this.ResetTimer();  // Set this.startTime to NOW

            this.colourEffect = this.player.gameObject.AddComponent<ReversibleColorEffect>();
            this.colourEffect.SetColor(this.colour);
            this.colourEffect.SetLivesToEffect(1);
        }

        public override void OnUpdate()
        {
            if (Time.time >= this.startTime + this.increment)
            {
                this.heat.DoDamage(
                    this.data.maxHealth / 5f * this.fireMult * Vector2.down,
                    this.player.transform.position,
                    this.colour,
                    null, null, false, true, true);

                this.ResetTimer();

                this.count += 0.1f;
                if (this.count >= this.timeOnFire)
                {
                    base.Destroy();
                }

                if (this.player.data.dead)
                {
                    this.ResetTimer();
                    base.Destroy();
                }
            }
        }

        public override void OnOnEnable()
        {
            if (this.colourEffect != null) { this.colourEffect.Destroy(); }
        }

        public override void OnOnDisable()
        {
            if (this.colourEffect != null) { this.colourEffect.Destroy();  }
        }

        public override void OnOnDestroy()
        {
            if (this.colourEffect != null) { this.colourEffect.Destroy(); }
        }

        private void ResetTimer()
        {
            this.startTime = Time.time;
        }
    }
}
