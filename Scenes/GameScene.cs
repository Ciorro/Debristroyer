using HlyssMG.Actions;
using HlyssMG.Nodes;
using HlyssMG.Nodes.Graphics;
using HlyssMG.Nodes.Graphics.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using TrijamJDS.Objects;

namespace TrijamJDS.Scenes
{
    internal class GameScene : Scene
    {
        Sprite _background;
        Sprite _planet;
        Spaceship _spaceship;
        DebrisManager _debrisManager;
        BulletManager _bulletManager;
        HpDisplay _hp;

        SoundEffect _gameOverSound;

        Text _pointsTxt;
        int _points;

        public override void OnAttached()
        {
            base.OnAttached();

            Opacity = 0;
            StartAction(new FadeIn(1));

            _background = new Sprite(LocalContent.Load<Texture2D>("Graphics/Background"));
            _planet = new Sprite(LocalContent.Load<Texture2D>("Graphics/Planet"));
            _debrisManager = new DebrisManager();
            _bulletManager = new BulletManager();
            _spaceship = new Spaceship();
            _pointsTxt = new Text(LocalContent.Load<SpriteFont>("Fonts/NicoClean"), "Points: 0");
            _hp = new HpDisplay();

            AddNode(_background);
            AddNode(_planet);
            AddNode(_debrisManager);
            AddNode(_bulletManager);
            AddNode(_spaceship);
            AddNode(_pointsTxt);
            AddNode(_hp);

            _bulletManager.SetDebrisManager(_debrisManager);

            _background.Position = new Vector2(1920 / 2, 1080 / 2);
            _planet.Position = new Vector2(1920 / 2, 1080 / 2);
            _spaceship.Position = new Vector2(1920 / 2, 1080 / 2);
            _pointsTxt.Position = new Vector2(1920 / 2, 20);
            _hp.Position = new Vector2(1920 / 2, 1080 - 80);

            _background.Scale = new Vector2(1920f / 400) + new Vector2(0.05f);
            _planet.Scale = _background.Scale / 2f;
            _hp.Scale = new Vector2(3);

            _pointsTxt.Origin = new Vector2(0.5f, 0);
            _hp.Origin = new Vector2(0.5f, 0);

            _gameOverSound = LocalContent.Load<SoundEffect>("Audio/GameOver");
        }

        public void RemoveLife()
        {
            _hp.Hp = _hp.Hp - 1;

            if (_hp.Hp == 0)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            _gameOverSound.Play();

            RemoveNode(_debrisManager);
            RemoveNode(_bulletManager);

            //Explosion
            _planet.Texture = LocalContent.Load<Texture2D>("Graphics/PlanetDestroyed");

            AnimatedSprite expl = new AnimatedSprite();
            expl.FramesPerSecond = 24;
            expl.IsLooping = false;
            expl.Position = new Vector2(1920 / 2, 1080 / 2);
            expl.Scale = new Vector2(5.5f);
            expl.Origin = new Vector2(0.5f);
            expl.AddAnimation("expl", new SpriteSheetProvider(LocalContent.Load<Texture2D>("Graphics/Explosion"), 8, 1));

            expl.StartAction(new ActionSequence(
                new CallFunc((anim) => (anim as AnimatedSprite).Play("expl"), expl),
                new Wait(1),
                new CallFunc((anim) => (anim as Node).RemoveFromParent(), expl)
            ));

            AddNode(expl);

            StartAction(
                new ActionSequence(
                    new Wait(2),
                    new FadeOut(1),
                    new CallFunc((node) =>
                    {
                        (node as Node).RemoveNode(_spaceship);
                        Game.RootNode.RemoveNode(node as Node);
                        Game.RootNode.AddNode(new GameOverScene(_points));
                    }, this)
                )
            );
        }

        public void AddPoints(int points)
        {
            _points += points;
            _pointsTxt.String = $"Points: {_points}";
        }

        public void ShakeCamera()
        {
            StopActions();
            Position = Vector2.Zero;

            StartAction(
                new ActionSequence(
                    new MoveTo(new Vector2(-5, -5), 0.05f),
                    new MoveTo(new Vector2(-5, 5), 0.05f),
                    new MoveTo(new Vector2(5, -5), 0.05f),
                    new MoveTo(new Vector2(5, 5), 0.05f),
                    new MoveTo(new Vector2(0, 0), 0.05f)
                )
            );
        }
    }
}
