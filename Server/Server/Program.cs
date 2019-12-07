using System;

class MainClass
{
	public static void Main (string[] args)
	{
		if(!DbManager.Connect("sustech_majiang", "127.0.0.1", 3306, "root", "123456")){
			return;
		}
        XMLManager.ReadXML();
		NetManager.StartLoop(8888);
	}
}

