using API.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace API
{
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    public class Startup
    {
        public static MySqlConnection con;
        public static string _constring;
        public static string _companyLockID = "", _companyLockKey = "";
        public static bool connProgress = false;
        //private string ErrorlineNo, Errormsg, extype, exurl, hostIp, ErrorLocation, HostAdd;
        public static DBLogic dbl = new DBLogic();
        public CancellationTokenSource cts = new CancellationTokenSource();
        public CancellationToken token;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, Microsoft.AspNetCore.Hosting.IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseOwin(x => x.UseNancy());

            applicationLifetime.ApplicationStarted.Register(OnStartup);
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }

        private async void OnStartup()
        {
            token = cts.Token;
            await Connection(token);
        }

        private void OnShutdown()
        {
            cts.Cancel();
            ConClose();
        }


        public static async Task Connection(CancellationToken token)
        {
            try
            {
                connProgress = true;
                Console.WriteLine("MYSQL Connecton is in progress... !!!");
                _constring = "server=dbialphacrm.cixxkjoxvink.ap-south-1.rds.amazonaws.com;port=3342;uid=alphacrm;pwd=Dhairya951!;database=digitaldiary;charset=utf8;Pooling=True;";
                //_constring = "server=alphaerp.in;uid=digitald2;pwd=Alpha654!;database=yogesh_digitaldiary;charset=utf8;Pooling=True;";
                _companyLockID = ConfigurationManager.AppSettings.Get("cmpid");
                _companyLockKey = ConfigurationManager.AppSettings.Get("cmpkey");
                con = new MySqlConnection(_constring);
                await ConOpen(token);
            }
            catch (Exception ex)
            {
                connProgress = false;
                //Log this error to file system
            }
        }

        public static async Task ConOpen(CancellationToken token)
        {
            try
            {
                if (con.State != System.Data.ConnectionState.Open && con.State != System.Data.ConnectionState.Connecting)
                {
                    await con.OpenAsync(token);
                    Console.WriteLine("MYSQL Connecton is Opened... !!!");
                }

            }
            catch (Exception ex)
            {
                connProgress = false;
                Console.WriteLine("MYSQL Error " + ex.Message);
                //Log this error to file system
            }

        }

        public static void ConClose()
        {
            try
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    Console.WriteLine("MYSQL Connecton is Closed... !!!");
                }

            }
            catch (Exception ex)
            {
                //Log this error to file system
            }

        }


    }

}
