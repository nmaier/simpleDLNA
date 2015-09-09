using NMaier.SimpleDlna.Utilities;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace NMaier.SimpleDlna.FileMediaServer
{
  public class FileStoreWriter : Logging
  {
    IFileStore _store;

    public FileStoreWriter(IFileStore store) {
      _store = store;
    }

    private static byte[] Encode(object obj) {
        using (var s = new MemoryStream()) {
          var ctx = new StreamingContext(
            StreamingContextStates.Persistence,
            null
            );
          var formatter = new BinaryFormatter(null, ctx) {
            TypeFormat = FormatterTypeStyle.TypesWhenNeeded,
            AssemblyFormat = FormatterAssemblyStyle.Simple
          };
          formatter.Serialize(s, obj);
        return s.ToArray();
      }
    }

    /*
    , byte[] coverData
    */

    public void StoreFile(BaseFile file) {
      byte[] coverData = null;
      try {
        coverData = Encode(file.MaybeGetCover());
      }
      catch (Exception) {
        // Ignore and store null.
      }

      _store.MaybeStoreFile(file, Encode(file), coverData);
    }
  }
}
