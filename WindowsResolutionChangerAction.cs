using StreamDeckLib;
using StreamDeckLib.Messages;
using System.Threading.Tasks;

namespace WindowsResolutionChanger
{
    [ActionUuid(Uuid="WindowsResolutionChanger.jcfiorenzano.DefaultPluginAction")]
    public class WindowsResolutionChangerAction : BaseStreamDeckActionWithSettingsModel<Models.ScreenResolutionModel>
    {
        public override async Task OnKeyUp(StreamDeckEventPayload args)
        {

            if (SettingsModel.Width <= 0 || SettingsModel.Heigth <= 0 )
            {
                await Manager.ShowAlertAsync(args.context);
                return;
            }

            DisplayManager.ChangeResolution(SettingsModel.Width, SettingsModel.Heigth);

            await Manager.SetSettingsAsync(args.context, SettingsModel);
        }

        public override async Task OnDidReceiveSettings(StreamDeckEventPayload args)
        {
            await base.OnDidReceiveSettings(args);
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);
        }

    }
}
