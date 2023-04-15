using BlazorStrap;
using Desktop.AI.App.Interop.SpeechToText;
using Desktop.AI.App.Interop.TextToSpeech;
using ElectronNET.API;
using ElectronNET.API.Entities;

using OpenAI.Net;

namespace Desktop.AI.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
                o.MaximumReceiveMessageSize = long.MaxValue;
            });

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            builder.Services.AddOpenAIServices(o =>
            {
                o.ApiKey = builder.Configuration["OpenAI:ApiKey"];
            });

            //Interop
            builder.Services.AddTransient<ISpeechToText,SpeechToText>();
            builder.Services.AddTransient<ITextToSpeech, TextToSpeech>();

            builder.Services.AddElectron();
            builder.WebHost.UseElectron(args);

            builder.Services.AddBlazorStrap();

            if (HybridSupport.IsElectronActive)
            {
                // Open the Electron-Window here
                Task.Run(async () => {
                    var window = await Electron
                    .WindowManager
                    .CreateWindowAsync(new BrowserWindowOptions()
                        { 
                            AutoHideMenuBar = true,
                        }
                        );

                    window.OnClosed += () => {
                        Electron.App.Quit();
                    };
                });
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}