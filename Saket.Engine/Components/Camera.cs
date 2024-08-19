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
                    camera.viewMatrix =  Matrix4x4.CreateTranslation(transform.Position); // Matrix4x4.CreateFromQuaternion(transform.Rotation) *
                }
                if(camera.cameraType == CameraType.Perspective)
                {
                    camera.projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView((MathF.PI/180) *  camera.size, aspectRatio, 0.01f, 1000f);
                }
                else
                {
                    camera.projectionMatrix = Matrix4x4.CreateOrthographic(camera.size, camera.size / aspectRatio, camera.near, camera.far);
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
                   
                    transform.Rotation = LookAt2(transform.Position, control.lookAtTarget, Vector3.UnitY);

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
        public static Quaternion LookAt(Vector3 sourcePoint, Vector3 destPoint)
        {
            Vector3 forwardVector = Vector3.Normalize(destPoint - sourcePoint);

            float dot = Vector3.Dot(Vector3.UnitZ, forwardVector);

            if (MathF.Abs(dot - (-1.0f)) < 0.000001f)
            {
                return new Quaternion(Vector3.UnitY, MathF.PI);
            }
            if (MathF.Abs(dot - (1.0f)) < 0.000001f)
            {
                return Quaternion.Identity;
            }

            float rotAngle = (float)MathF.Acos(dot);
            Vector3 rotAxis = Vector3.Cross(Vector3.UnitZ, forwardVector);
            rotAxis = Vector3.Normalize(rotAxis);
            return Quaternion.CreateFromAxisAngle(rotAxis, rotAngle);
        }
        public static Quaternion LookAt2(Vector3 sourcePoint, Vector3 targetPoint, Vector3 up)
        {
            // Step 1: Calculate the forward direction from source to target
            Vector3 forward = Vector3.Normalize(targetPoint - sourcePoint);

            // Step 2: Calculate the right vector using cross product
            Vector3 right = Vector3.Normalize(Vector3.Cross(up, forward));

            // Step 3: Recalculate the orthonormal up vector using cross product
            Vector3 recalculatedUp = Vector3.Cross(forward, right);

            // Step 4: Create a rotation matrix from the basis vectors
            Matrix4x4 rotationMatrix = new Matrix4x4(
                right.X, right.Y, right.Z, 0,
                recalculatedUp.X, recalculatedUp.Y, recalculatedUp.Z, 0,
                forward.X, forward.Y, forward.Z, 0,
                0, 0, 0, 1);

            // Step 5: Convert the rotation matrix to a quaternion
            return Quaternion.CreateFromRotationMatrix(rotationMatrix);
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


        public Matrix4x4 viewMatrix;
        public Matrix4x4 projectionMatrix;
        
        public Camera(CameraType cameraType, float size, float ratio, float near, float far)
        {
            this.cameraType = cameraType;
            this.size = size;
            this.near = near;
            this.far = far;
            this.viewMatrix = Matrix4x4.Identity;
            this.projectionMatrix = Matrix4x4.Identity;
        }

        public Vector3 ScreenToWorldPoint(Vector3 position)
        {
            throw new NotImplementedException();
            /*
            Matrix4x4.Invert(projectionMatrix, out var invproj);
            Matrix4x4.Invert(projectionMatrix, out var invView);

            Vector4 transformedWorldPosition = Vector4.Transform(new Vector4(position, 1f), invView);

            //Divide by w 
            transformedWorldPosition /= -transformedWorldPosition.W;

            return transformedWorldPosition.XYZ();*/
        }

        public Vector3 WorldToScreenPoint(Vector3 position)
        {
           throw new NotImplementedException();
            /*return Vector4.Transform(new Vector4(position, 1f), viewMatrix * projectionMatrix).XYZ();*/
        }

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