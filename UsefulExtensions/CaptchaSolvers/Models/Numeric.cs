namespace UsefulExtensions.CaptchaSolvers.Models
{
    /// <summary>
    /// Перечисление параметров для решения текстовой капчи, которые указывают на то, из каких символов состоит капча
    /// </summary>
    public enum Numeric
    {
        /// <summary>
        /// Никаких требований
        /// </summary>
        None = 0,

        /// <summary>
        /// Только цифры
        /// </summary>
        OnlyNumbers = 1,

        /// <summary>
        /// Только буквы
        /// </summary>
        OnlyLetters = 2,

        /// <summary>
        /// И буквы, и цифры должны быть в капче
        /// </summary>
        NumbersAndLetters = 3
    }
}
