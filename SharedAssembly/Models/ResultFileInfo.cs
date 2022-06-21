namespace SharedAssembly.Models
{
    public class ResultFileInfo
    {
        public string Path { get; private set; }
        public string Extension { get; private set; }

        public ResultFileInfo(string? path, string? extension)
        {
            ArgumentNullException.ThrowIfNull(path);
            ArgumentNullException.ThrowIfNull(extension);

            Path = path;
            Extension = extension;
        }
    }
}
