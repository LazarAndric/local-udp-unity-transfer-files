using g12;
using System;

namespace TestServer
{
    class Program
    {
        static UDPController uDPCOntroller;
        static void Main(string[] args)
        {
            Start();
            while (true)
            {
                Update();
            }

        }
        public static void Start()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            uDPCOntroller = new UDPController();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            uDPCOntroller.OnDisable();
        }

        public static void Update()
        {
            uDPCOntroller.Update();
        }

    }
}
