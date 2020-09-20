using System.Collections.Generic;
using System.Linq;
using docs_finder.models;
using Microsoft.EntityFrameworkCore;

namespace docs_finder
{
    public class DocSearcher
    {
        public List<Doc> Search(string query)
        {
            using var ctx = new DocContext();

            return ctx.Docs
                .Where(d =>
                    EF.Functions.Like(d.LoweredTitle, $"%{query}%")
                ).ToList();
        }
    }
}