namespace Mosaico.NetCore.Models
{
    public class MosaicoEmail
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public MosaicoTemplate Template { get; set; }

        public string Metadata { get; set; }

        public string Content { get; set; }

        public string Html { get; set; }
    }
}