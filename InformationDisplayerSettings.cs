using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.InformationDisplayer {
    [SettingName(DialogId.INFORMATION_DISPLAYER)]
    public class InformationDisplayerSettings : EverestModuleSettings {
        public bool Enabled { get; set; } = true;

        [SettingIgnore] public float FontSize { get; set; } = 0.5f;
        [SettingIgnore] public float FontAlpha { get; set; } = 0.8f;
        [SettingIgnore] public float BackgroundAlpha { get; set; } = 0.8f;
        [SettingIgnore] public int Margin { get; set; } = 10;
        [SettingIgnore] public int Padding { get; set; } = 10;

        public Position Position { get; set; } = Position.TopRight;

        public List<string> DisplayContent { get; set; } = new List<string> {
            "posX=%posX%  posY=%posY%",
            "speedX=%speedX%",
            "speedY=%speedY%",
            "冲刺: %dash%",
            "体力=%stamina%",
            "地面=%ground%",
            "墙壁=%wall%",
            "新浪=%seeker%",
            "differentSeeker=%differentSeeker%",
            "雪球=%snowball%",
            "不同雪球=%differentSnowball%",
        };

        [SettingName(DialogId.INFORMATION_DISPLAYER_ENABLED)]
        [DefaultButtonBinding(0, Keys.OemPlus)]
        public ButtonBinding ButtonEnabled { get; set; }

        [SettingName(DialogId.INFORMATION_DISPLAYER_CLEAR_DATA)]
        [DefaultButtonBinding(0, Keys.OemMinus)]
        public ButtonBinding ButtonClearData { get; set; }

        public void UpdateFrom(InformationDisplayerSettings otherSettings) {
            FontSize = otherSettings.FontSize;
            FontAlpha = otherSettings.FontAlpha;
            BackgroundAlpha = otherSettings.BackgroundAlpha;
            Margin = otherSettings.Margin;
            Padding = otherSettings.Padding;
            Position = otherSettings.Position;
            DisplayContent = otherSettings.DisplayContent;
        }
    }


    public enum Position {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    }
}