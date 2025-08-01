using Terraria;
using Terraria.UI;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using System;

namespace LPBossFightStats.src.UIElements;

public class MyMod : ModSystem
{
    public override void Load()
    {
        if (!Main.dedServ)
        {
            MyUI = new UserInterface();
            MyUIPanel = new MyUIPanel();
            MyUI.SetState(MyUIPanel);
        }
    }

    public override void UpdateUI(GameTime gameTime)
    {
        if (MyUI != null)
        {
            MyUI.Update(gameTime);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "MyMod: MyUI",
                delegate
                {
                    if (MyUI?.CurrentState != null)
                    {
                        MyUI.Draw(Main.spriteBatch, new GameTime());
                    }
                    return true;
                },
                InterfaceScaleType.UI)
            );
        }
    }

    internal UserInterface MyUI;
    internal MyUIPanel MyUIPanel;
}

public class MyUIPanel : UIState
{
    public override void OnInitialize()
    {
        UIPanel panel = new UIPanel();
        panel.SetPadding(0);
        panel.Left.Set(400f, 0f);
        panel.Top.Set(100f, 0f);
        panel.Width.Set(200f, 0f);
        panel.Height.Set(100f, 0f);
        panel.BackgroundColor = new Color(73, 94, 171);

        UIText text = new UIText("Hello, World!", 0.8f);
        text.Width.Set(0, 1f);
        text.Height.Set(0, 1f);
        text.Top.Set(40f, 0f);
        panel.Append(text);

        UITextButton button = new UITextButton("Click Me!", delegate {
            Main.NewText("Button clicked!", 255, 255, 0);
        });
        button.Width.Set(0, 1f);
        button.Height.Set(30f, 0f);
        button.Top.Set(70f, 0f);
        panel.Append(button);

        Append(panel);
    }
}

public class UITextButton : UIElement
{
    private UIText text;
    private Action onClick;

    public UITextButton(string buttonText, Action onClickAction)
    {
        this.onClick = onClickAction;
        text = new UIText(buttonText);
        Append(text);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        if (IsMouseHovering)
        {
            Main.LocalPlayer.mouseInterface = true;
            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                onClick?.Invoke();
            }
        }
    }
}