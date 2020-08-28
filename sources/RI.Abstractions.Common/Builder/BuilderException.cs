using System;
using System.Runtime.Serialization;




namespace RI.Abstractions.Builder
{
    /// <summary>
    ///     The <see cref="BuilderException" /> is thrown when an <see cref="IBuilder" /> could not successfully perform its <see cref="IBuilder.Build" /> or <see cref="IBuilderExtensions.BuildStandalone" /> call.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    [Serializable,]
    public class BuilderException : Exception
    {
        #region Constants

        private const string ExceptionMessageWithException = "Failed to perform build: {0}";

        private const string ExceptionMessageWithoutException = "Failed to perform build.";

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="BuilderException" />.
        /// </summary>
        public BuilderException ()
            : base(BuilderException.ExceptionMessageWithoutException) { }

        /// <summary>
        ///     Creates a new instance of <see cref="BuilderException" />.
        /// </summary>
        /// <param name="message"> The message which describes the failure. </param>
        public BuilderException (string message)
            : base(message) { }

        /// <summary>
        ///     Creates a new instance of <see cref="BuilderException" />.
        /// </summary>
        /// <param name="innerException"> The exception which triggered this exception. </param>
        public BuilderException (Exception innerException)
            : base(string.Format(BuilderException.ExceptionMessageWithException, innerException.Message), innerException) { }

        /// <summary>
        ///     Creates a new instance of <see cref="BuilderException" />.
        /// </summary>
        /// <param name="message"> The message which describes the failure. </param>
        /// <param name="innerException"> The exception which triggered this exception. </param>
        public BuilderException (string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        ///     Creates a new instance of <see cref="BuilderException" />.
        /// </summary>
        /// <param name="info"> The serialization data. </param>
        /// <param name="context"> The type of the source of the serialization data. </param>
        protected BuilderException (SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        #endregion
    }
}
