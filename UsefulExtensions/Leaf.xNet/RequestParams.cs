using System;
using System.Collections.Generic;

namespace Leaf.xNet
{
    /// <inheritdoc />
    /// <summary>
    /// Представляет коллекцию строк, представляющих параметры запроса.
    /// </summary>
    public class RequestParams : List<KeyValuePair<string,string>>
    {
        /// <summary>
        /// Запрос перечислением параметров и их значений.
        /// </summary>
        public string Query => Http.ToQueryString(this, ValuesUnescaped, KeysUnescaped);
        
        /// <summary>
        /// Указывает, нужно ли пропустить кодирование значений параметров запроса.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public readonly bool ValuesUnescaped;
        
        /// <summary>
        /// Указывает, нужно ли пропустить кодирование имен параметров запроса.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public readonly bool KeysUnescaped;

        /// <inheritdoc />
        /// <param name="valuesUnescaped">Указывает, нужно ли пропустить кодирование значений параметров запроса.</param>
        /// <param name="keysUnescaped">Указывает, нужно ли пропустить кодирование имен параметров запроса.</param>
        public RequestParams(bool valuesUnescaped = false, bool keysUnescaped = false)
        {
            ValuesUnescaped = valuesUnescaped;
            KeysUnescaped = keysUnescaped;
        }
        
        /// <summary>
        /// Задаёт новый параметр запроса.
        /// </summary>
        /// <param name="paramName">Название параметра запроса.</param>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="paramName"/> равно <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметра <paramref name="paramName"/> является пустой строкой.</exception>
        public object this[string paramName]
        {
            set
            {
                #region Проверка параметра
                if (paramName == null)
                    throw new ArgumentNullException(nameof(paramName));

                if (paramName.Length == 0)
                    throw ExceptionHelper.EmptyString(nameof(paramName));

                #endregion

                string str = value?.ToString() ?? string.Empty;

                Add(new KeyValuePair<string, string>(paramName, str));
            }
        }
    }
}
