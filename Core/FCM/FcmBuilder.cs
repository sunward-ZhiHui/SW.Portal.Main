using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.FCM
{
    public class FcmBuilder
    {
        private Fcm Fcm;

        public FcmBuilder()
        {
            Fcm = new Fcm();
        }

        public Fcm GetFcm()
        {
            return Fcm;
        }

        public FcmBuilder WithProjectID(string projectID)
        {
            Fcm.ProjectID = projectID;
            return this;
        }

        public FcmBuilder WithApiKey(string apiKey)
        {
            Fcm.Apikey = apiKey;
            return this;
        }

        public FcmBuilder WithV1EndPoint()
        {
            Fcm.EndPointUrl = $"https://fcm.googleapis.com/v1/projects/{Fcm.ProjectID}/messages:send";
            return this;
        }

    }
}
