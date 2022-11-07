namespace Api.Models
{
    public class MetadataModel
    {
        public Guid TempId { get; set; }
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!; // Тип файла
        public long Size { get; set; } // размер файла
    }
}
