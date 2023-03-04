using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DSharpPlus
{

    // This class is used to extend the DiscordBot client, You can add any methods you want directly into the client.
    static class DiscordClientExt
    {

        // Creates a task that runs every x time interval.
        public static async Task CreatePeriodicTask(this DiscordClient client, Func<Task> action, TimeSpan interval, CancellationToken cancellationToken = default)
        {
        
            while (true)
            {
                var delayTask = Task.Delay(interval, cancellationToken);
                await action();
                await delayTask;
            }

        }

    }

}
