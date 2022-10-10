using HlyssMG.Actions;
using HlyssMG.Nodes;
using HlyssMG.Nodes.Graphics;
using HlyssMG.Nodes.Graphics.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TrijamJDS.Scenes;

namespace TrijamJDS.Objects
{
    internal class DebrisManager : Drawable
    {
        Texture2D[] _textures;
        Texture2D _explosionTex;
        List<FlyingObject> _debris;

        SoundEffect _hitSound;
        SoundEffect _explSound;

        int debrisSpawned = 0;
        int debrisDestroyed = 0;
        float elapsedTime = 0;

        public float DebrisSpeed = 2;
        public float Interval = 3.5f;
        public float DebrisPerRound = 1;

        public override void OnInitialized()
        {
            base.OnInitialized();

            _debris = new List<FlyingObject>();
            _textures = new Texture2D[3]
            {
                Scene.LocalContent.Load<Texture2D>("Graphics/BrokenPart1"),
                Scene.LocalContent.Load<Texture2D>("Graphics/BrokenPart2"),
                Scene.LocalContent.Load<Texture2D>("Graphics/BrokenPart3")
            };
            _explosionTex = Scene.LocalContent.Load<Texture2D>("Graphics/Explosion");

            _hitSound = Scene.LocalContent.Load<SoundEffect>("Audio/Hit");
            _explSound = Scene.LocalContent.Load<SoundEffect>("Audio/Explosion");
        }

        public void Spawn(Vector2 position)
        {
            var rotation = RotationTowardsPoint(position, new Vector2(1920 / 2, 1080 / 2));
            var movementVec = VectorFromRotation(rotation);

            FlyingObject d = new FlyingObject();
            d.X = position.X;
            d.Y = position.Y;
            d.dX = movementVec.X;
            d.dY = movementVec.Y;
            d.R = rotation;
            d.Id = debrisSpawned++;
            d.Hp = 2 * ((debrisSpawned % 3) + 1);

            _debris.Add(d);
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
            elapsedTime += dt;

            if (elapsedTime >= Interval)
            {
                elapsedTime = 0;

                int chanceOfFloor = (int)((DebrisPerRound % (int)DebrisPerRound) * 100);
                int debrisToSpawn = (int)((Game.Random.Next(100) > chanceOfFloor) ? DebrisPerRound : DebrisPerRound + 1);

                Console.WriteLine($"DEBRIS PER ROUND: {DebrisPerRound}");
                Console.WriteLine($"DEBRIS TO SPAWN: {debrisToSpawn}\n");

                for (int i = 0; i < debrisToSpawn; i++)
                {
                    float angle = Game.Random.Next(360);
                    var spawnPos = VectorFromRotation(angle);
                    spawnPos *= 2202 / 2f;

                    Spawn(spawnPos + new Vector2(1920 / 2, 1080 / 2));
                }
            }

            for (int i = 0; i < _debris.Count; i++)
            {
                if (_debris[i].Hp == 0)
                {
                    ExplodeAt(new Vector2(_debris[i].X, _debris[i].Y));
                    _debris.RemoveAt(i--);
                    debrisDestroyed++;

                    (Scene as GameScene).ShakeCamera();
                    (Scene as GameScene).AddPoints(10);

                    if (debrisDestroyed % 5 == 0 && Interval > 2f)
                    {
                        Interval -= 0.1f;
                    }
                    if (debrisDestroyed % 5 == 0 && DebrisPerRound < 2.3f)
                    {
                        DebrisPerRound += 0.075f;
                    }

                    //Sometimes generate more debris
                    if (DebrisPerRound > 2.3f)
                        DebrisPerRound = 2.3f;

                    if (Game.Random.Next(100) > 95 && DebrisPerRound > 2.2f)
                        DebrisPerRound = 4;
                    //-----------------------------

                    _explSound.Play();
                }
                else
                {
                    _debris[i].X += _debris[i].dX * DebrisSpeed;
                    _debris[i].Y += _debris[i].dY * DebrisSpeed;

                    var len = Vector2.DistanceSquared(new Vector2(1920 / 2, 1080 / 2), new Vector2(_debris[i].X, _debris[i].Y));

                    if (len < 150 * 150 && Scene != null)
                    {
                        ExplodeAt(new Vector2(_debris[i].X, _debris[i].Y));
                        _debris.RemoveAt(i--);

                        (Scene as GameScene).ShakeCamera();
                        (Scene as GameScene).RemoveLife();
                    }
                }
            }
        }

        public override Rectangle GetLocalBounds()
        {
            return new Rectangle(0, 0, 1920, 1080);
        }

        public override void OnDraw(SpriteBatch batch)
        {
            foreach (var d in _debris)
            {
                batch.Draw(_textures[d.Id % 3], new Vector2(d.X, d.Y), null, Color.White, MathHelper.ToRadians(d.R), new Vector2(16, 16), 3, SpriteEffects.None, 0);
            }
        }

        public void Collision(FlyingObject bullet)
        {
            foreach (var d in _debris)
            {
                var len = Vector2.DistanceSquared(new Vector2(bullet.X, bullet.Y), new Vector2(d.X, d.Y));

                if (len <= 48 * 48)
                {
                    bullet.Hp = 0;
                    d.Hp--;
                    _hitSound.Play();
                }
            }
        }

        private float RotationTowardsPoint(Vector2 axis, Vector2 target)
        {
            return (float)-(Math.Atan2(axis.X - target.X, axis.Y - target.Y) * (180f / Math.PI));
        }

        private Vector2 VectorFromRotation(float rotation)
        {
            Vector2 v;
            float rad = (rotation * (float)Math.PI) / 180.0f;

            v.X = (float)Math.Sin(rad);
            v.Y = (float)-Math.Cos(rad);

            return v;
        }

        private void ExplodeAt(Vector2 position)
        {
            AnimatedSprite expl = new AnimatedSprite();
            expl.FramesPerSecond = 24;
            expl.IsLooping = false;
            expl.Position = position;
            expl.Scale = new Vector2(3);
            expl.Origin = new Vector2(0.5f);
            expl.AddAnimation("expl", new SpriteSheetProvider(_explosionTex, 8, 1));

            expl.StartAction(new ActionSequence(
                new CallFunc((anim) => (anim as AnimatedSprite).Play("expl"), expl),
                new Wait(1),
                new CallFunc((anim) => (anim as Node).RemoveFromParent(), expl)
            ));

            AddNode(expl);
        }
    }
}
