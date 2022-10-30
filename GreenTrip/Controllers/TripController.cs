using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using GreenTrip.Models;
using System.Drawing;
using System.Web.UI.WebControls;
using static GreenTrip.Models.Estimatation;
using System.Web.Security;

namespace GreenTrip.Controllers
{
    [Authorize]
    public class TripController : Controller
    {
        GreenTripEntities2 DB = new GreenTripEntities2();
        // GET: Index
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return View();
        }
        [AllowAnonymous, HttpPost]
        public ActionResult Login(string Id, string Pwd)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = DB.User.Where(m=>m.accountName==Id&& m.password== Pwd).FirstOrDefault();
            if (user == null)
            {
                ViewBag.Err = "帳號或密碼錯誤，請再試一次!";
                return View();
            }
            else
            {

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1,
                user.nickName,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(90),
                isPersistent: false,//關閉所有瀏覽器後cookies更新
                userData: user.nickName,
                cookiePath: FormsAuthentication.FormsCookiePath
                );
                string encTicket = FormsAuthentication.Encrypt(ticket);

                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket)
                {
                    Secure = true,
                    HttpOnly = true
                };

                Response.Cookies.Add(cookie);
                //當成全域變數用
                Session["userId"] = user.userId;
                Session["userName"] = user.nickName;
                Session["userSex"] = user.sex;
            }

            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [AllowAnonymous, HttpPost]
        public ActionResult Register(User user, string secPwd)//註冊
        {
            if (ModelState.IsValid)
            {
                if (user.password != secPwd)
                {
                    ViewBag.Err = "兩組密碼不相符!!";
                    return View();
                }
                var temp = DB.User.Where(m => m.accountName == user.accountName).FirstOrDefault();

                if (temp != null)
                {
                    ViewBag.Err = "帳號已經被註冊過了!!";
                }
                else
                {
                    var lastUser = DB.User.OrderByDescending(m => m.userId).FirstOrDefault();
                    var userId = (Convert.ToInt32(lastUser.userId) + 1).ToString();
                    var tempId = "";
                    int totalLen = 6;//userId允許的最大長度 000001
                    for(int i=0; i < totalLen-userId.Length; i++)
                    {
                        tempId=tempId+ "0";
                    }
                    tempId = tempId + userId;
                    user.userId = tempId;
                    user.signUpTime= DateTime.Now;
                    DB.User.Add(user);
                    DB.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
        }
        public ActionResult Form()
        {
            var CountyList = DB.County.ToList();
            ViewBag.CountyList = CountyList;
            return View();
        }


        public ActionResult Place()
        {
            int CityCount = DB.City
                .Where(m => m.countyId == "014")
                .Count();
            var CityList = DB.City
                .Where(m => m.countyId == "014")
                .Select(m => m.cityId)
                .ToList();

            var CityNameList = DB.City
                .Where(m => m.countyId == "014")
                .Select(m => m.cityName)
                .ToList();

            var CityIdList = DB.City
            .Where(m => m.countyId == "014")
            .Select(m => m.cityId)
            .ToList();

            ViewBag.CityNameList = CityNameList;
            ViewBag.CityIdList = CityIdList;

            var Locations = DB.Site
                .Where(m => CityIdList.Contains(m.cityId))
                .ToList();

            return View(Locations);
        }
        public async Task<ActionResult> Path()
        {
            //宣告區域
            var foodList = DB.Site.ToList();
            ViewBag.foodList = foodList;

            string[] address = DB.Site
                .Select(m => m.address)
                .ToArray();

            String[] TravelAddressList = { "台南市中西區大德街38號", "台南市中西區環河街129巷27號2樓",
            "台南市中西區民族路三段13號", "台南市中西區慈聖街32號", "台南市中西區忠義路二段1號",
            "台南市東區林森路一段276號","台南市東區長榮路一段234巷17號","台南市東區府東街122號",
            "台南市北區北門路二段15巷17號","台南市中西區中山路79巷6號","台南市中西區府前路一段87號"};

            String[] TravelList = { "鼎富發豬油拌飯", "UBUNTU烏邦圖書店",
            "富盛號", "北山雜貨", "臺南市美術館2館", "大東夜市",
            "a Room房間咖啡","桑原商店",
            "KOEMON","旭峯號","台南民宿 二弄八號"};

            string[] XYaddresses = TravelAddressList;

            string[] foodName = DB.Site
                .Select(m => m.siteName)
                .ToArray();

            string[] XYName = TravelList;
            string apiKey = "AIzaSyAk3dGWQh8y5rnhNoG91SBesiTvYQUrjj0";
            string[] travel_Modes = { "driving ", "walking", "bicycling" };
            HttpClient httpClient = new HttpClient();
            httpClient.MaxResponseContentBufferSize = Int32.MaxValue;
            //倆倆距離程式區域

            //marker程式專用
            Location location = new Location();
            List<Location> locations = new List<Location>();
            for (var i = 0; i < XYaddresses.Length; i++)
            {
                string showXY = $"https://maps.googleapis.com/maps/api/geocode/json?address={XYaddresses[i]}&key={apiKey}";
                httpClient = new HttpClient();
                httpClient.MaxResponseContentBufferSize = Int32.MaxValue;
                var response1 = await httpClient.GetStringAsync(showXY);
                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(response1);
                location = myDeserializedClass.results[0].geometry.location; //經緯度等於location.lat&lng
                location.name = XYName[i];
                locations.Add(location);
            }
            List<Models.Estimatation.estimatation> estimatations = new List<Models.Estimatation.estimatation>();
            for (int j = 0; j < locations.Count() - 1; j++)
            {
                string showDistance = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={locations[j].lat},{locations[j + 1].lng}&destinations={locations[j].lat},{locations[j].lng}&mode={travel_Modes[1]}&key={apiKey}";
                httpClient = new HttpClient();
                httpClient.MaxResponseContentBufferSize = Int32.MaxValue;
                var response2 = await httpClient.GetStringAsync(showDistance);
                Root2 myDeserializedClass = JsonConvert.DeserializeObject<Root2>(response2);
                Models.Estimatation.estimatation estimation = new Models.Estimatation.estimatation();
                estimation.distance = myDeserializedClass.rows[0].elements[0].distance.text;
                estimation.duration = myDeserializedClass.rows[0].elements[0].duration.text;
                estimatations.Add(estimation);
            }
            Estimation_Path estimation_Path = new Estimation_Path();
            estimation_Path.estimatations = estimatations;
            estimation_Path.locations = locations;
            return View(estimation_Path);
        }
    }
}