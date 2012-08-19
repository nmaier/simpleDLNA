using System;
using System.IO;
using NMaier.sdlna.Server;
using NMaier.sdlna.Server.Metadata;

namespace NMaier.sdlna.FileMediaServer
{
  internal class ImageFile : File, IMetaImageItem
  {

    private string creator;
    private string description;
    private uint? width, height;
    private bool initialized = false;
    private string title;



    internal ImageFile(IMediaFolder aParent, FileInfo aFile, DlnaTypes aType) : base(aParent, aFile, aType, MediaTypes.IMAGE) { }



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




    private void MaybeInit()
    {
      if (initialized) {
        return;
      }

      try {
        using (var tl = TagLib.File.Create(file.FullName)) {
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
        Debug("Failed to read metadata via taglib for file " + file.FullName, ex);
      }
      catch (Exception ex) {
        Warn("Unhandled exception reading metadata for file " + file.FullName, ex);
      }


      initialized = true;
    }
  }
}
