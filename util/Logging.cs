using System;
using log4net;
using log4net.Core;

namespace NMaier.SimpleDlna.Utilities
{
  public class Logging : ILog
  {
    private ILog instance;

    private ILog InternalLogger => instance ?? (instance = LogManager.GetLogger(GetType()));

    public bool IsNoticeEnabled => Logger.IsEnabledFor(Level.Notice);

    public bool IsDebugEnabled => InternalLogger.IsDebugEnabled;

    public bool IsErrorEnabled => InternalLogger.IsErrorEnabled;

    public bool IsFatalEnabled => InternalLogger.IsFatalEnabled;

    public bool IsInfoEnabled => InternalLogger.IsInfoEnabled;

    public bool IsWarnEnabled => InternalLogger.IsWarnEnabled;

    public ILogger Logger => InternalLogger.Logger;

    public void Debug(object message)
    {
      InternalLogger.Debug(message);
    }

    public void Debug(object message, Exception exception)
    {
      InternalLogger.Debug(message, exception);
    }

    public void DebugFormat(string format, params object[] args)
    {
      InternalLogger.DebugFormat(format, args);
    }

    public void DebugFormat(string format, object arg0)
    {
      InternalLogger.DebugFormat(format, arg0);
    }

    public void DebugFormat(string format, object arg0, object arg1)
    {
      InternalLogger.DebugFormat(format, arg0, arg1);
    }

    public void DebugFormat(IFormatProvider provider, string format,
      params object[] args)
    {
      InternalLogger.DebugFormat(provider, format, args);
    }

    public void DebugFormat(string format, object arg0, object arg1,
      object arg2)
    {
      InternalLogger.DebugFormat(format, arg0, arg1, arg2);
    }

    public void Error(object message)
    {
      InternalLogger.Error(message);
    }

    public void Error(object message, Exception exception)
    {
      InternalLogger.Error(message, exception);
    }

    public void ErrorFormat(string format, params object[] args)
    {
      InternalLogger.ErrorFormat(format, args);
    }

    public void ErrorFormat(string format, object arg0)
    {
      InternalLogger.ErrorFormat(format, arg0);
    }

    public void ErrorFormat(string format, object arg0, object arg1)
    {
      InternalLogger.ErrorFormat(format, arg0, arg1);
    }

    public void ErrorFormat(IFormatProvider provider, string format,
      params object[] args)
    {
      InternalLogger.ErrorFormat(provider, format, args);
    }

    public void ErrorFormat(string format, object arg0, object arg1,
      object arg2)
    {
      InternalLogger.ErrorFormat(format, arg0, arg1, arg2);
    }

    public void Fatal(object message)
    {
      InternalLogger.Fatal(message);
    }

    public void Fatal(object message, Exception exception)
    {
      InternalLogger.Fatal(message, exception);
    }

    public void FatalFormat(string format, params object[] args)
    {
      InternalLogger.FatalFormat(format, args);
    }

    public void FatalFormat(string format, object arg0)
    {
      InternalLogger.FatalFormat(format, arg0);
    }

    public void FatalFormat(string format, object arg0, object arg1)
    {
      InternalLogger.FatalFormat(format, arg0, arg1);
    }

    public void FatalFormat(IFormatProvider provider, string format,
      params object[] args)
    {
      InternalLogger.FatalFormat(provider, format, args);
    }

    public void FatalFormat(string format, object arg0, object arg1,
      object arg2)
    {
      InternalLogger.FatalFormat(format, arg0, arg1, arg2);
    }

    public void Info(object message)
    {
      InternalLogger.Info(message);
    }

    public void Info(object message, Exception exception)
    {
      InternalLogger.Info(message, exception);
    }

    public void InfoFormat(string format, params object[] args)
    {
      InternalLogger.InfoFormat(format, args);
    }

    public void InfoFormat(string format, object arg0)
    {
      InternalLogger.InfoFormat(format, arg0);
    }

    public void InfoFormat(string format, object arg0, object arg1)
    {
      InternalLogger.InfoFormat(format, arg0, arg1);
    }

    public void InfoFormat(IFormatProvider provider, string format,
      params object[] args)
    {
      InternalLogger.InfoFormat(provider, format, args);
    }

    public void InfoFormat(string format, object arg0, object arg1,
      object arg2)
    {
      InternalLogger.InfoFormat(format, arg0, arg1, arg2);
    }

    public void Warn(object message)
    {
      InternalLogger.Warn(message);
    }

    public void Warn(object message, Exception exception)
    {
      InternalLogger.Warn(message, exception);
    }

    public void WarnFormat(string format, params object[] args)
    {
      InternalLogger.WarnFormat(format, args);
    }

    public void WarnFormat(string format, object arg0)
    {
      InternalLogger.WarnFormat(format, arg0);
    }

    public void WarnFormat(string format, object arg0, object arg1)
    {
      InternalLogger.WarnFormat(format, arg0, arg1);
    }

    public void WarnFormat(IFormatProvider provider, string format,
      params object[] args)
    {
      InternalLogger.WarnFormat(provider, format, args);
    }

    public void WarnFormat(string format, object arg0, object arg1,
      object arg2)
    {
      InternalLogger.WarnFormat(format, arg0, arg1, arg2);
    }

    public void Notice(object message)
    {
      Logger.Log(GetType(), Level.Notice, message, null);
    }

    public void Notice(object message, Exception exception)
    {
      Logger.Log(GetType(), Level.Notice, message, exception);
    }

    public void NoticeFormat(string format, params object[] args)
    {
      Logger.Log(GetType(), Level.Notice, string.Format(format, args), null);
    }

    public void NoticeFormat(string format, object arg0)
    {
      Logger.Log(GetType(), Level.Notice, string.Format(format, arg0), null);
    }

    public void NoticeFormat(string format, object arg0, object arg1)
    {
      Logger.Log(
        GetType(), Level.Notice, string.Format(format, arg0, arg1), null);
    }

    public void NoticeFormat(IFormatProvider provider, string format,
      params object[] args)
    {
      Logger.Log(
        GetType(), Level.Notice, string.Format(provider, format, args), null);
    }

    public void NoticeFormat(string format, object arg0, object arg1,
      object arg2)
    {
      Logger.Log(
        GetType(), Level.Notice, string.Format(format, arg0, arg1, arg2),
        null);
    }
  }
}
