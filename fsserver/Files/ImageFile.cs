using System;
using System.IO;
using System.Runtime.Serialization;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Metadata;

namespace NMaier.SimpleDlna.FileMediaServer.Files
{
  [Serializable]
  internal sealed class ImageFile : BaseFile, IMetaImageItem, ISerializable
  {
    private string creator;
    private string description;
    private int? width, height;
    private bool initialized = false;
    private string title;



    internal ImageFile(FileServer server, FileInfo aFile, DlnaMime aType)
      : base(server, aFile, aType, DlnaMediaTypes.Image)
    {
    }

    private ImageFile(SerializationInfo info, StreamingContext ctx)
      : this((ctx.Context as DeserializeInfo).Server, (ctx.Context as DeserializeInfo).Info, (ctx.Context as DeserializeInfo).Type)
    {
    }

    private ImageFile(SerializationInfo info, DeserializeInfo di)
      : this(di.Server, di.Info, di.Type)
    {
      creator = info.GetString("cr");
      description = info.GetString("d");
      title = info.GetString("t");
      width = info.GetInt32("w");
      height = info.GetInt32("h");

      initialized = true;
    }



    public string MetaCreator
    {
      get
      {
        MaybeInit();
        return creator;
      }
    }

    public string MetaDescription
    {
      get
      {
        MaybeInit();
        return description;
      }
    }

    public int? MetaHeight
    {
      get
      {
        MaybeInit();
        return height;
      }
    }

    public int? MetaWidth
    {
      get
      {
        MaybeInit();
        return width;
      }
    }

    public override IHeaders Properties
    {
      get
      {
        MaybeInit();
        var rv = base.Properties;
        if (description != null) {
          rv.Add("Description", description);
        }
        if (creator != null) {
          rv.Add("Creator", creator);
        }
        if (width != null && height != null) {
          rv.Add("Resolution", string.Format("{0}x{1}", width.Value, height.Value));
        }
        return rv;
      }
    }

    public override string Title
    {
      get
      {
        if (!string.IsNullOrWhiteSpace(title)) {
          return string.Format("{0} — {1}", base.Title, title);
        }
        return base.Title;
      }
    }




    public void GetObjectData(SerializationInfo info, StreamingContext ctx)
    {
      MaybeInit();
      info.AddValue("cr", creator);
      info.AddValue("d", description);
      info.AddValue("t", title);
      info.AddValue("w", width);
      info.AddValue("h", height);
    }

    private void MaybeInit()
    {
      if (initialized) {
        return;
      }

      try {
        using (var tl = TagLib.File.Create(Item.FullName)) {
          try {
            width = tl.Properties.PhotoWidth;
            height = tl.Properties.PhotoHeight;
          }
          catch (Exception ex) {
            Debug("Failed to transpose Properties props", ex);
          }

          try {
            var t = (tl as TagLib.Image.File).ImageTag;
            if (string.IsNullOrWhiteSpace(title = t.Title)) {
              title = null;
            }
            if (string.IsNullOrWhiteSpace(description = t.Comment)) {
              description = null;
            }
            if (string.IsNullOrWhiteSpace(creator = t.Creator)) {
              creator = null;
            }
          }
          catch (Exception ex) {
            Debug("Failed to transpose Tag props", ex);
          }
        }


        initialized = true;

        Server.UpdateFileCache(this);
      }
      catch (TagLib.CorruptFileException ex) {
        Debug("Failed to read metadata via taglib for file " + Item.FullName, ex);
        initialized = true;
      }
      catch (TagLib.UnsupportedFormatException ex) {
        Debug("Failed to read metadata via taglib for file " + Item.FullName, ex);
        initialized = true;
      }
      catch (Exception ex) {
        Warn("Unhandled exception reading metadata for file " + Item.FullName, ex);
      }
    }
  }
}
