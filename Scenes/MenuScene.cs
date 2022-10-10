using HlyssMG.Actions;
using HlyssMG.Actions.Easings;
using HlyssMG.Input;
using HlyssMG.Input.Events;
using HlyssMG.Nodes;
using HlyssMG.Nodes.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TrijamJDS.Scenes
{
    internal class MenuScene : Scene
    {
        Sprite _background;
        Sprite _planet;
        Text _goInfo;
        Text _goInfo2;
        Text _conInfo;
        Text _credits;
        Text _fullScr;

        public override void OnInitialized()
        {
            base.OnInitialized();

            Opacity = 0;
            StartAction(new FadeIn(1));

            string title = "Debristroyer";

            _goInfo = new Text(LocalContent.Load<SpriteFont>("Fonts/NicoClean"), title);
            _goInfo.Origin = new Vector2(0.5f, 0);
            _goInfo.Position = new Vector2(1920 / 2, 80);
            _goInfo.Scale = new Vector2(2);

            _goInfo2 = new Text(LocalContent.Load<SpriteFont>("Fonts/NicoClean"), title);
            _goInfo2.Origin = new Vector2(0.5f, 0);
            _goInfo2.Position = new Vector2(1920 / 2, 80) + new Vector2(2);
            _goInfo2.Tint = Color.Red;
            _goInfo2.Scale = new Vector2(2);

            _conInfo = new Text(LocalContent.Load<SpriteFont>("Fonts/NicoClean"), "Press SPACE to start!");
            _conInfo.Origin = new Vector2(0.5f, 1);
            _conInfo.Position = new Vector2(1920 / 2, 1080 - 80);

            _credits = new Text(LocalContent.Load<SpriteFont>("Fonts/NicoClean"), "Music by DOS88 [itch.io]. Planets and background generated with Deep-Fold's tools [itch.io]");
            _credits.Origin = new Vector2(0.5f, 1);
            _credits.Position = new Vector2(1920 / 2, 1080 - 40);
            _credits.Scale = new Vector2(0.5f);

            _fullScr = new Text(LocalContent.Load<SpriteFont>("Fonts/NicoClean"), "[F11] Fullscreen");
            _fullScr.Origin = new Vector2(0, 1);
            _fullScr.Position = new Vector2(40, 1080 - 40);
            _fullScr.Scale = new Vector2(0.5f);

            _background = new Sprite(LocalContent.Load<Texture2D>("Graphics/Background"));
            _background.Position = new Vector2(1920 / 2, 1080 / 2);
            _background.Scale = new Vector2(1920f / 400) + new Vector2(0.05f);

            _planet = new Sprite(LocalContent.Load<Texture2D>("Graphics/Planet"));
            _planet.Position = new Vector2(1920 / 2, 1080 / 2);
            _planet.Scale = _background.Scale / 2f;

            AddNode(_background);
            AddNode(_planet);
            AddNode(_goInfo);
            AddNode(_goInfo2);
            AddNode(_conInfo);
            AddNode(_credits);
            AddNode(_fullScr);
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            if(_goInfo2.ActionCount == 0)
            {
                _goInfo2.StartAction(
                    new ActionSequence(
                        new MoveBy(new Vector2(-4, 0), 0.5f, Ease.Linear),
                        new MoveBy(new Vector2(0, -4), 0.5f, Ease.Linear),
                        new MoveBy(new Vector2(4, 0), 0.5f, Ease.Linear),
                        new MoveBy(new Vector2(0, 4), 0.5f, Ease.Linear)
                    )
                );
            }

            if (_conInfo.ActionCount == 0)
            {
                _conInfo.StartAction(
                    new ActionSequence(
                        new ScaleTo(new Vector2(1.2f), 1),
                        new ScaleTo(new Vector2(1f), 1)
                    )
                );
            }
        }

        public override void OnEvent(IInputEvent e)
        {
            base.OnEvent(e);

            if (e is KeyboardButton keb)
            {
                if (keb.Key == Keys.Space && ActionCount == 0)
                {
                    StartAction(
                        new ActionSequence(
                            new FadeOut(1),
                            new CallFunc((node) =>
                            {
                                Game.RootNode.RemoveNode(node as Node);
                                Game.RootNode.AddNode(new GameScene());
                            }, this)
                        )
                    );
                }

                if (keb.Key == Keys.F11 && keb.State == InputState.Pressed)
                {
                    Game.WindowWidth = Game.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                    Game.WindowHeight = Game.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
                    Game.IsFullscreen = !Game.IsFullscreen;

                    if(!Game.IsFullscreen)
                    {
                        Game.WindowWidth = 1366;
                        Game.WindowHeight = 768;
                    }
                }
            }
        }
    }
}
