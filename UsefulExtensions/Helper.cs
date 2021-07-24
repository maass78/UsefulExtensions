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

        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Проверен ли аккаунт. Обычно используется для чекеров.
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Возвращает строковое представление аккаунта в формате <c>login:password</c>
        /// </summary>
        /// <returns>Cтроковое представление аккаунта в формате <c>login:password</c></returns>
        public override string ToString() => ToString(":");

        /// <summary>
        /// Возвращает логин и пароль аккаунта, разделенные указанным разделителем
        /// </summary>
        /// <param name="separator">Разделитель между логином и паролем</param>
        /// <returns>Логин и пароль аккаунта, разделенные указанным разделителем</returns>
        public string ToString(string separator) => $"{Login}{separator}{Password}";
    }

    /// <summary>
    /// Статический класс, содержащий методы, часто используемые в создании софта (чекеры, регеры и пр. софт для автоматизации действий на сайтах)
    /// </summary>
    public static class Helper
    {
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
            value = value.Replace("\r", string.Empty);
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
            value = value.Replace("\r", string.Empty);
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

        /// <summary>
        /// Преобразует строку в массив байт, используя <see cref="Encoding.UTF8"/>
        /// </summary>
        /// <param name="value">Строка, которую необходимо преобразовать в массив байтов</param>
        public static byte[] GetBytes(this string value) => Encoding.UTF8.GetBytes(value);

        /// <summary>
        /// Преобразует массив байт в строку, используя <see cref="Encoding.UTF8"/>
        /// </summary>
        /// <param name="value">Массив байт, который необходимо преобразовать в строку</param>
        public static string GetString(this byte[] value) => Encoding.UTF8.GetString(value);

        /// <summary>
        /// Копирует существующую папку в новую папку включая подкаталоги, если это указано
        /// </summary>
        /// <param name="sourceDirName">Копируемая папка</param>
        /// <param name="destDirName">Имя целевой папки. Если такой папки не существует, она будет создана</param>
        /// <param name="copySubDirs"><see langword="true"/>, чтобы скопировать папку вместе с подкаталогами и файлами в них</param>
        /// <param name="overwrite"><see langword="true"/>, чтобы разрешить перезапись файлов</param>
        public static void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs, bool overwrite = true)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }
            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, overwrite);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    CopyDirectory(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
    }
}
