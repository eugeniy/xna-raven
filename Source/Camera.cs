using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Raven
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        protected Vector3 m_position = new Vector3(0, 0, 10);
        protected Vector3 m_up = Vector3.Up;
        protected Vector3 m_direction;

        protected const float m_pitchLimit = 1.4f;

        protected const float m_speed = 0.25f;
        protected const float m_mouseSpeedX = 0.0045f;
        protected const float m_mouseSpeedY = 0.0025f;
        protected const int m_edgeSize = 20;

        protected int m_windowWidth;
        protected int m_windowHeight;
        protected float m_aspectRatio;
        protected MouseState m_prevMouse;


        /// <summary>
        /// Creates the instance of the camera.
        /// </summary>
        /// <param name="game">Provides graphics device initialization, game logic, 
        /// rendering code, and a game loop.</param>
        public Camera(Game game) : base(game)
        {
            m_windowWidth = Game.Window.ClientBounds.Width;
            m_windowHeight = Game.Window.ClientBounds.Height;
            m_aspectRatio = (float)m_windowWidth / (float)m_windowHeight;

            // Create the direction vector and normalize it since it will be used for movement
            m_direction = Vector3.Zero - m_position;
            m_direction.Normalize();

            // Create default camera matrices
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, m_aspectRatio, 0.01f, 1000);
            View = CreateLookAt();
        }


        /// <summary>
        /// Creates the instance of the camera at the given location.
        /// </summary>
        /// <param name="game">Provides graphics device initialization, game logic, 
        /// rendering code, and a game loop.</param>
        /// <param name="position">Position of the camera.</param>
        /// <param name="target">The target towards which the camera is pointing.</param>
        public Camera(Game game, Vector3 position, Vector3 target) : this(game)
        {
            m_position = position;
            m_direction = target - m_position;
            m_direction.Normalize();

            View = CreateLookAt();
        }


        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // Lock and center the mouse
            m_prevMouse = Mouse.GetState();
            LockMouse(ref m_prevMouse);

            base.Initialize();
        }


        /// <summary>
        /// Handle the camera movement using user input.
        /// </summary>
        protected virtual void ProcessInput()
        {
            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();

            // Move camera with WASD keys
            if (keyboard.IsKeyDown(Keys.W))
                // Move forward and backwards by adding m_position and m_direction vectors
                m_position += m_direction * m_speed;

            if (keyboard.IsKeyDown(Keys.S))
                m_position -= m_direction * m_speed;

            if (keyboard.IsKeyDown(Keys.A))
                // Strafe by adding a cross product of m_up and m_direction vectors
                m_position += Vector3.Cross(m_up, m_direction) * m_speed;

            if (keyboard.IsKeyDown(Keys.D))
                m_position -= Vector3.Cross(m_up, m_direction) * m_speed;

            if (keyboard.IsKeyDown(Keys.Space))
                m_position += m_up * m_speed;

            if (keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.C))
                m_position -= m_up * m_speed;


            // Calculate yaw to look around with a mouse
            m_direction = Vector3.Transform(m_direction,
                Matrix.CreateFromAxisAngle(m_up, -m_mouseSpeedX * (mouse.X - m_prevMouse.X))
            );

            // Pitch is limited to m_pitchLimit
            float angle = m_mouseSpeedY * (mouse.Y - m_prevMouse.Y);
            if ((Pitch < m_pitchLimit || angle > 0) && (Pitch > -m_pitchLimit || angle < 0))
            {
                m_direction = Vector3.Transform(m_direction,
                    Matrix.CreateFromAxisAngle(Vector3.Cross(m_up, m_direction), angle)
                );
            }

            LockMouse(ref mouse);
            m_prevMouse = mouse;
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Handle camera movement
            ProcessInput();

            View = CreateLookAt();

            base.Update(gameTime);
        }


        /// <summary>
        /// Create a view matrix using camera vectors.
        /// </summary>
        protected Matrix CreateLookAt()
        {
            return Matrix.CreateLookAt(m_position, m_position + m_direction, m_up);
        }


        /// <summary>
        /// Lock the mouse inside the window, preventing it from leaving.
        /// </summary>
        protected virtual void LockMouse(ref MouseState mouse)
        {
            if (mouse.X < m_edgeSize || mouse.X > m_windowWidth - m_edgeSize
                || mouse.Y < m_edgeSize || mouse.Y > m_windowHeight - m_edgeSize)
            {
                Mouse.SetPosition(m_windowWidth / 2, m_windowHeight / 2);
                mouse = Mouse.GetState();

                // Resetting previous state will prevent camera from rotating back
                m_prevMouse = mouse;
            }
        }


        /// <summary>
        /// Position vector.
        /// </summary>
        public Vector3 Position
        {
            get { return m_position; }
        }


        /// <summary>
        /// Yaw of the camera in radians.
        /// </summary>
        public float Yaw
        {
            get { return (float)(Math.PI - Math.Atan2(m_direction.X, m_direction.Z)); }
        }

        /// <summary>
        /// Pitch of the camera in radians.
        /// </summary>
        public float Pitch
        {
            get { return (float)Math.Asin(m_direction.Y); }
        }


        /// <summary>
        /// View matrix accessor.
        /// </summary>
        public Matrix View { get; protected set; }


        /// <summary>
        /// Projection matrix accessor.
        /// </summary>
        public Matrix Projection { get; protected set; }

    }
}
