using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
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
                    UnzipFromStream(stream, output, response.ContentLength);
                }
            }
        }

        private void UnzipFromStream(Stream zipStream, string outFolder)
        {
            ZipInputStream zipInputStream = new ZipInputStream(zipStream);
            ZipEntry zipEntry = zipInputStream.GetNextEntry();
            while (zipEntry != null)
            {
                var entryFileName = zipEntry.Name;
                log.Info("Unpacking [{0}]", entryFileName);
                // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                // Optionally match entrynames against a selection list here to skip as desired.
                // The unpacked length is available in the zipEntry.Size property.

                byte[] buffer = new byte[4096];     // 4K is optimum

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
                    zipEntry = zipInputStream.GetNextEntry();
                    continue;
                }

                // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                // of the file, but does not waste memory.
                // The "using" will close the stream even if an exception occurs.
                using (FileStream streamWriter = File.Create(fullZipToPath))
                {
                    StreamUtils.Copy(zipInputStream, streamWriter, buffer);
                }

                zipEntry = zipInputStream.GetNextEntry();
            }
        }
    }
}
