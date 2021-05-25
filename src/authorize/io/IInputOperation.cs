using System.Collections.Generic;

namespace Authorize.IO
{
    /// <summary>
    /// Defines a way to read all input information from
    /// mechanism and line-by-line returns a object to
    /// to interpret the operation
    /// </summary>
    public interface IInputOperation : IEnumerator<string>
    {
        /// <summary>
        /// Defines a reading operation from data container and 
        /// returns a type parameter instance to execute a specific
        /// operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T Read<T>() where T : class;
    }
}