using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.InformationDisplayer {
    public static class InformationDisplayer {
        private static InformationDisplayerSettings Settings => InformationDisplayerModule.Settings;
        private static string DisplayContent => string.Join("\n", Settings.DisplayContent);

        public static void Load() {
            On.Celeste.Level.Render += LevelOnRender;
        }

        public static void Unload() {
            On.Celeste.Level.Render -= LevelOnRender;
        }

        private static void LevelOnRender(On.Celeste.Level.orig_Render orig, Level self) {
            orig(self);
            if (!Settings.Enabled) {
                return;
            }

            Draw.SpriteBatch.Begin();

            int viewWidth = Engine.ViewWidth;
            int viewHeight = Engine.ViewHeight;

            string text = Variable.Parse(DisplayContent);
            Vector2 size = ActiveFont.Measure(text) * Settings.FontSize;

            float x;
            float y;
            switch (Settings.Position) {
                case Position.TopLeft:
                    x = Settings.Margin;
                    y = Settings.Margin;
                    break;
                case Position.TopRight:
                    x = viewWidth - size.X - Settings.Margin - Settings.Padding * 2;
                    y = Settings.Margin;
                    break;
                case Position.BottomLeft:
                    x = Settings.Margin;
                    y = viewHeight - size.Y - Settings.Margin - Settings.Padding * 2;
                    break;
                case Position.BottomRight:
                    x = viewWidth - size.X - Settings.Margin - Settings.Padding * 2;
                    y = viewHeight - size.Y - Settings.Margin - Settings.Padding * 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Vector2 rectPosition = new Vector2(x, y);
            Draw.Rect(rectPosition, size.X + Settings.Padding * 2, size.Y + Settings.Padding * 2 , Color.Black * Settings.BackgroundAlpha);
            
            Vector2 textPosition = new Vector2(x + Settings.Padding, y + Settings.Padding);
            Vector2 scale = new Vector2(Settings.FontSize);
            ActiveFont.Draw(text, textPosition, Vector2.Zero, scale, Color.White * Settings.FontAlpha);

            Draw.SpriteBatch.End();
        }
    }
}