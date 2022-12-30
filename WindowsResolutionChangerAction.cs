using StreamDeckLib;
using StreamDeckLib.Messages;
using System.Runtime.InteropServices;
using System;
using System.Threading.Tasks;
using Windows.Foundation;

namespace WindowsResolutionChanger
{
    [ActionUuid(Uuid="WindowsResolutionChanger.jcfiorenzano.DefaultPluginAction")]
    public class WindowsResolutionChangerAction : BaseStreamDeckActionWithSettingsModel<Models.ScreenResolutionModel>
    {
        [DllImport("user32.dll", EntryPoint = "ChangeDisplaySettingsA")]
        public static extern int ChangeDisplaySettingsA([In] IntPtr lpDevMode, uint dwFlags);

        public override async Task OnKeyUp(StreamDeckEventPayload args)
        {

            SettingsModel.Width = 1920;
            SettingsModel.Heigth = 1080;
            //   await Manager.SetTitleAsync(args.context, SettingsModel.Counter.ToString());

            //   if (SettingsModel.Counter % 10 == 0)
            //   {
            // 	await Manager.ShowAlertAsync(args.context);
            //   }
            //   else if (SettingsModel.Counter % 15 == 0)
            //   {
            // 	await Manager.OpenUrlAsync(args.context, "https://www.bing.com");
            //   }
            //   else if (SettingsModel.Counter % 3 == 0)
            //   {
            // 	await Manager.ShowOkAsync(args.context);
            //   }
            //   else if (SettingsModel.Counter % 7 == 0)
            //   {
            // 	await Manager.SetImageAsync(args.context, "images/Fritz.png");
            //   }

            //update settings
            await Manager.SetSettingsAsync(args.context, SettingsModel);
            DisplayManager.ChangeResolution(SettingsModel.Width, SettingsModel.Heigth);
        }

        public override async Task OnDidReceiveSettings(StreamDeckEventPayload args)
        {
            await base.OnDidReceiveSettings(args);
            await Manager.SetTitleAsync(args.context, "");
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);
            await Manager.SetTitleAsync(args.context, "");
        }

    }
}
