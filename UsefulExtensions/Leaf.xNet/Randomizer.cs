using System;
using System.Security.Cryptography;

namespace Leaf.xNet
{
    /// <summary>
    /// Класс-обёртка- для потокобезопасной генерации псевдослучайных чисел.
    /// Lazy-load singleton для ThreadStatic <see cref="Random"/>.
    /// </summary>
    public static class Randomizer
    {
        private static readonly RNGCryptoServiceProvider Generator = new RNGCryptoServiceProvider();

        private static Random Generate()
        {
            var buffer = new byte[4];
            Generator.GetBytes(buffer);
            return new Random(BitConverter.ToInt32(buffer, 0));
        }

        public static Random Instance => _rand ?? (_rand = Generate());
        [ThreadStatic] private static Random _rand;
    }
}
