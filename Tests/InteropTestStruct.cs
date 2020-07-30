using System.Runtime.InteropServices;

namespace Test.Collections
{
    [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Ansi)]
    public struct InteropTestStruct
    {
        public static InteropTestStruct Create(int i)
        {
            InteropTestStruct t = new InteropTestStruct()
            {
                B = (byte)(i & 0xFF),
                SB = (sbyte)(-i / 10),
                US = (ushort)i,
                C = (char)i,
                I = i,
                F = (500 - i) * 0.5f,
                D = (500 - i) * 0.5d,
                S = (short)(i - 500),
                UI = (uint)i,
                Text = i.ToString(),
            };
            return t;
        }
        

        public long ID;

        public byte B;

        public sbyte SB;

        public char C;

        public short S;

        public ushort US;

        public int I;

        public uint UI;

        public double D;

        public float F;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string Text;
    }
}