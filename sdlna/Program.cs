using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using log4net.Config;
using NMaier.GetOptNet;
using NMaier.sdlna.FileMediaServer;
using NMaier.sdlna.Server;
using log4net.Appender;
using log4net.Layout;

namespace NMaier.sdlna
{
  class Program
  {

    private static ManualResetEvent BlockEvent = new ManualResetEvent(false);
    private static uint CancelHitCount = 0;




    private static void CancelKeyPressed(object sender, ConsoleCancelEventArgs e)
    {
      if (CancelHitCount++ == 3) {
        LogManager.GetLogger(typeof(Program)).Fatal("Emergency exit commencing");
        return;
      }
      e.Cancel = true;
      BlockEvent.Set();
      LogManager.GetLogger(typeof(Program)).Info("Shutdown requested");
    }

    static void Main(string[] args)
    {
      Console.WriteLine();
      var options = new Options();
      try {
        var appender = new ConsoleAppender();
        var layout = new PatternLayout();
        layout.ConversionPattern = "%6level [%3thread] %-20.20logger{1} - %message%newline%exception";
        layout.ActivateOptions();
        appender.Layout = layout;
        appender.ActivateOptions();
        Console.TreatControlCAsInput = false;
        Console.CancelKeyPress += CancelKeyPressed;

        options.Parse(args);
        if (options.ShowHelp) {
          options.PrintUsage();
          return;
        }
        if (options.ShowVersion) {
          var a = Assembly.GetEntryAssembly();
          var v = a.GetName().Version;
          Console.WriteLine("App:    {0} {1}.{2}.{3}", a.GetName().Name, v.Major, v.Minor, v.Revision);
          Console.WriteLine("Server: {0}", HttpServer.SERVER_SIGNATURE);
          return;
        }
        if (options.ListViews) {
          var items = from v in ViewRepository.ListItems()
                      orderby v.Key
                      select v;
          Console.WriteLine("Available views:");
          Console.WriteLine("----------------");
          Console.WriteLine();
          foreach (var i in items) {
            Console.WriteLine("  - " + i.Key);
            Console.WriteLine(i.Value);
            Console.WriteLine();
          }
          return;
        }
        if (options.ListOrders) {
          var items = from v in ComparerRepository.ListItems()
                      orderby v.Key
                      select v;
          Console.WriteLine("Available orders:");
          Console.WriteLine("----------------");
          Console.WriteLine();
          foreach (var i in items) {
            Console.WriteLine("  - " + i.Key);
            Console.WriteLine(i.Value);
            Console.WriteLine();
          }
          return;
        }
        if (options.Directories.Length == 0) {
          throw new GetOptException("No directories specified");
        }

        if (options.LogFile != null) {
          var fileAppender = new RollingFileAppender();
          fileAppender.File = options.LogFile.FullName;
          fileAppender.Layout = layout;
          fileAppender.MaximumFileSize = "1MB";
          fileAppender.MaxSizeRollBackups = 10;
          fileAppender.RollingStyle = RollingFileAppender.RollingMode.Size;
          fileAppender.ActivateOptions();
          BasicConfigurator.Configure(appender, fileAppender);
        }
        else {
          BasicConfigurator.Configure(appender);
        }

        var repo = LogManager.GetRepository();
        var level = repo.LevelMap[options.LogLevel.ToUpper()];
        if (level == null) {
          throw new GetOptException("Invalid log level");
        }
        repo.Threshold = level;

        HttpServer server = new HttpServer();
        server.Info("CTRL-C to terminate");

        MediaTypes types = options.Types[0];
        foreach (var t in options.Types) {
          types = types | t;
          server.InfoFormat("Enabled type {0}", t);
        }

        foreach (var d in options.Directories) {
          server.InfoFormat("Mounting FileServer for {0}", d.FullName);
          var fs = new FileServer(types, d);

          foreach (var v in options.Views) {
            try {
              fs.AddView(v);
            }
            catch (RepositoryLookupException) {
              throw new GetOptException("Invalid view " + v);
            }
          }

          try {
            fs.SetOrder(options.Order);
          }
          catch (RepositoryLookupException) {
            throw new GetOptException("Invalid order" + options.Order);
          }
          fs.DescendingOrder = options.DescendingOrder;

          fs.Load();
          server.RegisterMediaServer(fs);
          server.InfoFormat("{0} mounted", d.FullName);
        }


        // Basically the main loop, except we don't loop here at all ;)
        BlockEvent.WaitOne();

        server.Info("Going down!");
        server.Dispose();
        server.Info("Closed!");
      }
      catch (GetOptException ex) {
        Console.Error.WriteLine("Error: {0}\n\n", ex.Message);
        options.PrintUsage();
      }
#if !DEBUG
      catch (Exception ex) {
        LogManager.GetLogger(typeof(Program)).Fatal("Failed to run", ex);
      }
#endif
    }




    [GetOptOptions(AcceptPrefixType = ArgumentPrefixType.Dashes)]
    private class Options : GetOpt
    {
      [Parameters(HelpVar = "Directory")]
      public DirectoryInfo[] Directories = new DirectoryInfo[] { new DirectoryInfo(".") };

      [Argument("log-level", Helptext = "Log level of OFF, DEBUG, INFO, WARN, ERROR, FATAL")]
      [ShortArgument('l')]
      public string LogLevel = "INFO";

      [Argument("log-file", Helptext = "Log to specified file as well", Helpvar = "File")]
      public FileInfo LogFile = null;

      [Argument("help", Helptext = "Print usage")]
      [ShortArgument('?')]
      [ShortArgumentAlias('h')]
      [FlagArgument(true)]
      public bool ShowHelp = false;

      [Argument("version", Helptext = "Print version")]
      [ShortArgument('V')]
      [FlagArgument(true)]
      public bool ShowVersion = false;

      [Argument("list-sort-orders", Helptext = "List all available sort orders")]
      [FlagArgument(true)]
      public bool ListOrders = false;


      [Argument("list-views", Helptext = "List all available views")]
      [FlagArgument(true)]
      public bool ListViews = false;

      [Argument("type", Helptext = "Types to serv (IMAGE, VIDEO)")]
      [ArgumentAlias("what")]
      [ShortArgument('t')]
      public MediaTypes[] Types = new MediaTypes[] { MediaTypes.VIDEO };

      [Argument("view", Helptext = "Apply a view")]
      [ShortArgument('v')]
      public string[] Views = new string[0];

      [Argument("sort", Helptext = "Sort order; see --list-sort-orders")]
      [ShortArgument('s')]
      public string Order = "title";

      [Argument("sort-descending", Helptext = "Sort order; see --list-sort-orders")]
      [ShortArgument('d')]
      [FlagArgument(true)]
      public bool DescendingOrder = false;
    }
  }
}
