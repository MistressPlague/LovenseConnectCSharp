using Newtonsoft.Json.Linq; // Tools > NuGet Package Manager > Package Manager Console > Install-Package Newtonsoft.Json
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

/// <summary>
/// The Namespace Of The Lovense Connect API; Created By Plague.
/// </summary>
namespace Lovense_VRChat_Tool.LovenseConnectAPI
{
    /// <summary>
    /// The Main Class Of The Lovense Connect API; Created By Plague.
    /// </summary>
    public class Main
    {
        /// <summary>
        /// Information About The Lovense Toy, Held In A Convenient Package.
        /// </summary>
        public class LovenseToy
        {
            #region Debug Info
            public string FirmwareVersion = "0";
            public string ToyVersion = "0";
            #endregion

            #region Useful Info
            public ToyType LovenseToyType = ToyType.None;

            public string BatteryPercentage = "0%";
            public bool ToyStatus = false;

            public string ToyID = "";

            public string ToyNickName = "";
            #endregion
        }

        /// <summary>
        /// The Type Of Toy - Returns The Name Which Is Assigned A Number.
        /// </summary>
        public enum ToyType
        {
            None = 0,
            Ambi = 1,
            Domi = 2,
            Edge = 3,
            Hush = 4,
            Lush = 5,
            Max = 6,
            Mission = 7,
            Nora = 8,
            Osci = 9,
            Ferri = 10, // Unreleased - Single Motor Based
            Diamo = 11, // Unreleased - Single Motor Based
            Quake = 12 // Unreleased - Single Motor Based
        }

        /// <summary>
        /// The Local HttpClient To Send HTTP GET/POST Requests.
        /// </summary>
        private static HttpClient client = new HttpClient();

        /// <summary>
        /// Gets A List Of Toys Connected To This Local Lovense Connect Server URL.
        /// </summary>
        /// <param name="url">The Local Lovense Connect Server URL.</param>
        /// <returns>A List Of Toys Connected To This Local Lovense Connect Server URL.</returns>
        public async static Task<List<LovenseToy>> GetToys(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            if (client == null)
            {
                client = new HttpClient();
            }

            HttpResponseMessage response = await client.GetAsync(url.ToLower().Replace("/gettoys", "") + "/GetToys");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string RawData = await response.Content.ReadAsStringAsync();

            if (RawData == "{}")
            {
                return null;
            }

            JObject JsonData = JObject.Parse(RawData);

            if (JsonData.GetValue("code").ToString() != "200" || JsonData.GetValue("type").ToString().ToLower() != "ok")
            {
                return null;
            }

            List<LovenseToy> Toys = new List<LovenseToy>();

            JObject Data = JObject.Parse(JsonData.GetValue("data").ToString());

            foreach (JProperty toyid in Data.Properties())
            {
                if (toyid == null)
                {
                    continue;
                }

                string NickName = toyid.Value.Value<string>("nickName");

                string FirmwareVersion = toyid.Value.Value<string>("fVersion");

                ToyType Type = (ToyType)Enum.Parse(typeof(ToyType), toyid.Value.Value<string>("name"));

                string ToyID = toyid.Value.Value<string>("id");
                string BatteryPercentage = toyid.Value.Value<string>("battery") + "%";
                string ToyVersion = toyid.Value.Value<string>("version");
                bool ToyStatus = toyid.Value.Value<string>("status") == "0" ? false : true;

                Toys.Add(new LovenseToy()
                {
                    #region Debug Info
                    FirmwareVersion = FirmwareVersion,
                    ToyVersion = ToyVersion,
                    #endregion

                    #region Useful Info
                    LovenseToyType = Type,

                    BatteryPercentage = BatteryPercentage,
                    ToyStatus = ToyStatus,

                    ToyID = ToyID,

                    ToyNickName = NickName
                    #endregion
                });
            }

            if (Toys != null)
            {
                return Toys;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets A Instance Of LovenseToy Which Contains Info About The Toy From The url.ToLower().Replace("/gettoys", "")And ID.
        /// </summary>
        /// <param name="url">The Local Lovense Connect Server URL.</param>
        /// <param name="id">The Toy ID - Fun Fact: This Is The Device's MAC Address.</param>
        /// <returns>The LovenseToy Instance Containing Info About The Toy, Such As Battery Percentage, Type, Etc.</returns>
        public async static Task<LovenseToy> GetToyInfoFromID(string url, string id)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(id))
            {
                return null;
            }

            if (client == null)
            {
                client = new HttpClient();
            }

            HttpResponseMessage response = await client.GetAsync(url.ToLower().Replace("/gettoys", "") + "/GetToys");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string RawData = await response.Content.ReadAsStringAsync();

            if (RawData == "{}")
            {
                return null;
            }

            JObject JsonData = JObject.Parse(RawData);

            if (JsonData.GetValue("code").ToString() != "200" || JsonData.GetValue("type").ToString().ToLower() != "ok")
            {
                return null;
            }

            List<LovenseToy> Toys = new List<LovenseToy>();

            JObject Data = JObject.Parse(JsonData.GetValue("data").ToString());

            foreach (JProperty toyid in Data.Properties())
            {
                if (toyid == null)
                {
                    continue;
                }

                string NickName = toyid.Value.Value<string>("nickName");

                string FirmwareVersion = toyid.Value.Value<string>("fVersion");

                ToyType Type = (ToyType)Enum.Parse(typeof(ToyType), toyid.Value.Value<string>("name"));

                string ToyID = toyid.Value.Value<string>("id");
                string BatteryPercentage = toyid.Value.Value<string>("battery") + "%";
                string ToyVersion = toyid.Value.Value<string>("version");
                bool ToyStatus = toyid.Value.Value<string>("status") == "0" ? false : true;

                return new LovenseToy()
                {
                    #region Debug Info
                    FirmwareVersion = FirmwareVersion,
                    ToyVersion = ToyVersion,
                    #endregion

                    #region Useful Info
                    LovenseToyType = Type,

                    BatteryPercentage = BatteryPercentage,
                    ToyStatus = ToyStatus,

                    ToyID = ToyID,

                    ToyNickName = NickName
                    #endregion
                };
            }

            return null;
        }

