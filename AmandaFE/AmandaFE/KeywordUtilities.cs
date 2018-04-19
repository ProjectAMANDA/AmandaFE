using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using AmandaFE.Models;
using AmandaFE.Data;
using Microsoft.EntityFrameworkCore;

namespace AmandaFE
{
    public static class KeywordUtilities
    {
        /// <summary>
        /// Tries to find a Keyword entity with text matching keywordText. If found, that Keyword
        /// entity is returned; otherwise, a new keyword will be created, added to the database,
        /// and returned.
        /// </summary>
        /// <param name="keywordText">The keyword text to find or to place into a new Keyword entity</param>
        /// <param name="context">The database context to operate on</param>
        /// <returns>The existing or newly added Keyword entity joined with the PostKeywords reference</returns>
        public static async Task<Keyword> GetOrCreateKeywordAsync(string keywordText, BlogDBContext context)
        {
            if (string.IsNullOrWhiteSpace(keywordText) || context is null)
            {
                throw new ArgumentNullException();
            }

            keywordText = keywordText.ToLower().Trim();

            Keyword keyword = await context.Keyword.Include(k => k.PostKeywords)
                                                   .FirstOrDefaultAsync(k => k.Text == keywordText);

            // If the keyword does not exist then create it and add it
            if (keyword is null)
            {
                keyword = new Keyword() { Text = keywordText };
                await context.Keyword.AddAsync(keyword);
                // TODO(taylorjoshuaw): This shouldn't be needed!
                await context.SaveChangesAsync();
            }

            return keyword;
        }

        /// <summary>
        /// Tries to find a PostKeyword relational table entity for the specified keyword and post id's.
        /// If the relation does not currently exist, a new relational table entity is generated,
        /// added to the database, and returned; otherwise, that relational table entity is simply returned
        /// to the caller. Does not commit changes to the database (call context.SaveChanges{Async} when ready)
        /// Returns null if the specified keyword or post does not exist on the database.
        /// </summary>
        /// <param name="keywordId">The primary key for the keyword</param>
        /// <param name="postId">The primary key for the post</param>
        /// <param name="context">The database context to operate on</param>
        /// <returns>A new or existing PostKeyword relational entity (null if either Id does not exist
        /// on the database</returns>
        public static async Task<PostKeyword> GetOrCreatePostKeywordAsync(int keywordId, int postId, BlogDBContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            PostKeyword postKeyword = await context.PostKeyword.Include(pk => pk.Keyword)
                                                               .Include(pk => pk.Post)
                                                               .FirstOrDefaultAsync(pk => pk.PostId == postId && pk.KeywordId == keywordId);

            // If the relationship does not exist yet, then generate it
            if (postKeyword is null)
            {
                if (await context.Post.FindAsync(postId) is null ||
                    await context.Keyword.FindAsync(keywordId) is null)
                {
                    return null;
                }

                postKeyword = new PostKeyword()
                {
                    // TODO(taylorjoshuaw): Add check to make sure these id's exist
                    KeywordId = keywordId,
                    PostId = postId
                };

                await context.AddAsync(postKeyword);
                // TODO(taylorjoshuaw): This shouldn't be needed!
                await context.SaveChangesAsync();
            }

            return postKeyword;
        }

        /// <summary>
        /// Merges all comma-separated keywords into the provided postId, creating any Keyword entities that do not yet
        /// exist. Avoids generating duplicate entries into any of the affected tables. Commits all changes to the database.
        /// </summary>
        /// <param name="keywordString">String of comma-separated keywords' text</param>
        /// <param name="postId">The post ID to merge the keywords into</param>
        /// <param name="context">The database context to operate on</param>
        /// <returns>Task (async)</returns>
        public static async Task MergeKeywordStringIntoPostAsync(string keywordString, int postId, BlogDBContext context)
        {
            if (string.IsNullOrWhiteSpace(keywordString) || context is null)
            {
                throw new ArgumentNullException();
            }

            List<Keyword> keywords = new List<Keyword>();

            foreach (string token in keywordString.Split(','))
            {
                keywords.Add(await GetOrCreateKeywordAsync(token, context));
            }

            // Commit the new keyword entities (if there are any)
            await context.SaveChangesAsync();

            foreach (Keyword keyword in keywords)
            {
                await GetOrCreatePostKeywordAsync(keyword.Id, postId, context);
            }

            // Commit any added PostKeyword relational entities
            await context.SaveChangesAsync();
        }
    }
}
