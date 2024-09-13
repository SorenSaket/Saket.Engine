using System;
using System.Numerics;
using System.Runtime.CompilerServices;

using Saket.ECS;
using Saket.Engine.Input;

namespace Saket.Engine
{
    public static class Component_Camera
    {
        private static Query query = new Query().With<Camera>();
        public static void System_Camera(World world)
        {
            var entities = world.Query(query);

            WindowInfo window = world.GetResource<WindowInfo>();
            // 
            float aspectRatio = (float)window.width / (float)window.height;

            foreach (var entity in entities)
            {
                var camera = entity.Get<Camera>();

                if (entity.Has<Transform>())
                {
                    var transform = entity.Get<Transform>();
                    camera.UpdateView(transform);

                    camera.UpdateProjection(transform, aspectRatio);
                }
                else
                {
                    camera.UpdateProjection(new Transform(), aspectRatio);
                }



                entity.Set(camera);
            }
        }

        private static Query query_cc = new Query().With<Camera>().With<CameraControl>().With<Transform>();

        public static void System_CameraControl(World world)
        {
            var entities = world.Query(query_cc);

            WindowInfo window = world.GetResource<WindowInfo>();
            KeyboardState keyboard = world.GetResource<KeyboardState>();
            MouseState mouse = world.GetResource<MouseState>();
            // 
            float aspectRatio = (float)window.width / (float)window.height;

            foreach (var entity in entities)
            {
                var control = entity.Get<CameraControl>();
                var transform = entity.Get<Transform>();
                var camera = entity.Get<Camera>();

                Vector2 movement = new Vector2();
                movement.Y += keyboard.IsKeyDown(26) ? 1 : 0;
                movement.Y -= keyboard.IsKeyDown(22) ? 1 : 0;
                movement.X += keyboard.IsKeyDown(7) ? 1 : 0;
                movement.X -= keyboard.IsKeyDown(4) ? 1 : 0;
                control.zoom = mouse.Scroll.Y * world.Delta * control.sentitivity_zoom; //mouse.IsButtonDown(MouseButton.Middle)

                movement *= world.Delta * control.sentitivity;


                Vector3 forward = Vector3.Normalize(transform.Position - control.lookAtTarget);
                
                if (control.lookAt)
                {
                    transform.Rotation = Extensions_Quaternions.LookAtRelative(transform.Position, control.lookAtTarget, Vector3.UnitY);

                    if (control.zoom != 0)
                    {
                        if(camera.cameraType == CameraType.Orthographic)
                            camera.size += control.zoom;
                        else
                            transform.Position += forward * control.zoom;
                    }

                    transform.Position -= Vector3.Transform(Vector3.UnitX, transform.Rotation) * movement.X;
                    transform.Position -= Vector3.Transform(Vector3.UnitY, transform.Rotation) * movement.Y;
                }
                
                entity.Set(transform);
                entity.Set(camera);
            }
        }
       
    }

    public enum CameraType : byte
    {
        Orthographic,
        Perspective,
    }
    
    public interface ICamera
    {
        public Matrix4x4 ViewMatrix { get; }
        public Matrix4x4 ProjectionMatrix { get; }
    }

    public struct Camera : ICamera
    {
        /// <summary>
        /// The size of the othographic camera or fov of perspective camera
        /// </summary>
        public float size;
        public float near;
        public float far;
        public CameraType cameraType;

        public Matrix4x4 ViewMatrix { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => viewMatrix; }
        public Matrix4x4 ProjectionMatrix { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ProjectionMatrix; }


        public Matrix4x4 viewProjectionMatrix;
        public Matrix4x4 inverseViewProjectionMatrix;
        public Matrix4x4 viewMatrix;
        public Matrix4x4 projectionMatrix;
        public Matrix4x4 inverseViewMatrix;
        public Matrix4x4 inversprojectionMatrix;


        public Camera(CameraType cameraType, float size, float ratio, float near, float far)
        {
            this.cameraType = cameraType;
            this.size = size;
            this.near = near;
            this.far = far;
            this.viewMatrix = Matrix4x4.Identity;
            this.projectionMatrix = Matrix4x4.Identity;
        }

     

        public void UpdateView(Transform transform)
        {
            inverseViewMatrix = Matrix4x4.CreateScale(transform.Scale) * Matrix4x4.CreateFromQuaternion(transform.Rotation) * Matrix4x4.CreateTranslation(transform.Position);
            Matrix4x4.Invert(inverseViewMatrix, out viewMatrix);
        }

        public void UpdateProjection(Transform transform, float screenAspectRatio)
        {
            if (cameraType == CameraType.Perspective)
            {
                projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView((MathF.PI / 180) * size, screenAspectRatio, 0.01f, 1000f);
            }
            else
            {
                float halfSize = size/2f;  // The "size" of the orthographic view

                // Adjust the orthographic projection based on the camera position
                float left = transform.Position.X - halfSize * screenAspectRatio;
                float right = transform.Position.X + halfSize * screenAspectRatio;
                float bottom = transform.Position.Y - halfSize;
                float top = transform.Position.Y + halfSize;

                projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, near, far);
                //projectionMatrix = Matrix4x4.CreateOrthographic(size * screenAspectRatio, size, near, far);

            }
            Matrix4x4.Invert(projectionMatrix, out inversprojectionMatrix);
        }


        public void UpdateInternalValues()
        {
            viewProjectionMatrix = projectionMatrix * viewMatrix;
            Matrix4x4.Invert(viewProjectionMatrix, out inverseViewProjectionMatrix);
        }

        #region Utils

        /// <summary>
        /// 
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <param name="depth"></param>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        /// <returns></returns>
        public Vector3 ScreenToWorldPoint(Vector2 screenPosition, float depth, int screenWidth, int screenHeight)
        {
            // Convert screen position to normalized device coordinates (NDC)
            Vector3 ndc = new Vector3(
                ((2.0f * screenPosition.X) / screenWidth) - 1.0f,
                ((2.0f * screenPosition.Y) / screenHeight) - 1.0f,
                depth // Z-value in NDC space
            );

            // Construct a clip-space position from NDC (homogeneous coordinates)
            Vector4 clipSpacePos = new Vector4(ndc, 1.0f);
            
            // Transform clip space coordinates to world coordinates
            Vector4 worldPos = Vector4.Transform(clipSpacePos, inverseViewProjectionMatrix);

            // Perform perspective divide
            Vector3 worldPosition = new Vector3(worldPos.X, worldPos.Y, worldPos.Z) / worldPos.W;

            return worldPosition;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        /// <returns></returns>
        public Vector2 WorldToScreenPoint(Vector3 worldPosition, int screenWidth, int screenHeight)
        {
            // Transform the world position by the view and projection matrices
            Vector4 clipSpacePos = Vector4.Transform(worldPosition, viewProjectionMatrix);

            // Perform perspective divide to get normalized device coordinates (NDC)
            Vector3 ndc = new Vector3(clipSpacePos.X, clipSpacePos.Y, clipSpacePos.Z) / clipSpacePos.W;

            // Convert NDC to screen coordinates
            Vector2 screenPos = new Vector2(
                (ndc.X + 1.0f) * 0.5f * screenWidth,
                (1.0f - ndc.Y) * 0.5f * screenHeight
            );

            return screenPos;
        }
        
        #endregion
    }

    public struct CameraControl
    {
        public float sentitivity_zoom = 50;
        public float sentitivity = 20;

        public float yaw, pitch, roll, zoom;

        public Vector3 lookAtTarget;
        public bool lookAt;


        // input




        public CameraControl()
        {
        }
    }
    
}