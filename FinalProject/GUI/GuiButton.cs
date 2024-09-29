using FinalProject.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.GUI
{

    public enum HorizontalTextAlign
    {
        Left,
        Center,
        Right
    }

    // Enum to hold the possible alignments of text Vertically 
    public enum VerticalTextAlign
    {
        Top,
        Center,
        Bottom
    }
    internal class GuiButton
    {
        private Rectangle area;

        private Vector2 textPosition;
        private static SpriteFont buttonFont;
        private ButtonStates state;
        private string text;

        private VerticalTextAlign verticalAlign;
        private HorizontalTextAlign horizontalAlign;

        //
        // Private constants
        //

        private const int DefaultWidth = 100;
        private const int DefaultHeight = 50;

        //
        // Public fields
        //
        public event EventHandler GuiButtonClicked;

        //
        // Constructors
        //

        public GuiButton(Point position, string text, VerticalTextAlign verticalAlign = VerticalTextAlign.Center, HorizontalTextAlign horizontalAlign = HorizontalTextAlign.Center)
           : this(new Rectangle(position, new Point(DefaultWidth, DefaultHeight)), text, verticalAlign, horizontalAlign)
        { }

        public GuiButton(Point position, Point size, string text, VerticalTextAlign verticalAlign = VerticalTextAlign.Center, HorizontalTextAlign horizontalAlign = HorizontalTextAlign.Center)
           : this(new Rectangle(position, new Point(size.X, size.Y)), text, verticalAlign, horizontalAlign)
        { }

        public GuiButton(Rectangle area, string text, VerticalTextAlign verticalAlign = VerticalTextAlign.Center, HorizontalTextAlign horizontalAlign = HorizontalTextAlign.Center)
        {
            if (buttonFont == null)
                buttonFont = ContentHelper.GetFont("KenneySpace");

            this.area = area;
            this.text = text;
            this.verticalAlign = verticalAlign;
            this.horizontalAlign = horizontalAlign;

            AssignDrawPositions();

            InputHelper.ButtonPressed += OnButtonPressed;
            InputHelper.MouseMoved += OnMouseMoved;
        }


        // Method for when button is pressed
        private void OnButtonPressed(Utilities.Buttons button)
        {
            if (button == Utilities.Buttons.Swap
               && InputHelper.MouseIsOver(area))
            {
                state = ButtonStates.Click;
                GuiButtonClicked?.Invoke(this, EventArgs.Empty);
            }
        }

        // Method for when mouse is moved (check if it is over)
        private void OnMouseMoved(Point mousePosition)
        {
            if (InputHelper.MouseIsOver(area))
            {
                if (state != ButtonStates.Hover)
                {
                    state = ButtonStates.Hover;
                    area.Y += 10; area.Height -= 10; // #DIRTY
                    AssignDrawPositions();
                }
            }
            else if (!(state == ButtonStates.None))
            {
                state = ButtonStates.None;
                area.Y -= 10; area.Height += 10; // #DIRTY
                AssignDrawPositions();
            }
        }

        // Assuming that position, width & height are set correctly, refresh all other 
        // positions based on those
        private void AssignDrawPositions()
        {
            Vector2 textVector = buttonFont.MeasureString(text);

            // Horizontal alignment
            switch (horizontalAlign)
            {
                case HorizontalTextAlign.Left:
                    textPosition.X = area.Left;
                    break;
                case HorizontalTextAlign.Center:
                    textPosition.X = area.Left + (((area.Right - area.Left) - (int)textVector.X) / 2);
                    break;
                case HorizontalTextAlign.Right:
                    textPosition.X = area.Right - (int)textVector.X;
                    break;
            }

            // Vertical alignment
            switch (verticalAlign)
            {
                case VerticalTextAlign.Bottom:
                    textPosition.Y = area.Bottom - (int)textVector.Y;
                    break;
                case VerticalTextAlign.Center:
                    textPosition.Y = area.Top + (((area.Bottom - area.Top) - (int)textVector.Y) / 2);
                    break;
                case VerticalTextAlign.Top:
                    textPosition.Y = area.Top;
                    break;
            }
        }

        // Main update method
        public void Update()
        {

        }

        // Main draw method
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the button
            switch (state)
            {
                case ButtonStates.None:
                    spriteBatch.Draw(ContentHelper.GetTexture("yellow_button04"), area, Color.White);
                    break;
                case ButtonStates.Hover:
                    spriteBatch.Draw(ContentHelper.GetTexture("yellow_button05"), area, Color.White);
                    break;
                case ButtonStates.Click:
                    // spriteBatch.Draw();
                    break;
            }

            // Draw the text
            spriteBatch.DrawString(
               ContentHelper.GetFont("KenneySpace"),
               text,
               textPosition,
               Color.Black,
               0f,
               Vector2.Zero,
               ScaleHelper.ScreenScaleVector,
               SpriteEffects.None,
               0f);
        }
    }
}
