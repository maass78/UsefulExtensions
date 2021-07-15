using System.Collections.Generic;

namespace UsefulExtensions
{
    /// <summary>
    /// Класс, позволяющий перечислять по порядку список объектов типа <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">Тип объектов, список которых будет использоваться для перечисления</typeparam>
    public class ObjectManager<T>
    {
        /// <summary>
        /// Объект блокировки потока
        /// </summary>
        public readonly object Sync = new object();

        /// <summary>
        /// Список объектов для перечисления
        /// </summary>
        public List<T> Objects { get; }

        private int _curIndex; //текущий индекс объекта в списке 

        /// <summary>
        /// Конструктор класса <see cref="ObjectManager{T}"/>
        /// </summary>
        /// <param name="objects">Список объектов для перечисления</param>
        public ObjectManager(List<T> objects)
        {
            AutoReset = false;
            Objects = objects;
            _curIndex = 0;
        }

        /// <summary>
        /// Возвращает следующий по списку объект
        /// </summary>
        /// <returns>Следующий по списку объект. Если достигнут конец списка, значение по умолчанию для value-типов или <see langword="null"/> для ссылочных</returns>
        public T GetNextObject()
        {
            lock (Sync)
            {
                if (_curIndex >= Objects.Count)
                {
                    if (AutoReset)
                        _curIndex = 0;
                    else
                        return default;
                }

                return Objects[_curIndex++];
            }
        }

        /// <summary>
        /// Начинает перечисление с нулевого элемента
        /// </summary>
        public void Reset()
        {
            _curIndex = 0;
        }

        /// <summary>
        /// Возвращает или задаёт значение, будет ли перечисление начинаться заново после достижения конца списка. По умолчанию - <see langword="false"/>
        /// </summary>
        public bool AutoReset { get; set; }
    }
}
