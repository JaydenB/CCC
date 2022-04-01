using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using ModdingUtils.MonoBehaviours;
using HarmonyLib;
using Photon.Pun;
using Sonigon;

namespace CCC.Cards
{
    class BulletMagnetCard : CustomCard
    {

        /*
         *  Other bullets attempt to aim at you.
         */

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.gravity *= 0.75f;
            gunAmmo.reloadTime *= 0.75f;
            gun.ammo += 3;

            BMagnet_Mono bulletMagnetMono = player.gameObject.GetOrAddComponent<BMagnet_Mono>();

            List<Player> otherPlayers = PlayerManager.instance.players.Where(p => (p.playerID != player.playerID)).ToList();
            foreach (Player otherPlayer in otherPlayers)
            {
                Gun otherGun = otherPlayer.data.weaponHandler.gun;

                List<ObjectsToSpawn> list_other = otherGun.objectsToSpawn.ToList<ObjectsToSpawn>();
                list_other.Add(new ObjectsToSpawn
                {
                    AddToProjectile = new GameObject("A_RepellBullet", new Type[]
                    {
                    typeof(BMagnetBullet_Mono)
                    })
                });
                otherGun.objectsToSpawn = list_other.ToArray();
            }
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            BMagnet_Mono bulletMagnetMono = player.gameObject.GetComponent<BMagnet_Mono>();
            if (bulletMagnetMono != null) { Destroy(bulletMagnetMono); }
        }

        protected override string GetTitle()
        {
            return "Bullet Magnet";
        }

        protected override string GetDescription()
        {
            return "Other player's bullets try to aim for you.";
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
                    stat = "Ammo",
                    amount = "+3",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Reload Time",
                    amount = "-25%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotOf
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bullet Gravity",
                    amount = "-25%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                }
            };
        }

        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
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
    public class BMagnet_Mono : MonoBehaviour
    {
        private Player player;
        private void Awake()
        {
            this.player = base.GetComponent<Player>();
        }
    }

    [DisallowMultipleComponent]
    public class BMagnetBullet_Mono : MonoBehaviour
    {
        private MoveTransform move;
        private PhotonView view;

        private readonly float effectDistance = 10f;
        private readonly float minEffectDistance = 0.5f;
        private readonly float magnetMultiplier = 0.4f;

        public float amount = 1f;
        public float scalingDrag = 1f;
        public float drag = 1f;
        public float spread = 1f;
        private bool isOn;

        public RotSpring rot1;
        public RotSpring rot2;

        private void Start()
        {
            this.move = base.GetComponentInParent<MoveTransform>();
            this.view = base.GetComponentInParent<PhotonView>();
            base.GetComponentInParent<SyncProjectile>().active = true;
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }

        private void Update()
        {
            if (base.gameObject.transform.parent != null)
            {
                Player closestPlayer = PlayerManager.instance.GetClosestPlayer(base.transform.position, true);
                if (!closestPlayer)
                {
                    if (this.isOn)
                    {
                        this.move.simulateGravity--;
                    }
                    this.isOn = false;
                    return;
                }

                var avoidPlayer = closestPlayer.gameObject.GetComponent<BMagnet_Mono>();
                if (!avoidPlayer)
                {
                    if (this.isOn)
                    {
                        this.move.simulateGravity--;
                    }
                    this.isOn = false;
                    return;
                }

                Vector3 a = closestPlayer.transform.position + base.transform.right * this.move.selectedSpread * Vector3.Distance(base.transform.position, closestPlayer.transform.position) * this.spread;
                float num = Vector3.Angle(base.transform.root.forward, a - base.transform.position);
                float dist = Vector3.Distance(base.transform.position, closestPlayer.transform.position);
                
                if ((dist < effectDistance) && (dist > minEffectDistance))
                {
                    this.move.velocity -= this.move.velocity * num * TimeHandler.deltaTime * this.scalingDrag * 1.2f;
                    this.move.velocity -= this.move.velocity * TimeHandler.deltaTime * this.drag * 1.2f;
                    this.move.velocity += Vector3.ClampMagnitude(a - base.transform.position, 4f) * TimeHandler.deltaTime * this.move.localForce.magnitude * 2f * this.amount * this.magnetMultiplier;
                    this.move.velocity.z = 0f;
                    this.move.velocity += Vector3.up * TimeHandler.deltaTime * this.move.gravity * this.move.multiplier * 2f;
                    if (!this.isOn)
                    {
                        this.move.simulateGravity++;
                    }
                    this.isOn = true;
                    return;
                }

                if (this.isOn)
                {
                    this.move.simulateGravity--;
                }
                this.isOn = false;
            }
        }
    }
}
