using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace KleynGroup.Data
{
    public class YoutubeActions
    {
        string WebURL { get; set; }
        string Method { get; set; }

        public async System.Threading.Tasks.Task<string> UploadVideo(MediaFile file) {
            return "DELETED HAHAAHHAH";
        }
    }
}
