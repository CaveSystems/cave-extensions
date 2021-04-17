namespace Cave
{
    /// <summary>Gets extensions working on the bits of system types.</summary>
    public static class BitwiseExtensions
    {
        #region int

        /// <summary>
        /// ROL rotates the bits within the destination operand to the left, where left is toward the most significant bit (MSB). A rotate is
        /// a shift (see SHL and SHR) that wraps around; the leftmost bit of the operand is shifted into the rightmost bit, and all intermediate bits
        /// are shifted one bit to the left. Except for the direction the shift operation takes, ROL is identical to ROR.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="count">The number of bits to rotate.</param>
        /// <returns>The bitwise rotated value.</returns>
        public static int BitwiseRotateLeft(this int value, int count) => (int)(((uint)value << count) | ((uint)value >> (32 - count)));

        /// <summary>
        /// ROL rotates the bits within the destination operand to the left, where left is toward the most significant bit (MSB). A rotate is
        /// a shift (see SHL and SHR) that wraps around; the leftmost bit of the operand is shifted into the rightmost bit, and all intermediate bits
        /// are shifted one bit to the left. Except for the direction the shift operation takes, ROL is identical to ROR.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <returns>The bitwise rotated value.</returns>
        public static int BitwiseRotateLeft(this int value) => (int)(((uint)value << 1) | ((uint)value >> 31));

        /// <summary>
        /// ROR rotates the bits within the destination operand to the right, where right is toward the least significant bit (LSB). A rotate
        /// is a shift (see SHL and SHR) that wraps around; the rightmost bit of the operand is shifted into the leftmost bit, and all intermediate
        /// bits are shifted one bit to the right. Except for the direction the shift operation takes, ROR is identical to ROL.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="count">The number of bits to rotate.</param>
        /// <returns>The bitwise rotated value.</returns>
        public static int BitwiseRotateRight(this int value, int count) => (int)(((uint)value >> count) | ((uint)value << (32 - count)));

        /// <summary>
        /// ROR rotates the bits within the destination operand to the right, where right is toward the least significant bit (LSB). A rotate
        /// is a shift (see SHL and SHR) that wraps around; the rightmost bit of the operand is shifted into the leftmost bit, and all intermediate
        /// bits are shifted one bit to the right. Except for the direction the shift operation takes, ROR is identical to ROL.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <returns>The bitwise rotated value.</returns>
        public static int BitwiseRotateRight(this int value) => (int)(((uint)value >> 1) | ((uint)value << 31));

        #endregion

        #region uint

        /// <summary>
        /// ROL rotates the bits within the destination operand to the left, where left is toward the most significant bit (MSB). A rotate is
        /// a shift (see SHL and SHR) that wraps around; the leftmost bit of the operand is shifted into the rightmost bit, and all intermediate bits
        /// are shifted one bit to the left. Except for the direction the shift operation takes, ROL is identical to ROR.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="count">The number of bits to rotate.</param>
        /// <returns>The bitwise rotated value.</returns>
        public static uint BitwiseRotateLeft(this uint value, int count) => (value << count) | (value >> (32 - count));

        /// <summary>
        /// ROL rotates the bits within the destination operand to the left, where left is toward the most significant bit (MSB). A rotate is
        /// a shift (see SHL and SHR) that wraps around; the leftmost bit of the operand is shifted into the rightmost bit, and all intermediate bits
        /// are shifted one bit to the left. Except for the direction the shift operation takes, ROL is identical to ROR.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <returns>The bitwise rotated value.</returns>
        public static uint BitwiseRotateLeft(this uint value) => (value << 1) | (value >> 31);

        /// <summary>
        /// ROR rotates the bits within the destination operand to the right, where right is toward the least significant bit (LSB). A rotate
        /// is a shift (see SHL and SHR) that wraps around; the rightmost bit of the operand is shifted into the leftmost bit, and all intermediate
        /// bits are shifted one bit to the right. Except for the direction the shift operation takes, ROR is identical to ROL.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="count">The number of bits to rotate.</param>
        /// <returns>The bitwise rotated value.</returns>
        public static uint BitwiseRotateRight(this uint value, int count) => (value >> count) | (value << (32 - count));

        /// <summary>
        /// ROR rotates the bits within the destination operand to the right, where right is toward the least significant bit (LSB). A rotate
        /// is a shift (see SHL and SHR) that wraps around; the rightmost bit of the operand is shifted into the leftmost bit, and all intermediate
        /// bits are shifted one bit to the right. Except for the direction the shift operation takes, ROR is identical to ROL.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <returns>The bitwise rotated value.</returns>
        public static uint BitwiseRotateRight(this uint value) => (value >> 1) | (value << 31);

        #endregion

        #region long

        /// <summary>
        /// ROL rotates the bits within the destination operand to the left, where left is toward the most significant bit (MSB). A rotate is
        /// a shift (see SHL and SHR) that wraps around; the leftmost bit of the operand is shifted into the rightmost bit, and all intermediate bits
        /// are shifted one bit to the left. Except for the direction the shift operation takes, ROL is identical to ROR.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="count">The number of bits to rotate.</param>
        /// <returns>The bitwise rotated value.</returns>
        public static long BitwiseRotateLeft(this long value, int count) => (long)(((ulong)value << count) | ((ulong)value >> (64 - count)));

        /// <summary>
        /// ROL rotates the bits within the destination operand to the left, where left is toward the most significant bit (MSB). A rotate is
        /// a shift (see SHL and SHR) that wraps around; the leftmost bit of the operand is shifted into the rightmost bit, and all intermediate bits
        /// are shifted one bit to the left. Except for the direction the shift operation takes, ROL is identical to ROR.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <returns>The bitwise rotated value.</returns>
        public static long BitwiseRotateLeft(this long value) => (long)(((ulong)value << 1) | ((ulong)value >> 63));

        /// <summary>
        /// ROR rotates the bits within the destination operand to the right, where right is toward the least significant bit (LSB). A rotate
        /// is a shift (see SHL and SHR) that wraps around; the rightmost bit of the operand is shifted into the leftmost bit, and all intermediate
        /// bits are shifted one bit to the right. Except for the direction the shift operation takes, ROR is identical to ROL.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="count">The number of bits to rotate.</param>
        /// <returns>The bitwise rotated value.</returns>
        public static long BitwiseRotateRight(this long value, int count) => (long)(((ulong)value >> count) | ((ulong)value << (64 - count)));

        /// <summary>
        /// ROR rotates the bits within the destination operand to the right, where right is toward the least significant bit (LSB). A rotate
        /// is a shift (see SHL and SHR) that wraps around; the rightmost bit of the operand is shifted into the leftmost bit, and all intermediate
        /// bits are shifted one bit to the right. Except for the direction the shift operation takes, ROR is identical to ROL.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <returns>The bitwise rotated value.</returns>
        public static long BitwiseRotateRight(this long value) => (long)(((ulong)value >> 1) | ((ulong)value << 63));

        #endregion

        #region ulong

        /// <summary>
        /// ROL rotates the bits within the destination operand to the left, where left is toward the most significant bit (MSB). A rotate is
        /// a shift (see SHL and SHR) that wraps around; the leftmost bit of the operand is shifted into the rightmost bit, and all intermediate bits
        /// are shifted one bit to the left. Except for the direction the shift operation takes, ROL is identical to ROR.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="count">The number of bits to rotate.</param>
        /// <returns>The bitwise rotated value.</returns>
        public static ulong BitwiseRotateLeft(this ulong value, int count) => (value << count) | (value >> (64 - count));

        /// <summary>
        /// ROL rotates the bits within the destination operand to the left, where left is toward the most significant bit (MSB). A rotate is
        /// a shift (see SHL and SHR) that wraps around; the leftmost bit of the operand is shifted into the rightmost bit, and all intermediate bits
        /// are shifted one bit to the left. Except for the direction the shift operation takes, ROL is identical to ROR.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <returns>The bitwise rotated value.</returns>
        public static ulong BitwiseRotateLeft(this ulong value) => (value << 1) | (value >> 63);

        /// <summary>
        /// ROR rotates the bits within the destination operand to the right, where right is toward the least significant bit (LSB). A rotate
        /// is a shift (see SHL and SHR) that wraps around; the rightmost bit of the operand is shifted into the leftmost bit, and all intermediate
        /// bits are shifted one bit to the right. Except for the direction the shift operation takes, ROR is identical to ROL.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="count">The number of bits to rotate.</param>
        /// <returns>The bitwise rotated value.</returns>
        public static ulong BitwiseRotateRight(this ulong value, int count) => (value >> count) | (value << (64 - count));

        /// <summary>
        /// ROR rotates the bits within the destination operand to the right, where right is toward the least significant bit (LSB). A rotate
        /// is a shift (see SHL and SHR) that wraps around; the rightmost bit of the operand is shifted into the leftmost bit, and all intermediate
        /// bits are shifted one bit to the right. Except for the direction the shift operation takes, ROR is identical to ROL.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <returns>The bitwise rotated value.</returns>
        public static ulong BitwiseRotateRight(this ulong value) => (value >> 1) | (value << 63);

        #endregion
    }
}
