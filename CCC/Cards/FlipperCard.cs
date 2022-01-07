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
    class FlipperCard : CustomCard
    {

        /*
         *  Flips some of your key stats around the 1.0 default point. 1.5->0.5 1.2->0.8 ish
         */

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            // Max Health
            data.maxHealth = data.maxHealth > 100.0f ? 100.0f - (data.maxHealth - 100.0f) : 100.0f + (100.0f - data.maxHealth)*2.0f;
            if (data.maxHealth < 25.0f) { data.maxHealth = 50.0f; }

            // Attack DMG
            gun.damage = gun.damage > 1.0f ? 1.0f - (gun.damage - 1.0f) : 1.0f + (1.0f - gun.damage)*2.0f;
            if (gun.damage < 0.25f) { gun.damage = 0.25f; }

            // Attack Speed
            gun.attackSpeed = gun.attackSpeed > 0.3f ? 0.3f - (gun.attackSpeed - 0.3f) : 0.3f + (0.3f - gun.attackSpeed) * 2.0f;
            if (gun.attackSpeed < 0.05f) { gun.attackSpeed = 0.05f; }

            // Knockback
            gun.knockback = gun.knockback > 1.0f ? 1.0f - (gun.knockback - 1.0f) : 1.0f + (1.0f - gun.knockback) * 2.0f;
            if (gun.knockback < 0.25f) { gun.knockback = 0.25f; }
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Flipper";
        }

        protected override string GetDescription()
        {
            return "Flips some stats around their default point!";
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
                    positive = true,
                    stat = "Max Health",
                    amount = "Flipped",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Attack DMG",
                    amount = "Flipped",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Attack Speed",
                    amount = "Flipped",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Knockback",
                    amount = "Flipped",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }

        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.NatureBrown;
        }

        public override string GetModName()
        {
            return CCC.ModInitials;
        }
    }
}
