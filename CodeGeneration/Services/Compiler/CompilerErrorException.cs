using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace CodeGeneration.Services.Compiler
{
    public class CompilerErrorException : Exception
    {
        public string ModelName { get; set; }
        public string SourceCode { get; set; }
        public CompilerErrorCollection Errors { get; set; }

        public CompilerErrorException()
        {
            
        }

        public CompilerErrorException(string message) : base(message)
        {
        }

        public CompilerErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CompilerErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Errors = (CompilerErrorCollection) info.GetValue("Errors", typeof(CompilerErrorCollection));
        }
    }
}
