# Project A.M.A.N.D.A.

## Web App

The web application consists of a frontend written in Razor views, HTML, CSS,
Bootstrap, Popper, and jQuery. An interface is provided to create new blog
posts, view existing blog posts, edit existing blog posts, delete existing
blog posts, and search by both keywords and usernames. All blog posts can be
enriched using Azure Language Services (part of Microsoft's Cognitive Services
suite), Bing Image API, and Parallel Dots (for automated tagging of posts via
key phrases detected within the post's body). Image enrichments can be added
based on the overall sentiment score (a range 0.0 - 1.0 related to the mood
of the post) and key phrases / keywords detected in the posts. Optionally, users
can choose to opt-out of these features for privacy or data collection concerns.

## Tools Used
Microsoft Visual Studio Community Version 15.5.7

C#

ASP.Net Core

xUnit

Bootstrap

Azure

## Getting Started

Clone this repository to your local machine.
```
$ git clone https://github.com/ProjectAMANDA/AmandaFE.git
```
Once downloaded, cd into the ```AmandaFE``` directory.
```
$ cd AmandFE
```
The cd into ```AmandaFE``` directory.
```
$ cd AmandaFE
```
The cd into the second ```AmandaFE``` directory.
```
$ cd AmandaFE
```
Then run .NET build.
```
$ dotnet build
```
Once that is complete, run the program.
```
$ dotnet run
```

## Usage

### Overview of Recent Posts
![Overview of Recent Posts](/assets/overview.png)

### Creating a Post
![Post Creation](/assets/create.png)

### Enriching a Post
![Enriching Post](/assets/enrich.png)

### Viewing Post Details
![Details of Post](/assets/details.png)

### Searching by User
![Searching by User](/assets/user.png)

### Searching by User and Keyword
![Searcing by User and Keyword](/assets/user_keyword.png)

## Data Flow (Frontend, Backend, REST API)

![Data Flow Diagram](/assets/Flowchart.png)

## Data Model

### Overall Project Schema

![Database Schema](/assets/ERD.png)

### Blog

| Parameter | Type | Required |
| --- | --- | --- |
| ID  | int | YES |
| Summary | string | YES |
| Content | string | YES |
| Tags | string(s) | NO |
| Picture | img jpeg/png | NO |
| Sentiment | float | NO |
| Keywords | string(s) | NO |
| Related Posts | links | NO |
| Date | date/time object | YES |


### User

| Parameter | Type | Required |
| --- | --- | --- |
| ID  | int | YES |
| Name/Author | string | YES |
| Posts | list | YES |

