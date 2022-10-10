using HlyssMG.Nodes.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrijamJDS.Objects
{
    internal class HpDisplay : Sprite
    {
        private int _hp = 5;

        public int Hp
        {
            get => _hp;
            set
            {
                if (value >= 0)
                {
                    _hp = value;
                    TextureRect = new Rectangle(0, 16 * (5 - Hp), 80, 16);
                }
            }
        }

        public override void OnInitialized()
        {
            base.OnInitialized();
            Texture = Scene.LocalContent.Load<Texture2D>("Graphics/Hearts");
            Hp = 5;
        }
    }
}
