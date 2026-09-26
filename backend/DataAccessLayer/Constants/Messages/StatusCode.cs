namespace DataAccessLayer.Constants.Messages
{
    public static class StatusCode
    {
        public const string InternalServerError500 = "Internal Service Error!";
        public const string TooManyRequests429 = "Too many requests! Please try again later.";
        public const string Unauthorized401 = "Unauthorized! Please check your credentials.";
        public const string NotFound404 = "Resource not found!";
    }
}
