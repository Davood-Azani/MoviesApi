namespace Movies.Api;

public static class ApiEndpoints
{
    private const string ApiBase = "api";

    // default for versioning package
    
        public static class Movies
        {
            private const string Base = $"{ApiBase}/movies";
            public const string Create = Base;
            //public const string Get = $"{Base}/{{id:guid}}";
            public const string Get = $"{Base}/{{idOrSlug}}";
            public const string GetAll = Base;
            public const string Update = $"{Base}/{{id:guid}}";
            public const string Delete = $"{Base}/{{id:guid}}";


            // ratings | as ratings are a sub resource of movies so we can define the endpoints here

            public const string Rate = $"{Base}/{{id:guid}}/ratings";
            public const string DeleteRating = $"{Base}/{{id:guid}}/ratings";

        }

        public static class Ratings
        {

            public const string Base = $"{ApiBase}/ratings";

            public const string GetUserRatings = $"{Base}/me"; //means  api/ratings/me

            // using me is a common practice to get the current user's data or sth about the Logined User



        }

    #region Url Versining without Package fro V1 and V2
    public static class V1

    {
        private const string VersionBase = $"{ApiBase}/v1";


        public static class Movies
        {
            private const string Base = $"{VersionBase}/movies";
            public const string Create = Base;
            //public const string Get = $"{Base}/{{id:guid}}";
            public const string Get = $"{Base}/{{idOrSlug}}";
            public const string GetAll = Base;
            public const string Update = $"{Base}/{{id:guid}}";
            public const string Delete = $"{Base}/{{id:guid}}";


            // ratings | as ratings are a sub resource of movies so we can define the endpoints here

            public const string Rate = $"{Base}/{{id:guid}}/ratings";
            public const string DeleteRating = $"{Base}/{{id:guid}}/ratings";

        }

        public static class Ratings
        {

            public const string Base = $"{VersionBase}/ratings";

            public const string GetUserRatings = $"{Base}/me"; //means  api/ratings/me

            // using me is a common practice to get the current user's data or sth about the Logined User



        }
    }
    public static class V2

    {
        private const string VersionBase = $"{ApiBase}/v2";


        public static class Movies
        {
            private const string Base = $"{VersionBase}/movies";
            public const string Create = Base;
            //public const string Get = $"{Base}/{{id:guid}}";
            public const string Get = $"{Base}/{{idOrSlug}}";
            public const string GetAll = Base;
            public const string Update = $"{Base}/{{id:guid}}";
            public const string Delete = $"{Base}/{{id:guid}}";


            // ratings | as ratings are a sub resource of movies so we can define the endpoints here

            public const string Rate = $"{Base}/{{id:guid}}/ratings";
            public const string DeleteRating = $"{Base}/{{id:guid}}/ratings";

        }

        public static class Ratings
        {

            public const string Base = $"{VersionBase}/ratings";

            public const string GetUserRatings = $"{Base}/me"; //means  api/ratings/me

            // using me is a common practice to get the current user's data or sth about the Logined User



        }
    }

    #endregion







}