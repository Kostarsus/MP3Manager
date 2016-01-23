using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public static class ArgumentGuard
    {
        /// <summary>
        /// Prüft ob das Argument Null ist
        /// </summary>
        /// <param name="argument">
        /// Argument das geprüft werden soll
        /// </param>
        /// <param name="argumentName">
        /// Der verwendete Name des Argumentes
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Wird ausgelöst wenn <paramref name="argument"/> null ist
        /// </exception>
        public static void IsNotNull(object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(string.Format("Das Argument {0} darf nicht null sein", argumentName));
            }
        }

        /// <summary>
        /// Prüft das Argument Null, Leer oder nur aus Leerzeichen besteht
        /// </summary>
        /// <param name="argument">
        /// Argument das geprfüt werden soll
        /// </param>
        /// <param name="argumentName">
        /// Der verwendete Name des Argumentes
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Wird ausgelöst, wenn <paramref name="argument"/> null ist
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Wird ausgelöst wenn <paramref name="argument"/> leer oder nur aus Leerzeichen besteht.
        /// </exception>
        public static void IsNotNullOrWhitespace(string argument, string argumentName)
        {
            IsNotNull(argument, argumentName);
            if (argument.Trim().Length == 0)
            {
                throw new ArgumentException(string.Format("Das Argument {0} darf nicht leer sein", argumentName));
            }
        }

        /// <summary>
        /// Prüft ob das Argument Null oder leer ist
        /// </summary>
        /// <param name="argument">
        /// Argument das geprfüt werden soll
        /// </param>
        /// <param name="argumentName">
        /// Der verwendete Name des Argumentes
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Wird ausgelöst, wenn <paramref name="argument"/> null ist
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Wird ausgelöst wenn <paramref name="argument"/> leer ist.
        /// </exception>
        public static void IsNotNullOrEmpty(CollectionBase argument, string argumentName)
        {
            IsNotNull(argument, argumentName);
            if (argument.Count == 0)
            {
                throw new ArgumentException(string.Format("Das Argument {0} darf nicht leer sein", argumentName));
            }
        }
    }
}