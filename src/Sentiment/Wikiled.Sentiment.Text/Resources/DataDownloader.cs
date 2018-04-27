using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using NLog;

namespace Wikiled.Sentiment.Text.Resources
{
    public class DataDownloader
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public async Task DownloadFile(Uri url, string output)
        {
            log.Info("Downloading <{0}> to <{1}>", url, output);
            var request = WebRequest.Create(url);
            using (var response = await request.GetResponseAsync().ConfigureAwait(false))
            {
                using (var stream = response.GetResponseStream())
                {
                    UnzipFromStream(stream, output);
                }
            }
        }

        private void UnzipFromStream(Stream zipStream, string outFolder)
        {
            using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
            {
                foreach (var entry in archive.Entries)
                {
                    using (var stream = entry.Open())
                    {
                        ProcessFile(outFolder, entry, stream);
                    }
                }
            }
        }

        private static void ProcessFile(string outFolder, ZipArchiveEntry entry, Stream stream)
        {
            var entryFileName = entry.Name;
            log.Info("Unpacking [{0}]", entryFileName);

            // Manipulate the output filename here as desired.
            String fullZipToPath = Path.Combine(outFolder, entryFileName);
            string directoryName = Path.GetDirectoryName(fullZipToPath);
            if (directoryName.Length > 0)
            {
                Directory.CreateDirectory(directoryName);
            }

            // Skip directory entry
            string fileName = Path.GetFileName(fullZipToPath);
            if (fileName.Length == 0)
            {
                return;
            }

            // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
            // of the file, but does not waste memory.
            // The "using" will close the stream even if an exception occurs.
            using (FileStream streamWriter = File.Create(fullZipToPath))
            {
                stream.CopyTo(streamWriter, 4096); // 4K is optimum
            }
        }
    }
}
