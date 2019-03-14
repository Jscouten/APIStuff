using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using ConsoleApp1;
using APIStuff;

namespace APIStuff
{
    class Program
    {
        // video manager: https://studio.youtube.com/channel/UCKRLGqXUTHFuzUdRX0rEIPA/videos?utm_medium=redirect&utm_source=%2Fmy_videos&utm_campaign=upgrade

        // AIzaSyB9XT-FVKK1rHzePhSrXanXWOeiF_X3GVg
        
        internal class UploadVideo
        {
            [STAThread]
            static void Main(string[] args)
            {
               


                Console.WriteLine("YouTube Data API: Upload Video");
                Console.WriteLine("==============================");

                try
                {
                    new UploadVideo().Run().Wait();
                }
                catch (AggregateException ex)
                {
                    foreach (var e in ex.InnerExceptions)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

            private async Task Run()
            {
                UserCredential credential;
                using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
                {
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        // This OAuth 2.0 access scope allows an application to upload files to the
                        // authenticated user's YouTube channel, but doesn't allow other types of access.
                        new[] { YouTubeService.Scope.YoutubeUpload },
                        "user",
                        CancellationToken.None
                    );
                }

                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
                });


                // hello
                Console.WriteLine("What is your video's filepath? (Make sure to put SpecialMovie.mp4.mp4 at the end)");
                string videofilepath = Console.ReadLine();
                Console.WriteLine("What is the title of the video?");
                string videotitle = Console.ReadLine();
                Console.WriteLine("What is the description of the video?");
                string videodescription = Console.ReadLine();
                Console.WriteLine("What is the number of tags on the video? (1-3)");
                int videotagnum = Convert.ToInt32(Console.ReadLine());
                string videotag1 = "";
                string videotag2 = "";
                string videotag3 = "";
                if (videotagnum >= 1)
                {
                    Console.WriteLine("What is the first tag?");
                    videotag1 = Console.ReadLine();
                    if (videotagnum >= 2)
                    {
                        Console.WriteLine("What is the second tag?");
                        videotag2 = Console.ReadLine();
                        if (videotagnum == 3)
                        {
                            Console.WriteLine("What is the third tag?");
                            videotag3 = Console.ReadLine();
                        }
                    }
                }
                
                Console.WriteLine("What is the Category ID? (should be able to use 1,2,10,15,17,19,20,22,23,24,25,26,27,28) - 22 guaranteed works");
                int videocategoryid = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Will the video be public, private, or unlisted? (choose 1 of the three)");
                string videoavailability = Console.ReadLine();

                VideoObject videoObject = new VideoObject(videotitle, videodescription, videotagnum, videotag1, videotag2, videotag3, videocategoryid, videoavailability, videofilepath);
                

                var video = new Video();
                video.Snippet = new VideoSnippet();
                video.Snippet.Title = videoObject.VideoTitle;
                video.Snippet.Description = videoObject.VideoDescription;
                if (videoObject.VideoTagNum == 1)
                {
                    video.Snippet.Tags = new string[] { videoObject.VideoTag1 };
                }
                if (videoObject.VideoTagNum == 2)
                {
                    video.Snippet.Tags = new string[] { videoObject.VideoTag1, videoObject.VideoTag2 };
                }
                if (videoObject.VideoTagNum == 3)
                {
                    video.Snippet.Tags = new string[] { videoObject.VideoTag1, videoObject.VideoTag2, videoObject.VideoTag3 };
                }

                
                video.Snippet.CategoryId = Convert.ToString(videoObject.VideoCategoryID); // See https://developers.google.com/youtube/v3/docs/videoCategories/list
                video.Status = new VideoStatus();
                video.Status.PrivacyStatus = videoObject.VideoAvailability; // or "private" or "public"
                var filePath = videoObject.VideoFilepath; // Replace with path to actual movie file.

                using (var fileStream = new FileStream(filePath, FileMode.Open))
                {
                    var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                    videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                    videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                    await videosInsertRequest.UploadAsync();
                }
            }

            void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
            {
                switch (progress.Status)
                {
                    case UploadStatus.Uploading:
                        Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                        break;

                    case UploadStatus.Failed:
                        Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                        break;
                }
            }

            void videosInsertRequest_ResponseReceived(Video video)
            {
                Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
            }
        }
    }




    //internal class Search
    //{
    //    [STAThread]
    //    static void Main(string[] args)
    //    {
    //        Console.WriteLine("YouTube Data API: Search");
    //        Console.WriteLine("========================");
    //        Timer t = new Timer(TimerCallback, null, 0, 10000);
    //        try
    //        {
    //            new Search().Run().Wait();
    //        }
    //        catch (AggregateException ex)
    //        {
    //            foreach (var e in ex.InnerExceptions)
    //            {
    //                Console.WriteLine("Error: " + e.Message);
    //            }
    //        }

    //        Console.WriteLine("Press any key to continue...");
    //        Console.ReadKey();
    //    }


    //    private async Task Run()
    //    {
    //        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
    //        {
    //            ApiKey = "AIzaSyB9XT-FVKK1rHzePhSrXanXWOeiF_X3GVg",
    //            ApplicationName = this.GetType().ToString()
    //        });

    //        var searchListRequest = youtubeService.Search.List("snippet");
    //        searchListRequest.Q = "Fortnite"; // Replace with your search term.
    //        searchListRequest.MaxResults = 50;

    //        // Call the search.list method to retrieve results matching the specified query term.
    //        var searchListResponse = await searchListRequest.ExecuteAsync();

    //        List<string> videos = new List<string>();
    //        List<string> channels = new List<string>();
    //        List<string> playlists = new List<string>();

    //        // Add each result to the appropriate list, and then display the lists of
    //        // matching videos, channels, and playlists.
    //        foreach (var searchResult in searchListResponse.Items)
    //        {
    //            switch (searchResult.Id.Kind)
    //            {
    //                case "youtube#video":
    //                    videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
    //                    break;

    //                case "youtube#channel":
    //                    channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
    //                    break;

    //                case "youtube#playlist":
    //                    playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
    //                    break;
    //            }
    //        }

    //        Console.WriteLine(String.Format("Videos:\n{0}\n", string.Join("\n", videos)));
    //        Console.WriteLine(String.Format("Channels:\n{0}\n", string.Join("\n", channels)));
    //        Console.WriteLine(String.Format("Playlists:\n{0}\n", string.Join("\n", playlists)));
    //    }
    //    private static void TimerCallback(Object o)
    //    {
    //        Console.Clear();
    //        try
    //        {
    //            new Search().Run().Wait();
    //        }
    //        catch (AggregateException ex)
    //        {
    //            foreach (var e in ex.InnerExceptions)
    //            {
    //                Console.WriteLine("Error: " + e.Message);
    //            }
    //        }
    //        GC.Collect();
    //    }
    //}
}




