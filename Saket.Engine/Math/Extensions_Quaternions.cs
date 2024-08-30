using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine;

public static class Extensions_Quaternions
{
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
    public static Quaternion LookAtRelative(Vector3 sourcePoint, Vector3 targetPoint, Vector3 up)
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
    public static Vector3 ToEulerAngles(this Quaternion q)
    {
        Vector3 angles = new();

        // roll / x
        double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
        double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
        angles.X = (float)System.Math.Atan2(sinr_cosp, cosr_cosp);

        // pitch / y
        double sinp = 2 * (q.W * q.Y - q.Z * q.X);
        if (System.Math.Abs(sinp) >= 1)
        {
            angles.Y = (float)System.Math.CopySign(System.Math.PI / 2, sinp);
        }
        else
        {
            angles.Y = (float)System.Math.Asin(sinp);
        }

        // yaw / z
        double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
        double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
        angles.Z = (float)System.Math.Atan2(siny_cosp, cosy_cosp);

        return angles;
    }
}
