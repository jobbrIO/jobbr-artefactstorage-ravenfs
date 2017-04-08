# Jobbr RavenFS Artefact Storage Provider [![Develop build status][ravenfs-badge-build-develop]][ravenfs-link-build]

This is an artefact storage provider implementation for the [Jobbr .NET JobServer](http://www.jobbr.io) to store artefacts related from job runs in RavenFS. 
The Jobbr main repository can be found on [JobbrIO/jobbr-server](https://github.com/jobbrIO).

[![NuGet-Stable][ravenfs-badge-nuget]][ravenfs-link-nuget]
[![Develop build status][ravenfs-badge-build-develop]][ravenfs-link-build] 
[![NuGet Pre-Release][ravenfs-badge-nuget-pre]][ravenfs-link-nuget] 

## Installation
First of all you'll need a working jobserver by using the usual builder as shown in the demos ([jobbrIO/jobbr-demo](https://github.com/jobbrIO/jobbr-demo)). In addition to that you'll need to install the NuGet Package for this extension.

### NuGet

    Install-Package Jobbr.ArtefactStorage.RavenFS

### Configuration
Configure RavenFS using the extension method on the `JobbrBuilder` instance.

```c#
using Jobbr.ArtefactStorage.RavenFS;

/* ... */

var builder = new JobbrBuilder();

jobbrBuilder.AddRavenFsArtefactStorage(config =>
{
    config.Url = "http://localhost:8080";
    config.FileSystem = "Jobbr";
});

server.Start();
```
# License
This software is licenced under GPLv3. See [LICENSE](LICENSE), and the related licences of 3rd party libraries below.

# Acknowledgements
This extension is built using the following great open source projects




# Credits
This application was built by the following awesome developers:
* Oliver Zürcher

[ravenfs-link-build]:            https://ci.appveyor.com/project/Jobbr/jobbr-artefactstorage-ravenfs         
[ravenfs-link-nuget]:            https://www.nuget.org/packages/Jobbr.ArtefactStorage.RavenFS

[ravenfs-badge-build-develop]:   https://img.shields.io/appveyor/ci/Jobbr/jobbr-artefactstorage-ravenfs/develop.svg?label=develop
[ravenfs-badge-nuget]:           https://img.shields.io/nuget/v/Jobbr.ArtefactStorage.RavenFS.svg?label=NuGet%20stable
[ravenfs-badge-nuget-pre]:       https://img.shields.io/nuget/vpre/Jobbr.ArtefactStorage.RavenFS.svg?label=NuGet%20pre
