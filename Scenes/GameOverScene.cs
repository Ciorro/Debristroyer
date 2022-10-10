using HlyssMG.Actions;
using HlyssMG.Input.Events;
using HlyssMG.Nodes;
using HlyssMG.Nodes.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TrijamJDS.Scenes
{
    internal class GameOverScene : Scene
    {
        Sprite _background;
        Sprite _planet;
        Text _goInfo;
        Text _conInfo;
        Text _ptsInfo;

        int _points;

        public GameOverScene(int score)
        {
            _points = score;
        }

        public override void OnInitialized()
        {
            base.OnInitialized();

            Opacity = 0;
            StartAction(new FadeIn(1));

            _goInfo = new Text(LocalContent.Load<SpriteFont>("Fonts/NicoClean"), "Game Over!");
            _goInfo.Origin = new Vector2(0.5f, 0);
            _goInfo.Position = new Vector2(1920 / 2, 80);

            _ptsInfo = new Text(LocalContent.Load<SpriteFont>("Fonts/NicoClean"), $"Your score: {_points}");
            _ptsInfo.Origin = new Vector2(0.5f, 0);
            _ptsInfo.Position = new Vector2(1920 / 2, 140);
            _ptsInfo.Scale = new Vector2(0.75f);

            _conInfo = new Text(LocalContent.Load<SpriteFont>("Fonts/NicoClean"), "Press SPACE to continue");
            _conInfo.Origin = new Vector2(0.5f, 1);
            _conInfo.Position = new Vector2(1920 / 2, 1080 - 80);

            _background = new Sprite(LocalContent.Load<Texture2D>("Graphics/Background"));
            _background.Position = new Vector2(1920 / 2, 1080 / 2);
            _background.Scale = new Vector2(1920f / 400) + new Vector2(0.05f);

            _planet = new Sprite(LocalContent.Load<Texture2D>("Graphics/PlanetDestroyed"));
            _planet.Position = new Vector2(1920 / 2, 1080 / 2);
            _planet.Scale = _background.Scale / 2f;

            AddNode(_background);
            AddNode(_planet);
            AddNode(_goInfo);
            AddNode(_conInfo);
            AddNode(_ptsInfo);

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
                                Game.RootNode.AddNode(new MenuScene());
                            }, this)
                        )
                    );
                }
            }
        }
    }
}
