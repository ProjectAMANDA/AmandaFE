using AmandaFE.Data;
using AmandaFE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmandaFE
{
    public static class Cookies
    {
        public static string UserId { get => "userId"; }

        /// <summary>
        /// Tries to get a User from the provided context using the UserId cookies stored
        /// on the provided HttpRequest object. If the cookie or the user is not found,
        /// then this method will return null.
        /// </summary>
        /// <param name="request">The HttpRequest for the action this is being called from</param>
        /// <param name="context">The database context to use for finding the returned user</param>
        /// <returns>The User object from the database matching the user's UserId cookie or
        /// null if it is not found</returns>
        public static async Task<User> GetUserFromCookieAsync(HttpRequest request, BlogDBContext context)
        {
            if (request is null || context is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Cookies.ContainsKey(UserId) &&
                Int32.TryParse(request.Cookies[UserId], out int userId))
            {
                return await context.User.FirstOrDefaultAsync(u => u.Id == userId);
            }
            else
            {
                return null;
            }
        }

        public static async Task WriteUserCookieByIdAsync(int userId, HttpResponse response, BlogDBContext context)
        {
            if (response is null || context is null)
            {
                throw new ArgumentNullException();
            }

            User user = await context.User.FindAsync(userId);

            if (user != null)
            {
                response.Cookies.Append(UserId, userId.ToString());
            }
        }

        public static async Task WriteUserCookieByNameAsync(string userName, HttpResponse response, BlogDBContext context)
        {
            if (string.IsNullOrWhiteSpace(userName) || response is null || context is null)
            {
                throw new ArgumentNullException();
            }

            User user = await context.User.FirstOrDefaultAsync(u => u.Name == userName);

            if (user != null)
            {
                response.Cookies.Append(UserId, user.Id.ToString());
            }
        }
    }
}
