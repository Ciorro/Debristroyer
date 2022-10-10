using HlyssMG.Input;
using HlyssMG.Input.Events;
using HlyssMG.Nodes;
using HlyssMG.Nodes.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TrijamJDS.Objects
{
    internal class Spaceship : Node
    {
        Sprite _spaceship;

        public override void OnInitialized()
        {
            base.OnInitialized();

            _spaceship = new Sprite(Scene.LocalContent.Load<Texture2D>("Graphics/Spaceship"));
            _spaceship.Scale = new Vector2(3);
            _spaceship.Position = new Vector2(0, -160);

            AddNode(_spaceship);
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            var mPos = Root.ScreenToWorld(Mouse.GetState().Position);
            Rotation = RotationTowardsPoint(Position, mPos.ToVector2());
        }

        public override void OnEvent(IInputEvent e)
        {
            base.OnEvent(e);

            if (e is PointerButton pb)
            {
                if (pb.Button == MouseButton.Left && pb.State == InputState.Pressed && Scene != null)
                {
                    var bMgr = Scene.FindNodeByType<BulletManager>();
                    if (bMgr != null)
                    {
                        bMgr.Shoot(Rotation);
                    }
                }
            }
        }

        private float RotationTowardsPoint(Vector2 axis, Vector2 target)
        {
            return (float)-(Math.Atan2(axis.X - target.X, axis.Y - target.Y) * (180f / Math.PI));
        }
    }
}
