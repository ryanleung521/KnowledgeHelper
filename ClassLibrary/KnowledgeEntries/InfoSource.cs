using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.KnowledgeEntries
{
    public abstract class KnowledgeSource
    {
        internal string sourcePath;

        public abstract string GetSourceType();
        public abstract string GetSourceInfo();
        public abstract void InvokeSource();
    }

    public class WebSource : KnowledgeSource
    {
        public override string GetSourceType()
        {
            return "Web"; 
        }
         public override string GetSourceInfo()
        {
            return sourcePath;
        }

        public override void InvokeSource()
        {
            //Get Path to browser
            var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddXmlFile("ClassLibrary.dll.config", optional: false, reloadOnChange: true);

            var configuration = builder.Build();
            string browserpath = configuration["browserpath:add:BrowserPath:browserpath"];

            Process openWebSource = new Process();
            openWebSource.StartInfo.FileName = browserpath;
            openWebSource.StartInfo.Arguments = sourcePath;
            openWebSource.Start();
        }

        public WebSource(string sourcePath) { this.sourcePath = sourcePath;  }
    }

    public class DocumentSource : KnowledgeSource
    {
        public override string GetSourceType()
        {
            return "Document";
        }
        public override string GetSourceInfo()
        {
            return sourcePath;
        }

        public override void InvokeSource()
        {
            //Get Path to browser
            var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddXmlFile("ClassLibrary.dll.config", optional: false, reloadOnChange: true);

            var configuration = builder.Build();
            string browserpath = configuration["browserpath:add:BrowserPath:browserpath"];

            Process openWebSource = new Process();
            openWebSource.StartInfo.FileName = browserpath;
            openWebSource.StartInfo.Arguments = sourcePath;
            openWebSource.Start();
        }

        public DocumentSource(string sourcePath) { this.sourcePath = sourcePath; }
    }
}
