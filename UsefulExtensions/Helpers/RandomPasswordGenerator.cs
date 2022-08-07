using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace UsefulExtensions
{
    /// <summary>
    /// The password generator.
    /// </summary>
    [Obsolete("Obsolete")]
    public class RandomPasswordGenerator
    {
        /// <summary>
        /// Contains rule minimum-Length for password generate. [Default: 4]
        /// </summary>
        public int MinimumLengthPassword { get; private set; }

        /// <summary>
        /// Contains rule maximum-Length for password generate. [Default: 32]
        /// </summary>
        public int MaximumLengthPassword { get; private set; }

        /// <summary>
        /// Contains rule minimum-LowerCaseChars must included in password. [Default: 1]
        /// </summary>
        public int MinimumLowerCaseChars { get; private set; }

        /// <summary>
        /// Contains rule minimum-UpperCaseChars must included in password. [Default: 0]
        /// </summary>
        public int MinimumUpperCaseChars { get; private set; }

        /// <summary>
        /// Contains rule minimum-NumericChars must included in password. [Default: 0]
        /// </summary>
        public int MinimumNumericChars { get; private set; }

        /// <summary>
        /// Contains rule minimum-SpecialChars must included in password. [Default: 0]
        /// </summary>
        public int MinimumSpecialChars { get; private set; }

        /// <summary>
        /// Define characters that are valid [a-z] and reject ambiguous characters [ilo]
        /// </summary>
        public static string AllLowerCaseChars { get; private set; }

        /// <summary>
        /// Define characters that are valid [A-Z] and reject ambiguous characters [ILO]
        /// </summary>
        public static string AllUpperCaseChars { get; private set; }

        /// <summary>
        /// Define characters that are valid [0-9] and reject ambiguous characters [01]
        /// </summary>
        public static string AllNumericChars { get; private set; }

        /// <summary>
        /// Define characters that are valid [!@()_-=]
        /// </summary>
        public static string AllSpecialChars { get; private set; }

        private readonly string _allAvailableChars;

        private readonly RandomSecureVersion _randomSecure = new RandomSecureVersion();
        private readonly int _minimumNumberOfChars;

        static RandomPasswordGenerator()
        {
            // Define characters that are valid and reject ambiguous characters such as ilo, IO and 1 or 0
            AllLowerCaseChars = GetCharRange('a', 'z', exclusiveChars: "ilo");
            AllUpperCaseChars = GetCharRange('A', 'Z', exclusiveChars: "IO");
            AllNumericChars = GetCharRange('1', '9');
            AllSpecialChars = "!@()_-=";
        }

        /// <summary>
        /// Minimal length rules for char sets
        /// </summary>
        /// <param name="minimumLengthPassword">default 4</param>
        /// <param name="maximumLengthPassword">default 32</param>
        /// <param name="minimumLowerCaseChars">default 1</param>
        /// <param name="minimumUpperCaseChars">default 0</param>
        /// <param name="minimumNumericChars">default 0</param>
        /// <param name="minimumSpecialChars">default 0</param>
        /// <exception cref="ArgumentException">Incorrect minimal charset rule</exception>
        public RandomPasswordGenerator(
            int minimumLengthPassword = 4,
            int maximumLengthPassword = 32,
            int minimumLowerCaseChars = 1,
            int minimumUpperCaseChars = 0,
            int minimumNumericChars = 0,
            int minimumSpecialChars = 0)
        {
            if(minimumLengthPassword < 4)
            {
                throw new ArgumentException("The minimum-Length is smaller than 4.",
                    nameof(minimumLengthPassword));
            }

            if(minimumLengthPassword > maximumLengthPassword)
            {
                throw new ArgumentException("The minimum-Length is bigger than the maximum (32) length.",
                    nameof(minimumLengthPassword));
            }

            if(minimumLowerCaseChars < 1)
            {
                throw new ArgumentException("The minimum-LowerCase is smaller than 1.",
                    nameof(minimumLowerCaseChars));
            }

            if(minimumUpperCaseChars < 0)
            {
                throw new ArgumentException("The minimum-UpperCase is smaller than 0.",
                    nameof(minimumUpperCaseChars));
            }

            if(minimumNumericChars < 0)
            {
                throw new ArgumentException("The minimum-Numeric is smaller than 0.",
                    nameof(minimumNumericChars));
            }

            if(minimumSpecialChars < 0)
            {
                throw new ArgumentException("The minimum-Special is smaller than 0.",
                    nameof(minimumSpecialChars));
            }

            _minimumNumberOfChars = minimumLowerCaseChars + minimumUpperCaseChars +
                                    minimumNumericChars + minimumSpecialChars;

            if(minimumLengthPassword < _minimumNumberOfChars)
            {
                throw new ArgumentException(
                    "The minimum length of the password is smaller than the sum " +
                    "of the minimum characters of all categories.",
                    nameof(maximumLengthPassword));
            }

            MinimumLengthPassword = minimumLengthPassword;
            MaximumLengthPassword = maximumLengthPassword;

            MinimumLowerCaseChars = minimumLowerCaseChars;
            MinimumUpperCaseChars = minimumUpperCaseChars;
            MinimumNumericChars = minimumNumericChars;
            MinimumSpecialChars = minimumSpecialChars;

            _allAvailableChars =
                OnlyIfOneCharIsRequired(minimumLowerCaseChars, AllLowerCaseChars) +
                OnlyIfOneCharIsRequired(minimumUpperCaseChars, AllUpperCaseChars) +
                OnlyIfOneCharIsRequired(minimumNumericChars, AllNumericChars) +
                OnlyIfOneCharIsRequired(minimumSpecialChars, AllSpecialChars);
        }

        private string OnlyIfOneCharIsRequired(int minimum, string allChars)
        {
            return minimum > 0 || _minimumNumberOfChars == 0 ? allChars : string.Empty;
        }

        private static string GetCharRange(char minimum, char maximum, string exclusiveChars = "")
        {
            var result = string.Empty;
            for(char value = minimum; value <= maximum; value++)
            {
                result += value;
            }
            if(!string.IsNullOrEmpty(exclusiveChars))
            {
                var inclusiveChars = result.Except(exclusiveChars).ToArray();
                result = new string(inclusiveChars);
            }
            return result;
        }
    }

    [Obsolete("Obsolete")]
    internal static class Extensions
    {
        private static readonly Lazy<RandomSecureVersion> RandomSecure =
            new Lazy<RandomSecureVersion>(() => new RandomSecureVersion());

        public static IEnumerable<T> ShuffleSecure<T>(this IEnumerable<T> source)
        {
            var sourceArray = source.ToArray();
            for(var counter = 0; counter < sourceArray.Length; counter++)
            {
                int randomIndex = RandomSecure.Value.Next(counter, sourceArray.Length);
                yield return sourceArray[randomIndex];

                sourceArray[randomIndex] = sourceArray[counter];
            }
        }

        public static string ShuffleTextSecure(this string source)
        {
            var shuffledChars = source.ShuffleSecure().ToArray();
            return new string(shuffledChars);
        }
    }

    [Obsolete("Obsolete")]
    internal class RandomSecureVersion
    {
        //Never ever ever never use Random() in the generation of anything that requires true security/randomness
        //and high entropy or I will hunt you down with a pitchfork!! Only RNGCryptoServiceProvider() is safe.
        private readonly RNGCryptoServiceProvider _rngProvider = new RNGCryptoServiceProvider();

        public int Next()
        {
            var randomBuffer = new byte[4];
            _rngProvider.GetBytes(randomBuffer);
            var result = BitConverter.ToInt32(randomBuffer, 0);
            return result;
        }

        public int Next(int maximumValue)
        {
            // Do not use Next() % maximumValue because the distribution is not OK
            return Next(0, maximumValue);
        }

        public int Next(int minimumValue, int maximumValue)
        {
            var seed = Next();

            //  Generate uniformly distributed random integers within a given range.
            return new Random(seed).Next(minimumValue, maximumValue);
        }
    }

    // var generator = new PasswordGenerator();
    // string password = generator.Generate();
    // Console.WriteLine(password);
}