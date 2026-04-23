using Godot;
using Godot.Collections;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using HttpClient = System.Net.Http.HttpClient;

public static class Tools
{
    public static Vector2 GetGeoLocation()
    {
        string ip = "84.137.162.144";
        //string ip = "127.0.0.1";
        string url = $"http://ip-api.com/json/{ip}";

        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                string s = response.Content.ReadAsStringAsync().Result;
                GeoData v = JsonSerializer.Deserialize<GeoData>(s);
                return new Vector2(v.lat, v.lon);
            }
        }
        catch (Exception ex)
        {
            GD.Print(ex.Message);
            return Vector2.Zero;
        }
    }


    public class GeoData
    {
        public string status { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string region { get; set; }
        public string regionName { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
        public string timezone { get; set; }
        public string isp { get; set; }
        public string org { get; set; }
        public string _as { get; set; }
        public string query { get; set; }
    }
}