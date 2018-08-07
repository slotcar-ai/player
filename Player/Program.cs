using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Player {
    
    class Program {
      
        public static int Main (String[] args) {
            var track = new TrackConnection();
            
            while (true)
            {
                if (!string.IsNullOrEmpty(track.GetLatestResponse()))
                {
                    Console.WriteLine(track.GetLatestResponse());
                }
            }
            

            return 0;
        }

    }
}