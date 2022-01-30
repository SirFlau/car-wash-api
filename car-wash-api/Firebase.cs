using FireSharp.Config;
using FireSharp.Interfaces;

namespace car_wash_api
{
    public class Firebase
    {
        private static readonly IFirebaseConfig firebaseConfig = new FirebaseConfig
        {
            AuthSecret = "oWCdYJpyRicz8D3RlpOUA0nd3hXlqeAnd3DGz8x8",
            BasePath = "https://carwash-616ca-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        public static IFirebaseConfig FirebaseConfig { get { return firebaseConfig; } }

    }
}
