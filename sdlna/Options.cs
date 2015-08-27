﻿using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using NMaier.GetOptNet;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Utilities;
using System;
using System.IO;
using System.Net;

namespace NMaier.SimpleDlna
{
  [GetOptOptions(AcceptPrefixType = ArgumentPrefixTypes.Dashes)]
  internal class Options : GetOpt
  {
    private string[] macs = new string[0];

    private string[] ips = new string[0];

    private string[] uas = new string[0];

    [Parameters(HelpVar = "Directory")]
    public DirectoryInfo[] Directories =
      new DirectoryInfo[] { new DirectoryInfo(".") };

    [Argument("type", HelpText = "Types to serv (IMAGE, VIDEO, AUDIO; default: all)")]
    [ArgumentAlias("what")]
    [ShortArgument('t')]
    public DlnaMediaTypes[] Types =
      new DlnaMediaTypes[] { DlnaMediaTypes.Video, DlnaMediaTypes.Image, DlnaMediaTypes.Audio };

    [Argument("view", HelpText = "Apply a view (default: no views applied)", HelpVar = "view")]
    [ShortArgument('v')]
    public string[] Views = new string[0];

    private int port = 0;

    //[Argument("cache", HelpVar = "file", HelpText = "Cache file to use for storing meta data (default: none)")]
    //[ShortArgument('c')]
    //public FileInfo CacheFile = null;

    [Argument("storage", HelpText = "Storage database; see --list-sort-orders")]
    [ShortArgument('o')]
    [FlagArgument(true)]
    public string FileStore = "RaptorDB";

    [Argument("name", HelpVar = "name", HelpText = "Friendly name for this server (group)")]
    [ShortArgument('n')]
    public string FriendlyName = string.Empty;

    [Argument("sort-descending", HelpText = "Sort order; see --list-sort-orders")]
    [ShortArgument('d')]
    [FlagArgument(true)]
    public bool DescendingOrder = false;

    [Argument("list-sort-orders", HelpText = "List all available sort orders")]
    [FlagArgument(true)]
    public bool ListOrders = false;

    [Argument("list-views", HelpText = "List all available views")]
    [FlagArgument(true)]
    public bool ListViews = false;

    [Argument("log-file", HelpText = "Log to specified file as well (default: none)", HelpVar = "File")]
    public FileInfo LogFile = null;

    [Argument("log-level", HelpText = "Log level of OFF, DEBUG, INFO, WARN, ERROR, FATAL (default: INFO)", HelpVar = "level")]
    [ShortArgument('l')]
    public string LogLevel = "INFO";

    [Argument("sort", HelpText = "Sort order; see --list-sort-orders", HelpVar = "order")]
    [ShortArgument('s')]
    public string Order = "title";

    [Argument("seperate", HelpText = "Mount directories as seperate servers")]
    [FlagArgument(true)]
    public bool Seperate = false;

    [Argument("help", HelpText = "Print usage")]
    [ShortArgument('?')]
    [ShortArgumentAlias('h')]
    [FlagArgument(true)]
    public bool ShowHelp = false;

    [Argument("license", HelpText = "Print license")]
    [ShortArgument('L')]
    [FlagArgument(true)]
    public bool ShowLicense = false;

    [Argument("version", HelpText = "Print version")]
    [ShortArgument('V')]
    [FlagArgument(true)]
    public bool ShowVersion = false;

    [Argument("no-rescanning", HelpText = "Disable rescanning of locations after first scan")]
    [FlagArgument(false)]
    public bool Rescanning = true;

    [Argument("ip", HelpText = "Allow only specified IPs", HelpVar = "IP")]
    [ShortArgument('i')]
    public string[] Ips
    {
      get
      {
        return ips;
      }
      set
      {
        foreach (var ip in value) {
          try {
            IPAddress.Parse(ip);
          }
          catch (Exception) {
            throw new GetOptException(string.Format(
              "Not a valid IP address: {0}", ip));
          }
        }
        ips = value;
      }
    }

    [Argument("mac", HelpText = "Allow only specified MACs", HelpVar = "MAC")]
    [ShortArgument('m')]
    public string[] Macs
    {
      get
      {
        return macs;
      }
      set
      {
        foreach (var mac in value) {
          if (!IP.IsAcceptedMAC(mac)) {
            throw new GetOptException(string.Format(
              "Not a valid mac address: {0}. Must have a form of 01:AF:BC:00:0A:FF!", mac));
          }
        }
        macs = value;
      }
    }

    [Argument("port", HelpVar = "port", HelpText = "Web server listen port (default: 0, bind an available port)")]
    [ShortArgument('p')]
    public int Port
    {
      get
      {
        return port;
      }
      set
      {
        if (value != 0 && (value < 1 || value > ushort.MaxValue)) {
          throw new GetOptException(
            "Port must be between 2 and " + ushort.MaxValue);
        }
        port = value;
      }
    }

    [Argument("ua", HelpText = "Allow only specified user-agents", HelpVar = "User-Agent")]
    [ShortArgument('u')]
    public string[] UserAgents
    {
      get
      {
        return uas;
      }
      set
      {
        foreach (var ua in value) {
          if (string.IsNullOrWhiteSpace(ua)) {
            throw new GetOptException(string.Format("Not a valid User-Agent: {0}.", ua));
          }
        }
        uas = value;
      }
    }

    public void SetupLogging()
    {
      var appender = new ConsoleAppender();
      var layout = new PatternLayout() {
        ConversionPattern = "%6level [%3thread] %-20.20logger{1} - %message%newline%exception"
      };
      layout.ActivateOptions();
      appender.Layout = layout;
      appender.ActivateOptions();
      if (LogFile != null) {
        var fileAppender = new RollingFileAppender() {
          File = LogFile.FullName,
          Layout = layout,
          MaximumFileSize = "1MB",
          MaxSizeRollBackups = 10,
          RollingStyle = RollingFileAppender.RollingMode.Size,
          ImmediateFlush = false,
          Threshold = Level.Debug
        };
        fileAppender.ActivateOptions();
        BasicConfigurator.Configure(appender, fileAppender);
      }
      else {
        BasicConfigurator.Configure(appender);
      }

      var repo = LogManager.GetRepository();
      var level = repo.LevelMap[LogLevel.ToUpperInvariant()];
      if (level == null) {
        throw new GetOptException("Invalid log level");
      }
      repo.Threshold = level;
    }
  }
}
