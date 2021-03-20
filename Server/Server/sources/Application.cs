using System;

namespace Server
{
    class Application
    {
        /// <summary>
        /// メインスレッド
        /// 60FPS
        /// </summary>
        private static ApplicationThread appThread = new ApplicationThread(60);

        /// <summary>
        /// ネットワーク通信スレッド
        /// 30FPS
        /// </summary>
        private static NetworkThread networkThread = new NetworkThread(30);

        /// <summary>
        /// エントリーポイント
        /// </summary>
        private static void Main()
        {
            Console.WriteLine("サーバー起動");

            //各スレッドの起動
            networkThread.StartThread();
            appThread.StartThread();
        }
    }
}
