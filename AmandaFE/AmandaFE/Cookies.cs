﻿using AmandaFE.Data;
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
                return await context.User.Include(u => u.Posts)
                                         .FirstOrDefaultAsync(u => u.Id == userId);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Writes out the user's id to the user's browser as a cookie
        /// </summary>
        /// <param name="userId">The user's user id</param>
        /// <param name="response">The HttpResponse object for the calling action</param>
        /// <param name="context">The database context to operate on</param>
        /// <returns>An empty async Task</returns>
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

        /// <summary>
        /// Writes out the user's id to the user's browser as a cookie
        /// </summary>
        /// <param name="userName">The user's username</param>
        /// <param name="response">The HttpResponse object for the calling action</param>
        /// <param name="context">The database context to operate on</param>
        /// <returns>An empty async Task</returns>
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
