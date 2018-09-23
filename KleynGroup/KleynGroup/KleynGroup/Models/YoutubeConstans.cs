using System;
using System.Collections.Generic;
using System.Text;

namespace KleynGroup.Models
{
    public class YoutubeConstans
    {
        public string client_id = "924710780360-lpfpvg2csvit4dkumt98p9iugdh2tsl8.apps.googleusercontent.com";
        public string project_id = "youtube-api-v-205008";
        public string auth_uri = "https://accounts.google.com/o/oauth2/auth";
        public string token_uri = "https://accounts.google.com/o/oauth2/token";
        public string auth_provider_x509_cert_url = "https://www.googleapis.com/oauth2/v1/certs";
        public string client_secret = "fPcqtLxqjNDPKh4tZbhIC8q4";
        public string[] redirect_uris = new string[]
               {
                "urn:ietf:wg:oauth:2.0:oob",
                "http://localhost"
               };
    }
}
