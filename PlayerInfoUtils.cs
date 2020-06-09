using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Celeste.Mod.InformationDisplayer.Extensions;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.InformationDisplayer {
    public static class PlayerInfoUtils {
        private static readonly FieldInfo CollectTimer = typeof(Strawberry).GetPrivateFieldInfo("collectTimer");
        private static readonly FieldInfo DashCooldownTimer = typeof(Player).GetPrivateFieldInfo("dashCooldownTimer");
        private static readonly FieldInfo JumpGraceTimer = typeof(Player).GetPrivateFieldInfo("jumpGraceTimer");
        private static readonly MethodInfo PlayerWallJumpCheck = typeof(Player).GetPrivateMethodInfo("WallJumpCheck");
        private static Vector2 lastPos = Vector2.Zero;

        public static String GetPlayerInfo() {
            if (!(Engine.Scene is Level level)) {
                return "";
            }

            Player player = level.Tracker.GetEntity<Player>();
            if (player == null) {
                return "";
            }

            double x, y;
            x = player.PositionRemainder.X % 0.25 < 0.01
                ? Math.Floor(player.PositionRemainder.X * 100) / 100
                : player.PositionRemainder.X;
            y = player.PositionRemainder.Y % 0.25 < 0.01
                ? Math.Floor(player.PositionRemainder.Y * 100) / 100
                : player.PositionRemainder.Y;
            x += player.X;
            y += player.Y;
            string pos = $"Pos: {x:0.00}, {y:0.00}";
            string speed = $"Speed: {player.Speed.X:0.00}, {player.Speed.Y:0.00}";
            Vector2 diff = (player.ExactPosition - lastPos) * 60;
            string vel = $"Vel: {diff.X:0.00}, {diff.Y:0.00}";
            string miscstats = $"Stamina: {player.Stamina:0}";

            int dashCooldown = (int) ((float) DashCooldownTimer.GetValue(player) * 60f);
            string statuses = (dashCooldown < 1 && player.Dashes > 0 ? "Dash " : string.Empty)
                              + (player.LoseShards ? "Ground " : string.Empty)
                              + ((bool) PlayerWallJumpCheck.Invoke(player, 1) ? "Wall-R " : string.Empty)
                              + ((bool) PlayerWallJumpCheck.Invoke(player, -1) ? "Wall-L " : string.Empty)
                              + (!player.LoseShards && (float) JumpGraceTimer.GetValue(player) > 0
                                  ? "Coyote "
                                  : string.Empty);
            statuses = (player.InControl && !level.Transitioning ? statuses : "NoControl ")
                       + (player.TimePaused ? "Paused " : string.Empty)
                       + (level.InCutscene ? "Cutscene " : string.Empty);

            if (player.Holding == null) {
                foreach (Component component in level.Tracker.GetComponents<Holdable>()) {
                    Holdable holdable = (Holdable) component;
                    if (holdable.Check(player)) {
                        statuses += "Grab ";
                        break;
                    }
                }
            }

            int berryTimer = -10;
            Follower firstRedBerryFollower =
                player.Leader.Followers.Find(follower => follower.Entity is Strawberry berry && !berry.Golden);
            if (firstRedBerryFollower?.Entity is Strawberry firstRedBerry) {
                object collectTimer;
                if (firstRedBerry.GetType() == typeof(Strawberry)
                    || (collectTimer = firstRedBerry.GetType().GetPrivateFieldInfo("collectTimer")) == null) {
                    // if this is a vanilla berry or a mod berry having no collectTimer, use the cached FieldInfo for Strawberry.collectTimer.
                    collectTimer = CollectTimer.GetValue(firstRedBerry);
                }

                berryTimer = (int) Math.Round(60f * (float) collectTimer);
            }

            string timers = (berryTimer != -10 ? $"BerryTimer: {berryTimer.ToString()} " : string.Empty)
                            + (dashCooldown != 0 ? $"DashTimer: {(dashCooldown).ToString()} " : string.Empty);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(pos);
            sb.AppendLine(speed);
            sb.AppendLine(vel);

            sb.AppendLine(miscstats);
            if (!string.IsNullOrEmpty(statuses))
                sb.AppendLine(statuses);
            sb.Append(timers);
            lastPos = player.ExactPosition;
            return sb.ToString().TrimEnd();
        }
    }
}