using System;
using System.IO;
using System.Runtime.Serialization;
using NMaier.sdlna.FileMediaServer.Folders;
using NMaier.sdlna.Server;
using NMaier.sdlna.Server.Metadata;

namespace NMaier.sdlna.FileMediaServer.Files
{
  [Serializable]
  internal class ImageFile : BaseFile, IMetaImageItem, ISerializable
  {

    private string creator;
    private string description;
    private uint? width, height;
    private bool initialized = false;
    private string title;



    internal ImageFile(BaseFolder aParent, FileInfo aFile, DlnaTypes aType) : base(aParent, aFile, aType, MediaTypes.IMAGE) { }

    protected ImageFile(SerializationInfo info, StreamingContext ctx)
      : this(null, (ctx.Context as DeserializeInfo).Info, (ctx.Context as DeserializeInfo).Type)
    {
      creator = info.GetString("cr");
      description = info.GetString("d");
      title = info.GetString("t");
      width = info.GetUInt32("w");
      height = info.GetUInt32("h");

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

    public uint? MetaHeight
    {
      get
      {
        MaybeInit();
        return height;
      }
    }

    public uint? MetaWidth
    {
      get
      {
        MaybeInit();
        return width;
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
      info.AddValue("al", creator);
      info.AddValue("ar", description);
      info.AddValue("g", title);
      info.AddValue("p", width);
      info.AddValue("ti", height);
    }

    private void MaybeInit()
    {
      if (initialized) {
        return;
      }

      try {
        using (var tl = TagLib.File.Create(Item.FullName)) {
          try {
            width = (uint)tl.Properties.PhotoWidth;
            height = (uint)tl.Properties.PhotoHeight;
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
      }
      catch (TagLib.CorruptFileException ex) {
        Debug("Failed to read metadata via taglib for file " + Item.FullName, ex);
      }
      catch (Exception ex) {
        Warn("Unhandled exception reading metadata for file " + Item.FullName, ex);
      }


      initialized = true;
    }
  }
}
