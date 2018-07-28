using System;
using NLog;

namespace CodeGeneration.Models.Context
{
    public class GenerationStatistics
    {
        public bool GenerationEnabled { get; set; }
        public int LinesOfCode { get; set; }
        public int NumFilesProcessed { get; set; }
        public int NumFilesOutput { get; set; }
        public DateTime StartTime { get; set; }

        private DateTime _endTime;
        public DateTime EndTime
        {
            get => _endTime;
            set
            {
                if (_endTime.Equals(value)) return;

                _endTime = value;
                Duration = EndTime - StartTime;
            }
        }

        public TimeSpan Duration { get; set; }

        public GenerationStatistics()
        {
            GenerationEnabled = true;
            LinesOfCode = 0;
            NumFilesProcessed = 0;
            NumFilesOutput = 0;
            StartTime = DateTime.UtcNow;
        }

        public GenerationStatistics Report(Logger logger, string header = "")
        {
            logger.Info("--------------------------------------------------------------------------------");

            if (!string.IsNullOrWhiteSpace(header))
            {
                logger.Info(header);
                logger.Info("--------------------------------------------------------------------------------");
            }

            logger.Info("Lines of Code: {0}", LinesOfCode);
            logger.Info("Number of Files Processed: {0}", NumFilesProcessed);
            logger.Info("Number of Files Output: {0}", NumFilesOutput);
            logger.Info("Start Time: {0}", StartTime.ToLocalTime().ToString("s"));
            logger.Info("End Time: {0}", EndTime.ToLocalTime().ToString("s"));
            logger.Info("Duration: {0}", Duration.ToString("g"));

            logger.Info("--------------------------------------------------------------------------------");

            return this;
        }
    }
}
