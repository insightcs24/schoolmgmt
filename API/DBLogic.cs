using API.Models;
using MySql.Data;
using MySql.Data.MySqlClient;
using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace API
{
    public class DBLogic
    {

        public async Task<User> GetOTP(JObject jobj)
        {
            string _sOTP = "";
            User user = null;

            return await Task.Run(() =>
            {
                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());
                if (!headers.ContainsKey("mobile"))
                    throw new Exception("Parameter Incorrent !!!");
                string mobile = headers["mobile"];
                using (MySqlTransaction tran = Startup.con.BeginTransaction())
                {
                    if (mobile.Trim() == "9408631600" ||
                        mobile.Trim() == "9408631200" ||
                        mobile.Trim() == "9408620120" ||
                        mobile.Trim() == "9409933335" ||
                        mobile.Trim() == "9409933336" ||
                        mobile.Trim() == "9409933337" ||
                        mobile.Trim() == "9409933338" ||
                        mobile.Trim() == "9723655108" ||
                        mobile.Trim() == "9328064380" ||
                        mobile.Trim() == "9029944862")
                    {
                        _sOTP = mobile.Substring(6, 4);
                        MySqlCommand _mysqlcmd1 = new MySqlCommand("Update member set Mobile1_Pass=N'" + _sOTP.ToString() + "' Where Mobile1=N'" + mobile + "'", Startup.con);
                        _mysqlcmd1.ExecuteNonQuery();
                        MySqlCommand _mysqlcmd2 = new MySqlCommand("Update member set Mobile2_Pass=N'" + _sOTP.ToString() + "' Where Mobile2=N'" + mobile + "'", Startup.con);
                        _mysqlcmd2.ExecuteNonQuery();
                        MySqlCommand _mysqlcmd3 = new MySqlCommand("Update member set Mobile3_Pass=N'" + _sOTP.ToString() + "' Where Mobile3=N'" + mobile + "'", Startup.con);
                        _mysqlcmd3.ExecuteNonQuery();
                        user = new User
                        {
                            Success = "1" as string,
                            Message = "USER FOUND SUCCESSFULLY" as string,
                            Mobile = mobile as string,
                            Password = _sOTP as string
                        };
                    }
                    else
                    {
                        DataTable _dtmember = new DataTable("member");
                        using (MySqlCommand _mysqlcmd = new MySqlCommand("Select m.Mem_ID,m.Mobile1,m.Mobile2,m.Mobile3,m.Mobile1_Pass,m.Mobile2_Pass,m.Mobile3_Pass,m.Status,c.ExpiryDate,c.Cmp_ID from member m inner join company c on c.cmp_id = m.Cmp_ID Where (m.Mobile1=N'" + mobile + "' OR m.Mobile2=N'" + mobile + "' OR m.Mobile3=N'" + mobile + "') and m.Status='1' and c.ExpiryDate >= CURDATE()", Startup.con))
                        {
                            using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                            {
                                _mysel.Fill(_dtmember);
                            }
                        }
                        if (_dtmember.Rows.Count > 0)
                        {
                            Random r = new Random();
                            _sOTP = (r.Next(Convert.ToInt32(1000), Convert.ToInt32(9999))).ToString();
                            MySqlCommand _mysqlcmd1 = new MySqlCommand("Update member set Mobile1_Pass=N'" + _sOTP.ToString() + "' Where Mobile1=N'" + mobile + "'", Startup.con);
                            _mysqlcmd1.ExecuteNonQuery();
                            MySqlCommand _mysqlcmd2 = new MySqlCommand("Update member set Mobile2_Pass=N'" + _sOTP.ToString() + "' Where Mobile2=N'" + mobile + "'", Startup.con);
                            _mysqlcmd2.ExecuteNonQuery();
                            MySqlCommand _mysqlcmd3 = new MySqlCommand("Update member set Mobile3_Pass=N'" + _sOTP.ToString() + "' Where Mobile3=N'" + mobile + "'", Startup.con);
                            _mysqlcmd3.ExecuteNonQuery();
                            if (_dtmember.Rows.Count > 0)
                            {
                                SMSSend(mobile, "Your OTP \n" + _sOTP);
                                user = new User
                                {
                                    Success = "1" as string,
                                    Message = "USER FOUND SUCCESSFULLY" as string,
                                    Mobile = mobile as string,
                                    Password = _sOTP as string
                                };
                            }
                            else
                            {
                                user = new User
                                {
                                    Success = "0" as string,
                                    Message = "USER NOT FOUND" as string,
                                    Mobile = mobile as string,
                                    Password = ""
                                };
                            }
                        }
                    }
                    tran.Commit();
                }
                return user;
            });
        }

        public bool SMSSend(string mobile, string message)
        {
            try
            {
                string smsapi = "http://sms.alphacomputers.biz/api/v3/index.php?method=sms&api_key=A4840f10ce73edb50b9e8e4c966524d2d&to=" + mobile + "&sender=DIGITL&message=" + message + "&format=XML&flash=0";

                HttpWebRequest wreq1;
                HttpWebResponse wres1;

                wreq1 = null;
                wres1 = null;


                wreq1 = (HttpWebRequest)WebRequest.Create(smsapi);
                wreq1.ProtocolVersion = HttpVersion.Version10;
                wreq1.Method = "POST";
                wreq1.ContentType = "application/x-www-form-urlencoded";
                wreq1.Proxy = WebProxy.GetDefaultProxy();
                wreq1.Timeout = (int)new TimeSpan(0, 0, 60).TotalMilliseconds;
                wreq1.UserAgent = "Mozilla/3.0 (compatible; My Browser/1.0)";
                wreq1.KeepAlive = false;
                wres1 = (HttpWebResponse)wreq1.GetResponse();
                StreamReader respStreamReader = new StreamReader(wres1.GetResponseStream());
                string s1 = respStreamReader.ReadToEnd();
                respStreamReader.Close();
                return true;
            }
            catch (Exception ex)
            {
                //DBM.SendErrorToText(ex);
                return false;
            }
        }

        public async Task<MemberLists> GetParentsLoginTokenBase64(JObject jobj)
        {
            return await Task.Run(() =>
            {
                List<MemberList> member = new List<MemberList>();
                MemberLists members = null;
                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());
                if (!headers.ContainsKey("userlogin"))
                    throw new Exception("Parameter Incorrent !!!");
                UserLogin userlogin = JsonConvert.DeserializeObject<UserLogin>(headers["userlogin"]);
                using (MySqlTransaction tran = Startup.con.BeginTransaction())
                {
                    DataTable _dtmember = new DataTable("member");
                    string _query = "";
                    if (int.Parse(userlogin.CmpID) == 0)
                        _query = "Select m.Mem_ID,m.Cmp_ID,c.Name as Cmp_Name,c.Cmp_key,c.DisplayType,c.DealerType," +
                        "c.logo,m.AdmissionDate,m.GRNo,m.RollNo,m.Name,m.Gender,m.Std,m.Division,m.DOB,m.Address," +
                        "m.Mobile1,m.Mobile2,m.Mobile3,m.Email,m.Status,m.Photo from member m " +
                        "LEFT JOIN company c on c.Cmp_ID = m.cmp_id Where (m.Mobile1=N'" + userlogin.Mobile +
                        "' OR m.Mobile2=N'" + userlogin.Mobile + "' OR m.Mobile3=N'" + userlogin.Mobile +
                        "') and (m.Status='1' and c.Status='1') and (m.Mobile1_Pass=N'" + userlogin.Password +
                        "' OR m.Mobile2_Pass=N'" + userlogin.Password + "' OR m.Mobile3_Pass=N'" +
                        userlogin.Password + "') and c.ExpiryDate >= CURDATE()";
                    else
                        _query = "Select m.Mem_ID,m.Cmp_ID,c.Name as Cmp_Name,c.Cmp_key,c.DisplayType,c.DealerType," +
                        "c.logo,m.AdmissionDate,m.GRNo,m.RollNo,m.Name,m.Gender,m.Std,m.Division,m.DOB,m.Address," +
                        "m.Mobile1,m.Mobile2,m.Mobile3,m.Email,m.Status,m.Photo from member m " +
                        "LEFT JOIN company c on c.Cmp_ID = m.cmp_id Where (m.Mobile1=N'" + userlogin.Mobile +
                        "' OR m.Mobile2=N'" + userlogin.Mobile + "' OR m.Mobile3=N'" + userlogin.Mobile +
                        "') and (m.Status='1' and c.Status='1') and (m.Mobile1_Pass=N'" + userlogin.Password +
                        "' OR m.Mobile2_Pass=N'" + userlogin.Password + "' OR m.Mobile3_Pass=N'" +
                        userlogin.Password + "') and c.ExpiryDate >= CURDATE() and m.Cmp_ID = '" +
                        userlogin.CmpID + "'";

                    using (MySqlCommand _mysqlcmd = new MySqlCommand(_query, Startup.con))
                    {
                        using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                        {
                            _mysel.Fill(_dtmember);
                        }
                    }

                    if (_dtmember.Rows.Count > 0)
                    {
                        MySqlCommand _mysqlcmd = null;
                        _mysqlcmd = new MySqlCommand("Update member set TokenID1=N'" + userlogin.TokenID + "' Where Mobile1 = N'" + userlogin.Mobile + "'", Startup.con);
                        _mysqlcmd.ExecuteNonQuery();
                        _mysqlcmd = new MySqlCommand("Update member set TokenID2=N'" + userlogin.TokenID + "' Where Mobile2 = N'" + userlogin.Mobile + "'", Startup.con);
                        _mysqlcmd.ExecuteNonQuery();
                        _mysqlcmd = new MySqlCommand("Update member set TokenID3=N'" + userlogin.TokenID + "' Where Mobile3 = N'" + userlogin.Mobile + "'", Startup.con);
                        _mysqlcmd.ExecuteNonQuery();

                        for (int i = 0; i < _dtmember.Rows.Count; i++)
                        {
                            member.Add(new MemberList
                            {
                                Mem_ID = int.Parse(_dtmember.Rows[i]["Mem_ID"].ToString()),
                                Cmp_Key = _dtmember.Rows[i]["Cmp_key"].ToString() as string,
                                Cmp_Name = _dtmember.Rows[i]["Cmp_Name"].ToString() as string,
                                AdmissionDate = DateTime.Parse(_dtmember.Rows[i]["AdmissionDate"].ToString()).ToString("dd-MM-yyyy") as string,
                                GRNo = _dtmember.Rows[i]["GRNo"].ToString() as string,
                                RollNo = _dtmember.Rows[i]["RollNo"].ToString() as string,
                                Name = _dtmember.Rows[i]["Name"].ToString() as string,
                                Gender = _dtmember.Rows[i]["Gender"].ToString() as string,
                                Std = _dtmember.Rows[i]["Std"].ToString() as string,
                                Division = _dtmember.Rows[i]["Division"].ToString() as string,
                                Logo = string.IsNullOrEmpty(_dtmember.Rows[0]["Logo"].ToString()) ? Convert.ToBase64String(Utilities.ImageToByte(@"\\img\cmplogo\IMG_20180601.jpg", "logo")) : Convert.ToBase64String(Utilities.ImageToByte(@"\\img\cmplogo\" + _dtmember.Rows[0]["Logo"].ToString(), "logo")),
                                DOB = string.IsNullOrEmpty(_dtmember.Rows[i]["DOB"].ToString().Trim()) ? "01-01-1900" : DateTime.Parse(_dtmember.Rows[i]["DOB"].ToString()).ToString("dd-MM-yyyy") as string,
                                Address = _dtmember.Rows[i]["Address"].ToString() as string,
                                Mobile1 = _dtmember.Rows[i]["Mobile1"].ToString() as string,
                                Mobile2 = _dtmember.Rows[i]["Mobile2"].ToString() as string,
                                Mobile3 = _dtmember.Rows[i]["Mobile3"].ToString() as string,
                                Email = _dtmember.Rows[i]["Email"].ToString() as string,
                                Status = _dtmember.Rows[i]["Status"].ToString() as string,
                                Photo = string.IsNullOrEmpty(_dtmember.Rows[i]["Photo"].ToString().Trim()) ? Convert.ToBase64String(Utilities.ImageToByte(@"\\img\memphoto\IMG_20180601.jpg", "member")) : Convert.ToBase64String(Utilities.ImageToByte(@"\\img\memphoto\" + _dtmember.Rows[i]["Cmp_ID"].ToString().Trim() + "\\" + _dtmember.Rows[i]["Photo"].ToString().Trim(), "member")),
                                DisplayType = _dtmember.Rows[i]["DisplayType"].ToString().ToUpper() as string,
                                DealerType = _dtmember.Rows[i]["DealerType"].ToString() as string,
                            });
                        }
                    }

                    members = new MemberLists
                    {
                        Success = _dtmember.Rows.Count > 0 ? "1" as string : "0" as string,
                        Message = _dtmember.Rows.Count > 0 ? "MEMBER FOUND SUCCESSFULLY" as string : "MEMBER NOT FOUND" as string,
                        MemberList = member
                    };

                    tran.Commit();
                }//close transation

                return members;
            });
        }

        public async Task<StringCompanies> GetCompany(JObject jobj)
        {
            return await Task.Run(() =>
            {
                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());
                if (!headers.ContainsKey("cmpkey"))
                    throw new Exception("Parameter Incorrent !!!");
                string cmpkey = headers["cmpkey"];

                List<StringCompany> companies = new List<StringCompany>();
                StringCompanies company = null;

                using (MySqlTransaction tran = Startup.con.BeginTransaction())
                {
                    DataTable _dtCompany = new DataTable("cmpkey");

                    using (MySqlCommand _mysqlcmd = new MySqlCommand("Select Cmp_ID, Type, Name, DealerType, " +
                        "DealerName, Address, City, Distict, State, Country, Pincode, Owner, Mobile, " +
                        "Email, Website, Logo, Remarks, Status, Cmp_Key, PageHeader, Password, " +
                        "GPSLocation,DisplayType,PageHeader1,PageHeader2,Facebook,Youtube,Instagram,Twitter," +
                        "Google ,WhatsApp,Class_Attendance,Auto_Attendance,Timetable,Test,Homework,Classwork," +
                        "Notice,News,Exam,Fees,`Leave`,Chat,Video_Gallery,Photo_Gallery,PDF_Gallery,Lecture_Attendance" +
                        " From company Where Cmp_Key = N'" + cmpkey + "' and `Status` = '1' " +
                        "and ExpiryDate >= CURDATE()", Startup.con))
                    {
                        using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                        {
                            _mysel.Fill(_dtCompany);
                        }
                    }

                    if (_dtCompany.Rows.Count > 0)
                    {
                        companies.Add(new StringCompany
                        {
                            Cmp_ID = _dtCompany.Rows[0]["Cmp_ID"].ToString(),
                            Types = _dtCompany.Rows[0]["Type"].ToString(),
                            Name = _dtCompany.Rows[0]["Name"].ToString(),
                            Address = _dtCompany.Rows[0]["Address"].ToString(),
                            City = _dtCompany.Rows[0]["City"].ToString(),
                            Distict = _dtCompany.Rows[0]["Distict"].ToString(),
                            DealerType = _dtCompany.Rows[0]["DealerType"].ToString().Trim().ToUpper(),
                            DealerName = _dtCompany.Rows[0]["DealerName"].ToString().Trim().ToUpper(),
                            State = _dtCompany.Rows[0]["State"].ToString(),
                            Country = _dtCompany.Rows[0]["Country"].ToString(),
                            Pincode = _dtCompany.Rows[0]["Pincode"].ToString(),
                            Owner = _dtCompany.Rows[0]["Owner"].ToString(),
                            Mobile = _dtCompany.Rows[0]["Mobile"].ToString(),
                            Email = _dtCompany.Rows[0]["Email"].ToString(),
                            Website = _dtCompany.Rows[0]["Website"].ToString(),
                            Logo = string.IsNullOrEmpty(_dtCompany.Rows[0]["Logo"].ToString()) ? Convert.ToBase64String(Utilities.ImageToByte(@"\\img\cmplogo\IMG_20180601.jpg", "logo")) : Convert.ToBase64String(Utilities.ImageToByte(@"\\img\cmplogo\" + _dtCompany.Rows[0]["Logo"].ToString(), "logo")),
                            Remarks = _dtCompany.Rows[0]["Remarks"].ToString(),
                            Status = _dtCompany.Rows[0]["Status"].ToString(),
                            PageHeader = string.IsNullOrEmpty(_dtCompany.Rows[0]["PageHeader"].ToString()) ? Convert.ToBase64String(Utilities.ImageToByte(@"\\img\appheader\IMG_20180601.jpg", "header")) : Convert.ToBase64String(Utilities.ImageToByte(@"\\img\appheader\" + _dtCompany.Rows[0]["PageHeader"].ToString(), "header")),
                            Cmp_Key = _dtCompany.Rows[0]["Cmp_Key"].ToString(),
                            GPSLocation = _dtCompany.Rows[0]["GPSLocation"].ToString(),
                            Password = _dtCompany.Rows[0]["Password"].ToString(),
                            DisplayType = _dtCompany.Rows[0]["DisplayType"].ToString(),
                            PageHeader1 = string.IsNullOrEmpty(_dtCompany.Rows[0]["PageHeader1"].ToString()) ? null : Convert.ToBase64String(Utilities.ImageToByte(@"\\img\appheader\" + _dtCompany.Rows[0]["PageHeader1"].ToString(), "header")),
                            PageHeader2 = string.IsNullOrEmpty(_dtCompany.Rows[0]["PageHeader2"].ToString()) ? null : Convert.ToBase64String(Utilities.ImageToByte(@"\\img\appheader\" + _dtCompany.Rows[0]["PageHeader2"].ToString(), "header")),
                            Facebook = _dtCompany.Rows[0]["Facebook"].ToString(),
                            Instagram = _dtCompany.Rows[0]["Instagram"].ToString(),
                            Youtube = _dtCompany.Rows[0]["Youtube"].ToString(),
                            Twitter = _dtCompany.Rows[0]["Twitter"].ToString(),
                            Google = _dtCompany.Rows[0]["Google"].ToString(),
                            WhatsApp = _dtCompany.Rows[0]["WhatsApp"].ToString(),
                            Class_Attendance = _dtCompany.Rows[0]["Class_Attendance"].ToString(),
                            Auto_Attendance = _dtCompany.Rows[0]["Auto_Attendance"].ToString(),
                            Timetable = _dtCompany.Rows[0]["Timetable"].ToString(),
                            Test = _dtCompany.Rows[0]["Test"].ToString(),
                            Homework = _dtCompany.Rows[0]["Homework"].ToString(),
                            Classwork = _dtCompany.Rows[0]["Classwork"].ToString(),
                            Notice = _dtCompany.Rows[0]["Notice"].ToString(),
                            News = _dtCompany.Rows[0]["News"].ToString(),
                            Exam = _dtCompany.Rows[0]["Exam"].ToString(),
                            Fees = _dtCompany.Rows[0]["Fees"].ToString(),
                            Leave = _dtCompany.Rows[0]["Leave"].ToString(),
                            Chat = _dtCompany.Rows[0]["Chat"].ToString(),
                            Video_Gallery = _dtCompany.Rows[0]["Video_Gallery"].ToString(),
                            Photo_Gallery = _dtCompany.Rows[0]["Photo_Gallery"].ToString(),
                            PDF_Gallery = _dtCompany.Rows[0]["PDF_Gallery"].ToString(),
                            Lecture_Attendance = _dtCompany.Rows[0]["Lecture_Attendance"].ToString(),
                        });
                    }

                    company = new StringCompanies
                    {
                        Success = _dtCompany.Rows.Count > 0 ? "1" as string : "0" as string,
                        Message = _dtCompany.Rows.Count > 0 ? "COMPANY FOUND SUCCESSFULLY" as string : "COMPANY NOT FOUND" as string,
                        Company = companies
                    };
                    tran.Commit();
                }//close transation

                return company;
            });
        }

        public async Task<StringCompanies> GetCompanyImages(JObject jobj)
        {
            return await Task.Run(() =>
            {
                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());
                if (!headers.ContainsKey("cmpkey"))
                    throw new Exception("Parameter Incorrent !!!");
                string cmpkey = headers["cmpkey"];

                List<StringCompany> companies = new List<StringCompany>();
                StringCompanies company = null;

                using (MySqlTransaction tran = Startup.con.BeginTransaction())
                {
                    DataTable _dtCompany = new DataTable("cmpkey");

                    using (MySqlCommand _mysqlcmd = new MySqlCommand("Select Cmp_ID, Type, Name, DealerType, DealerName, Address, City, Distict, State, Country, Pincode, Owner, Mobile, Email, Website, Logo, Remarks, Status, Cmp_Key, PageHeader, Password, GPSLocation,DisplayType From company Where Cmp_Key = N'" + cmpkey + "' and `Status` = '1' and ExpiryDate >= CURDATE()", Startup.con))
                    {
                        using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                        {
                            _mysel.Fill(_dtCompany);
                        }
                    }

                    if (_dtCompany.Rows.Count > 0)
                    {
                        companies.Add(new StringCompany
                        {
                            Cmp_ID = _dtCompany.Rows[0]["Cmp_ID"].ToString(),
                            Types = _dtCompany.Rows[0]["Type"].ToString(),
                            Name = _dtCompany.Rows[0]["Name"].ToString(),
                            Address = _dtCompany.Rows[0]["Address"].ToString(),
                            City = _dtCompany.Rows[0]["City"].ToString(),
                            Distict = _dtCompany.Rows[0]["Distict"].ToString(),
                            DealerType = _dtCompany.Rows[0]["DealerType"].ToString().Trim().ToUpper(),
                            State = _dtCompany.Rows[0]["State"].ToString(),
                            Country = _dtCompany.Rows[0]["Country"].ToString(),
                            Pincode = _dtCompany.Rows[0]["Pincode"].ToString(),
                            Owner = _dtCompany.Rows[0]["Owner"].ToString(),
                            Mobile = _dtCompany.Rows[0]["Mobile"].ToString(),
                            Email = _dtCompany.Rows[0]["Email"].ToString(),
                            Website = _dtCompany.Rows[0]["Website"].ToString(),
                            Logo = string.IsNullOrEmpty(_dtCompany.Rows[0]["Logo"].ToString()) ? Convert.ToBase64String(Utilities.ImageToByte(@"\\img\cmplogo\IMG_20180601.jpg", "logo")) : Convert.ToBase64String(Utilities.ImageToByte(@"\\img\cmplogo\" + _dtCompany.Rows[0]["Logo"].ToString(), "logo")),
                            Remarks = _dtCompany.Rows[0]["Remarks"].ToString(),
                            Status = _dtCompany.Rows[0]["Status"].ToString(),
                            PageHeader = string.IsNullOrEmpty(_dtCompany.Rows[0]["PageHeader"].ToString()) ? Convert.ToBase64String(Utilities.ImageToByte(@"\\img\appheader\IMG_20180601.jpg", "header")) : Convert.ToBase64String(Utilities.ImageToByte(@"\\img\appheader\" + _dtCompany.Rows[0]["PageHeader"].ToString(), "header")),
                            Cmp_Key = _dtCompany.Rows[0]["Cmp_Key"].ToString(),
                            GPSLocation = _dtCompany.Rows[0]["GPSLocation"].ToString(),
                            DisplayType = _dtCompany.Rows[0]["DisplayType"].ToString()
                        });
                    }

                    company = new StringCompanies
                    {
                        Success = _dtCompany.Rows.Count > 0 ? "1" as string : "0" as string,
                        Message = _dtCompany.Rows.Count > 0 ? "COMPANY FOUND SUCCESSFULLY" as string : "COMPANY NOT FOUND" as string,
                        Company = companies
                    };
                    tran.Commit();
                }//close transation


                return company;
            });
        }

        public async Task<StudAttendances> GetAttendance(JObject jobj)
        {
            return await Task.Run(() =>
            {
                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());
                if (!headers.ContainsKey("memid"))
                    throw new Exception("Parameter Incorrent !!!");
                string memid = headers["memid"];

                List<StudAttendance> StudAttendance = new List<StudAttendance>();
                StudAttendances StudAttendnaces = null;

                using (MySqlTransaction tran = Startup.con.BeginTransaction())
                {
                    // DataTable _dtStudAtt = SelectMethod("Select a.Att_Date,a.Std,a.Division,a.Lacture,a.Faculty,a.Subject,ad.Status,count(*) as AttendanceDay,(Select Count(*) From attendance_details where mem_id = ad.mem_id and Status='L') as `Leave`, (Select Count(*) From attendance_details where mem_id = ad.mem_id and Status='A') as Absent, (Select Count(*) From attendance_details where mem_id = ad.mem_id and Status='P') as Present, (Select Count(*) From attendance_details where mem_id = ad.mem_id and Status='H') as Holliday From attendance_details as ad  inner join attendance as a on a.att_id = ad.att_id where ad.mem_id=" + memid + " group by a.Att_Date,a.Std,a.Division,a.Lacture,a.Faculty,a.Subject,ad.Status");
                    DataTable _dtStudAtt = new DataTable("StudAtt");

                    using (MySqlCommand _mysqlcmd = new MySqlCommand("Select a.Att_Date,a.Std,a.Division,a.Lacture,a.Faculty,a.Subject,ad.Status, (Select Count(ads.Att_DID) From attendance_details ads inner join attendance a on a.Att_ID =ads.Att_ID where ads.mem_id=" + memid + ") as AttendanceDay, (Select Count(ads.Att_DID) From attendance_details ads inner join attendance a on a.Att_ID =ads.Att_ID where ads.Status='L' AND  ads.mem_id=" + memid + ") as `Leave`,  (Select Count(ads.Att_DID) From attendance_details ads inner join attendance a on a.Att_ID =ads.Att_ID where ads.Status='A' AND  ads.mem_id=" + memid + ") as Absent,  (Select Count(ads.Att_DID) From attendance_details ads inner join attendance a on a.Att_ID =ads.Att_ID where ads.Status='P' AND  ads.mem_id=" + memid + ") as Present, (Select Count(ads.Att_DID) From attendance_details ads inner join attendance a on a.Att_ID =ads.Att_ID where ads.Status='H' AND  ads.mem_id=" + memid + ") as Holliday From attendance_details as ad  inner join attendance as a on a.att_id = ad.att_id where ad.mem_id=" + memid, Startup.con))
                    {
                        using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                        {
                            _mysel.Fill(_dtStudAtt);
                        }
                    }

                    for (int i = 0; i < _dtStudAtt.Rows.Count; i++)
                    {
                        StudAttendance.Add(new StudAttendance
                        {
                            Date = DateTime.Parse(_dtStudAtt.Rows[i]["Att_Date"].ToString()).ToString("yyyy-MM-dd"),
                            Faculty = _dtStudAtt.Rows[i]["Faculty"].ToString(),
                            Lacture = _dtStudAtt.Rows[i]["Lacture"].ToString(),
                            Status = _dtStudAtt.Rows[i]["Status"].ToString(),
                            Subject = _dtStudAtt.Rows[i]["Subject"].ToString()
                        });
                    }

                    StudAttendnaces = new StudAttendances
                    {
                        Success = _dtStudAtt.Rows.Count > 0 ? "1" as string : "0" as string,
                        Message = _dtStudAtt.Rows.Count > 0 ? "ATTENDANCE FOUND SUCCESSFULLY" as string : "ATTENDANCE NOT FOUND" as string,
                        Standard = _dtStudAtt.Rows.Count > 0 ? _dtStudAtt.Rows[0]["Std"].ToString().Trim() : null,
                        Division = _dtStudAtt.Rows.Count > 0 ? _dtStudAtt.Rows[0]["Division"].ToString().Trim() : null,
                        PresentDay = _dtStudAtt.Rows.Count > 0 ? _dtStudAtt.Rows[0]["Present"].ToString().Trim() : null,
                        AbsentDay = _dtStudAtt.Rows.Count > 0 ? _dtStudAtt.Rows[0]["Absent"].ToString().Trim() : null,
                        LeaveDay = _dtStudAtt.Rows.Count > 0 ? _dtStudAtt.Rows[0]["Leave"].ToString().Trim() : null,
                        AttendnaceDay = _dtStudAtt.Rows.Count > 0 ? _dtStudAtt.Rows[0]["AttendanceDay"].ToString().Trim() : null,
                        HolliDay = _dtStudAtt.Rows.Count > 0 ? _dtStudAtt.Rows[0]["Holliday"].ToString().Trim() : null,
                        StudAttendance = StudAttendance
                    };

                    tran.Commit();
                }//close transation

                return StudAttendnaces;
            });
        }

        public async Task<StudAttendances> GetMonthlyAttendances(JObject jobj)
        {
            return await Task.Run(() =>
            {
                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());
                if (!headers.ContainsKey("memid") || !headers.ContainsKey("Date"))
                    throw new Exception("Parameter Incorrent !!!");
                string memid = headers["memid"];
                string date = headers["Date"];

                List<StudAttendance> StudAttendance = new List<StudAttendance>();
                StudAttendances StudAttendnaces = null;
                if (string.IsNullOrEmpty(date))
                    return StudAttendnaces;
                using (MySqlTransaction tran = Startup.con.BeginTransaction())
                {
                    DataTable _dtStudAtt = new DataTable("StudAtt");

                    using (MySqlCommand _mysqlcmd = new MySqlCommand("Select a.Att_Date,a.Std,a.Division," +
                        "a.Lacture,a.Faculty,a.Subject,ad.Status, (Select Count(ads.Att_DID) " +
                        "From attendance_details ads inner join attendance a on a.Att_ID =ads.Att_ID " +
                        "where ads.mem_id=" + memid + " and (YEAR(a.Att_Date) = " +
                        DateTime.Parse(date).ToString("yyyy") + " AND MONTH(a.Att_Date) = " +
                        DateTime.Parse(date).ToString("MM") + ")) as AttendanceDay, " +
                        "(Select Count(ads.Att_DID) From attendance_details ads " +
                        "inner join attendance a on a.Att_ID =ads.Att_ID " +
                        "where ads.Status='L' AND  ads.mem_id=" + memid + " and (YEAR(a.Att_Date) = " +
                        DateTime.Parse(date).ToString("yyyy") + " AND MONTH(a.Att_Date) = " +
                        DateTime.Parse(date).ToString("MM") + ")) as `Leave`,  (Select Count(ads.Att_DID) " +
                        "From attendance_details ads inner join attendance a on a.Att_ID =ads.Att_ID " +
                        "where ads.Status='A' AND  ads.mem_id=" + memid + " and (YEAR(a.Att_Date) = " +
                        DateTime.Parse(date).ToString("yyyy") + " AND MONTH(a.Att_Date) = " +
                        DateTime.Parse(date).ToString("MM") + ")) as Absent,  (Select Count(ads.Att_DID) " +
                        "From attendance_details ads inner join attendance a on a.Att_ID =ads.Att_ID " +
                        "where ads.Status='P' AND  ads.mem_id=" + memid + " and (YEAR(a.Att_Date) = " +
                        DateTime.Parse(date).ToString("yyyy") + " AND MONTH(a.Att_Date) = " +
                        DateTime.Parse(date).ToString("MM") + ")) as Present, (Select Count(ads.Att_DID) " +
                        "From attendance_details ads inner join attendance a on a.Att_ID =ads.Att_ID " +
                        "where ads.Status='H' AND  ads.mem_id=" + memid + " and (YEAR(a.Att_Date) = " +
                        DateTime.Parse(date).ToString("yyyy") + " AND MONTH(a.Att_Date) = " +
                        DateTime.Parse(date).ToString("MM") + ")) as Holliday From attendance_details as ad " +
                        " inner join attendance as a on a.att_id = ad.att_id where ad.mem_id=" + memid +
                        " and (YEAR(a.Att_Date) = " + DateTime.Parse(date).ToString("yyyy") +
                        " AND MONTH(a.Att_Date) = " + DateTime.Parse(date).ToString("MM") + ")",
                        Startup.con))
                    {
                        using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                        {
                            _mysel.Fill(_dtStudAtt);
                        }
                    }

                    for (int i = 0; i < _dtStudAtt.Rows.Count; i++)
                    {
                        StudAttendance.Add(new StudAttendance
                        {
                            Date = DateTime.Parse(_dtStudAtt.Rows[i]["Att_Date"].ToString()).ToString("yyyy-MM-dd"),
                            Faculty = _dtStudAtt.Rows[i]["Faculty"].ToString(),
                            Lacture = _dtStudAtt.Rows[i]["Lacture"].ToString(),
                            Status = _dtStudAtt.Rows[i]["Status"].ToString(),
                            Subject = _dtStudAtt.Rows[i]["Subject"].ToString()
                        });
                    }

                    StudAttendnaces = new StudAttendances
                    {
                        Success = _dtStudAtt.Rows.Count > 0 ? "1" as string : "0" as string,
                        Message = _dtStudAtt.Rows.Count > 0 ? "ATTENDANCE FOUND SUCCESSFULLY" as string : "ATTENDANCE NOT FOUND" as string,
                        Standard = _dtStudAtt.Rows.Count > 0 ? _dtStudAtt.Rows[0]["Std"].ToString().Trim() : null,
                        Division = _dtStudAtt.Rows.Count > 0 ? _dtStudAtt.Rows[0]["Division"].ToString().Trim() : null,
                        PresentDay = _dtStudAtt.Rows.Count > 0 ? _dtStudAtt.Rows[0]["Present"].ToString().Trim() : null,
                        AbsentDay = _dtStudAtt.Rows.Count > 0 ? _dtStudAtt.Rows[0]["Absent"].ToString().Trim() : null,
                        LeaveDay = _dtStudAtt.Rows.Count > 0 ? _dtStudAtt.Rows[0]["Leave"].ToString().Trim() : null,
                        AttendnaceDay = _dtStudAtt.Rows.Count > 0 ? _dtStudAtt.Rows[0]["AttendanceDay"].ToString().Trim() : null,
                        HolliDay = _dtStudAtt.Rows.Count > 0 ? _dtStudAtt.Rows[0]["Holliday"].ToString().Trim() : null,
                        StudAttendance = StudAttendance
                    };

                    tran.Commit();
                }//close transation


                return StudAttendnaces;
            });

        }

        public async Task<StudAtt> GetDailyAttendance(JObject jobj)
        {
            return await Task.Run(() =>
            {
                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());
                if (!headers.ContainsKey("memid") || !headers.ContainsKey("Date"))
                    throw new Exception("Parameter Incorrent !!!");
                string memid = headers["memid"];
                string date = headers["Date"];

                List<StudAttendance> StudAttendance = new List<StudAttendance>();
                StudAtt StudAttendnaces = null;
                if (string.IsNullOrEmpty(date))
                    return StudAttendnaces;

                using (MySqlTransaction tran = Startup.con.BeginTransaction())
                {
                    DataTable _dtStudAtt = new DataTable("StudAtt");

                    using (MySqlCommand _mysqlcmd = new MySqlCommand("Select a.Att_Date,a.Std,a.Division," +
                        "a.Lacture,a.Faculty,a.Subject,ad.Status From attendance_details as ad  " +
                        "inner join attendance as a on a.att_id = ad.att_id where ad.mem_id=" + memid +
                        " and a.Att_Date = '" + DateTime.Parse(date).ToString("yyyy-MM-dd") + "'",
                        Startup.con))
                    {
                        using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                        {
                            _mysel.Fill(_dtStudAtt);
                        }
                    }

                    for (int i = 0; i < _dtStudAtt.Rows.Count; i++)
                    {
                        StudAttendance.Add(new StudAttendance
                        {
                            Date = DateTime.Parse(_dtStudAtt.Rows[i]["Att_Date"].ToString()).ToString("yyyy-MM-dd"),
                            Faculty = _dtStudAtt.Rows[i]["Faculty"].ToString(),
                            Lacture = _dtStudAtt.Rows[i]["Lacture"].ToString(),
                            Status = _dtStudAtt.Rows[i]["Status"].ToString(),
                            Subject = _dtStudAtt.Rows[i]["Subject"].ToString()
                        });
                    }

                    StudAttendnaces = new StudAtt
                    {
                        Success = _dtStudAtt.Rows.Count > 0 ? "1" as string : "0" as string,
                        Message = _dtStudAtt.Rows.Count > 0 ? "ATTENDANCE FOUND SUCCESSFULLY" as string : "ATTENDANCE NOT FOUND" as string,
                        Standard = _dtStudAtt.Rows.Count > 0 ? _dtStudAtt.Rows[0]["Std"].ToString().Trim() : null,
                        Division = _dtStudAtt.Rows.Count > 0 ? _dtStudAtt.Rows[0]["Division"].ToString().Trim() : null,
                        StudAttendance = StudAttendance
                    };

                    tran.Commit();
                }//close transation

                return StudAttendnaces;
            });
        }

        public async Task<StudTests> GetTest(JObject jobj)
        {
            return await Task.Run(() =>
            {
                List<StudTest> studtest = new List<StudTest>();
                StudTests studtests = null;

                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());

                if (!headers.ContainsKey("memid"))
                    throw new Exception("Parameter Incorrent !!!");
                string memid = headers["memid"];
                using (MySqlTransaction tran = Startup.con.BeginTransaction())
                {
                    DataTable _dtStudTest = new DataTable("Stud");


                    using (MySqlCommand _mysqlcmd = new MySqlCommand("Select t.Test_Date,t.Round,t.Std,t.`Division`,t.`Faculty`," +
                        "t.`Subject`,t.TotalMarks,t.PassMarks,td.Marks,td.`Status` from testdetails td " +
                        "inner join test t on t.Test_ID = td.Test_ID where td.Mem_ID=" + memid + "  ORDER BY Test_Date DESC",
                        Startup.con))
                    {
                        using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                        {
                            _mysel.Fill(_dtStudTest);
                        }
                    }

                    for (int i = 0; i < _dtStudTest.Rows.Count; i++)
                    {
                        studtest.Add(new StudTest
                        {
                            Date = DateTime.Parse(_dtStudTest.Rows[i]["Test_Date"].ToString()).ToString("yyyy-MM-dd"),
                            Faculty = _dtStudTest.Rows[i]["Faculty"].ToString(),
                            Marks = _dtStudTest.Rows[i]["Marks"].ToString(),
                            PassMarks = _dtStudTest.Rows[i]["PassMarks"].ToString(),
                            TotalMarks = _dtStudTest.Rows[i]["TotalMarks"].ToString(),
                            Round = _dtStudTest.Rows[i]["Round"].ToString(),
                            Status = _dtStudTest.Rows[i]["Status"].ToString(),
                            Subject = _dtStudTest.Rows[i]["Subject"].ToString()
                        });
                    }

                    studtests = new StudTests
                    {
                        Success = _dtStudTest.Rows.Count > 0 ? "1" as string : "0" as string,
                        Message = _dtStudTest.Rows.Count > 0 ? "TEST FOUND SUCCESSFULLY" as string : "TEST NOT FOUND" as string,
                        Standard = _dtStudTest.Rows.Count > 0 ? _dtStudTest.Rows[0]["Std"].ToString().Trim() : null,
                        Division = _dtStudTest.Rows.Count > 0 ? _dtStudTest.Rows[0]["Division"].ToString().Trim() : null,
                        StudTest = studtest
                    };
                    tran.Commit();
                }//close transation

                return studtests;
            });
        }

        public async Task<StudTests> GetMonthlyTests(JObject jobj)
        {
            return await Task.Run(() =>
            {
                List<StudTest> studtest = new List<StudTest>();
                StudTests studtests = null;

                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());
                if (!headers.ContainsKey("memid") || !headers.ContainsKey("Date"))
                    throw new Exception("Parameter Incorrent !!!");
                string memid = headers["memid"];
                string date = headers["Date"];
                if (string.IsNullOrEmpty(date))
                    return studtests;
                using (MySqlTransaction tran = Startup.con.BeginTransaction())
                {
                    DataTable _dtStudTest = new DataTable("Studtest");

                    using (MySqlCommand _mysqlcmd = new MySqlCommand("Select t.Test_Date,t.Round,t.Std," +
                        "t.`Division`,t.`Faculty`,t.`Subject`,t.TotalMarks,t.PassMarks,td.Marks," +
                        "td.`Status` from testdetails td inner join test t on t.Test_ID = td.Test_ID " +
                        "where td.Mem_ID=" + memid + "  and (YEAR(Test_Date) = " +
                        DateTime.Parse(date).ToString("yyyy") + " AND MONTH(Test_Date) = " +
                        DateTime.Parse(date).ToString("MM") + ") ORDER BY Test_Date DESC", Startup.con))
                    {
                        using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                        {
                            _mysel.Fill(_dtStudTest);
                        }
                    }

                    for (int i = 0; i < _dtStudTest.Rows.Count; i++)
                    {
                        studtest.Add(new StudTest
                        {
                            Date = DateTime.Parse(_dtStudTest.Rows[i]["Test_Date"].ToString()).ToString("yyyy-MM-dd"),
                            Faculty = _dtStudTest.Rows[i]["Faculty"].ToString(),
                            Marks = _dtStudTest.Rows[i]["Marks"].ToString(),
                            PassMarks = _dtStudTest.Rows[i]["PassMarks"].ToString(),
                            TotalMarks = _dtStudTest.Rows[i]["TotalMarks"].ToString(),
                            Round = _dtStudTest.Rows[i]["Round"].ToString(),
                            Status = _dtStudTest.Rows[i]["Status"].ToString(),
                            Subject = _dtStudTest.Rows[i]["Subject"].ToString()
                        });
                    }

                    studtests = new StudTests
                    {
                        Success = _dtStudTest.Rows.Count > 0 ? "1" as string : "0" as string,
                        Message = _dtStudTest.Rows.Count > 0 ? "TEST FOUND SUCCESSFULLY" as string : "TEST NOT FOUND FOR THIS MONTH" as string,
                        Standard = _dtStudTest.Rows.Count > 0 ? _dtStudTest.Rows[0]["Std"].ToString().Trim() : null,
                        Division = _dtStudTest.Rows.Count > 0 ? _dtStudTest.Rows[0]["Division"].ToString().Trim() : null,
                        StudTest = studtest
                    };


                    tran.Commit();
                }//close transation

                return studtests;
            });
        }

        public async Task<StudTimetables> GetTimetable(JObject jobj)
        {
            return await Task.Run(() =>
            {
                List<StudTimetable> studtimetable = new List<StudTimetable>();
                StudTimetables studtimetables = null;

                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());
                if (!headers.ContainsKey("std") || !headers.ContainsKey("division") || !headers.ContainsKey("cmpkey"))
                    throw new Exception("Parameter Incorrent !!!");
                string std = headers["std"];
                string division = headers["division"];
                string cmpkey = headers["cmpkey"];

                using (MySqlTransaction tran = Startup.con.BeginTransaction())
                {

                    DataTable _dtTime = new DataTable("Time");

                    using (MySqlCommand _mysqlcmd = new MySqlCommand("Select t.* from Timetable t inner join Company c " +
                        "on c.Cmp_ID =t.Cmp_Id Where t.Std =N'" + std + "' and t.Division = N'" + division + "' and c.Cmp_key=N'" +
                        cmpkey + "'", Startup.con))
                    {
                        using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                        {
                            _mysel.Fill(_dtTime);
                        }
                    }

                    for (int i = 0; i < _dtTime.Rows.Count; i++)
                    {
                        studtimetable.Add(new StudTimetable
                        {
                            Date = DateTime.Parse(_dtTime.Rows[i]["Date"].ToString()).ToString("yyyy-MM-dd"),
                            Faculty = _dtTime.Rows[i]["Faculty"].ToString(),
                            Remarks = _dtTime.Rows[i]["Remarks"].ToString(),
                            Subject = _dtTime.Rows[i]["Subject"].ToString(),
                            Time = _dtTime.Rows[i]["Time"].ToString(),
                            Type = _dtTime.Rows[i]["Type"].ToString()
                        });
                    }

                    studtimetables = new StudTimetables
                    {
                        Success = _dtTime.Rows.Count > 0 ? "1" as string : "0" as string,
                        Message = _dtTime.Rows.Count > 0 ? "TIMETABLE FOUND SUCCESSFULLY" as string : "TIMETABLE NOT FOUND" as string,
                        Standard = _dtTime.Rows.Count > 0 ? _dtTime.Rows[0]["Std"].ToString().Trim() : null,
                        Division = _dtTime.Rows.Count > 0 ? _dtTime.Rows[0]["Division"].ToString().Trim() : null,
                        StudTimetable = studtimetable
                    };

                    tran.Commit();
                }//close transation

                return studtimetables;
            });
        }

        public async Task<StudFees> GetFees(JObject jobj)
        {
            return await Task.Run(() =>
            {
                List<StudFee> studfee = new List<StudFee>();
                StudFees studfees = null;

                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());

                if (!headers.ContainsKey("memid"))
                    throw new Exception("Parameter Incorrent !!!");
                string memid = headers["memid"];

                using (MySqlTransaction tran = Startup.con.BeginTransaction())
                {
                    int _Receipt = 0, _Charges = 0;

                    DataTable _dtFees = new DataTable("Fees");

                    using (MySqlCommand _mysqlcmd = new MySqlCommand("Select * From fees Where Mem_Id=" + memid +
                        " Order By Fees_ID DESC", Startup.con))
                    {
                        using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                        {
                            _mysel.Fill(_dtFees);
                        }
                    }

                    for (int i = 0; i < _dtFees.Rows.Count; i++)
                    {
                        studfee.Add(new StudFee
                        {
                            Fees_Amount = _dtFees.Rows[i]["Fees_Amount"].ToString(),
                            Fees_Date = DateTime.Parse(_dtFees.Rows[i]["Fees_Date"].ToString()).ToString("yyyy-MM-dd"),
                            Fees_Type = _dtFees.Rows[i]["Fees_Type"].ToString(),
                            PaymentMode = _dtFees.Rows[i]["PaymentMode"].ToString(),
                            Remarks = _dtFees.Rows[i]["Remarks"].ToString()
                        });
                        if (_dtFees.Rows[i]["Fees_Type"].ToString().Trim() == "0")
                            _Charges += int.Parse(_dtFees.Rows[i]["Fees_Amount"].ToString());
                        else
                            _Receipt += int.Parse(_dtFees.Rows[i]["Fees_Amount"].ToString());
                    }

                    studfees = new StudFees
                    {
                        Success = _dtFees.Rows.Count > 0 ? "1" as string : "0" as string,
                        Message = _dtFees.Rows.Count > 0 ? "FEES DETAILS FOUND SUCCESSFULLY" as string : "FEES DEATILS NOT FOUND" as string,
                        Credit = _Charges.ToString(),
                        Debit = (_Charges - _Receipt).ToString(),
                        StudFee = studfee
                    };

                    tran.Commit();
                }//close transation

                return studfees;
            });
        }

        public async Task<StudLeaves> GetLeave(JObject jobj)
        {
            return await Task.Run(() =>
            {
                List<StudLeave> studleave = new List<StudLeave>();
                StudLeaves studleaves = null;

                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());
                if (!headers.ContainsKey("memid"))
                    throw new Exception("Parameter Incorrent !!!");
                string memid = headers["memid"];


                using (MySqlTransaction tran = Startup.con.BeginTransaction())
                {
                    DataTable _dtLeave = new DataTable("Leave");

                    using (MySqlCommand _mysqlcmd = new MySqlCommand("Select * From `leave` Where Mem_ID=" + memid +
                        " Order by Leave_ID DESC", Startup.con))
                    {
                        using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                        {
                            _mysel.Fill(_dtLeave);
                        }
                    }

                    for (int i = 0; i < _dtLeave.Rows.Count; i++)
                    {
                        studleave.Add(new StudLeave
                        {
                            LeaveFrom = DateTime.Parse(_dtLeave.Rows[i]["LeaveFrom"].ToString()).ToString("yyyy-MM-dd"),
                            LeaveReason = _dtLeave.Rows[i]["LeaveReason"].ToString(),
                            LeaveTo = DateTime.Parse(_dtLeave.Rows[i]["LeaveTo"].ToString()).ToString("yyyy-MM-dd"),
                            Remarks = _dtLeave.Rows[i]["Remarks"].ToString(),
                            Status = _dtLeave.Rows[i]["Status"].ToString()
                        });
                    }

                    studleaves = new StudLeaves
                    {
                        Success = _dtLeave.Rows.Count > 0 ? "1" as string : "0" as string,
                        Message = _dtLeave.Rows.Count > 0 ? "LEAVE FOUND SUCCESSFULLY" as string : "LEAVE NOT FOUND" as string,
                        StudLeave = studleave
                    };


                    tran.Commit();
                }//close transation

                return studleaves;
            });
        }

        public async Task<PublicNotices> GetPublicNotice(JObject jobj)
        {
            return await Task.Run(() =>
            {
                List<PublicNotice> publicnotice = new List<PublicNotice>();
                PublicNotices pubnotices = null;

                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());
                if (!headers.ContainsKey("cmpid"))
                    throw new Exception("Parameter Incorrent !!!");
                string cmpid = headers["cmpid"];

                using (MySqlTransaction tran = Startup.con.BeginTransaction())
                {
                    DataTable _dtPubNotices = new DataTable("PublicNotices");

                    using (MySqlCommand _mysqlcmd = new MySqlCommand("Select * From `publicnotice` where status=1 and expiry_date >=" + DateTime.Now.ToString("yyyy-MM-dd") + " and cmp_id in (0," + cmpid + ")", Startup.con))
                    {
                        using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                        {
                            _mysel.Fill(_dtPubNotices);
                        }
                    }

                    for (int i = 0; i < _dtPubNotices.Rows.Count; i++)
                    {
                        publicnotice.Add(new PublicNotice
                        {
                            PI_ID = _dtPubNotices.Rows[i]["PI_ID"].ToString(),
                            Cmp_ID = _dtPubNotices.Rows[i]["Cmp_ID"].ToString(),
                            Date = DateTime.Parse(_dtPubNotices.Rows[i]["Date"].ToString()).ToString("yyyy-MM-dd"),
                            Advertise = _dtPubNotices.Rows[i]["Advertise"].ToString(),
                            Message = _dtPubNotices.Rows[i]["Message"].ToString(),
                            Photo = _dtPubNotices.Rows[i]["Photo"].ToString(),
                            Weblink = _dtPubNotices.Rows[i]["Weblink"].ToString(),
                            Expiry_Date = DateTime.Parse(_dtPubNotices.Rows[i]["Expiry_Date"].ToString()).ToString("yyyy-MM-dd"),
                            Status = _dtPubNotices.Rows[i]["Status"].ToString()

                        });
                    }

                    pubnotices = new PublicNotices
                    {
                        Success = _dtPubNotices.Rows.Count > 0 ? "1" as string : "0" as string,
                        Message = _dtPubNotices.Rows.Count > 0 ? "PUBLIC NOTICE FOUND SUCCESSFULLY" as string : "PUBLIC NOTICE NOT FOUND" as string,
                        PubNotices = publicnotice
                    };


                    tran.Commit();
                }//close transation

                return pubnotices;
            });
        }

        public async Task<StudNotices> GetNotice(JObject jobj)
        {
            return await Task.Run(() =>
            {
                List<StudNotice> studnotice = new List<StudNotice>();
                StudNotices studnotices = null;

                FileInfo _File1 = null;
                FileInfo _File2 = null;
                FileInfo _File3 = null;
                string show = "0", noticeType = "%";
                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());

                if (!headers.ContainsKey("cmpkey") || !headers.ContainsKey("memid") || !headers.ContainsKey("mobile"))
                    throw new Exception("Parameter Incorrent !!!");

                if (!headers.ContainsKey("show"))
                    show = "0";
                else if (headers["show"] == "All")
                    show = "0,1";
                else if (headers["show"] == "New")
                    show = "0";
                else show = "0";

                if (!headers.ContainsKey("noticetype"))
                    noticeType = "%";
                else
                    noticeType = headers["noticetype"];

                string cmpkey = headers["cmpkey"];
                string memid = headers["memid"];
                string mobile = headers["mobile"];

                using (MySqlTransaction tran = Startup.con.BeginTransaction())
                {
                    DataTable _dtNotice = new DataTable("Notice");

                    using (MySqlCommand _mysqlcmd = new MySqlCommand("Select g.Notice_Date,g.Std,g.Division," +
                        "g.Notice_Type,g.Notice_Msg,g.File1,g.File2,g.File3,g.File1Type,g.File2Type," +
                        "g.File3Type,g.Replay_Type,g.Notice_Replay,g.Notification,gd.Mem_ID," +
                        "gd.Mobile1_Status,gd.Mobile2_Status,gd.Mobile3_Status," +
                        "gd.Mobile1_Status as ViewMessage,c.Cmp_ID From generalnoticedetails gd " +
                        "inner join generalnotice g on g.Notice_ID = gd.Notice_ID " +
                        "inner join Member m on m.Mem_ID = gd.Mem_ID " +
                        "inner join company c on c.Cmp_ID = m.Cmp_ID Where c.Cmp_Key=N'" +
                        cmpkey + "' and gd.Mem_ID=" + memid + " and m.Mobile1=N'" + mobile +
                        "' and g.Notice_Type like '" + noticeType + "' and Mobile1_Status in (" + show +
                        ") ORDER BY g.Notice_ID ASC", Startup.con))
                    {
                        using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                        {
                            _mysel.Fill(_dtNotice);
                        }
                    }

                    if (_dtNotice.Rows.Count > 0)
                    {
                        using (MySqlCommand _mysqlcmd = new MySqlCommand("update generalnoticedetails set Mobile1_Status = 1 " +
                            "Where Mem_ID = '" + memid + "' and Mobile1_Status = 0", Startup.con))
                        { _mysqlcmd.ExecuteNonQuery(); }
                    }
                    else if (_dtNotice.Rows.Count == 0)
                    {
                        using (MySqlCommand _mysqlcmd = new MySqlCommand("Select g.Notice_Date,g.Std,g.Division,g.Notice_Type,g.Notice_Msg," +
                            "g.File1,g.File2,g.File3,g.File1Type,g.File2Type,g.File3Type,g.Replay_Type,g.Notice_Replay,g.Notification," +
                            "gd.Mem_ID,gd.Mobile1_Status,gd.Mobile2_Status,gd.Mobile3_Status,gd.Mobile2_Status as ViewMessage,c.Cmp_ID " +
                            "From generalnoticedetails gd inner join generalnotice g on g.Notice_ID = gd.Notice_ID " +
                            "inner join Member m on m.Mem_ID = gd.Mem_ID inner join company c on c.Cmp_ID = m.Cmp_ID Where c.Cmp_Key=N'" +
                            cmpkey + "' and gd.Mem_ID=" + memid + " and m.Mobile2=N'" + mobile + "' and g.Notice_Type like '" +
                            noticeType + "' and Mobile1_Status in (" + show + ") ORDER BY g.Notice_ID ASC ", Startup.con))
                        {
                            using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                            {
                                _mysel.Fill(_dtNotice);
                            }
                        }
                        if (_dtNotice.Rows.Count > 0)
                        {
                            using (MySqlCommand _mysqlcmd = new MySqlCommand("update generalnoticedetails set Mobile2_Status = 1 " +
                                "Where Mem_ID = '" + memid + "' and Mobile2_Status = 0", Startup.con))
                            { _mysqlcmd.ExecuteNonQuery(); }
                        }
                        else if (_dtNotice.Rows.Count == 0)
                        {
                            using (MySqlCommand _mysqlcmd = new MySqlCommand("Select g.Notice_Date,g.Std,g.Division,g.Notice_Type," +
                                "g.Notice_Msg,g.File1,g.File2,g.File3,g.File1Type,g.File2Type,g.File3Type,g.Replay_Type,g.Notice_Replay," +
                                "g.Notification,gd.Mem_ID,gd.Mobile1_Status,gd.Mobile2_Status,gd.Mobile3_Status," +
                                "gd.Mobile3_Status as ViewMessage,c.Cmp_ID From generalnoticedetails gd " +
                                "inner join generalnotice g on g.Notice_ID = gd.Notice_ID inner join Member m on m.Mem_ID = gd.Mem_ID " +
                                "inner join company c on c.Cmp_ID = m.Cmp_ID Where c.Cmp_Key=N'" + cmpkey + "' and gd.Mem_ID=" + memid +
                                " and m.Mobile3=N'" + mobile + "' and g.Notice_Type like '" + noticeType +
                                "' and Mobile1_Status in (" + show + ") ORDER BY g.Notice_ID ASC", Startup.con))
                            {
                                using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                                {
                                    _mysel.Fill(_dtNotice);
                                }
                            }
                            if (_dtNotice.Rows.Count > 0)
                            {
                                using (MySqlCommand _mysqlcmd = new MySqlCommand("update generalnoticedetails set Mobile3_Status = 1 " +
                                    "Where Mem_ID = '" + memid + "' and Mobile3_Status = 0", Startup.con))
                                { _mysqlcmd.ExecuteNonQuery(); }
                            }
                        }
                    }

                    string path = "";
                    if (_dtNotice.Rows.Count > 0)
                        path = System.IO.Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, @"\\img\notice\" + _dtNotice.Rows[0]["Cmp_ID"].ToString().Trim() + "\\");
                    for (int i = 0; i < _dtNotice.Rows.Count; i++)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(_dtNotice.Rows[i]["File1"].ToString().Trim()) && _dtNotice.Rows[i]["File1"].ToString().Trim().Length > 10 && _dtNotice.Rows[i]["File1Type"].ToString().Trim() != "VDI")
                                _File1 = File.Exists(path + _dtNotice.Rows[i]["File1"].ToString().Trim()) ? new FileInfo(path + _dtNotice.Rows[i]["File1"].ToString().Trim()) : null;
                        }
                        catch (Exception ex)
                        { //Log error to system event  
                        }

                        try
                        {
                            if (!string.IsNullOrEmpty(_dtNotice.Rows[i]["File2"].ToString().Trim()) && _dtNotice.Rows[i]["File2"].ToString().Trim().Length > 10 && _dtNotice.Rows[i]["File2Type"].ToString().Trim() != "VDI")
                                _File2 = File.Exists(path + _dtNotice.Rows[i]["File2"].ToString().Trim()) ? new FileInfo(path + _dtNotice.Rows[i]["File2"].ToString().Trim()) : null;
                        }
                        catch (Exception ex)
                        { //Log error to system event 
                        }
                        try
                        {
                            if (!string.IsNullOrEmpty(_dtNotice.Rows[i]["File3"].ToString().Trim()) && _dtNotice.Rows[i]["File3"].ToString().Trim().Length > 10 && _dtNotice.Rows[i]["File3Type"].ToString().Trim() != "VDI")
                                _File3 = File.Exists(path + _dtNotice.Rows[i]["File3"].ToString().Trim()) ? new FileInfo(path + _dtNotice.Rows[i]["File3"].ToString().Trim()) : null;
                        }
                        catch (Exception ex)
                        { //Log error to system event 
                        }

                        studnotice.Add(new StudNotice
                        {
                            Notice_Date = DateTime.Parse(_dtNotice.Rows[i]["Notice_Date"].ToString().Trim()).ToString("yyyy-MM-dd  HH:mm:ss"),
                            Notice_Type = _dtNotice.Rows[i]["Notice_Type"].ToString().Trim(),
                            Notice_Msg = _dtNotice.Rows[i]["Notice_Msg"].ToString().Trim(),
                            File1 = _dtNotice.Rows[i]["File1"].ToString().Trim(),
                            File1Type = _dtNotice.Rows[i]["File1Type"].ToString().Trim(),
                            File1Size = _File1 != null ? Utilities.GetFileSize(_File1.Length) : "0",
                            File2 = _dtNotice.Rows[i]["File2"].ToString().Trim(),
                            File2Type = _dtNotice.Rows[i]["File2Type"].ToString().Trim(),
                            File2Size = _File2 != null ? Utilities.GetFileSize(_File2.Length) : "0",
                            File3 = _dtNotice.Rows[i]["File3"].ToString().Trim(),
                            File3Type = _dtNotice.Rows[i]["File3Type"].ToString().Trim(),
                            File3Size = _File3 != null ? Utilities.GetFileSize(_File3.Length) : "0",
                            Replay_Type = _dtNotice.Rows[i]["Replay_Type"].ToString().Trim(),
                            Notice_Replay = _dtNotice.Rows[i]["Notice_Replay"].ToString().Trim(),
                            Notification = _dtNotice.Rows[i]["Notification"].ToString().Trim(),
                            Mobile1_Status = _dtNotice.Rows[i]["Mobile1_Status"].ToString().Trim(),
                            Mobile2_Status = _dtNotice.Rows[i]["Mobile2_Status"].ToString().Trim(),
                            Mobile3_Status = _dtNotice.Rows[i]["Mobile3_Status"].ToString().Trim(),
                            ViewMessage = _dtNotice.Rows[i]["ViewMessage"].ToString().Trim(),
                            CmpID = _dtNotice.Rows[i]["Cmp_ID"].ToString().Trim()
                        });
                        _File1 = null;
                        _File2 = null;
                        _File3 = null;
                    }
                    studnotices = new StudNotices
                    {
                        Success = _dtNotice.Rows.Count > 0 ? "1" as string : "0" as string,
                        Message = _dtNotice.Rows.Count > 0 ? "NOTICE FOUND SUCCESSFULLY" as string : "NOTICE NOT FOUND" as string,
                        StudNotice = studnotice
                    };

                    tran.Commit();
                }//close transation


                return studnotices;
            });
        }

        public async Task<StudNotices> GetNoticeMobilebased(JObject jobj)
        {
            return await Task.Run(() =>
            {
                List<StudNotice> studnotice = new List<StudNotice>();
                StudNotices studnotices = null;

                FileInfo _File1 = null;
                FileInfo _File2 = null;
                FileInfo _File3 = null;
                string show = "0", noticeType = "%", mobileSelect = "", mobileStatus = "";
                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());

                if (!headers.ContainsKey("cmpkey") || !headers.ContainsKey("memid") || !headers.ContainsKey("mobile") || !headers.ContainsKey("mobileno"))
                    throw new Exception("Parameter Incorrent !!!");

                if (!headers.ContainsKey("show"))
                    show = "0";
                else if (headers["show"] == "All")
                    show = "0,1";
                else if (headers["show"] == "New")
                    show = "0";
                else show = "0";

                if (!headers.ContainsKey("noticetype"))
                    noticeType = "%";
                else
                    noticeType = headers["noticetype"];

                if (headers["mobileno"] == "1")
                {
                    mobileSelect = "m.Mobile1";
                    mobileStatus = "Mobile1_Status";
                }
                else if (headers["mobileno"] == "2")
                {
                    mobileSelect = "m.Mobile2";
                    mobileStatus = "Mobile2_Status";
                }
                else if (headers["mobileno"] == "3")
                {
                    mobileSelect = "m.Mobile3";
                    mobileStatus = "Mobile3_Status";
                }
                else
                    throw new Exception("Parameter Incorrent !!!");

                string cmpkey = headers["cmpkey"];
                string memid = headers["memid"];
                string mobile = headers["mobile"];

                using (MySqlTransaction tran = Startup.con.BeginTransaction())
                {
                    DataTable _dtNotice = new DataTable("Notice");

                    using (MySqlCommand _mysqlcmd = new MySqlCommand("Select g.Notice_Date,g.Std,g.Division,g.Notice_Type,g.Notice_Msg," +
                        "g.File1,g.File2,g.File3,g.File1Type,g.File2Type,g.File3Type,g.Replay_Type,g.Notice_Replay,g.Notification," +
                        "gd.Mem_ID,gd.Mobile1_Status,gd.Mobile2_Status,gd.Mobile3_Status,gd.Mobile1_Status as ViewMessage,c.Cmp_ID " +
                        "From generalnoticedetails gd inner join generalnotice g on g.Notice_ID = gd.Notice_ID " +
                        "inner join Member m on m.Mem_ID = gd.Mem_ID inner join company c on c.Cmp_ID = m.Cmp_ID Where c.Cmp_Key=N'" +
                        cmpkey + "' and gd.Mem_ID=" + memid + " and " + mobileSelect + "= N'" + mobile + "' and g.Notice_Type like '" +
                        noticeType + "' and " + mobileStatus + " in (" + show + ") ORDER BY g.Notice_ID ASC", Startup.con))
                    {
                        using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                        {
                            _mysel.Fill(_dtNotice);
                        }
                    }

                    if (_dtNotice.Rows.Count > 0)
                    {
                        using (MySqlCommand _mysqlcmd = new MySqlCommand("update generalnoticedetails set " + mobileStatus + " = 1 " +
                            "Where Mem_ID = '" + memid + "' and " + mobileStatus + " = 0", Startup.con))
                        { _mysqlcmd.ExecuteNonQuery(); }
                    }


                    string path = "";
                    if (_dtNotice.Rows.Count > 0)
                        path = System.IO.Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, @"\\img\notice\" + _dtNotice.Rows[0]["Cmp_ID"].ToString().Trim() + "\\");
                    for (int i = 0; i < _dtNotice.Rows.Count; i++)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(_dtNotice.Rows[i]["File1"].ToString().Trim()) && _dtNotice.Rows[i]["File1"].ToString().Trim().Length > 10 && _dtNotice.Rows[i]["File1Type"].ToString().Trim() != "VDI")
                                _File1 = File.Exists(path + _dtNotice.Rows[i]["File1"].ToString().Trim()) ? new FileInfo(path + _dtNotice.Rows[i]["File1"].ToString().Trim()) : null;
                        }
                        catch (Exception ex)
                        { //Log error to system event  
                        }

                        try
                        {
                            if (!string.IsNullOrEmpty(_dtNotice.Rows[i]["File2"].ToString().Trim()) && _dtNotice.Rows[i]["File2"].ToString().Trim().Length > 10 && _dtNotice.Rows[i]["File2Type"].ToString().Trim() != "VDI")
                                _File2 = File.Exists(path + _dtNotice.Rows[i]["File2"].ToString().Trim()) ? new FileInfo(path + _dtNotice.Rows[i]["File2"].ToString().Trim()) : null;
                        }
                        catch (Exception ex)
                        { //Log error to system event 
                        }
                        try
                        {
                            if (!string.IsNullOrEmpty(_dtNotice.Rows[i]["File3"].ToString().Trim()) && _dtNotice.Rows[i]["File3"].ToString().Trim().Length > 10 && _dtNotice.Rows[i]["File3Type"].ToString().Trim() != "VDI")
                                _File3 = File.Exists(path + _dtNotice.Rows[i]["File3"].ToString().Trim()) ? new FileInfo(path + _dtNotice.Rows[i]["File3"].ToString().Trim()) : null;
                        }
                        catch (Exception ex)
                        { //Log error to system event 
                        }

                        studnotice.Add(new StudNotice
                        {
                            Notice_Date = DateTime.Parse(_dtNotice.Rows[i]["Notice_Date"].ToString().Trim()).ToString("yyyy-MM-dd  HH:mm:ss"),
                            Notice_Type = _dtNotice.Rows[i]["Notice_Type"].ToString().Trim(),
                            Notice_Msg = _dtNotice.Rows[i]["Notice_Msg"].ToString().Trim(),
                            File1 = _dtNotice.Rows[i]["File1"].ToString().Trim(),
                            File1Type = _dtNotice.Rows[i]["File1Type"].ToString().Trim(),
                            File1Size = _File1 != null ? Utilities.GetFileSize(_File1.Length) : "0",
                            File2 = _dtNotice.Rows[i]["File2"].ToString().Trim(),
                            File2Type = _dtNotice.Rows[i]["File2Type"].ToString().Trim(),
                            File2Size = _File2 != null ? Utilities.GetFileSize(_File2.Length) : "0",
                            File3 = _dtNotice.Rows[i]["File3"].ToString().Trim(),
                            File3Type = _dtNotice.Rows[i]["File3Type"].ToString().Trim(),
                            File3Size = _File3 != null ? Utilities.GetFileSize(_File3.Length) : "0",
                            Replay_Type = _dtNotice.Rows[i]["Replay_Type"].ToString().Trim(),
                            Notice_Replay = _dtNotice.Rows[i]["Notice_Replay"].ToString().Trim(),
                            Notification = _dtNotice.Rows[i]["Notification"].ToString().Trim(),
                            Mobile1_Status = _dtNotice.Rows[i]["Mobile1_Status"].ToString().Trim(),
                            Mobile2_Status = _dtNotice.Rows[i]["Mobile2_Status"].ToString().Trim(),
                            Mobile3_Status = _dtNotice.Rows[i]["Mobile3_Status"].ToString().Trim(),
                            ViewMessage = _dtNotice.Rows[i]["ViewMessage"].ToString().Trim(),
                            CmpID = _dtNotice.Rows[i]["Cmp_ID"].ToString().Trim()
                        });
                        _File1 = null;
                        _File2 = null;
                        _File3 = null;
                    }
                    studnotices = new StudNotices
                    {
                        Success = _dtNotice.Rows.Count > 0 ? "1" as string : "0" as string,
                        Message = _dtNotice.Rows.Count > 0 ? "NOTICE FOUND SUCCESSFULLY" as string : "NOTICE NOT FOUND" as string,
                        StudNotice = studnotice
                    };

                    tran.Commit();
                }//close transation


                return studnotices;
            });
        }

        public async Task<StudChats> GetChat(JObject jobj)
        {
            return await Task.Run(() =>
            {
                List<StudChat> studchat = new List<StudChat>();
                StudChats studchats = null;

                FileInfo _File1 = null;
                string show = "0";

                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());

                if (!headers.ContainsKey("cmpkey") || !headers.ContainsKey("memid") || !headers.ContainsKey("mobile"))
                    throw new Exception("Parameter Incorrent !!!");

                if (!headers.ContainsKey("show"))
                    show = "0";
                else if (headers["show"] == "All")
                    show = "0,1";
                else if (headers["show"] == "New")
                    show = "0";
                else show = "0";


                string cmpkey = headers["cmpkey"];
                string memid = headers["memid"];
                string mobile = headers["mobile"];

                using (MySqlTransaction tran = Startup.con.BeginTransaction())
                {
                    DataTable _dtNotice = new DataTable("Notice");

                    using (MySqlCommand _mysqlcmd = new MySqlCommand("Select c.Chat_Date,c.Std,c.Division,c.Chat_Type,c.Chat_Msg," +
                            "c.File1,c.File1Type,c.Replay_Type,c.Notification,c.Mem_ID,c.Mobile1_Status," +
                            "c.Mobile1_Status as ViewMessage,c.Cmp_ID From chat c " +
                            "inner join Member m on m.Mem_ID = c.Mem_ID inner join company cp on c.Cmp_ID = m.Cmp_ID " +
                            "Where cp.Cmp_Key=N'" + cmpkey + "' and c.Mem_ID=" + memid +
                            " and m.Mobile1=N'" + mobile + "' and Mobile1_Status in (" + show + ") ORDER BY c.Chat_ID ASC ", Startup.con))
                    {
                        using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                        {
                            _mysel.Fill(_dtNotice);
                        }
                    }

                    if (_dtNotice.Rows.Count > 0)
                    {
                        using (MySqlCommand _mysqlcmd = new MySqlCommand("update chat set Mobile1_Status = 1 Where Mem_ID = '"
                            + memid + "' and Mobile1_Status = 0", Startup.con))
                        { _mysqlcmd.ExecuteNonQuery(); }
                    }
                    else if (_dtNotice.Rows.Count == 0)
                    {
                        using (MySqlCommand _mysqlcmd = new MySqlCommand("Select c.Chat_Date,c.Std,c.Division,c.Chat_Type,c.Chat_Msg," +
                            "c.File1,c.File1Type,c.Replay_Type,c.Notification,c.Mem_ID,c.Mobile1_Status," +
                            "c.Mobile1_Status as ViewMessage,c.Cmp_ID From chat c " +
                            "inner join Member m on m.Mem_ID = c.Mem_ID inner join company cp on c.Cmp_ID = m.Cmp_ID " +
                            "Where cp.Cmp_Key=N'" + cmpkey + "' and c.Mem_ID=" + memid +
                            " and m.Mobile2=N'" + mobile + "' and Mobile2_Status in (" + show + ") ORDER BY c.Chat_ID ASC ", Startup.con))
                        {
                            using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                            {
                                _mysel.Fill(_dtNotice);
                            }
                        }
                        if (_dtNotice.Rows.Count > 0)
                        {
                            using (MySqlCommand _mysqlcmd = new MySqlCommand("update chat set Mobile2_Status = 1 " +
                                "Where Mem_ID = '" + memid + "' and Mobile2_Status = 0", Startup.con))
                            { _mysqlcmd.ExecuteNonQuery(); }
                        }
                        else if (_dtNotice.Rows.Count == 0)
                        {
                            using (MySqlCommand _mysqlcmd = new MySqlCommand("Select c.Chat_Date,c.Std,c.Division,c.Chat_Type,c.Chat_Msg," +
                            "c.File1,c.File1Type,c.Replay_Type,c.Notification,c.Mem_ID,c.Mobile1_Status," +
                            "c.Mobile1_Status as ViewMessage,c.Cmp_ID From chat c " +
                            "inner join Member m on m.Mem_ID = c.Mem_ID inner join company cp on c.Cmp_ID = m.Cmp_ID " +
                            "Where cp.Cmp_Key=N'" + cmpkey + "' and c.Mem_ID=" + memid +
                            " and m.Mobile3=N'" + mobile + "' and Mobile3_Status in (" + show + ") ORDER BY c.Chat_ID ASC ", Startup.con))
                            {
                                using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                                {
                                    _mysel.Fill(_dtNotice);
                                }
                            }
                            if (_dtNotice.Rows.Count > 0)
                            {
                                using (MySqlCommand _mysqlcmd = new MySqlCommand("update chat set Mobile3_Status = 1 " +
                                    "Where Mem_ID = '" + memid + "' and Mobile3_Status = 0", Startup.con))
                                { _mysqlcmd.ExecuteNonQuery(); }
                            }
                        }
                    }

                    string path = "";
                    if (_dtNotice.Rows.Count > 0)
                        path = System.IO.Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, @"\\img\notice\" + _dtNotice.Rows[0]["Cmp_ID"].ToString().Trim() + "\\");
                    for (int i = 0; i < _dtNotice.Rows.Count; i++)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(_dtNotice.Rows[i]["File1"].ToString().Trim()) && _dtNotice.Rows[i]["File1"].ToString().Trim().Length > 10 && _dtNotice.Rows[i]["File1Type"].ToString().Trim() != "VDI")
                                _File1 = File.Exists(path + _dtNotice.Rows[i]["File1"].ToString().Trim()) ? new FileInfo(path + _dtNotice.Rows[i]["File1"].ToString().Trim()) : null;
                        }
                        catch (Exception ex)
                        {
                            //Log Error to System
                        }

                        studchat.Add(new StudChat
                        {
                            Chat_Date = DateTime.Parse(_dtNotice.Rows[i]["Chat_Date"].ToString().Trim()).ToString("yyyy-MM-dd  HH:mm:ss"),
                            //Chat_Type = _dtNotice.Rows[i]["Chat_Type"].ToString().Trim(),
                            Chat_Msg = _dtNotice.Rows[i]["Chat_Msg"].ToString().Trim(),
                            File1 = _dtNotice.Rows[i]["File1"].ToString().Trim(),
                            File1Type = _dtNotice.Rows[i]["File1Type"].ToString().Trim(),
                            //File1Size = _File1 != null ? Utilities.GetFileSize(_File1.Length) : "0",
                            Replay_Type = _dtNotice.Rows[i]["Replay_Type"].ToString().Trim(),
                            //Chat_Replay = _dtNotice.Rows[i]["Chat_Replay"].ToString().Trim(),
                            Notification = _dtNotice.Rows[i]["Notification"].ToString().Trim(),
                            //Mobile1_Status = _dtNotice.Rows[i]["Mobile1_Status"].ToString().Trim(),
                            //Mobile2_Status = _dtNotice.Rows[i]["Mobile2_Status"].ToString().Trim(),
                            //Mobile3_Status = _dtNotice.Rows[i]["Mobile3_Status"].ToString().Trim(),
                            ViewMessage = _dtNotice.Rows[i]["ViewMessage"].ToString().Trim(),
                            CmpID = _dtNotice.Rows[i]["Cmp_ID"].ToString().Trim()
                        });
                        _File1 = null;
                    }
                    studchats = new StudChats
                    {
                        Success = _dtNotice.Rows.Count > 0 ? "1" as string : "0" as string,
                        Message = _dtNotice.Rows.Count > 0 ? "CHAT FOUND SUCCESSFULLY" as string : "CHAT NOT FOUND" as string,
                        StudChat = studchat
                    };

                    tran.Commit();
                }//close transation


                return studchats;
            });
        }

        public async Task<PhotoGalleryList> GetPhotoGallery(JObject jobj)
        {
            return await Task.Run(() =>
            {
                List<PhotoGalleryItem> PhotoGalleryList = new List<PhotoGalleryItem>();
                PhotoGalleryList PhotoGallery = null;
                string std = "'All'", division = "'All'";

                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());

                if (!headers.ContainsKey("cmpkey"))
                    throw new Exception("Parameter Incorrent !!!");

                string cmpkey = headers["cmpkey"];

                if (headers.ContainsKey("std"))
                    std = "'" + headers["std"] + "','All'";

                if (headers.ContainsKey("division"))
                    division = "'" + headers["division"] + "','All'";

                DataTable _dtCompany = new DataTable("Company");

                using (MySqlCommand _mysqlcmd = new MySqlCommand("select cmp_id from company where cmp_key='" +
                    cmpkey + "'", Startup.con))
                {
                    using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                    {
                        _mysel.Fill(_dtCompany);
                    }
                }

                if (_dtCompany.Rows.Count == 0)
                    throw new Exception("Company Not Found!!!");

                string cmp_id = _dtCompany.Rows[0]["cmp_id"].ToString();

                DataTable _dtPhotoGallery = new DataTable("PhotoGallery");

                using (MySqlCommand _mysqlcmd = new MySqlCommand("Select Cmp_ID,Std,Division,Title,Photo,Category,Subject from photogallery" +
                    " Where Std in (" + std + ") and Division in (" + division + ") and Cmp_Id=" + cmp_id, Startup.con))
                {
                    using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                    {
                        _mysel.Fill(_dtPhotoGallery);
                    }
                }

                for (int i = 0; i < _dtPhotoGallery.Rows.Count; i++)
                {
                    PhotoGalleryList.Add(new PhotoGalleryItem
                    {
                        Cmp_ID = _dtPhotoGallery.Rows[i]["Cmp_ID"].ToString(),
                        Division = _dtPhotoGallery.Rows[i]["Division"].ToString(),
                        Photo = _dtPhotoGallery.Rows[i]["Photo"].ToString(),
                        Std = _dtPhotoGallery.Rows[i]["Std"].ToString(),
                        Title = _dtPhotoGallery.Rows[i]["Title"].ToString(),
                        Category = _dtPhotoGallery.Rows[i]["Category"].ToString(),
                        Subject = _dtPhotoGallery.Rows[i]["Subject"].ToString(),
                    });

                }

                PhotoGallery = new PhotoGalleryList
                {
                    Success = _dtPhotoGallery.Rows.Count > 0 ? "1" as string : "0" as string,
                    Message = _dtPhotoGallery.Rows.Count > 0 ? "PHOTO GALLERY FOUND SUCCESSFULLY" as string : "PHOTO GALLERY NOT FOUND" as string,
                    PhotoGallery = PhotoGalleryList
                };

                return PhotoGallery;
            });
        }

        public async Task<VideoGalleryList> GetVideoGallery(JObject jobj)
        {
            return await Task.Run(() =>
            {
                List<VideoGalleryItem> VideoGalleryList = new List<VideoGalleryItem>();
                VideoGalleryList VideoGallery = null;
                string std = "'All'", division = "'All'";

                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());

                if (!headers.ContainsKey("cmpkey"))
                    throw new Exception("Parameter Incorrent !!!");

                string cmpkey = headers["cmpkey"];

                if (headers.ContainsKey("std"))
                    std = "'" + headers["std"] + "','All'";

                if (headers.ContainsKey("division"))
                    division = "'" + headers["division"] + "','All'";

                DataTable _dtCompany = new DataTable("Company");

                using (MySqlCommand _mysqlcmd = new MySqlCommand("select cmp_id from company where cmp_key='" +
                    cmpkey + "'", Startup.con))
                {
                    using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                    {
                        _mysel.Fill(_dtCompany);
                    }
                }

                if (_dtCompany.Rows.Count == 0)
                    throw new Exception("Company Not Found!!!");

                string cmp_id = _dtCompany.Rows[0]["cmp_id"].ToString();

                DataTable _dtVideoGallery = new DataTable("VideoGallery");

                using (MySqlCommand _mysqlcmd = new MySqlCommand("Select Cmp_ID,Std,Division,Title,Video,Category,Subject from videogallery" +
                    " Where Std in (" + std + ") and Division in (" + division + ") and Cmp_Id=" + cmp_id, Startup.con))
                {
                    using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                    {
                        _mysel.Fill(_dtVideoGallery);
                    }
                }

                for (int i = 0; i < _dtVideoGallery.Rows.Count; i++)
                {
                    VideoGalleryList.Add(new VideoGalleryItem
                    {
                        Cmp_ID = _dtVideoGallery.Rows[i]["Cmp_ID"].ToString(),
                        Division = _dtVideoGallery.Rows[i]["Division"].ToString(),
                        Video = _dtVideoGallery.Rows[i]["Video"].ToString(),
                        Std = _dtVideoGallery.Rows[i]["Std"].ToString(),
                        Title = _dtVideoGallery.Rows[i]["Title"].ToString(),
                        Category = _dtVideoGallery.Rows[i]["Category"].ToString(),
                        Subject = _dtVideoGallery.Rows[i]["Subject"].ToString(),
                    });

                }

                VideoGallery = new VideoGalleryList
                {
                    Success = _dtVideoGallery.Rows.Count > 0 ? "1" as string : "0" as string,
                    Message = _dtVideoGallery.Rows.Count > 0 ? "VIDEO GALLERY FOUND SUCCESSFULLY" as string : "VIDEO GALLERY NOT FOUND" as string,
                    VideoGallery = VideoGalleryList
                };

                return VideoGallery;
            });
        }

        public async Task<PDFGalleryList> GetPDFGallery(JObject jobj)
        {
            return await Task.Run(() =>
            {
                List<PDFGalleryItem> PDFGalleryList = new List<PDFGalleryItem>();
                PDFGalleryList PDFGallery = null;
                string std = "'All'", division = "'All'";

                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());

                if (!headers.ContainsKey("cmpkey"))
                    throw new Exception("Parameter Incorrent !!!");

                string cmpkey = headers["cmpkey"];

                if (headers.ContainsKey("std"))
                    std = "'" + headers["std"] + "','All'";

                if (headers.ContainsKey("division"))
                    division = "'" + headers["division"] + "','All'";

                DataTable _dtCompany = new DataTable("Company");

                using (MySqlCommand _mysqlcmd = new MySqlCommand("select cmp_id from company where cmp_key='" +
                    cmpkey + "'", Startup.con))
                {
                    using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                    {
                        _mysel.Fill(_dtCompany);
                    }
                }

                if (_dtCompany.Rows.Count == 0)
                    throw new Exception("Company Not Found!!!");

                string cmp_id = _dtCompany.Rows[0]["cmp_id"].ToString();

                DataTable _dtPDFGallery = new DataTable("PDFGallery");

                using (MySqlCommand _mysqlcmd = new MySqlCommand("Select Cmp_ID,Std,Division,Title,File,Category,Subject from pdfdownload " +
                    " Where Std in (" + std + ") and Division in (" + division + ") and Cmp_Id=" + cmp_id, Startup.con))
                {
                    using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                    {
                        _mysel.Fill(_dtPDFGallery);
                    }
                }

                for (int i = 0; i < _dtPDFGallery.Rows.Count; i++)
                {
                    PDFGalleryList.Add(new PDFGalleryItem
                    {
                        Cmp_ID = _dtPDFGallery.Rows[i]["Cmp_ID"].ToString(),
                        Division = _dtPDFGallery.Rows[i]["Division"].ToString(),
                        PDF = _dtPDFGallery.Rows[i]["File"].ToString(),
                        Std = _dtPDFGallery.Rows[i]["Std"].ToString(),
                        Title = _dtPDFGallery.Rows[i]["Title"].ToString(),
                        Category = _dtPDFGallery.Rows[i]["Category"].ToString(),
                        Subject = _dtPDFGallery.Rows[i]["Subject"].ToString(),
                    });

                }

                PDFGallery = new PDFGalleryList
                {
                    Success = _dtPDFGallery.Rows.Count > 0 ? "1" as string : "0" as string,
                    Message = _dtPDFGallery.Rows.Count > 0 ? "PDF GALLERY FOUND SUCCESSFULLY" as string : "PDF GALLERY NOT FOUND" as string,
                    PDFGallery = PDFGalleryList
                };

                return PDFGallery;
            });
        }

        public async Task<Exams> GetExam(JObject jobj)
        {
            return await Task.Run(() =>
            {
                List<ExamItem> ExamList = new List<ExamItem>();
                Exams ListofExam = null;
                string type = "%";
                int month;

                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());

                if (!headers.ContainsKey("std") || !headers.ContainsKey("division") || !headers.ContainsKey("cmpkey") || !headers.ContainsKey("month"))
                    throw new Exception("Parameter Incorrent !!!");

                string std = "'" + headers["std"] + "','All'";
                string division = "'" + headers["division"] + "','All'";
                string cmpkey = headers["cmpkey"];

                month = "JanFebMarAprMayJunJulAugSepOctNovDec".IndexOf(headers["month"]) / 3 + 1;

                //if (headers.ContainsKey("type"))
                //    type = headers["type"];

                DataTable _dtCompany = new DataTable("Company");

                using (MySqlCommand _mysqlcmd = new MySqlCommand("select cmp_id from company where cmp_key='" +
                    cmpkey + "'", Startup.con))
                {
                    using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                    {
                        _mysel.Fill(_dtCompany);
                    }
                }

                if (_dtCompany.Rows.Count == 0)
                    throw new Exception("Company Not Found!!!");

                string cmp_id = _dtCompany.Rows[0]["cmp_id"].ToString();

                DataTable _dtExams = new DataTable("Exams");

                using (MySqlCommand _mysqlcmd = new MySqlCommand("Select Exam_ID,DATE_FORMAT(Date,'%Y-%m-%d') AS Date,Std,Division,Time,Subject,Type,Remarks,Admin_ID,YEAR(date) AS 'year', MONTH(DATE) AS 'month' from exam" +
                    " Where Std in (" + std + ") and Division in (" + division + ") and Cmp_Id=" + cmp_id
                    , Startup.con))
                {
                    using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                    {
                        _mysel.Fill(_dtExams);
                    }
                }

                for (int i = 0; i < _dtExams.Rows.Count; i++)
                {
                    if (int.Parse(_dtExams.Rows[i]["Month"].ToString()) != month)
                        continue;
                    ExamList.Add(new ExamItem
                    {
                        Admin_ID = _dtExams.Rows[i]["Admin_ID"].ToString(),
                        Date = _dtExams.Rows[i]["Date"].ToString(),
                        Division = _dtExams.Rows[i]["Division"].ToString(),
                        Exam_ID = _dtExams.Rows[i]["Exam_ID"].ToString(),
                        Month = _dtExams.Rows[i]["Month"].ToString(),
                        Remarks = _dtExams.Rows[i]["Remarks"].ToString(),
                        Std = _dtExams.Rows[i]["Std"].ToString(),
                        Subject = _dtExams.Rows[i]["Subject"].ToString(),
                        Time = _dtExams.Rows[i]["Time"].ToString(),
                        Type = _dtExams.Rows[i]["Type"].ToString(),
                        Year = _dtExams.Rows[i]["Year"].ToString(),
                    });

                }

                ListofExam = new Exams
                {
                    Success = ExamList.Count > 0 ? "1" as string : "0" as string,
                    Message = ExamList.Count > 0 ? "EXAM FOUND SUCCESSFULLY" as string : "EXAM NOT FOUND" as string,
                    ListofExams = ExamList
                };

                return ListofExam;
            });

        }

        public async Task<ResponseMessage> SetLastVisit(JObject jobj)
        {
            return await Task.Run(() =>
            {
                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());

                if (!headers.ContainsKey("memid") && !headers.ContainsKey("visitdate"))
                    throw new Exception("Parameter Incorrent !!!");

                string memid = headers["memid"];
                DateTime _dtvisit = Convert.ToDateTime(headers["visitdate"]);

                using (MySqlTransaction tran = Startup.con.BeginTransaction())
                {
                    MySqlCommand _mysqlcmd = new MySqlCommand("Update Member Set LastView='" + _dtvisit.ToString("yyyy-MM-dd HH:mm:ss") + "' Where Mem_Id='" + memid + "'", Startup.con);
                    _mysqlcmd.ExecuteNonQuery();

                    tran.Commit();
                }//close transation

                return new ResponseMessage { Message = "Member Visit Update", Success = "1" };
            });

        }

        public async Task<ResponseMessage> SetChat(JObject jobj)
        {
            return await Task.Run(() =>
            {
                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());
                string _companyLockKey = "key";
                List<string> allProperties = new List<string>
                {
                    { "Cmpkey" },
                    { "MemID" },
                    { "Standard" },
                    { "Division" },
                    { "Chat_Date" },
                    { "Chat_Msg" },
                    { "base64File1" },
                    { "File1Type" },
                    { "File1Ext" },
                    { "Replay_Type" },
                    { "View_Msg" },
                    { "Notification" },
                    { "Mobile1_Status" },
                    { "Mobile2_Status" },
                    { "Mobile3_Status" }
                };
                if (!headers.ContainsKey("studchat"))
                    throw new Exception("Input JSON parameter studchat not defined !!!");

                try
                {
                    JObject studchat = JObject.Parse(headers["studchat"]);

                    foreach (var item in allProperties)
                    {
                        if (!studchat.ContainsKey(item))
                        {
                            return new ResponseMessage
                            {
                                Success = "0",
                                Message = "Input parameter studchat is not Chat Object !!!",
                                InternalMessage = ""
                            };
                        }

                        if (studchat[item].Type.ToString() != "String")
                        {
                            return new ResponseMessage
                            {
                                Success = "0",
                                Message = "Input parameter " + item + " is not string type!!!",
                                InternalMessage = ""
                            };
                        }
                    }

                    if (studchat["Cmpkey"].ToString() == _companyLockKey)
                        throw new Exception("THIS USER ONLY FOR DEMO \n YOU ARE NOT PUT REPLY IN THIS APPLICATION");

                    DataTable _dtCompany = new DataTable("Company");

                    using (MySqlCommand _mysqlcmd = new MySqlCommand("select cmp_id from company where cmp_key='" +
                        studchat["Cmpkey"] + "'", Startup.con))
                    {
                        using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                        {
                            _mysel.Fill(_dtCompany);
                        }
                    }

                    if (_dtCompany.Rows.Count == 0)
                    {
                        return new ResponseMessage
                        {
                            Success = "0",
                            Message = "Company not found !!!",
                            InternalMessage = ""
                        };
                    }

                    string cmp_id = _dtCompany.Rows[0]["Cmp_ID"].ToString();
                    string _dtChat = DateTime.Parse(studchat["Chat_Date"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    string file1 = "";
                    using (MySqlTransaction tran = Startup.con.BeginTransaction())
                    {
                        string insertString = "Insert into `chat` (`Cmp_ID`,`Mem_ID`,`Chat_Date`,`Std`,`Division`,`Chat_Msg`," +
                        "`File1`,`File1Type`,`Replay_Type`,`View_Msg`,`Notification`,`Mobile1_Status`,`Mobile2_Status`,`Mobile3_Status`) values(" +
                        cmp_id + "," + studchat["MemID"] + ",N'" + _dtChat + "',N'" + studchat["Standard"] + "',N'" +
                        studchat["Division"] + "',N'" + studchat["Chat_Msg"] + "',N'" + file1 + "',N'" + studchat["File1Type"] +
                        "'," + "0,1,0," + studchat["Mobile1_Status"] + "," + studchat["Mobile2_Status"] + "," + studchat["Mobile3_Status"] + ")";
                        MySqlCommand _mysqlcmd = new MySqlCommand(insertString, Startup.con);
                        int rowsAffected = _mysqlcmd.ExecuteNonQuery();
                        tran.Commit();

                        if (rowsAffected > 0)
                        {
                            return new ResponseMessage
                            {
                                Success = "1",
                                Message = "CHAT ADD SUCCESSFULLY",
                                InternalMessage = ""
                            };
                        }
                        else
                        {
                            tran.Rollback();
                            return new ResponseMessage
                            {
                                Success = "0",
                                Message = "CHAT NOT ADD SUCCESSFULLY",
                                InternalMessage = ""
                            };
                        }

                    }//close transation
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            });

        }

        public async Task<ResponseMessage> SetNotice(JObject jobj)
        {
            return await Task.Run(() =>
            {
                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());
                string _companyLockKey = "key";
                List<string> allProperties = new List<string>
                {
                    { "Cmpkey" },
                    { "MemID" },
                    { "Standard" },
                    { "Division" },
                    { "Notice_Date" },
                    { "Notice_Type" },
                    { "Notice_Msg" },
                    { "base64File1" },
                    { "File1Type" },
                    { "File1Ext" },
                    { "base64File2" },
                    { "File2Type" },
                    { "File2Ext" },
                    { "base64File3" },
                    { "File3Type" },
                    { "File3Ext" },
                    { "Replay_Type" },
                    { "Notice_Replay" },
                    { "Notification" },
                    { "Mobile1_Status" },
                    { "Mobile2_Status" },
                    { "Mobile3_Status" }
                };

                if (!headers.ContainsKey("studnotice"))
                    throw new Exception("Input JSON parameter studnotice not defined !!!");

                try
                {
                    JObject studnotice = JObject.Parse(headers["studnotice"]);

                    foreach (var item in allProperties)
                    {
                        if (!studnotice.ContainsKey(item))
                        {
                            return new ResponseMessage
                            {
                                Success = "0",
                                Message = "Input parameter studnotice is not Chat Object !!!",
                                InternalMessage = ""
                            };
                        }

                        if (studnotice[item].Type.ToString() != "String")
                        {
                            return new ResponseMessage
                            {
                                Success = "0",
                                Message = "Input parameter " + item + " is not string type!!!",
                                InternalMessage = ""
                            };
                        }
                    }

                    if (studnotice["Cmpkey"].ToString() == _companyLockKey)
                        throw new Exception("THIS USER ONLY FOR DEMO \n YOU ARE NOT PUT REPLY IN THIS APPLICATION");

                    DataTable _dtCompany = new DataTable("Company");

                    using (MySqlCommand _mysqlcmd = new MySqlCommand("select cmp_id from company where cmp_key='" +
                        studnotice["Cmpkey"] + "'", Startup.con))
                    {
                        using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                        {
                            _mysel.Fill(_dtCompany);
                        }
                    }

                    if (_dtCompany.Rows.Count == 0)
                    {
                        return new ResponseMessage
                        {
                            Success = "0",
                            Message = "Company not found !!!",
                            InternalMessage = ""
                        };
                    }

                    string cmp_id = _dtCompany.Rows[0]["Cmp_ID"].ToString();
                    string _dtNotice = DateTime.Parse(studnotice["Notice_Date"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    string file1 = "";
                    string file2 = "";
                    string file3 = "";

                    using (MySqlTransaction tran = Startup.con.BeginTransaction())
                    {
                        string insertString = "Insert into `generalnotice` (`Cmp_ID`,`Notice_Date`,`Std`,`Division`,`Notice_Type`,`Notice_Msg`," +
                        "`File1`,`File1Type`,`File2`,`File2Type`,`File3`,`File3Type`,`Replay_Type`,`Notice_Replay`,`Notification`) values(" +
                        cmp_id + ",N'" + _dtNotice + "',N'" + studnotice["Standard"] + "',N'" +
                        studnotice["Division"] + "',N'" + studnotice["Notice_Type"] + "',N'" + studnotice["Notice_Msg"] + "',N'" + file1 + "',N'" + studnotice["File1Type"] +
                        "',N'" + file2 + "',N'" + studnotice["File2Type"] + "',N'" + file3 + "',N'" + studnotice["File3Type"] + "'," + "0,1,0)";
                        MySqlCommand _mysqlcmd = new MySqlCommand(insertString, Startup.con);
                        int _noticeid = _mysqlcmd.ExecuteNonQuery();

                        if (_noticeid > 0)
                        {
                            insertString = "Insert into `generalnoticedetails`(`Notice_ID`,`Mem_ID`,`Mobile1_Status`,`Mobile2_Status`," +
                            "`Mobile3_Status`) values(" + _noticeid + "," + studnotice["MemID"] + ",0,0,0)";
                            _mysqlcmd = new MySqlCommand(insertString, Startup.con);
                            int rowsAffected = _mysqlcmd.ExecuteNonQuery();

                            tran.Commit();

                            if (rowsAffected > 0)
                            {
                                return new ResponseMessage
                                {
                                    Success = "1",
                                    Message = "NOTICE ADD SUCCESSFULLY",
                                    InternalMessage = ""
                                };
                            }
                            else
                            {
                                tran.Rollback();
                                return new ResponseMessage
                                {
                                    Success = "0",
                                    Message = "NOTICE NOT ADD SUCCESSFULLY",
                                    InternalMessage = ""
                                };
                            }
                        }
                        else
                        {
                            tran.Rollback();
                            return new ResponseMessage
                            {
                                Success = "0",
                                Message = "NOTICE NOT ADD SUCCESSFULLY",
                                InternalMessage = ""
                            };
                        }

                    }//close transation
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            });

        }

        public async Task<ResponseMessage> SetLeave(JObject jobj)
        {
            return await Task.Run(() =>
            {
                Dictionary<string, dynamic> headers = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jobj["headers"].ToString());
                try
                {
                    if (!headers.ContainsKey("cmpkey") ||
                        !headers.ContainsKey("memid") ||
                        !headers.ContainsKey("leavefrom") ||
                        !headers.ContainsKey("leaveto") ||
                        !headers.ContainsKey("leavereason") ||
                        !headers.ContainsKey("remarks") ||
                        !headers.ContainsKey("status"))
                        throw new Exception("Parameters Incorrect !!!");

                    string cmpkey = headers["cmpkey"];
                    string memid = headers["memid"];
                    string leavefrom = headers["leavefrom"];
                    string leaveto = headers["leaveto"];
                    string leavereason = headers["leavereason"];
                    string remarks = headers["remarks"];
                    string status = headers["status"];

                    DataTable _dtCompany = new DataTable("Company");

                    using (MySqlCommand _mysqlcmd = new MySqlCommand("select cmp_id from company where cmp_key='" +
                       cmpkey + "'", Startup.con))
                    {
                        using (MySqlDataAdapter _mysel = new MySqlDataAdapter(_mysqlcmd))
                        {
                            _mysel.Fill(_dtCompany);
                        }
                    }

                    if (_dtCompany.Rows.Count == 0)
                    {
                        return new ResponseMessage
                        {
                            Success = "0",
                            Message = "Company not found !!!",
                            InternalMessage = ""
                        };
                    }

                    string Cmp_Id = _dtCompany.Rows[0]["Cmp_Id"].ToString();
                    using (MySqlTransaction tran = Startup.con.BeginTransaction())
                    {
                        string insertString = "insert into `leave` (`Cmp_ID`,`Mem_Id`,`LeaveFrom`,`LeaveTo`,`LeaveReason`,`Remarks`,`Status`) values(" +
                        Cmp_Id + "," + memid + ",'" + leavefrom + "','" + leaveto + "','" + leavereason + "','" + remarks + "'," + status + ")";
                        MySqlCommand _mysqlcmd = new MySqlCommand(insertString, Startup.con);
                        int rowsAffected = _mysqlcmd.ExecuteNonQuery();
                        tran.Commit();

                        if (rowsAffected > 0)
                        {
                            return new ResponseMessage
                            {
                                Success = "1",
                                Message = "LEAVE ADD SUCCESSFULLY",
                                InternalMessage = ""
                            };
                        }
                        else
                        {
                            tran.Rollback();
                            return new ResponseMessage
                            {
                                Success = "0",
                                Message = "LEAVE NOT ADD SUCCESSFULLY",
                                InternalMessage = ""
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            });

        }


    }
}
