namespace ProjectManagementFrontend.Services
{
    public static class CommonVariables
    {
        // Provides access to the current HttpContext (request-specific data like session, user, etc.)
        private static IHttpContextAccessor _httpContextAccessor;

        // Static constructor initializes the IHttpContextAccessor
        static CommonVariables()
        {
            _httpContextAccessor = new HttpContextAccessor();
        }

        /// <summary>
        /// Retrieves the JWT token from the session, if available.
        /// </summary>
        /// <returns>JWT token as string if exists, otherwise null.</returns>
        public static string? Token()
        {
            string? Token = null;

            // Check if the session contains the "JWTToken" key
            if (_httpContextAccessor.HttpContext?.Session.GetString("JWTToken") != null)
            {
                // If it exists, get the token value from session
                Token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
            }

            return Token;
        }

        public static string? UserRole()
        {
            string? UserRole = null;

            // Check if the session contains the "UserRole" key
            if (_httpContextAccessor.HttpContext?.Session.GetString("UserRole") != null)
            {
                // If it exists, get the username value from session
                UserRole = _httpContextAccessor.HttpContext.Session.GetString("UserRole");
            }

            return UserRole;
        }
    }
}
