using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnboundLib.GameModes;
using UnityEngine;
using ModdingUtils.RoundsEffects;
using ModdingUtils.MonoBehaviours;
using HarmonyLib;

namespace CCC.Cards
{
    class BulkUpCard : CustomCard
    {

        /*
         *  More Damage. Grow in size and move slower when you take damage.
         */

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            characterStats.health *= 2.0f;
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Bulk Up!";
        }

        protected override string GetDescription()
        {
            return "Grow when you take damage! More health, slower speed.";
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
                    stat = "Health",
                    amount = "2x",
                    simepleAmount = CardInfoStat.SimpleAmount.aHugeAmountOf
                }
            };
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }

        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.EvilPurple;
        }

        public override string GetModName()
        {
            return CCC.ModInitials;
        }

        internal static IEnumerator ResetAllBulkUp()
        {
            foreach (GameObject gameObject in FindObjectsOfType(typeof(GameObject)) as GameObject[])
            {
                BulkUp_Mono bulkUp = gameObject.GetComponent<BulkUp_Mono>();
                if (bulkUp)
                {
                    bulkUp.ResetBulkUp();
                }
            }
            yield break;
        }
    }

    [DisallowMultipleComponent]
    public class BulkUp_Mono : MonoBehaviour
    {
        private CharacterData data = null;
        private Player player = null;

        private int hitsTaken = 0;

        private readonly float movementSpeedMult = 0.8f;
        private readonly float jumpMult = 0.8f;

        private void Start()
        {
            this.data = base.GetComponentInParent<CharacterData>();
            this.player = this.data.player;
        }

        private void ProcessHit()
        {
            this.hitsTaken += 1;

            this.data.stats.movementSpeed *= this.movementSpeedMult;
            this.data.stats.jump *= this.jumpMult;
        }

        public void ResetBulkUp()
        {
            this.data.stats.movementSpeed /= this.movementSpeedMult * this.hitsTaken;
            this.data.stats.jump /= this.jumpMult * this.hitsTaken;

            this.hitsTaken = 0;
        }
    }
}