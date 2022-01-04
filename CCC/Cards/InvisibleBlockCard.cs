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
    class InvisibleBlockCard : CustomCard
    {

        /*
         *  Blocking makes you partially invisible temporarily
         */

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            HealthBasedEffect healthEffect = player.gameObject.AddComponent<HealthBasedEffect>();
            healthEffect.blockModifier.cdMultiplier_mult = 2.5f;
            data.maxHealth *= 0.75f;

            InvisibleBlock_Mono invisBlockMono = player.gameObject.GetOrAddComponent<InvisibleBlock_Mono>();
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Invisibility Block";
        }

        protected override string GetDescription()
        {
            return "Blocking makes you partially invisible for 4 seconds.";
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
                    stat = "Max Health",
                    amount = "-25%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotOf
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Block Cooldown",
                    amount = "x2.5",
                    simepleAmount = CardInfoStat.SimpleAmount.aHugeAmountOf
                }
            };
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
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

    public class InvisibleBlock_Mono : MonoBehaviour
    {
        private CharacterData data = null;
        private Player player = null;
        private Block block = null;

        private void Start()
        {
            this.data = base.GetComponentInParent<CharacterData>();
            this.player = this.data.player;
            this.block = this.data.block;

            block.BlockAction += OnBlockInvisible;
        }

        private void OnBlockInvisible(BlockTrigger.BlockTriggerType type)
        {
            this.player.gameObject.GetOrAddComponent<InvisibleBlock_Effect>();
        }

        private void OnDestroy()
        {
            this.block.BlockAction -= OnBlockInvisible;
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }

    public class InvisibleBlock_Effect : ReversibleEffect
    {
        private ReversibleColorEffect colourEffect = null;
        private Color colour = Color.clear;

        private float startTime;
        private readonly float invisibilityLength = 4.0f;

        public override void OnOnEnable()
        {
            if (this.colourEffect != null) { this.colourEffect.Destroy(); }
        }

        public override void OnStart()
        {
            this.colourEffect = base.player.gameObject.AddComponent<ReversibleColorEffect>();
            this.colourEffect.SetColor(this.colour);
            this.colourEffect.SetLivesToEffect(1);

            this.startTime = Time.time;
        }

        public override void OnUpdate()
        {
            if (Time.time >= this.startTime + this.invisibilityLength)
            {
                base.Destroy();
            }
        }

        public override void OnOnDisable()
        {
            if (this.colourEffect != null) { this.colourEffect.Destroy(); }
        }

        public override void OnOnDestroy()
        {
            if (this.colourEffect != null) { this.colourEffect.Destroy(); }
        }
    }
}
