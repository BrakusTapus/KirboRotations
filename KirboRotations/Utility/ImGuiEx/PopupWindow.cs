using Dalamud.Interface;
using Dalamud.Interface.Utility;
using ECommons.DalamudServices;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirboRotations.Utility.ImGuiEx
{
    public class PopupWindow : IDisposable
    {
        public string Text = "";
        public PopupWindow(string Text)
        {
            this.Text = Text;
            Svc.PluginInterface.UiBuilder.Draw += UiBuilder_Draw;
        }

        public void Dispose()
        {
            Svc.PluginInterface.UiBuilder.Draw -= UiBuilder_Draw;
        }

        private void UiBuilder_Draw()
        {
            ImGuiHelpers.ForceNextWindowMainViewport();
        }
    }
}
