using System;

namespace NMaier.SimpleDlna.Utilities
{
  public interface ILogging
  {
    void Notice(object message);

    void Notice(object message, Exception exception);

    void NoticeFormat(string format, params object[] args);

    void NoticeFormat(string format, object arg0);

    void NoticeFormat(string format, object arg0, object arg1);

    void NoticeFormat(IFormatProvider provider, string format,
                             params object[] args);

    void NoticeFormat(string format, object arg0, object arg1,
                             object arg2);

    //
    // Summary:
    //     Checks if this logger is enabled for the log4net.Core.Level.Debug level.
    //
    // Remarks:
    //     This function is intended to lessen the computational cost of disabled log debug
    //     statements.
    //     For some ILog interface log, when you write:
    //     log.Debug("This is entry number: " + i );
    //     You incur the cost constructing the message, string construction and concatenation
    //     in this case, regardless of whether the message is logged or not.
    //     If you are worried about speed (who isn't), then you should write:
    //     if (log.IsDebugEnabled) { log.Debug("This is entry number: " + i ); }
    //     This way you will not incur the cost of parameter construction if debugging is
    //     disabled for log. On the other hand, if the log is debug enabled, you will incur
    //     the cost of evaluating whether the logger is debug enabled twice. Once in log4net.ILog.IsDebugEnabled
    //     and once in the Debug(object). This is an insignificant overhead since evaluating
    //     a logger takes about 1% of the time it takes to actually log. This is the preferred
    //     style of logging.
    //     Alternatively if your logger is available statically then the is debug enabled
    //     state can be stored in a static variable like this:
    //     private static readonly bool isDebugEnabled = log.IsDebugEnabled;
    //     Then when you come to log you can write:
    //     if (isDebugEnabled) { log.Debug("This is entry number: " + i ); }
    //     This way the debug enabled state is only queried once when the class is loaded.
    //     Using a private static readonly variable is the most efficient because it is
    //     a run time constant and can be heavily optimized by the JIT compiler.
    //     Of course if you use a static readonly variable to hold the enabled state of
    //     the logger then you cannot change the enabled state at runtime to vary the logging
    //     that is produced. You have to decide if you need absolute speed or runtime flexibility.
    bool IsDebugEnabled { get; }
    //
    // Summary:
    //     Checks if this logger is enabled for the log4net.Core.Level.Error level.
    //
    // Remarks:
    //     For more information see log4net.ILog.IsDebugEnabled.
    bool IsErrorEnabled { get; }
    //
    // Summary:
    //     Checks if this logger is enabled for the log4net.Core.Level.Fatal level.
    //
    // Remarks:
    //     For more information see log4net.ILog.IsDebugEnabled.
    bool IsFatalEnabled { get; }
    //
    // Summary:
    //     Checks if this logger is enabled for the log4net.Core.Level.Info level.
    //
    // Remarks:
    //     For more information see log4net.ILog.IsDebugEnabled.
    bool IsInfoEnabled { get; }
    //
    // Summary:
    //     Checks if this logger is enabled for the log4net.Core.Level.Warn level.
    //
    // Remarks:
    //     For more information see log4net.ILog.IsDebugEnabled.
    bool IsWarnEnabled { get; }

