using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AudioLoader : MonoBehaviour
{
    public UnityEngine.UI.Text statusText;
    public UnityEngine.UI.InputField url;
    public AudioSource source;
 
    void Start()
    {
        
    }

    public void LoadAudio()
    {
      StartCoroutine(LoadHelper(url.text));
    }

    /* bad, no mp3 support at runtime apparently
    IEnumerator LoadHelper(string uri)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                statusText.text = www.error;
            }
            else
            {
                statusText.text = "loaded?";
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                source.clip = myClip;
            }
        }
    }
    */

    /* MP3Stream (LGPL so not compatible with how Unity builds)
    IEnumerator LoadHelper(string uri)
    {
        statusText.text = "Loading...";
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError)
        {
            statusText.text = www.error;
        }
        else
        {
          statusText.text = "loaded";
          // Or retrieve results as binary data
          byte[] results = www.downloadHandler.data;
          var memStream = new System.IO.MemoryStream(results);
          var mp3Stream = new MP3Sharp.MP3Stream(memStream);

          var audioDataStream = new System.IO.MemoryStream();
          byte[] buffer = new byte[4096];
          int bytesReturned = -1;
   
          while( bytesReturned != 0 )
          {
              bytesReturned = mp3Stream.Read(buffer, 0, buffer.Length);
              audioDataStream.Write(buffer, 0, bytesReturned);
          }
   
          byte[] audioData = audioDataStream.ToArray();
   
          float[] floatArray = new float[audioData.Length / 2];
          for(int i = 0; i < floatArray.Length; ++i)
          {
              floatArray[i] = (float)(System.BitConverter.ToInt16(audioData, i * 2 ) / 32768.0f);
          }
   
          var clip = AudioClip.Create("foo", floatArray.Length, 2, mp3Stream.Frequency, false);
          clip.SetData(floatArray, 0);
          source.clip = clip;
        }
    }
    */

    IEnumerator LoadHelper(string uri)
    {
        statusText.text = "Loading...";
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError)
        {
            statusText.text = www.error;
        }
        else
        {
          statusText.text = "loaded";
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

    void Update()
    {
        if (!source.isPlaying && source.clip != null && source.clip.loadState == AudioDataLoadState.Loaded)
        {
            source.Play();
        }
    }
}
