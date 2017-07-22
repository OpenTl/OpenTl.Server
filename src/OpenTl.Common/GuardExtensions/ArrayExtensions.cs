namespace OpenTl.Common.GuardExtensions
{
    using BarsGroup.CodeGuard.Exceptions;
    using BarsGroup.CodeGuard.Internals;

    public static class ArrayExtensions
    {
        public static void IsItemsEquals<T>(this ArgBase<T[]> arg, T[] target)
        {
            var source = arg.Value;
                
            if (source.Length != target.Length)
            {
                throw new ArgumentException("not equals");
            }
            
            for (var i = 0; i < source.Length; i++)
            {
                if (!target[i].Equals(source[i]))
                {
                    throw new ArgumentException("not equals");
                }
            }
        }
        
    }
}