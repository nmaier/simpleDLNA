simple DLNA
===
A simple, zero-config DLNA media server, that you can just fire up and be done with it.

Motivation
---
Looking for a simple DLNA server that I could just fire up on some directory, watch some files on my TV and then be done with it, I came up empty. There are some decent servers out there, like Serviio and friends, but none that fits my requirements. Also there wasn't really a decent enough and uncomplicated enough open source implementation that I could borrow. The Coherence python project seemed to be a good starting point, but was already to complex for what I was trying to achieve. Also, python dealing with Unicode paths on Windows pretty much sucks. Then there is Mono.UPnP, which didn't really work, and the Intel(?) Developer Tools for UPnP, which seemed kinda nice at first, but turned out to be coming straight out of over-engineering hell, in my humble opinion.

And so I decided to write my own server, borrowing some ideas from Coherence, reading some of the UPnP specs and reverse engineering the various protocols involved (yuck, SOAP :p) by watching my TV interact with Coherence and Serviio under Wireshark.

Basic use
---

* `sdlna.exe` / `mono sdna.exe`
  Serve videos from current directory and subdirectories
* `sdlna.exe -t image -t audio -v bytitle`
   Serve images and audio files from the current directory and subdirectories, organizing files by title.
* `sdlna.exe -c cache.sqlite dir1 dir2`
   Serve videos from dir1 and dir2 (and subdirectories), storing meta data and thumbnails in the persistent cache store `cache.sqlite`.
* `sdlna.exe --help`
  Display usage help


Features
---

* Zero-config DLNA Server without persistent state (optional cache)
* Written in C# (See FAQ)
* Thumbnailing support for images and videos - if ffmpeg is found in the search path -, using any stream as input.
* Meta data as provided by TagLib#
* Serving of any and all file-system addressable  files, incl. some views (transformations)
* Should be relatively easy to code up additional media sources, like podcasts

Non-Features, maybe TODO
---

* Media transcoding
 * Although it should be easy enough to come up with something based on ffmpeg and the various freely available image libraries
* Full DLNA support - only browsing/playing supported at the moment
* Complete SSDP support. Because SSDP makes me cringe, I stopped after getting the basics working
* No Unit testing or test suite... Yeah, laziness is your enemy
* Microsoft WMP, while based on DLNA, won't accept media due to unimplemented MS specific commands. Same likely goes for an XBOX (which I don't own, so cannot test)
* ...

Run requirements
---

* Some .Net 4 compatible implementation to run the app
 * Tested on .Net 4 and Fedora17 + mono, OSX Lion + mono
* File system and networking :p
* Some DLNA renderer (e.g. TV) to actually display the served media.
 * Tested with: Samsung C-Series TV, Kinsky

External dependencies
---

* [log4net](http://logging.apache.org/log4net/)
* [GetOptNet](https://github.com/nmaier/getoptnet)
* [TagLib#](https://github.com/mono/taglib-sharp/)
* [System.Data.Sqlite](http://system.data.sqlite.org/)

The external dependencies are [nuget](http://nuget.org/) managed and can be auto-restored (except for Sqlite maybe... Just reinstall it using nuget into the fsserver project).

Design
---
The thing wasn't formally designed, but more written as I went along. However, the individual components are only loosely coupled and interact through some real, generic interfaces that allow for extensibility.

Most of the IO is asynchronous, as per .Net `Stream.BeginRead/.BeginWrite`. No forking (D'OH), no explicit thread management.

The structure is as follows:

* `server` class library - The actual core that implements an SSDP and HTTP server
  * `SSDPServer` implementing the important bits of the SSDP-based multicast protocol as used by UPnP/DLNA
  * `HttpServer` and `HttpClient` - Custom, stripped down HTTP/1.0 implementation
  * `Handlers` - Request processing and response composition
  * `Responses` - Implementing an interface the HttpClient knows how to ship over the net.
  * Some interfaces, types, enums, etc. to bring things together
* `fsserver` class library - Serving stuff from a file sytem. To be "mounted" by the `server`
 * Virtual folder/file trees the HTTPServer MediaMount will understand
 * "Views" that transform the virtual trees
* `thumbs` class library - Generating thumbnail pictures from arbriary stream sources
 * Image thumbnailer, using .Net System.Drawing
 * Video thumbnailer, using ffmpeg via async pipes and the .Net Process API
* `sdlna` CLI application - Bringing the various pieces together

Contributing
---
Feel free to drop me pull requests. If you plan to implement something more than a few lines, then open the pull request early so that there aren't any nasty surprises later.

If you want to add something that will require some for of persistence incl. persistent configuration or API keys, etc., then open a pull request/issue especially early!

Mini FAQ
---

* **Q**: Does it work in my network/with my TV?
* **A**: At the moment, if you got a somewhat recent Samsung TV, then probably yes. Otherwise probably not. Why don't you just try?
* **Q**: This thing does not work with my setup?!
* **A**: Either provide me with the setup (i.e. buy me that TV) or whip out your debugger and Wireshark and read Contributing ;)
* **Q**: Are you planning to support Podcasts, RTMP streaming, *insert name here*?
* **A**: Maybe, but probably not. See Contributing
* **Q**: Why .Net and C#? I'm on Linux and/or Mac!
* **A**: Yeah, well... Other languages have their own set of problems. C# is reasonably portable, managed and garbage collected, statically typed and comes with a huge stdlib.
* **Q**: Where are the binaries?
* **A**: There is no official release for now. I'm still cleaning up stuff a bit. Stay tuned...

Thanks
---
To Microsoft, the UPnP(-AV) folks and Samsung and DLNA gurus for designing such a set of specs and protocols, that includes HTTP-alike over multicast UDP sockets, SOAP and XML, dozens of poorly documented namespaces used in SOAP responses, the contentFeatures.dlna.org HTTP header and other fancy tech.
Also a big Kudos also to software engineers for messing up even the most basic things like http header field names are usually case insensitive or the Reason-Phrase of http status lines is purely informal.