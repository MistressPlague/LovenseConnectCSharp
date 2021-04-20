using Newtonsoft.Json.Linq; // Tools > NuGet Package Manager > Package Manager Console > Install-Package Newtonsoft.Json
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// The Namespace Of The Lovense Connect API; Created By Plague.
/// </summary>

namespace LovenseConnectAPI
{
    /// <summary>
    /// The Main Class Of The Lovense Connect API; Created By Plague.
    /// </summary>
    [Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
    public class Main
    {
        /// <summary>
        /// Information About The Lovense Toy, Held In A Convenient Package.
        /// </summary>
        public class LovenseToy
        {
            #region Debug Info
            public string FirmwareVersion = "0";
            public string ToyVersion = "";
            #endregion

            #region Useful Info
            public string LovenseToyType = "Unknown";

            public string BatteryPercentage = "0%";
            public bool ToyStatus = false;

            public string ToyID = "";

            public string ToyNickName = "";
            #endregion
        }

        /// <summary>
        /// The Local WebClient To Send Requests.
        /// </summary>
        private static WebClient client = new WebClient();

        /// <summary>
        /// Toys Caching For Lovense To Function Efficiently.
        /// </summary>
        public static List<LovenseToy> Toys = new List<LovenseToy>();

        /// <summary>
        /// Gets A List Of Toys Connected To This Local Lovense Connect Server URL.
        /// </summary>
        /// <param name="url">The Local Lovense Connect Server URL.</param>
        /// <returns>A List Of Toys Connected To This Local Lovense Connect Server URL.</returns>
        public static async Task<List<LovenseToy>> GetToys(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            if (client == null)
            {
                client = new WebClient();
            }

            IsRequestPending = true;

            var response = await client.DownloadStringTaskAsync(new Uri(url.ToLower().Replace("/gettoys", "") + "/GetToys"));

            IsRequestPending = false;

            if (!response.ToLower().Contains("\"ok\""))
            {
                return null;
            }

            if (response == "{}")
            {
                return null;
            }

            JObject JsonData = JObject.Parse(response);

            if (JsonData.GetValue("code").ToString() != "200" || JsonData.GetValue("type").ToString().ToLower() != "ok")
            {
                return null;
            }

            List<LovenseToy> _Toys = new List<LovenseToy>();

            JObject Data = JObject.Parse(JsonData.GetValue("data").ToString());

            foreach (JProperty toyid in Data.Properties())
            {
                string NickName = toyid.Value.Value<string>("nickName");

                string FirmwareVersion = toyid.Value.Value<string>("fVersion");

                string Type = toyid.Value.Value<string>("name");

                string ToyID = toyid.Value.Value<string>("id");
                string BatteryPercentage = toyid.Value.Value<string>("battery") + "%";
                string ToyVersion = toyid.Value.Value<string>("version");
                bool ToyStatus = toyid.Value.Value<string>("status") != "0";

                _Toys.Add(new LovenseToy()
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

            Toys = _Toys;

            return _Toys;
        }

        /// <summary>
        /// Gets A Instance Of LovenseToy Which Contains Info About The Toy From The URL And ID.
        /// </summary>
        /// <param name="url">The Local Lovense Connect Server URL.</param>
        /// <param name="id">The Toy ID - Fun Fact: This Is The Device's MAC Address.</param>
        /// <returns>The LovenseToy Instance Containing Info About The Toy, Such As Battery Percentage, Type, Etc.</returns>
        public static async Task<LovenseToy> GetToyInfoFromID(string url, string id)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(id))
            {
                return null;
            }

            return (await GetToys(url)).First(o => o.ToyID == id);
        }

        /// <summary>
        /// The Current Lovense Amount Of All Toy IDs.
        /// </summary>
        private static Dictionary<string, int> CurrentLovenseAmount = new Dictionary<string, int>();

        /// <summary>
        /// Is A Lovense Request In Progress?
        /// </summary>
        public static bool IsRequestPending = false;

        /// <summary>
        /// The Last Known Latency Recorded.
        /// </summary>
        public static int LastKnownLatency = 225;

        /// <summary>
        /// Stopwatch Used For Calculating Latency.
        /// </summary>
        public static Stopwatch DelayWatch = new Stopwatch();

        /// <summary>
        /// A Simple "Vibrate This Much" Method Which Will Vibrate The Toy {x} Amount From The Local Lovense Connect Server URL And ID Of The Toy.
        /// </summary>
        /// <param name="url">The Local Lovense Connect Server URL.</param>
        /// <param name="id">The Toy ID - Fun Fact: This Is The Device's MAC Address.</param>
        /// <param name="amount">The Vibration Intensity.</param>
        /// <param name="IgnoreDuplicateRequests">Whether To Ignore Duplicate Requests Or Not.</param>
        /// <returns></returns>
        public static async Task<bool> VibrateToy(string url, string id, int amount, bool IgnoreDuplicateRequests = false)
        {
            try
            {
                if (IgnoreDuplicateRequests)
                {
                    if (CurrentLovenseAmount != null)
                    {
                        if (CurrentLovenseAmount.ContainsKey(id))
                        {
                            if (CurrentLovenseAmount[id] == amount)
                            {
                                return false;
                            }
                        }
                    }
                }

                if (IsRequestPending)
                {
                    return false;
                }

                if (amount > 20)
                {
                    amount = 20;
                }

                if (client == null)
                {
                    client = new WebClient();
                }

                DelayWatch.Reset();

                if (Toys == null || Toys.Count == 0)
                {
                    return false;
                }

                LovenseToy toy = Toys.Find(o => o.ToyID == id);

                IsRequestPending = true;

                if (!DelayWatch.IsRunning)
                {
                    DelayWatch.Start();
                }

                if (toy.LovenseToyType == "Unknown")
                {
                    IsRequestPending = false;
                    return false; // Assume Toy Disconnected
                }

                var response = await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate?v=" + amount + "&t=" + toy.ToyID);

                if (!string.IsNullOrEmpty(response))
                {
                    if (!response.ToLower().Contains("\"ok\""))
                    {
                        IsRequestPending = false;

                        return false;
                    }
                }

                if (toy.LovenseToyType.ToLower().Contains("edge"))
                {
                    await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate1?v=" + amount + "&t=" + toy.ToyID);

                    response = await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate2?v=" + amount + "&t=" + toy.ToyID);

                    if (!string.IsNullOrEmpty(response))
                    {
                        if (!response.ToLower().Contains("\"ok\""))
                        {
                            IsRequestPending = false;

                            return false;
                        }
                    }
                }
                else
                {
                    response = await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate?v=" + amount + "&t=" + toy.ToyID);

                    if (!string.IsNullOrEmpty(response))
                    {
                        if (!response.ToLower().Contains("\"ok\""))
                        {
                            IsRequestPending = false;

                            return false;
                        }
                    }
                }

                if (toy.LovenseToyType.ToLower().Contains("max"))
                {

                    response = await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/AirAuto?v=" + RangeConv(amount, 0, 20, 0, 3) + "&t=" + toy.ToyID);
                }
                else if (toy.LovenseToyType.ToLower().Contains("nora"))
                {

                    response = await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/Rotate?v=" + amount + "&t=" + toy.ToyID);
                }

                if (DelayWatch.IsRunning)
                {
                    DelayWatch.Stop();

                    LastKnownLatency = (int)DelayWatch.Elapsed.TotalMilliseconds;
                }

                IsRequestPending = false;

                if (!string.IsNullOrEmpty(response))
                {
                    if (response.ToLower().Contains("\"ok\""))
                    {
                        if (CurrentLovenseAmount != null)
                        {
                            CurrentLovenseAmount[id] = amount;
                        }

                        IsRequestPending = false;

                        return true;
                    }
                }

                IsRequestPending = false;

                return false;
            }
            catch
            {
                IsRequestPending = false;

                return false;
            }
        }

        /// <summary>
        /// A Simple "Vibrate This Much" Method Which Will Vibrate The Toy {x} Amount; Iterating Through The Amounts List Per The Delay From The Local Lovense Connect Server URL And ID Of The Toy.
        /// </summary>
        /// <param name="url">The Local Lovense Connect Server URL.</param>
        /// <param name="id">The Toy ID - Fun Fact: This Is The Device's MAC Address.</param>
        /// <param name="amounts">The Vibration Intensitys.</param>
        /// <param name="DelayBetweenAmountsMilliseconds">The Delay Between Lovenses Defined In The Amounts.</param>
        /// <returns></returns>
        public static async Task<bool> VibrateToyWithPattern(string url, string id, List<int> amounts, float DelayBetweenAmountsMilliseconds = 200)
        {
            try
            {
                if (IsRequestPending)
                {
                    return false;
                }

                if (client == null)
                {
                    client = new WebClient();
                }

                if (Toys == null || Toys.Count == 0)
                {
                    return false;
                }

                IsRequestPending = true;

                var toy = Toys.Find(o => o.ToyID == id);

                foreach (int amount in amounts)
                {
                    DelayWatch.Reset();

                    var response = await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate?v=" + amount + "&t=" + toy.ToyID);

                    if (!string.IsNullOrEmpty(response))
                    {
                        if (!response.ToLower().Contains("\"ok\""))
                        {
                            IsRequestPending = false;

                            return false;
                        }
                    }

                    if (toy.LovenseToyType.ToLower().Contains("edge"))
                    {
                        await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate1?v=" + amount + "&t=" + toy.ToyID);

                        response = await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate2?v=" + amount + "&t=" + toy.ToyID);

                        if (!string.IsNullOrEmpty(response))
                        {
                            if (!response.ToLower().Contains("\"ok\""))
                            {
                                IsRequestPending = false;

                                return false;
                            }
                        }
                    }
                    else
                    {
                        response = await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate?v=" + amount + "&t=" + toy.ToyID);

                        if (!string.IsNullOrEmpty(response))
                        {
                            if (!response.ToLower().Contains("\"ok\""))
                            {
                                IsRequestPending = false;

                                return false;
                            }
                        }
                    }

                    if (toy.LovenseToyType.ToLower().Contains("max"))
                    {
                        response = await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/AirAuto?v=" + RangeConv(amount, 0, 20, 0, 3) + "&t=" + toy.ToyID);
                    }
                    else if (toy.LovenseToyType.ToLower().Contains("nora"))
                    {
                        response = await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/Rotate?v=" + amount + "&t=" + toy.ToyID);
                    }

                    if (DelayWatch.IsRunning)
                    {
                        DelayWatch.Stop();

                        LastKnownLatency = (int)DelayWatch.Elapsed.TotalMilliseconds;
                    }

                    if (!string.IsNullOrEmpty(response))
                    {
                        if (!response.ToLower().Contains("\"ok\""))
                        {
                            IsRequestPending = false;

                            return false;
                        }
                    }

                    CurrentLovenseAmount[id] = amount;

                    if (amounts.Count > 1)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(DelayBetweenAmountsMilliseconds));
                    }
                }

                IsRequestPending = false;

                return true;
            }
            catch
            {
                IsRequestPending = false;

                return false;
            }
        }

        public static int RangeConv(float input, float MinPossibleInput, float MaxPossibleInput, float MinConv, float MaxConv)
        {
            return Convert.ToInt32((((input - MinPossibleInput) * (MaxConv - MinConv)) / (MaxPossibleInput - MinPossibleInput)) + MinConv);
        }
    }
}
