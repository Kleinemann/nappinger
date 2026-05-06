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
    public static class SOUNDS
    {
        public static readonly AudioStream WALK_1 = GD.Load<AudioStream>("res://assets/sounds/walk/Footsteps_Dirt_01.ogg");
        public static readonly AudioStream WALK_2 = GD.Load<AudioStream>("res://assets/sounds/walk/Footsteps_Dirt_02.ogg");
        public static readonly AudioStream WALK_3 = GD.Load<AudioStream>("res://assets/sounds/walk/Footsteps_Dirt_03.ogg");
        public static readonly AudioStream WALK_4 = GD.Load<AudioStream>("res://assets/sounds/walk/Footsteps_Dirt_04.ogg");

        public static readonly AudioStream HIT_1 = GD.Load<AudioStream>("res://assets/sounds/walk/GS1_Hit_1.ogg");
        public static readonly AudioStream HIT_2 = GD.Load<AudioStream>("res://assets/sounds/walk/GS1_Hit_2.ogg");
        public static readonly AudioStream HIT_3 = GD.Load<AudioStream>("res://assets/sounds/walk/GS1_Hit_3.ogg");
        public static readonly AudioStream HIT_4 = GD.Load<AudioStream>("res://assets/sounds/walk/GS1_Hit_4.ogg");

        public static readonly AudioStream COLLECT = GD.Load<AudioStream>("res://assets/sounds/walk/Collect.ogg");
        public static readonly AudioStream Pressed = GD.Load<AudioStream>("res://assets/sounds/walk/BtnPresed.ogg");
    } 

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