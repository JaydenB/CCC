using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using ModdingUtils.MonoBehaviours;
using HarmonyLib;

namespace CCC.Cards
{
    class DoofCard : CustomCard
    {

        /*
         *  Blocking Pushes Back nearby Players from you.
         */

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            // Modify Block Cooldown Multiplayer
            HealthBasedEffect healthEffect = player.gameObject.AddComponent<HealthBasedEffect>();
            healthEffect.blockModifier.cdMultiplier_mult = 0.75f;

            // Check or Add the BlockBlockEffect Class as a Component to the player
            Doof_Mono doofEffect = player.gameObject.GetOrAddComponent<Doof_Mono>();
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Doof";
        }
        protected override string GetDescription()
        {
            return "Blocking pushes back nearby players.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Doof",
                    amount = "+100%",
                    simepleAmount = CardInfoStat.SimpleAmount.aHugeAmountOf
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Block Cooldown",
                    amount = "-25%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotLower
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DefensiveBlue;
        }
        public override string GetModName()
        {
            return CCC.ModInitials;
        }
    }

    [DisallowMultipleComponent]
    public class Doof_Mono : MonoBehaviour
    {
        private CharacterData data;
        private Player player;
        private Block block;

        private readonly float pushbackMultiplier = 150.0f;
        private readonly float maxDistance = 20.0f;

        private void Start()
        {
            this.data = base.GetComponentInParent<CharacterData>();
            this.player = this.data.player;
            this.block = this.data.block;

            block.BlockAction += OnBlock;
        }

        private void OnBlock(BlockTrigger.BlockTriggerType type)
        {
            List<Player> otherPlayers = PlayerManager.instance.players.Where(player => (player.playerID != this.player.playerID)).ToList();
            Vector2 displacement;
            foreach (Player otherPlayer in otherPlayers)
            {
                displacement = (otherPlayer.transform.position - this.player.transform.position);
                if ((displacement.magnitude < maxDistance) && PlayerManager.instance.CanSeePlayer(this.player.transform.position, otherPlayer).canSee)
                {
                    Vector2 pushback = (1.0f - (displacement.magnitude / maxDistance)) * displacement.normalized * pushbackMultiplier;

                    otherPlayer.data.movement.Move(pushback);
                }
            }
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
