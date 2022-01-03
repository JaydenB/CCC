﻿using BepInEx;
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
        public const string Version = "0.1.1";
        public const string ModInitials = "CCC";

        void Awake()
        {
            new Harmony(ModId).PatchAll();

            // Clean up Custom Effect Components from Player GameObjects when Game Ends
            GameModeManager.AddHook(GameModeHooks.HookGameEnd, ResetEffects);
        }

        IEnumerator ResetEffects(IGameModeHandler gm)
        {
            DestroyAll<Doof_Mono>();
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
        }
    }
}
