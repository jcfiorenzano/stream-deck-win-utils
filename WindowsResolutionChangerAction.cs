using StreamDeckLib;
using StreamDeckLib.Messages;
using System.Threading.Tasks;
using WindowsResolutionChanger.Models;

namespace WindowsResolutionChanger
{
    [ActionUuid(Uuid="WindowsResolutionChanger.jcfiorenzano.DefaultPluginAction")]
    public class WindowsResolutionChangerAction : BaseStreamDeckActionWithSettingsModel<Models.ScreenResolutionModel>
    {
        class WindowsResolutionChangeActionState
        {
            public bool resolutionChanged { get; set; }
        }

        private ScreenResolutionModel baseResolution;
        private WindowsResolutionChangeActionState state;

        public WindowsResolutionChangerAction()
            : base()
        {
            baseResolution = DisplayManager.GetCurrentResolution();

            state = new WindowsResolutionChangeActionState
            {
                resolutionChanged = false
            };
        }

        public override async Task OnKeyUp(StreamDeckEventPayload args)
        {

            if (SettingsModel.Width <= 0 || SettingsModel.Heigth <= 0 )
            {
                await Manager.ShowAlertAsync(args.context);
                return;
            }

            ScreenResolutionModel newResolution = SettingsModel;

            if (state.resolutionChanged)
            {
                newResolution = baseResolution;
                state.resolutionChanged = false;
            }
            else
            {
                state.resolutionChanged = true;
            }

            DisplayManager.ChangeResolution(newResolution.Width, newResolution.Heigth);

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
