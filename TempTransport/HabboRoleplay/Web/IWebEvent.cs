using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fleck;

using Plus.HabboHotel.GameClients;

namespace Bobba.HabboRoleplay.Web
{
    public interface IWebEvent
    {
        void Execute(GameClient Client, string Data, IWebSocketConnection Socket);
    }
}
