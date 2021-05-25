using System;
using System.Collections;
using System.IO;
using System.Text.Json;

namespace Authorize.IO.Json
{
    public class InputJsonOperation : IInputOperation
    {
        /// <summary>
        /// Store current string line from TextReader
        /// </summary>
        private string currentLine;

        /// <summary>
        /// Collection of strings
        /// </summary>
        private readonly TextReader reader;

        /// <summary>
        /// Create an instace of json input iterator from TextReader,
        /// this instance will provide a way to read line by line and 
        /// deserialize into type parameter
        /// </summary>
        /// <param name="reader"></param>
        public InputJsonOperation(TextReader reader)
        {
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        /// <inheritdoc cref="IEnumerable<string>" />
        public string Current => currentLine;

        /// <inheritdoc cref="IEnumerable<string>" />
        object IEnumerator.Current => currentLine;

        /// <inheritdoc cref="IEnumerable<string>" />
        public bool MoveNext()
        {
            this.currentLine = this.reader.ReadLine();
            return this.currentLine != null;
        }

        /// <inheritdoc cref="IInputOperation" />
        public T Read<T>() where T : class
        {
            var operation = JsonSerializer.Deserialize<T>(this.currentLine);
            return operation;
        }

        /// <inheritdoc cref="IEnumerable<string>" />
        public void Reset() {/* Do nothing */}

        /// <inheritdoc cref="IEnumerable<string>" />
        public void Dispose() {/* Do nothing */}
    }
}