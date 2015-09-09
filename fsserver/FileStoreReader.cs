using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Utilities;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace NMaier.SimpleDlna.FileMediaServer
{
  public class FileStoreReader : Logging
  {
    IFileStore _store;

    public FileStoreReader(IFileStore store) {
      _store = store;
    }
    /* ------------------ BaseFile serialize ------------------
        using (var s = new MemoryStream()) {
          var ctx = new StreamingContext(
            StreamingContextStates.Persistence,
            null
            );
          var formatter = new BinaryFormatter(null, ctx) {
            TypeFormat = FormatterTypeStyle.TypesWhenNeeded,
            AssemblyFormat = FormatterAssemblyStyle.Simple
          };
          formatter.Serialize(s, file);
        }
    */
    /*
        using (var s = new MemoryStream(data)) {
          var ctx = new StreamingContext(
            StreamingContextStates.Persistence,
            new DeserializeInfo(null, info, DlnaMime.ImageJPEG)
            );
          var formatter = new BinaryFormatter(null, ctx) {
            TypeFormat = FormatterTypeStyle.TypesWhenNeeded,
            AssemblyFormat = FormatterAssemblyStyle.Simple
          };
          var rv = formatter.Deserialize(s) as Cover;
          return rv;
        }    */

    /* ------------------ Cover deserialize ------------------
        using (var s = new MemoryStream(data)) {
          var ctx = new StreamingContext(
            StreamingContextStates.Persistence,
            new DeserializeInfo(null, info, DlnaMime.ImageJPEG)
            );
          var formatter = new BinaryFormatter(null, ctx) {
            TypeFormat = FormatterTypeStyle.TypesWhenNeeded,
            AssemblyFormat = FormatterAssemblyStyle.Simple
          };
          var rv = formatter.Deserialize(s) as Cover;
          return rv;
        }
    */
    /* ------------------ BaseFile deserialize ------------------
        using (var s = new MemoryStream(data)) {
          var ctx = new StreamingContext(
            StreamingContextStates.Persistence,
            new DeserializeInfo(server, info, type));
          var formatter = new BinaryFormatter(null, ctx) {
            TypeFormat = FormatterTypeStyle.TypesWhenNeeded,
            AssemblyFormat = FormatterAssemblyStyle.Simple
          };
          var rv = formatter.Deserialize(s) as BaseFile;
          rv.Item = info;
          return rv;
        }
    */

    private static T Decode<T>(byte[] data, DeserializeInfo dinfo) where T : class {
      if (data == null) return (T)null;
      using (var s = new MemoryStream(data)) {
        var ctx = new StreamingContext(StreamingContextStates.Persistence,dinfo);
        var formatter = new BinaryFormatter(null, ctx) {
          TypeFormat = FormatterTypeStyle.TypesWhenNeeded,
          AssemblyFormat = FormatterAssemblyStyle.Simple
        };
        return formatter.Deserialize(s) as T;
      }
    }

    public Cover GetCover(IStoreItem file) {
        try {
          return Decode<Cover>(_store.MaybeGetCover(file), new DeserializeInfo(null, file.Item, DlnaMime.ImageJPEG));
        }
        catch (SerializationException ex) {
          Debug("Failed to deserialize a cover", ex);
          return null;
        }
    }

    public BaseFile GetFile(FileInfo info, FileServer server, DlnaMime type) {
        return Decode<BaseFile>(_store.MaybeGetFile(info), new DeserializeInfo(server, info, type));
    }
  }
}
