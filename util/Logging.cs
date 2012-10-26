using System;
using log4net;

namespace NMaier.sdlna.Util
{
  public class Logging : ILog
  {

    private ILog _logger;



    public bool IsDebugEnabled
    {
      get { return logger.IsDebugEnabled; }
    }

    public bool IsErrorEnabled
    {
      get { return logger.IsErrorEnabled; }
    }

    public bool IsFatalEnabled
    {
      get { return logger.IsFatalEnabled; }
    }

    public bool IsInfoEnabled
    {
      get { return logger.IsInfoEnabled; }
    }

    public bool IsWarnEnabled
    {
      get { return logger.IsWarnEnabled; }
    }

    private ILog logger
    {
      get
      {
        if (_logger == null) {
          _logger = LogManager.GetLogger(this.GetType());
        }
        return _logger;
      }
    }

    public log4net.Core.ILogger Logger
    {
      get { return logger.Logger; }
    }




    public void Debug(object message)
    {
      logger.Debug(message);
    }

    public void Debug(object message, Exception exception)
    {
      logger.Debug(message, exception);
    }

    public void DebugFormat(string format, params object[] args)
    {
      logger.DebugFormat(format, args);
    }

    public void DebugFormat(string format, object arg0)
    {
      logger.DebugFormat(format, arg0);
    }

    public void DebugFormat(string format, object arg0, object arg1)
    {
      logger.DebugFormat(format, arg0, arg1);
    }

    public void DebugFormat(IFormatProvider provider, string format, params object[] args)
    {
      logger.DebugFormat(provider, format, args);
    }

    public void DebugFormat(string format, object arg0, object arg1, object arg2)
    {
      logger.DebugFormat(format, arg0, arg1, arg2);
    }

    public void Error(object message)
    {
      logger.Error(message);
    }

    public void Error(object message, Exception exception)
    {
      logger.Error(message, exception);
    }

    public void ErrorFormat(string format, params object[] args)
    {
      logger.ErrorFormat(format, args);
    }

    public void ErrorFormat(string format, object arg0)
    {
      logger.ErrorFormat(format, arg0);
    }

    public void ErrorFormat(string format, object arg0, object arg1)
    {
      logger.ErrorFormat(format, arg0, arg1);
    }

    public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
    {
      logger.ErrorFormat(provider, format, args);
    }

    public void ErrorFormat(string format, object arg0, object arg1, object arg2)
    {
      logger.ErrorFormat(format, arg0, arg1, arg2);
    }

    public void Fatal(object message)
    {
      logger.Fatal(message);
    }

    public void Fatal(object message, Exception exception)
    {
      logger.Fatal(message, exception);
    }

    public void FatalFormat(string format, params object[] args)
    {
      logger.FatalFormat(format, args);
    }

    public void FatalFormat(string format, object arg0)
    {
      logger.FatalFormat(format, arg0);
    }

    public void FatalFormat(string format, object arg0, object arg1)
    {
      logger.FatalFormat(format, arg0, arg1);
    }

    public void FatalFormat(IFormatProvider provider, string format, params object[] args)
    {
      logger.FatalFormat(provider, format, args);
    }

    public void FatalFormat(string format, object arg0, object arg1, object arg2)
    {
      logger.FatalFormat(format, arg0, arg1, arg2);
    }

    public void Info(object message)
    {
      logger.Info(message);
    }

    public void Info(object message, Exception exception)
    {
      logger.Info(message, exception);
    }

    public void InfoFormat(string format, params object[] args)
    {
      logger.InfoFormat(format, args);
    }

    public void InfoFormat(string format, object arg0)
    {
      logger.InfoFormat(format, arg0);
    }

    public void InfoFormat(string format, object arg0, object arg1)
    {
      logger.InfoFormat(format, arg0, arg1);
    }

    public void InfoFormat(IFormatProvider provider, string format, params object[] args)
    {
      logger.InfoFormat(provider, format, args);
    }

    public void InfoFormat(string format, object arg0, object arg1, object arg2)
    {
      logger.InfoFormat(format, arg0, arg1, arg2);
    }

    public void Warn(object message)
    {
      logger.Warn(message);
    }

    public void Warn(object message, Exception exception)
    {
      logger.Warn(message, exception);
    }

    public void WarnFormat(string format, params object[] args)
    {
      logger.WarnFormat(format, args);
    }

    public void WarnFormat(string format, object arg0)
    {
      logger.WarnFormat(format, arg0);
    }

    public void WarnFormat(string format, object arg0, object arg1)
    {
      logger.WarnFormat(format, arg0, arg1);
    }

    public void WarnFormat(IFormatProvider provider, string format, params object[] args)
    {
      logger.WarnFormat(provider, format, args);
    }

    public void WarnFormat(string format, object arg0, object arg1, object arg2)
    {
      logger.WarnFormat(format, arg0, arg1, arg2);
    }
  }
}