        /// <summary>
        /// The Current Vibration Amount Of All Toy IDs.
        /// </summary>
        private static Dictionary<string, int> CurrentVibrationAmount = new Dictionary<string, int>();

        public static int LastKnownLatency = 175;

        /// <summary>
        /// A Simple "Vibrate This Much" Method Which Will Vibrate The Toy {x} Amount From The Local Lovense Connect Server url.ToLower().Replace("/gettoys", "")And ID Of The Toy.
        /// </summary>
        /// <param name="url">The Local Lovense Connect Server URL.</param>
        /// <param name="id">The Toy ID - Fun Fact: This Is The Device's MAC Address.</param>
        /// <param name="amount">The Vibration Intensity.</param>
        /// <param name="IgnoreDuplicateRequests">Whether To Ignore Duplicate Requests Or not.</param>
        /// <returns></returns>
        public async static Task<bool> VibrateToy(string url, string id, int amount, bool IgnoreDuplicateRequests = false)
        {
            if (IgnoreDuplicateRequests)
            {
                if (CurrentVibrationAmount != null)
                {
                    if (CurrentVibrationAmount.ContainsKey(id))
                    {
                        if (CurrentVibrationAmount[id] == amount)
                        {
                            return false;
                        }
                    }
                }
            }

            if (amount > 20)
            {
                amount = 20;
            }

            LovenseToy toy = await GetToyInfoFromID(url, id);

            if (client == null)
            {
                client = new HttpClient();
            }

            HttpResponseMessage response = null;

            Stopwatch DelayWatch = new Stopwatch();

            switch (toy.LovenseToyType)
            {
                case ToyType.None:
                    return false; // Assume Toy Disconnected
                case ToyType.Ambi:
                    if (!DelayWatch.IsRunning)
                    {
                        DelayWatch.Start();
                    }

                    response = await client.PostAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate?v=" + amount + "&t=" + toy.ToyID, null);
                    break;
                case ToyType.Domi:
                    if (!DelayWatch.IsRunning)
                    {
                        DelayWatch.Start();
                    }

                    response = await client.PostAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate?v=" + amount + "&t=" + toy.ToyID, null);
                    break;
                case ToyType.Edge:
                    if (!DelayWatch.IsRunning)
                    {
                        DelayWatch.Start();
                    }

                    await client.PostAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate1?v=" + amount + "&t=" + toy.ToyID, null);

                    response = await client.PostAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate2?v=" + amount + "&t=" + toy.ToyID, null);
                    break;
                case ToyType.Hush:
                    if (!DelayWatch.IsRunning)
                    {
                        DelayWatch.Start();
                    }

                    response = await client.PostAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate?v=" + amount + "&t=" + toy.ToyID, null);
                    break;
                case ToyType.Lush:
                    if (!DelayWatch.IsRunning)
                    {
                        DelayWatch.Start();
                    }

                    response = await client.PostAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate?v=" + amount + "&t=" + toy.ToyID, null);
                    break;
                case ToyType.Max:
                    if (!DelayWatch.IsRunning)
                    {
                        DelayWatch.Start();
                    }

                    await client.PostAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate?v=" + amount + "&t=" + toy.ToyID, null);

                    response = await client.PostAsync(url.ToLower().Replace("/gettoys", "") + "/AirIn?&t=" + toy.ToyID, null);
                    break;
                case ToyType.Mission:
                    if (!DelayWatch.IsRunning)
                    {
                        DelayWatch.Start();
                    }

                    response = await client.PostAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate?v=" + amount + "&t=" + toy.ToyID, null);
                    break;
                case ToyType.Nora:
                    if (!DelayWatch.IsRunning)
                    {
                        DelayWatch.Start();
                    }

                    await client.PostAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate?v=" + amount + "&t=" + toy.ToyID, null);

                    response = await client.PostAsync(url.ToLower().Replace("/gettoys", "") + "/Rotate?v=" + amount + "&t=" + toy.ToyID, null);
                    break;
                case ToyType.Osci:
                    if (!DelayWatch.IsRunning)
                    {
                        DelayWatch.Start();
                    }

                    response = await client.PostAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate?v=" + amount + "&t=" + toy.ToyID, null);
                    break;
                case ToyType.Ferri:
                    if (!DelayWatch.IsRunning)
                    {
                        DelayWatch.Start();
                    }

                    response = await client.PostAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate?v=" + amount + "&t=" + toy.ToyID, null);
                    break;
                case ToyType.Diamo:
                    if (!DelayWatch.IsRunning)
                    {
                        DelayWatch.Start();
                    }

                    response = await client.PostAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate?v=" + amount + "&t=" + toy.ToyID, null);
                    break;
                case ToyType.Quake:
                    if (!DelayWatch.IsRunning)
                    {
                        DelayWatch.Start();
                    }

                    response = await client.PostAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate?v=" + amount + "&t=" + toy.ToyID, null);
                    break;
            }

            if (DelayWatch.IsRunning)
            {
                DelayWatch.Stop();

                LastKnownLatency = (int)DelayWatch.Elapsed.TotalMilliseconds;
            }

            if (response != null)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    CurrentVibrationAmount[id] = amount;

                    return true;
                }
            }

            return false;
        }
    }
}
