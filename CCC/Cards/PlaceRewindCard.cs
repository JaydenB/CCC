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
    class PlaceRewindCard : CustomCard
    {

        /*
         *  Ability to place a rewind point and go back to there within the current round.
         */

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            block.cdMultiplier *= 1.5f;
            PlaceRewind_Mono placeRewindEffect = player.gameObject.GetOrAddComponent<PlaceRewind_Mono>();
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Place Rewind";
        }

        protected override string GetDescription()
        {
            return "Place a rewind point by blocking, which you can go back once when you block!";
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
                    stat = "Block Cooldown",
                    amount = "1.5x",
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
            return CardThemeColor.CardThemeColorType.DestructiveRed;
        }

        public override string GetModName()
        {
            return CCC.ModInitials;
        }

        internal static IEnumerator ResetAllRewinds()
        {
            foreach (GameObject gameObject in FindObjectsOfType(typeof(GameObject)) as GameObject[])
            {
                PlaceRewind_Mono placeRewind = gameObject.GetComponent<PlaceRewind_Mono>();
                if (placeRewind)
                {
                    placeRewind.ResetRewind();
                }
            }
            yield break;
        }
    }

    [DisallowMultipleComponent]
    public class PlaceRewind_Mono: MonoBehaviour
    {
        private CharacterData data = null;
        private Player player = null;
        private Block block = null;

        private Vector2 rewindPoint;
        private bool rewindAvailable = true;

        private void Start()
        {
            this.data = base.GetComponentInParent<CharacterData>();
            this.player = this.data.player;
            this.block = this.data.block;

            // Setup Actions
            block.BlockAction += OnBlockPlaceRewind;

            this.rewindPoint = Vector2.zero;
        }

        private void OnBlockPlaceRewind(BlockTrigger.BlockTriggerType type)
        {
            if (this.rewindAvailable)
            {
                this.rewindPoint = base.transform.position;
                this.rewindAvailable = false;
            }
            else
            {
                if (this.rewindPoint == Vector2.zero) { return; }

                this.player.transform.position = this.rewindPoint;
                this.player.transform.GetComponentInChildren<PlayerCollision>().IgnoreWallForFrames(2);

                this.rewindAvailable = true;
            }
        }

        public void ResetRewind()
        {
            this.rewindAvailable = true;
            this.rewindPoint = Vector2.zero;
        }

        private void OnDestroy()
        {
            this.block.BlockAction -= OnBlockPlaceRewind;
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}
