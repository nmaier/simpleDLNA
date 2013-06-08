using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;
using NMaier.GetOptNet;
using NMaier.SimpleDlna.FileMediaServer;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Comparers;
using NMaier.SimpleDlna.Server.Views;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna
{
  public static class Program
  {
    private readonly static ManualResetEvent BlockEvent = new ManualResetEvent(false);

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
      Console.Title = "simple DLNA - shutting down ...";
    }

    private static void ListOrders()
    {
      var items = from v in ComparerRepository.ListItems()
                  orderby v.Key
                  select v.Value;
      Console.WriteLine("Available orders:");
      Console.WriteLine("----------------");
      Console.WriteLine();
      foreach (var i in items) {
        Console.WriteLine("  - " + i);
        Console.WriteLine();
      }
    }

    private static void ListViews()
    {
      var items = from v in ViewRepository.ListItems()
                  orderby v.Key
                  select v.Value;
      Console.WriteLine("Available views:");
      Console.WriteLine("----------------");
      Console.WriteLine();
      foreach (var i in items) {
        Console.WriteLine("  - " + i);
        Console.WriteLine();
      }
    }

    private static void Main(string[] args)
    {
      Console.WriteLine();
      var options = new Options();
      try {
        Console.TreatControlCAsInput = false;
        Console.CancelKeyPress += CancelKeyPressed;

        options.Parse(args);
        if (options.ShowHelp) {
          options.PrintUsage();
          return;
        }
        if (options.ShowVersion) {
          ShowVersion();
          return;
        }
        if (options.ShowLicense) {
          ShowLicense();
          return;
        }
        if (options.ListViews) {
          ListViews();
          return;
        }
        if (options.ListOrders) {
          ListOrders();
          return;
        }
        if (options.Directories.Length == 0) {
          throw new GetOptException("No directories specified");
        }

        options.SetupLogging();

        var server = new HttpServer(options.Port);
        try {
          server.Info("CTRL-C to terminate");

          Console.Title = "simple DLNA - starting ...";

          var types = options.Types[0];
          foreach (var t in options.Types) {
            types = types | t;
            server.InfoFormat("Enabled type {0}", t);
          }

          var friendlyName = "sdlna";

          if (options.Seperate) {
            foreach (var d in options.Directories) {
              server.InfoFormat("Mounting FileServer for {0}", d.FullName);
              var fs = SetupFileServer(options, types, new DirectoryInfo[] { d });
              friendlyName = fs.FriendlyName;
              server.RegisterMediaServer(fs);
              server.InfoFormat("{0} mounted", d.FullName);
            }
          }
          else {
            server.InfoFormat("Mounting FileServer for {0} ({1})", options.Directories[0], options.Directories.Length);
            var fs = SetupFileServer(options, types, options.Directories);
            friendlyName = fs.FriendlyName;
            server.RegisterMediaServer(fs);
            server.InfoFormat("{0} ({1}) mounted", options.Directories[0], options.Directories.Length);
          }

          Console.Title = String.Format("{0} - running ...", friendlyName);

          Run(server);
        }
        finally {
          server.Dispose();
        }
      }
      catch (GetOptException ex) {
        Console.Error.WriteLine("Error: {0}\n\n", ex.Message);
        options.PrintUsage();
      }
#if DEBUG
      catch (Exception ex) {
        LogManager.GetLogger(typeof(Program)).Fatal("Failed to run", ex);
      }
#endif
    }

    private static void Run(HttpServer server)
    {
      BlockEvent.WaitOne();

      server.Info("Going down!");
      server.Info("Closed!");
    }

    private static FileServer SetupFileServer(Options options, DlnaMediaTypes types, DirectoryInfo[] d)
    {
      var ids = new Identifiers(ComparerRepository.Lookup(options.Order), options.DescendingOrder);
      foreach (var v in options.Views) {
        try {
          ids.AddView(v);
        }
        catch (RepositoryLookupException) {
          throw new GetOptException("Invalid view " + v);
        }
      }
      var fs = new FileServer(types, ids, d);
      try {
        if (options.CacheFile != null) {
          fs.SetCacheFile(options.CacheFile);
        }
        fs.Load();
      }
      catch (Exception) {
        fs.Dispose();
        throw;
      }
      return fs;
    }

    private static void ShowLicense()
    {
      Console.Write(Encoding.UTF8.GetString(Properties.Resources.LICENSE));
    }

    private static void ShowVersion()
    {
      var a = Assembly.GetEntryAssembly();
      var v = a.GetName().Version;
      Console.WriteLine("App:    {0} {1}.{2}.{3}", a.GetName().Name, v.Major, v.Minor, v.Revision);
      a = Assembly.GetAssembly(typeof(FileServer));
      v = a.GetName().Version;
      Console.WriteLine("Files:  {0} {1}.{2}.{3}", a.GetName().Name, v.Major, v.Minor, v.Revision);
      a = Assembly.GetAssembly(typeof(HttpServer));
      v = a.GetName().Version;
      Console.WriteLine("Server: {0} {1}.{2}.{3}", a.GetName().Name, v.Major, v.Minor, v.Revision);
      a = Assembly.GetAssembly(typeof(AttributeCollection));
      v = a.GetName().Version;
      Console.WriteLine("Utils:  {0} {1}.{2}.{3}", a.GetName().Name, v.Major, v.Minor, v.Revision);

      Console.WriteLine("Http:   {0}", HttpServer.Signature);
    }
  }
}
