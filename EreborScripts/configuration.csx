using System.Net;

public IPEndPoint localhost = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593);
public IPEndPoint ereborServer = new IPEndPoint(IPAddress.Parse("89.185.244.24"), 2593);
public IPEndPoint currentConnection = localhost;
