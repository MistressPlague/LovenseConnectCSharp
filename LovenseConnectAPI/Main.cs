using Newtonsoft.Json.Linq; // Tools > NuGet Package Manager > Package Manager Console > Install-Package Newtonsoft.Json
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The Namespace Of The Lovense Connect API; Created By Plague.
/// </summary>
namespace Real_Feel.LovenseConnectAPI
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
            public string ToyVersion = "";
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
        /// The Local WebClient To Send Requests.
        /// </summary>
        private static WebClient client = new WebClient();

        /// <summary>
        /// Toys Caching For Vibration To Function Efficiently.
        /// </summary>
        private static List<LovenseToy> Toys = new List<LovenseToy>();

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
                client = new WebClient();
            }

            string response = await client.DownloadStringTaskAsync(new Uri(url.ToLower().Replace("/gettoys", "") + "/GetToys"));

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

            if (_Toys != null)
            {
                Toys = _Toys;

                return _Toys;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets A Instance Of LovenseToy Which Contains Info About The Toy From The URL And ID.
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
                client = new WebClient();
            }

            string response = await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/GetToys");

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

        /// <summary>
        /// Is A Vibration Request In Progress?
        /// </summary>
        private static bool IsRequestPending = false;

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
        public async static Task<bool> VibrateToy(string url, string id, int amount, bool IgnoreDuplicateRequests = false)
        {
            try
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

                string response = null;

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

                if (toy.LovenseToyType == ToyType.None)
                {
                    IsRequestPending = false;
                    return false; // Assume Toy Disconnected
                }

                response = await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate?v=" + amount + "&t=" + toy.ToyID);

                if (!string.IsNullOrEmpty(response))
                {
                    if (!response.ToLower().Contains("\"ok\""))
                    {
                        CurrentVibrationAmount[id] = amount;

                        IsRequestPending = false;

                        return false;
                    }
                }

                if (toy.LovenseToyType == ToyType.Edge)
                {
                    await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate1?v=" + amount + "&t=" + toy.ToyID);

                    response = await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate2?v=" + amount + "&t=" + toy.ToyID);

                    if (!string.IsNullOrEmpty(response))
                    {
                        if (!response.ToLower().Contains("\"ok\""))
                        {
                            CurrentVibrationAmount[id] = amount;

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
                            CurrentVibrationAmount[id] = amount;

                            IsRequestPending = false;

                            return false;
                        }
                    }
                }

                if (toy.LovenseToyType == ToyType.Max)
                {
                    //response = await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/AirIn?&t=" + toy.ToyID);
                }
                else if (toy.LovenseToyType == ToyType.Nora)
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
                        CurrentVibrationAmount[id] = amount;

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
        /// <param name="DelayBetweenAmountsMilliseconds">The Delay Between Vibrations Defined In The Amounts.</param>
        /// <returns></returns>
        public async static Task<bool> VibrateToyWithPattern(string url, string id, List<int> amounts, float DelayBetweenAmountsMilliseconds = 200)
        {
            try
            {
                if (client == null)
                {
                    client = new WebClient();
                }

                string response = null;

                if (Toys == null || Toys.Count == 0)
                {
                    return false;
                }

                IsRequestPending = true;

                LovenseToy toy = Toys.Find(o => o.ToyID == id);

                foreach (int amount in amounts)
                {
                    DelayWatch.Reset();

                    response = await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate?v=" + amount + "&t=" + toy.ToyID);

                    if (!string.IsNullOrEmpty(response))
                    {
                        if (!response.ToLower().Contains("\"ok\""))
                        {
                            CurrentVibrationAmount[id] = amount;

                            IsRequestPending = false;

                            return false;
                        }
                    }

                    if (toy.LovenseToyType == ToyType.Edge)
                    {
                        await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate1?v=" + amount + "&t=" + toy.ToyID);

                        response = await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/Vibrate2?v=" + amount + "&t=" + toy.ToyID);

                        if (!string.IsNullOrEmpty(response))
                        {
                            if (!response.ToLower().Contains("\"ok\""))
                            {
                                CurrentVibrationAmount[id] = amount;

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
                                CurrentVibrationAmount[id] = amount;

                                IsRequestPending = false;

                                return false;
                            }
                        }
                    }

                    if (toy.LovenseToyType == ToyType.Max)
                    {
                        //response = await client.DownloadStringTaskAsync(url.ToLower().Replace("/gettoys", "") + "/AirIn?&t=" + toy.ToyID);
                    }
                    else if (toy.LovenseToyType == ToyType.Nora)
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
                            CurrentVibrationAmount[id] = amount;

                            IsRequestPending = false;

                            return false;
                        }
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(DelayBetweenAmountsMilliseconds));
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
    }
}
