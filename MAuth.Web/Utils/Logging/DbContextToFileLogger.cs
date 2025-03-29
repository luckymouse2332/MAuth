using System.Diagnostics;

namespace MAuth.Web.Utils.Logging
{
    using static System.DateTime;

    public class DbContextToFileLogger
    {
        /// <summary>
        /// 日志文件名称
        /// </summary>
        private readonly string _fileName =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "LogFiles", $"{Now.Year}-{Now.Month}-{Now.Day}", $"EF_Log.log");

        /// <summary>
        /// 覆盖默认的文件名和路径
        /// </summary>
        /// <param name="fileName"></param>
        public DbContextToFileLogger(string fileName)
        {
            _fileName = fileName;
        }

        /// <summary>
        /// Setup to use default file name for logging
        /// </summary>
        public DbContextToFileLogger()
        {

        }
        /// <summary>
        /// 将日志信息添加到存在的文件流中
        /// </summary>
        /// <param name="message"></param>
        [DebuggerStepThrough]
        public void Log(string message)
        {

            if (!File.Exists(_fileName))
            {
                File.CreateText(_fileName).Close();
            }

            StreamWriter streamWriter = new(_fileName, true);

            streamWriter.WriteLine(message);

            streamWriter.WriteLine(new string('-', 40));

            streamWriter.Flush();
            streamWriter.Close();
        }
    }
}
