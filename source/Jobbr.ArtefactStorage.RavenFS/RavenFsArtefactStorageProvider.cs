using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Jobbr.ComponentModel.ArtefactStorage;
using Raven.Client.FileSystem;
using Raven.Json.Linq;

namespace Jobbr.ArtefactStorage.RavenFS
{
    public class RavenFsArtefactStorageProvider : IArtefactsStorageProvider
    {
        private readonly FilesStore _filesStore;

        public RavenFsArtefactStorageProvider(RavenFsConfiguration configuration)
        {
            _filesStore = new FilesStore
            {
                Url = configuration.Url,
                DefaultFileSystem = configuration.FileSystem
            };

            _filesStore.Initialize();
        }

        public void Save(string container, string fileName, Stream content)
        {
            AsyncHelper.RunSync(() => SaveAsync(container, fileName, content));
        }

        public async Task SaveAsync(string container, string fileName, Stream content)
        {
            using (var session = _filesStore.OpenAsyncSession())
            {
                var metadata = new RavenJObject
                {
                    { "Container", container },
                    { "Name", fileName }
                };

                session.RegisterUpload($"{container}/{fileName}", content, metadata);
                await session.SaveChangesAsync();
            }
        }

        public async Task<Stream> LoadAsync(string container, string fileName)
        {
            using (var session = _filesStore.OpenAsyncSession())
            {
                var file = await session.Query()
                    .WhereEquals("Container", container)
                    .WhereEquals("Name", fileName)
                    .FirstOrDefaultAsync();

                var stream = await session.DownloadAsync(file.Name);

                return stream;
            }
        }

        public List<JobbrArtefact> GetArtefacts(string container)
        {
            return AsyncHelper.RunSync<List<JobbrArtefact>>(() => GetArtefactsAsync(container));
        }

        public async Task<List<JobbrArtefact>> GetArtefactsAsync(string container)
        {
            using (var session = _filesStore.OpenAsyncSession())
            {
                var files = await session.Query()
                    .WhereEquals("Container", container)
                    .ToListAsync();

                var list = new List<JobbrArtefact>();

                foreach (var file in files)
                {
                    var stream = await session.DownloadAsync(file.FullPath);
                    list.Add(new JobbrArtefact { Data = stream, FileName = file.Metadata["Name"].ToString() });
                }

                return list;
            }
        }

        public Stream Load(string container, string fileName)
        {
            return AsyncHelper.RunSync<Stream>(() => LoadAsync(container, fileName));
        }
    }
}
