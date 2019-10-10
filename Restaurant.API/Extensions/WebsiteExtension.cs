using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Restaurant.Common.Constants;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Restaurant.API.Extensions
{
    public static class WebsiteExtension
    {
        private static readonly string[] VietnameseSigns =
        {

            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"

        };

        public static string Slug(string value)
        {
            //First to lower case
            value = _stripVietnameseSigns(value.ToLowerInvariant());
            //Remove all accents
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);
            value = Encoding.ASCII.GetString(bytes);
            //Replace spaces
            value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);
            //Remove invalid chars
            value = Regex.Replace(value, @"[^\w\s\p{Pd}]", "", RegexOptions.Compiled);
            //Trim dashes from end
            value = value.Trim('-', '_');
            //Replace double occurences of - or \_
            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);
            return value;
        }

        private static string _stripVietnameseSigns(string str)
        {
            for (var i = 1; i < VietnameseSigns.Length; i++)
            {
                for (var j = 0; j < VietnameseSigns[i].Length; j++)
                {
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
                }
            }

            return str;
        }

        public static List<T> ConvertToList<T>(this IEnumerable<object[]> records) where T : class, new()
        {
            try
            {
                var list = new List<T>();

                foreach (var item in records)
                {
                    var obj = Activator.CreateInstance<T>();
                    var properties = obj.GetType().GetProperties();
                    for (var i = 0; i < properties.Length; i++)
                    {
                        try
                        {
                            var propertyInfo = obj.GetType().GetProperty(properties[i].Name);
                            var t = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                            var safeValue = (item[i] == null) ? null : Convert.ChangeType(item[i], t);
                            propertyInfo.SetValue(obj, safeValue, null);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }
        public static string EncryptPassword(string sPassword)
        {
            var md5Hasher = new MD5CryptoServiceProvider();
            var hashedDataBytes = md5Hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(sPassword));
            var sEncryptPass = Convert.ToBase64String(hashedDataBytes);
            return sEncryptPass;
        }

        public static string ConvertDate(string date)
        {
            var dateConvert = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                        .ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            return dateConvert;
        }
        public static bool CheckRightBeforeAction(int curRestaurantId, int curBranchId, int modelRestaurantId, int modelBranchId)
        {
            var result = false;

            if (curRestaurantId == Constant.ADMIN_RES_AND_BRANCH_DEFAULT && curBranchId == Constant.ADMIN_RES_AND_BRANCH_DEFAULT)
            {
                result = true;
            }
            else if (curRestaurantId > 0 && curBranchId == 0)
            {
                if (curRestaurantId == modelRestaurantId)
                {
                    result = true;
                }
            }
            else if (curRestaurantId > 0 && curBranchId > 0)
            {
                if (curRestaurantId == modelRestaurantId && curBranchId == modelBranchId)
                {
                    result = true;
                }
            }

            return result;
        }
        //public static bool CheckPermission(string permissions)
        //{
        //    bool au = false;
        //    string[] roles = permissions.Split(',');
        //    List<string> privilegeLevels = new List<string>();
        //    var typeId = HttpContext.Current.Session["TypeId"].ToString();
        //    List<Admin_Group_Permission_View00> Per = Admin_Group_Permission_View00.Query("Where GroupId=@0 AND Status=1", typeId).ToList();

        //    if (Per.Any())
        //    {
        //        privilegeLevels = Per.Select(c => c.PermissionIdName.ToString()).ToList();
        //    }
        //    foreach (var r in roles)
        //    {
        //        if (privilegeLevels.Contains(r))
        //        {
        //            au = true;
        //        }
        //    }
        //    if (Convert.ToInt32(HttpContext.Current.Session["UserID"]) == 1)
        //    {
        //        au = true;
        //    }
        //    return au;
        //}
    }
}
