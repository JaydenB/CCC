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
    class WatchThisCard : CustomCard
    {

        /*
         *  Blocking causes you to jump in the air with decreased gravity. Deal 25% more damage until you land.
         */

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WatchThis_Mono watchThisMono = player.gameObject.GetOrAddComponent<WatchThis_Mono>();
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Watch This!";
        }

        protected override string GetDescription()
        {
            return "Blocking jumps you in the air, and kill with style!";
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
                    amount = "15%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotOf
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Temporary DMG",
                    amount = "+50%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Temporary Knockback",
                    amount = "x2.5%",
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
    public class WatchThis_Mono: MonoBehaviour
    {
        private CharacterData data;
        private CharacterStatModifiers stats;
        private Player player;
        private WeaponHandler weaponHandler;
        private Gun gun;
        private Block block;
        private Gravity gravity;

        private bool isActive = false;

        private readonly float gravityMult = 0.15f;
        private readonly float damageMult = 1.5f;
        private readonly float knockbackMult = 2.5f;

        private void Start()
        {
            this.data = base.GetComponentInParent<CharacterData>();
            this.player = this.data.player;
            this.stats = this.data.stats;
            this.weaponHandler = this.data.weaponHandler;
            this.gun = this.weaponHandler.gun;
            this.block = this.data.block;
            this.gravity = base.GetComponentInParent<Gravity>();

            block.BlockAction += OnBlock;

            this.isActive = false;
        }

        private void Update()
        {
            if (!this.isActive) return;
            if (this.data.isGrounded) ResetWatchThis();
        }

        private void OnBlock(BlockTrigger.BlockTriggerType type)
        {
            if (this.isActive) return;

            this.isActive = true;

            this.data.jump.Jump(true, 1.5f);

            this.gravity.gravityForce *= gravityMult;
            this.gun.damage *= damageMult;
            this.gun.knockback *= knockbackMult;
        }

        private void ResetWatchThis()
        {
            this.isActive = false;

            this.gravity.gravityForce /= gravityMult;
            this.gun.damage /= damageMult;
            this.gun.knockback /= knockbackMult;
        }

        private void OnDestroy()
        {
            this.block.BlockAction -= OnBlock;
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}
