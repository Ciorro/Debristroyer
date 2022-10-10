using HlyssMG.Nodes;
using HlyssMG.Nodes.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TrijamJDS.Objects
{
    internal class BulletManager : Drawable
    {
        Texture2D _bulletTex;
        List<FlyingObject> _bullets;
        DebrisManager _debrisManager;
        SoundEffect _shootSound;

        public float BulletSpeed = 20;

        public override void OnInitialized()
        {
            base.OnInitialized();

            _bullets = new List<FlyingObject>();
            _bulletTex = Scene.LocalContent.Load<Texture2D>("Graphics/Bullet");
            _shootSound = Scene.LocalContent.Load<SoundEffect>("Audio/Shoot");
        }

        public void Shoot(float rotation)
        {
            var movementVec = VectorFromRotation(rotation);

            FlyingObject b = new FlyingObject();
            b.X = 1920 / 2 + movementVec.X * 170;
            b.Y = 1080 / 2 + movementVec.Y * 170;
            b.dX = movementVec.X;
            b.dY = movementVec.Y;
            b.R = rotation;
            b.Hp = 1;

            _bullets.Add(b);

            _shootSound.Play(0.5f, 0, 0);
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            for (int i = 0; i < _bullets.Count; i++)
            {
                _bullets[i].X += _bullets[i].dX * BulletSpeed;
                _bullets[i].Y += _bullets[i].dY * BulletSpeed;

                _debrisManager.Collision(_bullets[i]);

                if (_bullets[i].Hp == 0 || IsOutsideScreen(_bullets[i]))
                {
                    _bullets.RemoveAt(i--);
                }
            }
        }

        private bool IsOutsideScreen(FlyingObject flyingObject)
        {
            return flyingObject.X > 1920 || flyingObject.X < 0 || flyingObject.Y > 1080 || flyingObject.Y < 0;
        }

        public override Rectangle GetLocalBounds()
        {
            return new Rectangle(0, 0, 1920, 1080);
        }

        public override void OnDraw(SpriteBatch batch)
        {
            foreach (var b in _bullets)
            {
                batch.Draw(_bulletTex, new Vector2(b.X, b.Y), null, Color.White, MathHelper.ToRadians(b.R), new Vector2(2, 0), 3, SpriteEffects.None, 0);
            }
        }

        public void SetDebrisManager(DebrisManager debrisMgr)
        {
            _debrisManager = debrisMgr;
        }

        private Vector2 VectorFromRotation(float rotation)
        {
            Vector2 v;
            float rad = (rotation * (float)Math.PI) / 180.0f;

            v.X = (float)Math.Sin(rad);
            v.Y = (float)-Math.Cos(rad);

            return v;
        }
    }
}
