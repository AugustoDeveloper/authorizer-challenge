namespace Authorize.IO
{
    /// <summary>
    /// Defines a way to write the operation results 
    /// </summary>
    public interface IOutputOperation
    {
        /// <summary>
        /// Defines a method to store a operation result
        /// </summary>
        /// <typeparam name="TOperationResult"></typeparam>
        void AppendLine<TOperationResult>(TOperationResult operationResult) where TOperationResult : class;

        /// <summary>
        /// Defines a method to write all operation results stored in output data container
        /// </summary>
        void WriteAllResults();
    }
}