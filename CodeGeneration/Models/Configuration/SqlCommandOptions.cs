namespace CodeGeneration.Models.Configuration
{
    public class SqlCommandOptions
    {
        public bool ExecuteSql { get; set; }
        public string EmbeddedExecutable { get; set; }
        public string TargetServer { get; set; }
        public string TargetDatabase { get; set; }
        public bool UseTrustedConnection { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public bool EchoInput { get; set; }

        public SqlCommandOptions()
        {
            EmbeddedExecutable = "sqlcmd";
        }
    }
}
