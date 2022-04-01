using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using UnboundLib.GameModes;
using CCC.Cards;
using HarmonyLib;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using System.Collections;
using UnityEngine;

namespace CCC
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]  // necessary for most modding stuff
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)] // utilities for cards and cardbars
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]   // fixes allowMultiple and blacklistedCategories

    // Declares our mod to Bepin
    [BepInPlugin(ModId, ModName, Version)]

    // The game our mod is associated with
    [BepInProcess("Rounds.exe")]
    public class CCC : BaseUnityPlugin
    {
        private const string ModId = "jaydenb.rounds.plugins.couchescustomcards";
        private const string ModName = "Couches' Custom Cards (CCC)";
        public const string Version = "0.4.1";
        public const string ModInitials = "CCC";

        void Awake()
        {
            new Harmony(ModId).PatchAll();

            // Clean up Custom Effect Components from Player GameObjects when Game Ends
            GameModeManager.AddHook(GameModeHooks.HookGameEnd, ProcessGameEnd);
        }

        IEnumerator ProcessGameEnd(IGameModeHandler gm)
        {
            DestroyAll<Doof_Mono>();
            DestroyAll<Boof_Mono>();
            DestroyAll<BMagnet_Mono>();
            DestroyAll<BMagnetBullet_Mono>();
            DestroyAll<BRepeller_Mono>();
            DestroyAll<BRepellingBullet_Mono>();
            DestroyAll<WatchThis_Mono>();
            DestroyAll<InvisibleBlock_Mono>();
            DestroyAll<InvisibleBlock_Effect>();
            DestroyAll<FeatherFall_Mono>();
            DestroyAll<FireShield_Mono>();
            DestroyAll<FireShield_Effect>();
            DestroyAll<OneForAll_Mono>();
            DestroyAll<PlaceRewind_Mono>();
            DestroyAll<BulkUp_Mono>();
            yield break;
        }

        void DestroyAll<T>() where T : UnityEngine.Object
        {
            var objects = GameObject.FindObjectsOfType<T>();
            for (int i = objects.Length - 1; i >= 0; i--)
            {
                Destroy(objects[i]);
            }
        }

        void Start()
        {
            // Register Credits with Unbound
            Unbound.RegisterCredits(ModName, new string[] { "JaydenB/Couches_Collaborative" }, new string[] { "github" }, new string[] { "https://github.com/JaydenB/CCC" });

            // Build All Cards
            CustomCard.BuildCard<DoofCard>();
            CustomCard.BuildCard<BoofCard>();
            CustomCard.BuildCard<BulletRepellentCard>();
            CustomCard.BuildCard<BulletMagnetCard>();
            CustomCard.BuildCard<WatchThisCard>();
            CustomCard.BuildCard<InvisibleBlockCard>();
            CustomCard.BuildCard<FlipperCard>();
            CustomCard.BuildCard<CatapultCard>();
            CustomCard.BuildCard<FeatherFallCard>();
            CustomCard.BuildCard<FireShieldCard>();
            CustomCard.BuildCard<OneForAllCard>();
            CustomCard.BuildCard<PlaceRewindCard>();
            CustomCard.BuildCard<BulkUpCard>();

            GameModeManager.AddHook(GameModeHooks.HookBattleStart, (gm) => PlaceRewindCard.ResetAllRewinds());
            GameModeManager.AddHook(GameModeHooks.HookPointEnd, (gm) => PlaceRewindCard.ResetAllRewinds());

            GameModeManager.AddHook(GameModeHooks.HookBattleStart, (gm) => BulkUpCard.ResetAllBulkUp());
            GameModeManager.AddHook(GameModeHooks.HookPointEnd, (gm) => BulkUpCard.ResetAllBulkUp());
        }
    }
}
