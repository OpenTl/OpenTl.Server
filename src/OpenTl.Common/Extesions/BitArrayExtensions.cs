namespace OpenTl.Common.Extesions
{
    using System;
    using System.Collections;

    public static class BitArrayExtensions
    {
        public static byte[] ToByteArray(this BitArray bitArray)
        {
            var ret = new byte[(bitArray.Length - 1) / 8 + 1];
            ((ICollection)bitArray).CopyTo(ret, 0);
            
            return ret;
        }
    }
}