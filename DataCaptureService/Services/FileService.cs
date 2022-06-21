namespace DataCaptureService.Services
{
    public class FileService
    {
        private readonly string _dataPath;
        private readonly HashSet<string> _processedFiles = new HashSet<string>();
        public FileService(string dataPath)
        {
            _dataPath = dataPath;

            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
        }

        public string[] GetNewFiles(string fileExtension)
        {
            var files = Directory.GetFiles(_dataPath, $"*.{fileExtension}");

            return files
                .Where(f => !_processedFiles.Contains(f))
                .ToArray();
        }

        public void SetProcessedFile(string fileName)
        {
            _processedFiles.Add(fileName);
        }
    }
}
