using System.IO.Compression;
using System.Text;

namespace DotNetLauncher.Helpers;

public static class DownloadHelper
{
    public static byte[] BuildTemplate(Stream stream, string database, string solution)
    {
        using var ms1 = new MemoryStream();
        using var ms2 = new MemoryStream();
        stream.CopyToAsync(ms1);
        ms1.Seek(0, SeekOrigin.Begin);

        using (var zipArchive = new ZipArchive(ms1, ZipArchiveMode.Update, true))
        using (var newZipArchive = new ZipArchive(ms2, ZipArchiveMode.Update, true))
        {
            foreach (var zipArchiveEntry in zipArchive.Entries)
            {
                var newZipArchiveEntry = zipArchiveEntry.Name.Contains(".sln")
                    ? newZipArchive.CreateEntry($"{solution}/{solution}.sln")
                    : newZipArchive.CreateEntry(zipArchiveEntry.FullName.Replace(database, solution));

                using var s1 = zipArchiveEntry.Open();
                using var s2 = newZipArchiveEntry.Open();
                s1.CopyTo(s2);
                s2.Seek(0, SeekOrigin.Begin);

                using var streamReader = new StreamReader(s2, Encoding.Default);
                var content = new StringBuilder(streamReader.ReadToEnd());
                content.Replace($"Template{database}", solution);

                if (newZipArchiveEntry.Name.Contains("docker-compose"))
                    content.Replace($"Template{database}".ToLowerInvariant(), solution.ToLowerInvariant());

                s2.SetLength(0);

                using var streamWriter = new StreamWriter(s2, Encoding.Default);
                streamWriter.Write(content);
            }
        }

        return ms2.ToArray();
    }
}