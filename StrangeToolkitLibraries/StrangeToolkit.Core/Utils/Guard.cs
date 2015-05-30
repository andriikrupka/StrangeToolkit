namespace StrangeToolkit.Utils
{
    using System;

    /// <summary>
    /// Common guard clauses
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Checks a string argument to ensure it isn't null or empty
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ArgumentNotNullOrEmptyString(string argumentValue, string argumentName)
        {
            ArgumentNotNull(argumentValue, argumentName);

            if (argumentValue.Length == 0)
                throw new ArgumentException(string.Format("StringCannotBeEmpty {0}", argumentName));
        }

        /// <summary>
        /// Checks an argument to ensure it isn't null
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
                throw new ArgumentNullException(argumentName);
        }

        /// <summary>
        /// Checks an Enum argument to ensure that its value is defined by the specified Enum type.
        /// </summary>
        /// <param name="enumType">The Enum type the value should correspond to.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="argumentName">The name of the argument holding the value.</param>
        public static void EnumValueIsDefined(Type enumType, object value, string argumentName)
        {
            if (Enum.IsDefined(enumType, value) == false)
                throw new ArgumentException(String.Format("InvalidEnumValue argumentName:{0} enumType:{1}",
                 argumentName, enumType));
        }
    }
}
