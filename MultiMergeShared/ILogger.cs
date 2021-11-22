
using System;

namespace MultiMerge
{
    public interface ILogger
    {
        void Debug(string message);

        void Debug(string message, params object[] args);

        void Error(string message);

        void Error(string message, Exception exception); 
    }
}
