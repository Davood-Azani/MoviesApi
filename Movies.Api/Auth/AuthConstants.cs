namespace Movies.Api.Auth
{
    public static class AuthConstants
    {
        public const string AdminUserPolicyName = "JustAdmin";
        public const string AdminUserClaimName = "admin";
        public const string TrustedMemberPolicyName = "JustTrustedMember";
        public const string TrustedMemberClaimName = "trusted_member";
        public const string True = "true";
        public const string False = "false";


        public const string ApiKeyHeaderName = "x-api-key";

    }
}
