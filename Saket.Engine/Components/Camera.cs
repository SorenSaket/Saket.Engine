using System.Numerics;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Saket.ECS;

namespace Saket.Engine
{
    public static class Camera
    {
        public static Query query = new Query().With<CameraOrthographic>();

        public static void CameraSystemOrthographic(World world)
        {
            var entities = world.Query(query);

            WindowInfo window = world.GetResource<WindowInfo>();

            float aspectRatio = (float)window.width / (float)window.height;

            foreach (var entity in entities)
            {
                var camera = entity.Get<CameraOrthographic>();

                camera.projectionMatrix = Matrix4x4.CreateOrthographic(camera.size, camera.size / aspectRatio, camera.near, camera.far);

                entity.Set(camera);
            }
        }

        public static Query query_transformOth = new Query().With<Transform2D>().With<CameraOrthographic>();
        public static void CameraSystem2D(World world)
        {
            var entities = world.Query(query_transformOth);

            foreach (var entity in entities)
            {
                var transform = entity.Get<Transform2D>();
                var camera = entity.Get<CameraOrthographic>();

                camera.viewMatrix = Matrix4x4.CreateTranslation(transform.Position.X, transform.Position.Y, 0);

                entity.Set(camera);
            }
        }
    }

    enum CameraType : byte
    {
        Orthographic,
        Perspective,
    }
    
    public struct CameraOrthographic
    {
        public float size;
        public float near;
        public float far;

        public Matrix4x4 viewMatrix;
        public Matrix4x4 projectionMatrix;

        public CameraOrthographic(float size, float near, float far)
        {
            this.size = size;
            this.near = near;
            this.far = far;
            this.viewMatrix = Matrix4x4.Identity;
            this.projectionMatrix = Matrix4x4.CreateOrthographic(size, size, 0.1f, 100f);
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



    [StructLayout(LayoutKind.Explicit)]
    public struct CameraPerspective
    {
        /*
        [FieldOffset(0)]
        CameraType type;

        // Union based on camera Type
        [FieldOffset(4)]
        public float fieldOfView;
        [FieldOffset(4)]
        public float size;

        public float aspectRatio;
        public float near;
        public float far;

        public Matrix4x4 viewMatrix;
        public Matrix4x4 projectionMatrix;



        public Camera(float fieldOfView, float aspectRatio, float near, float far)
        {
            this.fieldOfView = fieldOfView;
            this.aspectRatio = aspectRatio;
            this.near = near;
            this.far = far;
            this.viewMatrix = Matrix4x4.Identity;
            this.projectionMatrix = Matrix4x4.Identity;
        }

        public Camera()
        {

        }


        private void CalcuateViewMatrix()
        {
            viewMatrix = Matrix4.LookAt(Position, Position + _front, _up);
        }
        private void CalculateProjectionMatrix()
        {
            projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(_fov, aspectRatio, 0.01f, 1000f);
        }

        private void CalculateOrthographicProjectionMatrix()
        {
            projectionMatrix = Matrix4x4.CreateOrthographic(_fov, aspectRatio, 0.01f, 1000f);
        }

        // This function is going to update the direction vertices using some of the math learned in the web tutorials.
        private void UpdateVectors()
        {
            // First, the front matrix is calculated using some basic trigonometry.
            _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            _front.Y = MathF.Sin(_pitch);
            _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            // We need to make sure the vectors are all normalized, as otherwise we would get some funky results.
            _front = Vector3.Normalize(_front);

            // Calculate both the right and the up vector using cross product.
            // Note that we are calculating the right from the global up; this behaviour might
            // not be what you need for all cameras so keep this in mind if you do not want a FPS camera.
            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
            CalcuateViewMatrix();
        }*/
    }

}