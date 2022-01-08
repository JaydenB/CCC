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
    class FeatherFallCard : CustomCard
    {

        /*
         *  Blocking makes you fall like a feather. Increased block cooldown.
         */

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            FeatherFall_Mono featherFallMono = player.gameObject.GetOrAddComponent<FeatherFall_Mono>();
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Feather Fall";
        }

        protected override string GetDescription()
        {
            return "Blocking makes you float like a feather until you touch ground.";
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
                    stat = "Temporary Gravity",
                    amount = "5%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Block Cooldown",
                    amount = "200%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                }
            };
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
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
    public class FeatherFall_Mono : MonoBehaviour
    {
        private CharacterData data;
        private CharacterStatModifiers stats;
        private Player player;
        private WeaponHandler weaponHandler;
        private Gun gun;
        private Block block;
        private Gravity gravity;

        private bool isActive = false;

        private readonly float gravityMult = 0.05f;

        private void Start()
        {
            this.data = base.GetComponentInParent<CharacterData>();
            this.player = this.data.player;
            this.stats = this.data.stats;
            this.weaponHandler = this.data.weaponHandler;
            this.gun = this.weaponHandler.gun;
            this.block = this.data.block;
            this.gravity = base.GetComponentInParent<Gravity>();

            block.BlockAction += OnBlockFeatherFall;

            this.isActive = false;
        }

        private void Update()
        {
            if (!this.isActive) return;
            if (this.data.isGrounded) ResetFeatherFall();
        }

        private void OnBlockFeatherFall(BlockTrigger.BlockTriggerType type)
        {
            if (this.isActive) return;

            this.isActive = true;
            this.gravity.gravityForce *= gravityMult;
        }

        private void ResetFeatherFall()
        {
            this.isActive = false;
            this.gravity.gravityForce /= gravityMult;
        }

        private void OnDestroy()
        {
            this.block.BlockAction -= OnBlockFeatherFall;
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}
