using System;
using System.Collections.Generic;
using System.Reflection;
using Celeste.Mod.InformationDisplayer.Extensions;
using Monocle;

namespace Celeste.Mod.InformationDisplayer {
    public static class Variable {
        private static readonly MethodInfo PlayerWallJumpCheck = typeof(Player).GetPrivateMethodInfo("WallJumpCheck");
        
        private static string lastText;
        private static InformationDisplayerSaveData SaveData => InformationDisplayerModule.SaveData;

        public static readonly Dictionary<Type, string> TypeNeedCount = new Dictionary<Type, string> {
            {typeof(Seeker), "seeker"},
            {typeof(Snowball), "snowball"},
        };

        private static readonly Lazy<Dictionary<string, Func<Level, Player, string>>> ReplaceFunctions =
            new Lazy<Dictionary<string, Func<Level, Player, string>>>(() => {
                var result = new Dictionary<string, Func<Level, Player, string>> {
                    {"room", (level, player) => level.Session.Level},

                    {"speedX", (level, player) => player.Speed.X.ToString("0.00")},
                    {"speedY", (level, player) => player.Speed.Y.ToString("0.00")},
                    {"posX", (level, player) => player.ExactPosition.X.ToString("0.00")},
                    {"posY", (level, player) => player.ExactPosition.Y.ToString("0.00")},
                    {"dash", (level, player) => player.Dashes.ToString()},
                    {"ground", (level, player) => player.LoseShards ? "ground " : string.Empty},
                    {"wall", (level, player) => {
                            if ((bool) PlayerWallJumpCheck.Invoke(player, 1)) {
                                return "right";
                            }

                            if ((bool) PlayerWallJumpCheck.Invoke(player, -1)) {
                                return "left";
                            }

                            return string.Empty;
                        }
                    },

                    {"stamina", (level, player) => player.Stamina.ToString("0")}
                };
                foreach (var pair in TypeNeedCount) {
                    result.Add(pair.Value, (level, player) => SaveData.EntityCount.ContainsKey(pair.Value)
                        ? SaveData.EntityCount[pair.Value].ToString()
                        : "0");

                    string diffVariable = GetDifferentVariable(pair.Value);
                    result.Add(diffVariable, (level, player) => SaveData.DifferentEntityCount.ContainsKey(diffVariable)
                        ? SaveData.DifferentEntityCount[diffVariable].Count.ToString()
                        : "0");
                }

                return result;
            });

        public static string Parse(string text) {
            if (GetLevel() == null || GetPlayer() == null) {
                return lastText;
            }

            foreach (var pair in ReplaceFunctions.Value) {
                string variable = $"%{pair.Key}%";
                text = text.Replace(variable, pair.Value(GetLevel(), GetPlayer()));
            }

            lastText = text;

            return text;
        }

        public static string GetDifferentVariable(string variable) {
            return "different" + variable.Substring(0, 1).ToUpper() + variable.Substring(1, variable.Length - 1);
        }
        private static Player GetPlayer() {
            if (GetLevel()?.Tracker?.GetEntity<Player>() is Player player) {
                return player;
            }

            return null;
        }

        private static Level GetLevel() {
            if (Engine.Scene is Level level) {
                return level;
            }

            return null;
        }
    }
}