﻿using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LPBossFightStats.src
{
    public class BossEventsManager : GlobalNPC
    {
        // Triggered when an NPC spawns
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (!npc.boss)
                return;

            // Activate boss fight if a boss NPC spawns
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                BossFightManager.IsBossFightActive = true;
            }
        }

        // Triggered when an NPC is killed
        public override void OnKill(NPC npc)
        {
            if (!BossFightManager.IsBossFightActive)
                return;

            if (!npc.friendly)
            {
                BossFightManager.AddKillReport();
            }
        }

        // Triggered when an NPC is hit by a Trap or TownNPC
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (!BossFightManager.IsBossFightActive)
                return;

            if (!npc.friendly && (projectile.trap || projectile.npcProj))
            {
                BossFightManager.AddDamageDealt(255, damageDone);
            }
        }
    }
}
