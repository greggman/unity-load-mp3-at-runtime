# Example of loading mp3 at runtime in Unity

I am not a unity expert but apparently Unity doesn't
load mp3s at runtime.

This example uses the [NLayer library](https://github.com/naudio/NLayer) which is just enough to decode an mp3.

Note: It does not stream mp3s. Instead, first the entire mp3 file will be downloaded. It will then be converted
to raw audio data. That data is then passed to Unity.

The relevant code is in `AudioLoader.cs`

```
IEnumerator LoadHelper(string uri)
{
    UnityWebRequest www = UnityWebRequest.Get(uri);
    yield return www.SendWebRequest();

    if(www.isNetworkError || www.isHttpError)
    {
        Debug.Log(www.error);
    }
    else
    {
      // Or retrieve results as binary data
      byte[] results = www.downloadHandler.data;
      var memStream = new System.IO.MemoryStream(results);
      var mpgFile = new NLayer.MpegFile(memStream);
      var samples = new float[mpgFile.Length];
      mpgFile.ReadSamples(samples, 0, (int)mpgFile.Length);

      var clip = AudioClip.Create("foo", samples.Length, mpgFile.Channels, mpgFile.SampleRate, false);
      clip.SetData(samples, 0);
      source.clip = clip;
    }
}
```

Tested on Windows and on [a WebGL build](https://greggman.github.io/unity-load-mp3-at-runtime/) in 2019.4.1. (man I want to punch Unity in the face for that stupid naming! 😆)

## Notes

1. I have no idea what the implications of mp3 licensing are

2. The NLayer code is licensed under the MIT License as is this example

3. I originally tried the [MP3Stream library](https://github.com/ZaneDubya/MP3Sharp). 
   It works but it is [LGPL](https://github.com/ZaneDubya/MP3Sharp/blob/master/license.txt)
   which AFAICT is entirely incompatible with how Unity works. The LGPL requires
   linking at runtime via dynamic libraries. Unity doesn't support that. Instead
   it re-builds the library into your app. So, you can't ship a game made with
   Unity using the MP3Sharp library unless you either (a) license your game as
   LGPL or GPL... not sure that's even possible or (b) you find some creative way to keep
   the MP3Stream code separate and load it at runtime.
