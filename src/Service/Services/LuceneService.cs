using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lucene.Net.Util.Packed.PackedInt32s;

namespace Service.Services;
public class LuceneService
{

    public void GetData()
    {

    }

    public void CreateIndex()
    {
        const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;

        // Construct a machine-independent path for the index
        var basePath = Environment.GetFolderPath(
            Environment.SpecialFolder.CommonApplicationData);
        var indexPath = Path.Combine(basePath, "index");

        using var dir = FSDirectory.Open(indexPath);

        // Create an analyzer to process the text
        var analyzer = new StandardAnalyzer(AppLuceneVersion);

        // Create an index writer
        var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
        using var writer = new IndexWriter(dir, indexConfig);

        var source = new
        {
            Name = "Kermit the Frog",
            FavoritePhrase = "The quick brown fox jumps over the lazy dog"
        };
        var doc = new Document
                    {
                        // StringField indexes but doesn't tokenize
                        new StringField("name",
                            source.Name,
                            Field.Store.YES),
                        new TextField("favoritePhrase",
                            source.FavoritePhrase,
                            Field.Store.YES)
                    };

        writer.AddDocument(doc);
        writer.Flush(triggerMerge: false, applyAllDeletes: false);

        var phrase = new MultiPhraseQuery
                        {
                            new Term("favoritePhrase", "brown"),
                            new Term("favoritePhrase", "fox")
                        };


        // Re-use the writer to get real-time updates
        using var reader = writer.GetReader(applyAllDeletes: true);
        var searcher = new IndexSearcher(reader);
        var hits = searcher.Search(phrase, 20 /* top 20 */).ScoreDocs;

        // Display the output in a table
        Console.WriteLine($"{"Score",10}" + $" {"Name",-15}" + $" {"Favorite Phrase",-40}");
        
        foreach (var hit in hits)
        {
            var foundDoc = searcher.Doc(hit.Doc);
            Console.WriteLine($"{hit.Score:f8}" +
                $" {foundDoc.Get("name"),-15}" +
                $" {foundDoc.Get("favoritePhrase"),-40}");
        }
    }

}
