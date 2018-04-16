# Project A.M.A.N.D.A
## Acquiring Media And Nascient Data API

### Components
- Original web app with database
- Custom API with database
- Related topics API
- Language analysis API
- Bing image search API

### Description
The project’s intent is to allow a user to create small blog or snipit posts that will then be matched with similar topic data, an appropriate picture, and key words. The first iteration will have posts stored locally. A future goal will be to have posts pushed to social media sites.

The web app will handle user input, blog post storage, and display. It will also reach out to and display results from the related topics API.

The custom backend API will handle the calls to the Azure language analysis API which will return the sentiment and key words of a blog post. It will also store a series of sanitized images that can be used to match both keywords and sentiment of the blog posts from the web app. The custom API can be scaled to utilize the Bing search API to return a series of more random but possibly less safe images. These images can be used with the set already stored to provider a wider range of matches for blog posts.

### Flow
When a user lands on the page they are shown a list of previous posts. When they choose to create a new post they are taken to a text field page. Once their entry is complete, a call is made to the custom backend API. The custom API will pass the blog post through the Azure Text Analytics API. The results will then be matched with a relevant image based on sentiment or keywords and sent back to the web app. Once received, the frontend application will make a call to the related topics API using the keywords parsed from the custom API. It will amend the results as well as the results from the custom API to the blog post and display it for the user.
