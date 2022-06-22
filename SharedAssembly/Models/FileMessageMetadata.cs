namespace SharedAssembly.Models
{
    public class FileMessageMetadata
    {
        public string FileName { get; set; }
        public int Sequence { get; set; }

        public FileMessageMetadata(string? fileName, int? sequence)
        {
            ArgumentNullException.ThrowIfNull(fileName);
            ArgumentNullException.ThrowIfNull(sequence);

            if (sequence < 1)
                throw new ArgumentException("Sequence can't be less than 1");

            FileName = fileName;
            Sequence = sequence.Value;
        }
    }
}