    //
    // Summary:
    //     Log a message object with the log4net.Core.Level.Debug level.
    //
    // Parameters:
    //   message:
    //     The message object to log.
    //
    // Remarks:
    //     This method first checks if this logger is DEBUG enabled by comparing the level
    //     of this logger with the log4net.Core.Level.Debug level. If this logger is DEBUG
    //     enabled, then it converts the message object (passed as parameter) to a string
    //     by invoking the appropriate log4net.ObjectRenderer.IObjectRenderer. It then proceeds
    //     to call all the registered appenders in this logger and also higher in the hierarchy
    //     depending on the value of the additivity flag.
    //     WARNING Note that passing an System.Exception to this method will print the name
    //     of the System.Exception but no stack trace. To print a stack trace use the Debug(object,Exception)
    //     form instead.
    void Debug(object message);
    //
    // Summary:
    //     Log a message object with the log4net.Core.Level.Debug level including the stack
    //     trace of the System.Exception passed as a parameter.
    //
    // Parameters:
    //   message:
    //     The message object to log.
    //
    //   exception:
    //     The exception to log, including its stack trace.
    //
    // Remarks:
    //     See the Debug(object) form for more detailed information.
    void Debug(object message, Exception exception);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Debug level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   arg0:
    //     An Object to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Debug(object,Exception) methods instead.
    void DebugFormat(string format, object arg0);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Debug level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   args:
    //     An Object array containing zero or more objects to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Debug(object,Exception) methods instead.
    void DebugFormat(string format, params object[] args);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Debug level.
    //
    // Parameters:
    //   provider:
    //     An System.IFormatProvider that supplies culture-specific formatting information
    //
    //   format:
    //     A String containing zero or more format items
    //
    //   args:
    //     An Object array containing zero or more objects to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Debug(object,Exception) methods instead.
    void DebugFormat(IFormatProvider provider, string format, params object[] args);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Debug level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   arg0:
    //     An Object to format
    //
    //   arg1:
    //     An Object to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Debug(object,Exception) methods instead.
    void DebugFormat(string format, object arg0, object arg1);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Debug level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   arg0:
    //     An Object to format
    //
    //   arg1:
    //     An Object to format
    //
    //   arg2:
    //     An Object to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Debug(object,Exception) methods instead.
    void DebugFormat(string format, object arg0, object arg1, object arg2);
    //
    // Summary:
    //     Logs a message object with the log4net.Core.Level.Error level.
    //
    // Parameters:
    //   message:
    //     The message object to log.
    //
    // Remarks:
    //     This method first checks if this logger is ERROR enabled by comparing the level
    //     of this logger with the log4net.Core.Level.Error level. If this logger is ERROR
    //     enabled, then it converts the message object (passed as parameter) to a string
    //     by invoking the appropriate log4net.ObjectRenderer.IObjectRenderer. It then proceeds
    //     to call all the registered appenders in this logger and also higher in the hierarchy
    //     depending on the value of the additivity flag.
    //     WARNING Note that passing an System.Exception to this method will print the name
    //     of the System.Exception but no stack trace. To print a stack trace use the Error(object,Exception)
    //     form instead.
    void Error(object message);
    //
    // Summary:
    //     Log a message object with the log4net.Core.Level.Error level including the stack
    //     trace of the System.Exception passed as a parameter.
    //
    // Parameters:
    //   message:
    //     The message object to log.
    //
    //   exception:
    //     The exception to log, including its stack trace.
    //
    // Remarks:
    //     See the Error(object) form for more detailed information.
    void Error(object message, Exception exception);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Error level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   arg0:
    //     An Object to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Error(object,Exception) methods instead.
    void ErrorFormat(string format, object arg0);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Error level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   args:
    //     An Object array containing zero or more objects to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Error(object) methods instead.
    void ErrorFormat(string format, params object[] args);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Error level.
    //
    // Parameters:
    //   provider:
    //     An System.IFormatProvider that supplies culture-specific formatting information
    //
    //   format:
    //     A String containing zero or more format items
    //
    //   args:
    //     An Object array containing zero or more objects to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Error(object) methods instead.
    void ErrorFormat(IFormatProvider provider, string format, params object[] args);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Error level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   arg0:
    //     An Object to format
    //
    //   arg1:
    //     An Object to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Error(object,Exception) methods instead.
    void ErrorFormat(string format, object arg0, object arg1);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Error level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   arg0:
    //     An Object to format
    //
    //   arg1:
    //     An Object to format
    //
    //   arg2:
    //     An Object to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Error(object,Exception) methods instead.
    void ErrorFormat(string format, object arg0, object arg1, object arg2);
    //
    // Summary:
    //     Log a message object with the log4net.Core.Level.Fatal level.
    //
    // Parameters:
    //   message:
    //     The message object to log.
    //
    // Remarks:
    //     This method first checks if this logger is FATAL enabled by comparing the level
    //     of this logger with the log4net.Core.Level.Fatal level. If this logger is FATAL
    //     enabled, then it converts the message object (passed as parameter) to a string
    //     by invoking the appropriate log4net.ObjectRenderer.IObjectRenderer. It then proceeds
    //     to call all the registered appenders in this logger and also higher in the hierarchy
    //     depending on the value of the additivity flag.
    //     WARNING Note that passing an System.Exception to this method will print the name
    //     of the System.Exception but no stack trace. To print a stack trace use the Fatal(object,Exception)
    //     form instead.
    void Fatal(object message);
    //
    // Summary:
    //     Log a message object with the log4net.Core.Level.Fatal level including the stack
    //     trace of the System.Exception passed as a parameter.
    //
    // Parameters:
    //   message:
    //     The message object to log.
    //
    //   exception:
    //     The exception to log, including its stack trace.
    //
    // Remarks:
    //     See the Fatal(object) form for more detailed information.
    void Fatal(object message, Exception exception);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Fatal level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   arg0:
    //     An Object to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Fatal(object,Exception) methods instead.
    void FatalFormat(string format, object arg0);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Fatal level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   args:
    //     An Object array containing zero or more objects to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Fatal(object) methods instead.
    void FatalFormat(string format, params object[] args);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Fatal level.
    //
    // Parameters:
    //   provider:
    //     An System.IFormatProvider that supplies culture-specific formatting information
    //
    //   format:
    //     A String containing zero or more format items
    //
    //   args:
    //     An Object array containing zero or more objects to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Fatal(object) methods instead.
    void FatalFormat(IFormatProvider provider, string format, params object[] args);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Fatal level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   arg0:
    //     An Object to format
    //
    //   arg1:
    //     An Object to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Fatal(object,Exception) methods instead.
    void FatalFormat(string format, object arg0, object arg1);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Fatal level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   arg0:
    //     An Object to format
    //
    //   arg1:
    //     An Object to format
    //
    //   arg2:
    //     An Object to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Fatal(object,Exception) methods instead.
    void FatalFormat(string format, object arg0, object arg1, object arg2);
    //
    // Summary:
    //     Logs a message object with the log4net.Core.Level.Info level.
    //
    // Parameters:
    //   message:
    //     The message object to log.
    //
    // Remarks:
    //     This method first checks if this logger is INFO enabled by comparing the level
    //     of this logger with the log4net.Core.Level.Info level. If this logger is INFO
    //     enabled, then it converts the message object (passed as parameter) to a string
    //     by invoking the appropriate log4net.ObjectRenderer.IObjectRenderer. It then proceeds
    //     to call all the registered appenders in this logger and also higher in the hierarchy
    //     depending on the value of the additivity flag.
    //     WARNING Note that passing an System.Exception to this method will print the name
    //     of the System.Exception but no stack trace. To print a stack trace use the Info(object,Exception)
    //     form instead.
    void Info(object message);
    //
    // Summary:
    //     Logs a message object with the INFO level including the stack trace of the System.Exception
    //     passed as a parameter.
    //
    // Parameters:
    //   message:
    //     The message object to log.
    //
    //   exception:
    //     The exception to log, including its stack trace.
    //
    // Remarks:
    //     See the Info(object) form for more detailed information.
    void Info(object message, Exception exception);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Info level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   arg0:
    //     An Object to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Info(object,Exception) methods instead.
    void InfoFormat(string format, object arg0);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Info level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   args:
    //     An Object array containing zero or more objects to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Info(object) methods instead.
    void InfoFormat(string format, params object[] args);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Info level.
    //
    // Parameters:
    //   provider:
    //     An System.IFormatProvider that supplies culture-specific formatting information
    //
    //   format:
    //     A String containing zero or more format items
    //
    //   args:
    //     An Object array containing zero or more objects to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Info(object) methods instead.
    void InfoFormat(IFormatProvider provider, string format, params object[] args);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Info level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   arg0:
    //     An Object to format
    //
    //   arg1:
    //     An Object to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Info(object,Exception) methods instead.
    void InfoFormat(string format, object arg0, object arg1);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Info level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   arg0:
    //     An Object to format
    //
    //   arg1:
    //     An Object to format
    //
    //   arg2:
    //     An Object to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Info(object,Exception) methods instead.
    void InfoFormat(string format, object arg0, object arg1, object arg2);
    //
    // Summary:
    //     Log a message object with the log4net.Core.Level.Warn level.
    //
    // Parameters:
    //   message:
    //     The message object to log.
    //
    // Remarks:
    //     This method first checks if this logger is WARN enabled by comparing the level
    //     of this logger with the log4net.Core.Level.Warn level. If this logger is WARN
    //     enabled, then it converts the message object (passed as parameter) to a string
    //     by invoking the appropriate log4net.ObjectRenderer.IObjectRenderer. It then proceeds
    //     to call all the registered appenders in this logger and also higher in the hierarchy
    //     depending on the value of the additivity flag.
    //     WARNING Note that passing an System.Exception to this method will print the name
    //     of the System.Exception but no stack trace. To print a stack trace use the Warn(object,Exception)
    //     form instead.
    void Warn(object message);
    //
    // Summary:
    //     Log a message object with the log4net.Core.Level.Warn level including the stack
    //     trace of the System.Exception passed as a parameter.
    //
    // Parameters:
    //   message:
    //     The message object to log.
    //
    //   exception:
    //     The exception to log, including its stack trace.
    //
    // Remarks:
    //     See the Warn(object) form for more detailed information.
    void Warn(object message, Exception exception);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Warn level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   arg0:
    //     An Object to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Warn(object,Exception) methods instead.
    void WarnFormat(string format, object arg0);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Warn level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   args:
    //     An Object array containing zero or more objects to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Warn(object) methods instead.
    void WarnFormat(string format, params object[] args);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Warn level.
    //
    // Parameters:
    //   provider:
    //     An System.IFormatProvider that supplies culture-specific formatting information
    //
    //   format:
    //     A String containing zero or more format items
    //
    //   args:
    //     An Object array containing zero or more objects to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Warn(object) methods instead.
    void WarnFormat(IFormatProvider provider, string format, params object[] args);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Warn level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   arg0:
    //     An Object to format
    //
    //   arg1:
    //     An Object to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Warn(object,Exception) methods instead.
    void WarnFormat(string format, object arg0, object arg1);
    //
    // Summary:
    //     Logs a formatted message string with the log4net.Core.Level.Warn level.
    //
    // Parameters:
    //   format:
    //     A String containing zero or more format items
    //
    //   arg0:
    //     An Object to format
    //
    //   arg1:
    //     An Object to format
    //
    //   arg2:
    //     An Object to format
    //
    // Remarks:
    //     The message is formatted using the String.Format method. See String.Format(string,
    //     object[]) for details of the syntax of the format string and the behavior of
    //     the formatting.
    //     This method does not take an System.Exception object to include in the log event.
    //     To pass an System.Exception use one of the Warn(object,Exception) methods instead.
    void WarnFormat(string format, object arg0, object arg1, object arg2);

  }
}
