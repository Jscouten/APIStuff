using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class VideoObject
    {
        public string VideoTitle { get; set; }
        public string VideoDescription { get; set; }
        public int VideoTagNum { get; set; }
        public string VideoTag1 { get; set; }
        public string VideoTag2 { get; set; }
        public string VideoTag3 { get; set; }
        public int VideoCategoryID { get; set; }
        public string VideoAvailability { get; set; }
        public string VideoFilepath { get; set; }
        public VideoObject(string videoTitle, string videoDescription, int videoTagNum, string videoTag1, string videoTag2, string videoTag3, int videoCategoryID, string videoAvailability, string videoFilepath)
        {
            VideoTitle = videoTitle;
            VideoDescription = videoDescription;
            VideoTagNum = videoTagNum;
            VideoTag1 = videoTag1;
            VideoTag2 = videoTag2;
            VideoTag3 = videoTag3;
            VideoCategoryID = videoCategoryID;
            VideoAvailability = videoAvailability;
            VideoFilepath = videoFilepath;
        }
      
    }
}
