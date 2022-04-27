using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DarkestGun
{
    public class Camera
    {
        public float Zoom { get; set; }
        private float currentMouseWheelValue, previousMouseWheelValue;

        public Matrix Transform { get; private set; }

        public Camera()
        {
            Zoom = 1f;
        }

        public void Update(Player target)
        {
            ZoomCheck();
            Matrix position = Matrix.CreateTranslation(
              (int)(-target.Position.X - (target.SourceRect.Width / 2)),
              (int)(-target.Position.Y - (target.SourceRect.Height / 2)),
              0);

            Matrix zoom = Matrix.CreateScale(Zoom, Zoom, 1);

            Matrix offset = Matrix.CreateTranslation(
                (int)(Main.ScreenDimension.Width / 2),
                (int)(Main.ScreenDimension.Height / 2),
                0);

            Transform = position * zoom * offset;
        }

        public void AdjustZoom(float ZoomAmount)
        {
            Zoom += ZoomAmount;
            if (Zoom < .35f)
            {
                Zoom = .35f;
            }
            if (Zoom > 5f)
            {
                Zoom = 5f;
            }
        }

        public void ZoomCheck()
        {
            previousMouseWheelValue = currentMouseWheelValue;
            currentMouseWheelValue = Mouse.GetState().ScrollWheelValue;

            if (currentMouseWheelValue > previousMouseWheelValue)
            {
                AdjustZoom(.1f);
            }

            if (currentMouseWheelValue < previousMouseWheelValue)
            {
                AdjustZoom(-.1f);
            }
        }
    }
}
