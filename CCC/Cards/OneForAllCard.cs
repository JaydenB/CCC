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
using Photon.Pun;
using Sonigon;

namespace CCC.Cards
{
    class OneForAllCard : CustomCard
    {

        /*
         *  Heavily Increased Knockback with physics force on nearby objects and players.
         */

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.knockback *= 8.0f;
            gunAmmo.maxAmmo = 1;
            gun.projectileSpeed *= 0.5f;
            block.cdMultiplier = 0.5f;
            characterStats.jump *= 1.5f;

            OneForAll_Mono oneForAllEffect = player.gameObject.GetOrAddComponent<OneForAll_Mono>();
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }

        protected override string GetTitle()
        {
            return "One For All";
        }

        protected override string GetDescription()
        {
            return "Harness the powers of 'One For All' to pushback nearby players and objects on firing.";
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
                    stat = "Knockback",
                    amount = "8x",
                    simepleAmount = CardInfoStat.SimpleAmount.aHugeAmountOf
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Jump Height",
                    amount = "150%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Bullet Speed",
                    amount = "50%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotLower
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Block Cooldown",
                    amount = "50%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotLower
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Bullet Ammo",
                    amount = "1",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }

        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.TechWhite;
        }

        public override string GetModName()
        {
            return CCC.ModInitials;
        }
    }

    [DisallowMultipleComponent]
    public class OneForAll_Mono : MonoBehaviour
    {
        private Player player;
        private CharacterData data;
        private WeaponHandler weaponHandler;
        private Gun gun;

        private readonly float pushbackDistanceCap = 8.0f;
        private readonly float pushbackMultiplier = 350.0f;

        private void Start()
        {
            this.data = base.GetComponentInParent<CharacterData>();
            this.player = this.data.player;
            this.weaponHandler = this.data.weaponHandler;
            this.gun = this.weaponHandler.gun;

            this.gun.ShootPojectileAction += OnShootDetroitSmash;
        }

        private void OnShootDetroitSmash(GameObject obj)
        {
            // Get Players nearby and knockback
            List<Player> otherPlayers = PlayerManager.instance.players.Where(player => (player.playerID != this.player.playerID)).ToList();
            foreach (Player otherPlayer in otherPlayers)
            {
                Vector2 displacement = (otherPlayer.transform.position - this.player.transform.position);
                if ((displacement.magnitude < pushbackDistanceCap) && PlayerManager.instance.CanSeePlayer(this.player.transform.position, otherPlayer).canSee){
                    otherPlayer.data.movement.Move(displacement * pushbackMultiplier);
                }
            }

            // Get NetworkedPhysicsObjects nearby and knockback
            Collider2D[] collider2ds = Physics2D.OverlapCircleAll(base.transform.position, this.pushbackDistanceCap);
            for (int i = 0; i < collider2ds.Length; i++)
            {
                PhotonMapObject physicsObject = collider2ds[i].GetComponent<PhotonMapObject>();
                if (physicsObject)
                {
                    Rigidbody2D rigb = physicsObject.GetComponentInParent<Rigidbody2D>();
                    if (rigb)
                    {
                        Vector2 displacement = (collider2ds[i].gameObject.transform.position - this.player.transform.position);
                        rigb.AddForce(displacement.normalized * (1.0f - (displacement.magnitude/this.pushbackDistanceCap)) * this.pushbackMultiplier * rigb.mass);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            this.gun.ShootPojectileAction -= OnShootDetroitSmash;
        }
        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}
