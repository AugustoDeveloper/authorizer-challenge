using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Authorize.IO.Json
{
    public class OutputJsonOperation : IOutputOperation
    {
        /// <summary>
        /// Stream way to output all operations
        /// </summary>
        private readonly TextWriter writer;

        /// <summary>
        /// Storing all json string of operation results
        /// </summary>
        private readonly StringBuilder builder;

        /// <summary>
        /// Create an instance of output in json formatted 
        /// of all operations results
        /// </summary>
        /// <param name="writer"></param>
        public OutputJsonOperation(TextWriter writer)
        {
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            builder = new StringBuilder();
        }

        /// <inheritdoc cref="IOutputOperation"/>
        public void AppendLine<TOperationResult>(TOperationResult operationResult) where TOperationResult : class
        {
            var result = JsonSerializer.Serialize(operationResult);
            result = (builder.Length > 0 ? Environment.NewLine : "") + result;
            builder.Append(result);
        }

        /// <inheritdoc cref="IOutputOperation"/>
        public void WriteAllResults()
        {
            if (builder.Length > 0)
            {
                writer.Write(builder.ToString());
            }
        }
    }
}