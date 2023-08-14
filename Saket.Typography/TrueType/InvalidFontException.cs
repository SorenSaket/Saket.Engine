using System;
using System.Runtime.Serialization;

namespace Saket.Engine.Typography.TrueType
{
    /// <summary>
    /// Represents errors that occur due to invalid data in a font file.
    /// </summary>
    [Serializable]
    public class InvalidFontException : Exception
    {
        public InvalidFontException()
        {
        }

        public InvalidFontException(string? message) : base(message)
        {
        }

        public InvalidFontException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidFontException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
