using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoLibrary;

namespace youtube_dl
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var argsList = args.ToList();
                if (argsList.Count != 1) throw new Exception("Invalid argument count. Please provide a link");

                var downloadDirectory = Directory.GetCurrentDirectory() + "/";
                
                var video = YouTube.Default
                    .GetAllVideos(argsList.FirstOrDefault())
                    .OrderByDescending(videoFile => videoFile.AudioBitrate)
                    .FirstOrDefault();

                File.WriteAllBytes(downloadDirectory + video.FullName, await video.GetBytesAsync());

                var inputFile = new MediaFile { Filename = downloadDirectory + video.FullName };
                var outputFile = new MediaFile { Filename = $"{downloadDirectory + video.FullName}.mp3" };

                using (var engine = new Engine())
                {
                    engine.ConvertProgressEvent += (o, e) => 
                    {
                        Console.WriteLine($@"Converted {e.SizeKb} kilobytes");
                    };
                    
                    engine.GetMetadata(inputFile);
                    engine.Convert(inputFile, outputFile);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
