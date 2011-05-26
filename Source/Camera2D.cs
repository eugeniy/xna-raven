using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Raven
{
    public class Camera2D : Camera
    {
        protected const float m_minDistance = 5;
        protected const float m_maxDistance = 15;

        /// <summary>
        /// Creates the instance of the camera.
        /// </summary>
        /// <param name="game">Provides the main game type.</param>
        public Camera2D(Game game) : base(game)
        {
        }

        /// <summary>
        /// Handle the camera movement using user input.
        /// </summary>
        protected override void ProcessInput()
        {
            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.A))
                m_position += Vector3.Cross(m_up, m_direction) * m_speed;

            if (keyboard.IsKeyDown(Keys.D))
                m_position -= Vector3.Cross(m_up, m_direction) * m_speed;

            if (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.W))
                m_position += m_up * m_speed;

            if (keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.S))
                m_position -= m_up * m_speed;

            if (mouse != m_prevMouse)
            {
                // Zoom with a scroll wheel.
                if (mouse.ScrollWheelValue > m_prevMouse.ScrollWheelValue && m_position.Z > m_minDistance)
                    m_position += m_direction * m_speed;

                else if (mouse.ScrollWheelValue < m_prevMouse.ScrollWheelValue && m_position.Z < m_maxDistance)
                    m_position -= m_direction * m_speed;

                m_prevMouse = Mouse.GetState();
            }
        }
    }
}
