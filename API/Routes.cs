using API.Models;
using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class Routes : NancyModule
    {
        private BeforePipeline bpl;
        public Routes()
        {

            Before += async (ctx, token) =>
            {
                if (Startup.con.State != System.Data.ConnectionState.Connecting &&
                Startup.con.State == System.Data.ConnectionState.Closed)
                {
                    await Startup.ConOpen(token);
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN CONNECTION",
                        InternalMessage = "CONNECTON IS IN PROGRESS"
                    });
                }
                else if (Startup.con.State == System.Data.ConnectionState.Connecting)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN CONNECTION",
                        InternalMessage = "MYSQL CONNECTION IS IN PROGRESS"
                    });
                }
                else
                    return null;
            };

            Get("/", args =>
             {
                 return Request.Headers.ToDictionary(key => key, value => value);
             });

            Get("/GetOTP", async (args) =>
            {
                try
                {
                    User user = await Startup.dbl.GetOTP(new JObject {
                        {
                            "headers" ,
                            JsonConvert.SerializeObject(await GetRequestHeaders())
                        }
                    });

                    if (user == null)
                    {
                        user = new User
                        {
                            Success = "0" as string,
                            Message = "USER NOT FOUND SUCCESSFULLY" as string,
                            Mobile = "",
                            Password = ""
                        };
                    }
                    return user;
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Get("/GetParentsLoginTokenBase64", async (args) =>
            {
                try
                {
                    return await Startup.dbl.GetParentsLoginTokenBase64(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                 });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }


            });

            Get("/GetCompany", async (args) =>
            {
                try
                {
                    return await Startup.dbl.GetCompany(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                 });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Get("/GetCompanyImages", async (args) =>
            {
                try
                {
                    return await Startup.dbl.GetCompanyImages(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                 });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Get("/GetAttendance", async (args) =>
            {
                try
                {
                    return await Startup.dbl.GetAttendance(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                 });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Get("/GetMonthlyAttendances", async (args) =>
            {
                try
                {
                    return await Startup.dbl.GetMonthlyAttendances(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                 });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Get("/GetDailyAttendance", async (args) =>
            {
                try
                {
                    return await Startup.dbl.GetDailyAttendance(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                 });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Get("/GetTest", async (args) =>
            {
                try
                {
                    return await Startup.dbl.GetTest(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                 });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Get("/GetMonthlyTests", async (args) =>
            {
                try
                {
                    return await Startup.dbl.GetMonthlyTests(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                 });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Get("/GetTimetable", async (args) =>
            {
                try
                {
                    return await Startup.dbl.GetTimetable(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                 });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Get("/GetFees", async (args) =>
            {
                try
                {
                    return await Startup.dbl.GetFees(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                 });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Get("/GetLeave", async (args) =>
            {
                try
                {
                    return await Startup.dbl.GetLeave(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                 });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Get("/GetPublicNotice", async (args) =>
            {
                try
                {
                    return await Startup.dbl.GetPublicNotice(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                 });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Get("/GetNotice", async (args) =>
             {
                 try
                 {
                     return await Startup.dbl.GetNotice(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                    });
                 }
                 catch (Exception ex)
                 {
                     return Response.AsJson(new ResponseMessage
                     {
                         Success = "101",
                         Message = "ERROR IN DATA",
                         InternalMessage = ex.Message
                     });
                 }

             });

            Get("/GetNoticeMobilebased", async (args) =>
            {
                try
                {
                    return await Startup.dbl.GetNoticeMobilebased(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Get("/GetChat", async (args) =>
            {
                try
                {
                    return await Startup.dbl.GetChat(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Get("/GetPhotoGallery", async (args) =>
            {
                try
                {
                    return await Startup.dbl.GetPhotoGallery(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Get("/GetVideoGallery", async (args) =>
            {
                try
                {
                    return await Startup.dbl.GetVideoGallery(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Get("/GetPDFGallery", async (args) =>
            {
                try
                {
                    return await Startup.dbl.GetPDFGallery(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });


            Get("/GetExam", async (args) =>
            {
                try
                {
                    return await Startup.dbl.GetExam(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Post("/SetLeave", async (args) =>
            {
                try
                {
                    return await Startup.dbl.SetLeave(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Post("/SetNotice", async (args) =>
            {
                try
                {
                    return await Startup.dbl.SetNotice(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Post("/SetChat", async (args) =>
            {
                try
                {
                    return await Startup.dbl.SetChat(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });

            Post("/SetLastVisit", async (args) =>
            {
                try
                {
                    return await Startup.dbl.SetLastVisit(new JObject {
                    {
                        "headers" ,
                        JsonConvert.SerializeObject(await GetRequestHeaders())
                    }
                });
                }
                catch (Exception ex)
                {
                    return Response.AsJson(new ResponseMessage
                    {
                        Success = "101",
                        Message = "ERROR IN DATA",
                        InternalMessage = ex.Message
                    });
                }

            });
        }


        public Task<Dictionary<string, string>> GetRequestHeaders()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            return Task.Run(() =>
            {

                foreach (var item in Request.Headers)
                {
                    List<string> lstValues = item.Value.ToList();
                    if (lstValues.Count == 1)
                        dic.Add(item.Key, lstValues[0]);
                }

                return dic;
            });
        }
    }
}
