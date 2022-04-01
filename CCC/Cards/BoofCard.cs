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
    class BoofCard : CustomCard
    {

        /*
         *  Colliding with other players pushes knocks them back.
         */

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            Boof_Mono boofEffect = player.gameObject.GetOrAddComponent<Boof_Mono>();
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "Boof";
        }

        protected override string GetDescription()
        {
            return "Colliding with other players knocks them back.";
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
                    stat = "Collision Knockback",
                    amount = "Added",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.EvilPurple;
        }
        public override string GetModName()
        {
            return CCC.ModInitials;
        }
    }

    [DisallowMultipleComponent]
    public class Boof_Mono : MonoBehaviour
    {
        private CharacterData data;
        private Player player;
        private PlayerCollision playerCollision;

        private readonly float pushbackMultiplier = 40.0f;

        private void Start()
        {
            this.data = base.GetComponentInParent<CharacterData>();
            this.player = this.data.player;
            this.playerCollision = this.player.GetComponent<PlayerCollision>();

            this.playerCollision.collideWithPlayerAction += OnPlayerCollideBoof;
        }

        private void OnPlayerCollideBoof(Vector2 collisionPoint, Vector2 usToOtherDirection, Player otherPlayer)
        {
            otherPlayer.data.movement.Move((otherPlayer.transform.position - this.player.transform.position).normalized * pushbackMultiplier);
        }

        private void OnDestroy()
        {
            this.playerCollision.collideWithPlayerAction -= OnPlayerCollideBoof;
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}
