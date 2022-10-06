using System;
using System.Collections.Generic;
using System.Text;

namespace UAMX_2
{
    /// <summary>
    /// Флаговый енум состояния элемента УАМХ.
    /// </summary>
    [Flags]
    public enum EnumElementState
    {
        /// <summary>
        /// Нормальное состояние элемента
        /// </summary>
        Default = 0,
        /// <summary>
        /// Элемент неактивен (готовится к удалению?)
        /// </summary>
        Неактивен = 1,
    }
}
