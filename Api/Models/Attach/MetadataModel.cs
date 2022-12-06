namespace Api.Models.Attach
{
    public class MetadataModel
    {
        public Guid TempId { get; set; }
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public long Size { get; set; }
    }

    public class MetadataLinkModel : MetadataModel
    {
        public string FilePath { get; set; } = null!;
        public Guid AuthorId { get; set; }
       
    }
}
