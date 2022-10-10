using HlyssMG.Actions;
using HlyssMG.Input.Events;
using HlyssMG.Nodes;
using HlyssMG.Nodes.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TrijamJDS.Scenes
{
    internal class IntroScene : Scene
    {
        Sprite _background;
        Text _conInfo;

        public override void OnInitialized()
        {
            base.OnInitialized();

            string[] story = new string[]
            {
                "Hello Captain!",
                "",
                "Our planet is being attacked",
                "by falling space debris!",
                "Your task is to defend our planet!",
                "", "Good luck!"
            };

            _background = new Sprite(LocalContent.Load<Texture2D>("Graphics/Background"));
            _background.Position = new Vector2(1920 / 2, 1080 / 2);
            _background.Scale = new Vector2(1920f / 400) + new Vector2(0.05f);
            AddNode(_background);

            for (int i = 0; i < story.Length; i++)
            {
                Text line;

                line = new Text(LocalContent.Load<SpriteFont>("Fonts/NicoClean"), story[i]);
                line.Origin = new Vector2(0.5f, 0);
                line.Position = new Vector2(1920 / 2, 80 + 80 * i);

                AddNode(line);
            }

            _conInfo = new Text(LocalContent.Load<SpriteFont>("Fonts/NicoClean"), "Press SPACE to continue");
            _conInfo.Origin = new Vector2(0.5f, 1);
            _conInfo.Position = new Vector2(1920 / 2, 1080 - 80);
            AddNode(_conInfo);

            MediaPlayer.Play(Game.Content.Load<Song>("Audio/Race to Mars"));
            MediaPlayer.IsRepeating = true;
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
