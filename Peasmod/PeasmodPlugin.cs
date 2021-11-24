﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using InnerNet;
using Reactor;
using BepInEx.Logging;
using Hazel.Udp;
using PeasAPI;
using PeasAPI.Managers;
using Peasmod.Utility;
using UnityEngine;
using Random = System.Random;

namespace Peasmod
{
    [BepInPlugin(Id, "Peasmod", PluginVersion)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(PeasAPI.PeasAPI.Id)]
    public class PeasmodPlugin : BasePlugin
    {
        public const string Id = "tk.peasplayer.peasmod";

        public const string PluginName = "Peasmod";
        public const string PluginAuthor = "Peasplayer#2541";
        public const string PluginVersion = "3.0.0";

        public Harmony Harmony { get; } = new Harmony(Id);
        
        public static ManualLogSource Logger { get; private set; }
        
        public static ConfigFile ConfigFile { get; private set; }

        public static readonly Random Random = new Random();

        public static bool GameStarted =>
            GameData.Instance && ShipStatus.Instance && AmongUsClient.Instance &&
            (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started ||
             AmongUsClient.Instance.GameMode == GameModes.FreePlay);

        public override void Load()
        {
            Logger = this.Log;
            ConfigFile = Config;

            WatermarkManager.AddWatermark($" | {PluginName} v{PluginVersion} {StringColor.Green} by {PluginAuthor}", $" | {PluginName} v{PluginVersion}\n{StringColor.Green} by {PluginAuthor}", 
                new Vector3(0f, -0.3f), new Vector3(-0.9f, 0f));
            PeasAPI.PeasAPI.AccountTabOffset = new Vector3(0f, -0.3f);
            
            CustomServerManager.RegisterServer("Peaspowered", "au.peasplayer.tk", 22023);
            CustomServerManager.RegisterServer("matux.fr", "152.228.160.91", 22023);
            
            UpdateManager.RegisterUpdateListener("https://raw.githubusercontent.com/Peasplayer/Peasmod/dev/Peasmod/Data.json");
            
            CustomHatManager.RegisterNewHat("DreamMask", "Peasmod.Resources.Hats.DreamMask.png", new Vector2(0f, 0.2f));
            CustomHatManager.RegisterNewHat("KristalCrown", "Peasmod.Resources.Hats.KristalCrown.png", Vector2.zero, false, false);
            CustomHatManager.RegisterNewHat("PeasMask", "Peasmod.Resources.Hats.PeasMask.png", new Vector2(0f, 0.2f));
            CustomHatManager.RegisterNewHat("Elf Hat", "Peasmod.Resources.Hats.Elf.png", new Vector2(0f, 0.2f), true, false, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Hats.Elfb.png"));
            CustomHatManager.RegisterNewHat("Santa", "Peasmod.Resources.Hats.Santa.png", new Vector2(0f, 0.3f), true, false, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Hats.Santab.png"));
            CustomHatManager.RegisterNewHat("Christmas Tree", "Peasmod.Resources.Hats.XmasTree.png", new Vector2(0f, 0.2f), true, false, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Hats.XmasTreeb.png"));
            CustomHatManager.RegisterNewHat("Christmas Sock", "Peasmod.Resources.Hats.Sock.png", new Vector2(0f, 0.2f), true, false, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Hats.Sockb.png"));

            Settings.Load();

            Harmony.Unpatch(typeof(UdpConnection).GetMethod("HandleSend"), HarmonyPatchType.Prefix, ReactorPlugin.Id);
            Harmony.PatchAll();
        }
    }
}