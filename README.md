# Project A.M.A.N.D.A.

## Web App



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

## Models

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

