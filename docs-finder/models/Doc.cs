using System;

namespace docs_finder.models
{
    public class Doc
    {
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        public string LoweredTitle { get; set; }
        
        public string Uri { get; set; }
        
        public DateTime Date { get; set; }
        
        public long PeerId { get; set; }
    }
}