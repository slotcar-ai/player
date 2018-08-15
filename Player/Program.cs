using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Player
{

    class Program
    {

        public static int Main(String[] args)
        {
            using (var track = new TrackConnection())
            {
                GameLoop(track);
            }
            return 0;
        }

        private static void GameLoop(TrackConnection track)
        {
            var onOff = false;
            while (true)
            {
                if (!string.IsNullOrEmpty(track.GetLatestResponse()))
                {
                    Console.WriteLine(track.GetLatestResponse());
                }
                var random = new Random();
                var speed = onOff ? random.Next(60, 100) : 0;
                track.SendSpeed(speed);
                onOff = !onOff;
                Thread.Sleep(1000);
            }
        }
    }
}