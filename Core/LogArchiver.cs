using Microsoft.Extensions.Logging;
using System.IO.Compression;
using System.IO;
using System;

namespace AqbaApp.Core
{
    public class LogArchiver(string logDirectory, ILogger<LogArchiver> logger)
    {
        private readonly string _logDirectory = logDirectory;
        private readonly ILogger<LogArchiver> _logger = logger;
        private readonly string _logFormat = "*_log.txt";
        private readonly string _logZipFormat = "*.zip";

        public void ArchiveAndCleanLogs()
        {
            _logger.LogInformation("Start archiving and cleaning logs.");            

            try
            {
                ArchiveOldLogs();
                CleanOldArchives();
                _logger.LogInformation("Archiving and cleaning of logs is complete.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error archiving or clearing logs.");
            }
        }

        private void ArchiveOldLogs()
        {
            string[] files = Directory.GetFiles(_logDirectory, _logFormat);
            DateTime today = DateTime.Today;
            DateTime yesterday = today.AddDays(-1);

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);

                if (!DateTime.TryParseExact(fileName.Substring(0, 10), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime fileDate))
                {
                    _logger.LogWarning("Skipped unknown log file format: {FileName}", file);
                    continue;
                }

                if (fileDate >= yesterday)
                    continue; // пропускаем вчерашний и сегодняшний лог

                string zipFile = Path.Combine(_logDirectory, $"{Path.GetFileNameWithoutExtension(file)}.zip");

                try
                {
                    if (File.Exists(zipFile))
                    {
                        FileInfo info = new(zipFile);
                        if (info.Length == 22)
                            File.Delete(zipFile); // перезапишем пустой архив
                        else
                            continue; // архив уже существует
                    }

                    _logger.LogInformation("Archiving file {FileName}", file);

                    using (ZipArchive zip = ZipFile.Open(zipFile, ZipArchiveMode.Create))
                    {
                        zip.CreateEntryFromFile(file, Path.GetFileName(file));
                    }

                    File.Delete(file);
                    _logger.LogInformation("File {FileName} has been archived and deleted.", file);
                }
                catch (IOException ex)
                {
                    _logger.LogWarning(ex, "File {FileName} is in use and was skipped.", file);
                }
            }
        }

        private void CleanOldArchives()
        {
            string[] archives = Directory.GetFiles(_logDirectory, _logZipFormat);

            foreach (string archive in archives)
            {
                DateTime creationDate = File.GetCreationTime(archive);

                if (creationDate < DateTime.Now.AddMonths(-1))
                {
                    _logger.LogInformation("Delete obsolete archive {FileName}.", archive);
                    File.Delete(archive);
                }
            }
        }
    }
}
