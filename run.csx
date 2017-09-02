using System;

public static void Run(TimerInfo myTimer, TraceWriter log)
{
    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

    string iothubcs = "HostName=EGIoTHubD2C.azure-devices.net;DeviceId=emulator;SharedAccessKey=lVyFihjfImMQeKiddYWoA1aFIQXnh7tMEPg4/Zr81Q8=";

    double sAccelXBase = 0;
    double sAccelYBase = 0;
    double sAccelZBase = -1;
    double sTempBase = 23;
    double sHumBase = 50;

    double dAccelXSBase = 2;
    double dAccelYSBase = 2;
    double dAccelZSBase = 2;
    double dTempBase = 32;
    double dHumBase = 72;
    var now = DateTime.Now;
    bool isStable = true;
    if ((now.Minute % 3) == 0 || (now.Minute % 4 == 0))
    {
        isStable = false;
    }

    double accelX = 0;
    double accelY = 0;
    double accelZ = 0;
    double temp = 0;
    double humidity = 0;

    var random = new Random(now.Millisecond);

    var sensorPackees = new List<SensorPacket>();
    var startTime = now - TimeSpan.FromMinutes(1);
    for (int i = 0; i < 60; i++)
    {
        var time = startTime + TimeSpan.FromSeconds(i);
        if (isStable)
        {
            GenerateStableParams(sAccelXBase, sAccelYBase, sAccelZBase, sTempBase, sHumBase, out accelX, out accelY, out accelZ, out temp, out humidity, random);
        }
        else
        {
            GenerateDynamicParams(dAccelXSBase, dAccelYSBase, dAccelZSBase, dTempBase, dHumBase, out accelX, out accelY, out accelZ, out temp, out humidity, random);
        }
        sensorPackees.Add(new SensorPacket()
        {
            AccelX = accelX,
            AccelY = accelY,
            AccelZ = accelZ,
            Temp = temp,
            Humidity = humidity,
            time = time
        });
    }
     var iothubClient = Microsoft.Azure.Devices.Client.DeviceClient.CreateFromConnectionString(iothubcs, Microsoft.Azure.Devices.Client.TransportType.Amqp);
     try
     {
         iothubClient.OpenAsync().Wait();
         var msgContent = Newtonsoft.Json.JsonConvert.SerializeObject(sensorPackees);
         var msg = new Microsoft.Azure.Devices.Client.Message(System.Text.Encoding.UTF8.GetBytes(msgContent));
         iothubClient.SendEventAsync(msg).Wait();
         log.Info("Send Events - done.");
         iothubClient.CloseAsync();
     }
     catch (Exception ex)
     {
         log.Error("Send Events - failed");
     }
        
}

        private static void GenerateDynamicParams(double dAccelXSBase, double dAccelYSBase, double dAccelZSBase, double dTempBase, double dHumBase, out double accelX, out double accelY, out double accelZ, out double temp, out double humidity, Random random)
        {
            accelX = dAccelXSBase * (0.5 - random.NextDouble());
            accelY = dAccelYSBase * (0.5 - random.NextDouble());
            accelZ = dAccelZSBase * (0.5 - random.NextDouble());
            temp = dTempBase + 0.5 * (0.5 - random.NextDouble());
            humidity = dHumBase + 2 * (0.5 - random.NextDouble());
        }

        private static void GenerateStableParams(double sAccelXBase, double sAccelYBase, double sAccelZBase, double sTempBase, double sHumBase, out double accelX, out double accelY, out double accelZ, out double temp, out double humidity, Random random)
        {
            accelX = sAccelXBase + 0.01 * (0.5 - random.NextDouble());
            accelY = sAccelYBase + 0.01 * (0.5 - random.NextDouble());
            accelZ = sAccelZBase + 0.01 * (0.5 - random.NextDouble());
            temp = sTempBase + 0.5 * (0.5 - random.NextDouble());
            humidity = sHumBase + 0.7 * (0.5 - random.NextDouble());
        }

        class SensorPacket
        {
            public double AccelX { get; set; }
            public double AccelY { get; set; }
            public double AccelZ { get; set; }
            public double Temp { get; set; }
            public double Humidity { get; set; }
            public DateTime time { get; set; }
        }

