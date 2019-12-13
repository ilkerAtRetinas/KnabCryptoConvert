using System;
namespace Application.Exceptions
{
    public class CryptoConvertCoreException : Exception
    {
        /// <summary>
        /// Just a broad exception definition, which helps to distinct known type of exceptions coming from application core
        /// </summary>
        /// <param name="message"></param>
        public CryptoConvertCoreException(string message) : base(message)
        {
        }
    }
}
