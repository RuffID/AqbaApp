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
        private readonly string logFormat = "*_log.txt";

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
            string[] files = Directory.GetFiles(_logDirectory, logFormat);
            DateTime today = DateTime.Now.Date;

            foreach (string file in files)
            {
                DateTime creationDate = File.GetCreationTime(file);

                if (creationDate >= today)
                    continue; // лог за сегодня — он сейчас открыт, пропускаем

                if (creationDate.Date < DateTime.Now.Date)
                {
                    string zipFile = Path.Combine(_logDirectory, $"{Path.GetFileNameWithoutExtension(file)}.zip");
                    FileInfo info = new(zipFile);

                    // 22 - дефолтный размер пустого zip архива
                    if (File.Exists(zipFile) && info.Length == 22)
                        continue; // уже заархивировано, поэтому пропускаем файл
                    else 
                        File.Delete(zipFile);

                    _logger.LogInformation("Archiving file {FileName}", file);

                    using (ZipArchive zip = ZipFile.Open(zipFile, ZipArchiveMode.Create))
                    {
                        zip.CreateEntryFromFile(file, Path.GetFileName(file));
                    }

                    File.Delete(file);
                    _logger.LogInformation("File {FileName} has been archived and deleted.", file);
                }
            }
        }

        private void CleanOldArchives()
        {
            string[] archives = Directory.GetFiles(_logDirectory, logFormat);

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
