1. I've spent roughly 9 hours on the coding assignment. If I had more time, I would work on the front-end to make the look&feel better. And manage exception messages in the front-end in a more detailed way. Furthermore, I would implement more unit tests to test each and every possible response from the api.
2. I used .Net Core 3.1 but I did not use any new feature added in this version. But I want to mention that I used a feature that was introduced in .Net Core 2.1 which is IHttpClientFactory.
	It prevents wrong usages for HttpClient.
3. I beleive the key to track down performance issues is "logging". In a past project, after the delivery, the client reported performance issues which they stated was caused by our poor api performence and server was reponding too slow. By the help of the detailed logs I keep, I could easly wathched the time taken for each request. And this made it clear that we had no performance issues in our services. The issue was caused by a bottleneck in clients network infrastructure.
4. Latest technical book I read is "CLR Via C# (4Th Edition) By Jeffrey Richter". I got the book only to read the "Threading" topic. But while I was there I read half the book. I learnt what's going on behind the scenes, how CLR manages threads, and I refreshed valuable knowledge on value types and reference types.
5. I had fun implementing the solution. I beleive it's both short enough not the overwhelm someone working in another job, and also complex enough for developer to show her/his skills in the technology they choose.
6.
 {
  "name": "ilker",
  "surname": "benli",
  "dateOfBirth": "1988-03-18T00:15:00.000Z",
  "age": 31,
  "gender": "male",
  "email": "ilkerbenli@yahoo.com",
  "mobile": "+905336128743",
  "maritialStatus": "married",
  "nationality": "Turkish",
  "hometown": "Ankara",
  "addresses": {
    "home": {
      "disctrict": "Karapinar Mah.",
      "street": "1186. Cad.",
      "town": "Ankara"
    },
    "work": {
      "disctrict": "Ivedik Osb Mah.",
      "street": "2224. Cad.",
      "town": "Ankara"
    }
  },
  "occupation": "Computer Enginner",
  "professionalTitle": "Senior Software Developer",
  "hobbies": [
    "Motorcyles",
    "Travelling"
  ]

}