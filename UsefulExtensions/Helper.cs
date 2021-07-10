using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UsefulExtensions
{
    public class Account
    {
        /// <summary>
        /// Возвращает аккаунт с рандомно сгенерированными логином и паролем.
        /// </summary>
        /// <param name="usernameLength">Длина рандомно сгенерированного логина</param>
        /// <param name="passwordLength">Длина рандомно сгенерированного пароля</param>
        /// <returns>Аккаунт с рандомно сгенерированными логином и паролем</returns>
        /// <remarks>
        /// Для генерации используется <see cref="RandomStringGenerator.AllSymbolsGenerator"/>
        /// </remarks>
        public static Account GenerateRandomAccount(int usernameLength, int passwordLength)
        {
            RandomStringGenerator randomString = RandomStringGenerator.AllSymbolsGenerator;
            return new Account(randomString.Generate(usernameLength), randomString.Generate(passwordLength));
        }

        /// <summary>
        /// Конструктор класса <see cref="Account"/>
        /// </summary>
        /// <param name="login">Логин аккаунта</param>
        /// <param name="password">Пароль аккаунта</param>
        public Account(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public string Login { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Проверен ли аккаунт. Обычно используется для чекеров.
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Возвращает строковое представление аккаунта в формате login:password
        /// </summary>
        /// <returns>Cтроковое представление аккаунта в формате login:password</returns>
        public override string ToString() => $"{Login}:{Password}";
    }

    /// <summary>
    /// Статический класс, содержащий методы, часто используемые в создании софта (чекеры, регеры и пр. софт для автоматизации действий на сайтах)
    /// </summary>
    public static class Helper
    {
        //public const string RuCaptchaKey = "71c4c23aa69f4ccf131dbf5486961f18";
        //public const string AntiCaptchaKey = "c60034bb3e49f18aba261067b1f6f697";
        //public const string SmsHubKey = "58779Udcdca72b9beb17fa831a81bf34f5c36f";

        /// <summary>
        /// Возвращает Unix TimeStamp Seconds
        /// </summary>
        /// <returns>Количество секунд, прошедших с 1 января 1970 г. Часовой пояс UTC</returns>
        public static long GetUnixSeconds() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        /// <summary>
        /// Возвращает Unix TimeStamp Milliseconds
        /// </summary>
        /// <returns>Количество миллисекунд, прошедших с 1 января 1970 г. Часовой пояс UTC</returns>
        public static long GetUnixMilliseconds() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        /// <summary>
        /// Возращает MD5 хэш строки
        /// </summary>
        /// <param name="value">Исходная строка, хэш которой необходимо получить</param>
        /// <returns>MD5 хэш исходной строки</returns>
        public static string GetMD5Hash(string value) => string.Concat(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(value)).Select(x => x.ToString("x2")));

        /// <summary>
        /// Возращает MD5 хэш массива байтов
        /// </summary>
        /// <param name="value">Массив байтов, хэш которого необходимо получить</param>
        /// <returns>MD5 хэш исходного массива байтов</returns>
        public static string GetMD5Hash(byte[] value) => string.Concat(MD5.Create().ComputeHash(value).Select(x => x.ToString("x2")));

        public static List<List<T>> SplitToSublists<T>(this List<T> source, int count)
        {
            return source
                     .Select((x, i) => new { Index = i, Value = x })
                     .GroupBy(x => x.Index / count)
                     .Select(x => x.Select(v => v.Value).ToList())
                     .ToList();
        }

        public static List<Account> ParseAccountsFromString(string value)
        {
            string[] lines = value.Split('\n');
            List<Account> output = new List<Account>();
            for (int i = 0; i < lines.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]))
                {
                    string[] data;
                    if (lines[i].Contains(":"))
                        data = lines[i].Split(':');
                    else if (lines[i].Contains(";"))
                        data = lines[i].Split(';');
                    else
                        continue;

                    output.Add(new Account(data[0].Trim(), data[1].Trim()));
                }
            }
            return output;
        }
        public static async Task<List<Account>> ParseAccountsFromStringAsync(string value) => await Task.Run(() => ParseAccountsFromString(value));

        public static List<Account> ParseAccountsFromFile(string fileName) => ParseAccountsFromString(File.ReadAllText(fileName));
        public static async Task<List<Account>> ParseAccountsFromFileAsync(string fileName) => await Task.Run(() => ParseAccountsFromFile(fileName));

        public static List<ProxyClient> ParseProxiesFromString(string value, ProxyType proxyType)
        {
            List<ProxyClient> output = new List<ProxyClient>();
            string[] lines = value.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]))
                    output.Add(ProxyClient.Parse(proxyType, lines[i]));
            }
            return output;
        }
        public static async Task<List<ProxyClient>> ParseProxiesFromStringAsync(string value, ProxyType proxyType) => await Task.Run(() => ParseProxiesFromString(value, proxyType));

        public static List<ProxyClient> ParseProxiesFromFile(string fileName, ProxyType proxyType) => ParseProxiesFromString(File.ReadAllText(fileName), proxyType);
        public static async Task<List<ProxyClient>> ParseProxiesFromFileAsync(string fileName, ProxyType proxyType) => await Task.Run(() => ParseProxiesFromFile(fileName, proxyType));

        public static List<ProxyClient> ParseProxiesFromUrl(string url, ProxyType proxyType)
        {
            HttpRequest request = new HttpRequest();
            request.UserAgentRandomize();
            return ParseProxiesFromString(request.Get(url).ToString(), proxyType); 
        }
        public static async Task<List<ProxyClient>> ParseProxiesFromUrlAsync(string url, ProxyType proxyType) => await Task.Run(() => ParseProxiesFromUrl(url, proxyType));
    }
}
