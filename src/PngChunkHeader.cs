using System;
using System.Buffers.Binary;

namespace StbSharp
{
    public readonly struct PngChunkHeader
    {
        public int Length { get; }
        public PngChunkType Type { get; }

        public bool IsCritical => ((uint)Type & (1 << 29)) == 0;
        public bool IsPublic => ((uint)Type & (1 << 21)) == 0;
        public bool IsSafeToCopy => ((uint)Type & (1 << 21)) == 1;

        public PngChunkHeader(uint length, PngChunkType type)
        {
            if (length > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(length));

            Length = (int)length;
            Type = type;
        }

        public PngChunkHeader(int length, PngChunkType type) : this((uint)length, type)
        {
        }

        public PngChunkHeader(int length, string type) : this(length, ParseType(type))
        {
        }

        public static PngChunkType ParseType(string type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.Length != 4)
                throw new ArgumentException(
                    "The string must be exactly 4 characters long.", nameof(type));

            Span<byte> typeBytes = stackalloc byte[sizeof(uint)];
            for (int i = 0; i < type.Length; i++)
            {
                if (type[i] > byte.MaxValue)
                    throw new ArgumentException(
                        "The character '" + type[i] + "' is invalid.", nameof(type));

                typeBytes[i] = (byte)type[i];
            }

            return (PngChunkType)BinaryPrimitives.ReadUInt32BigEndian(typeBytes);
        }
    }
}