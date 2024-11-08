using DesafioPOO.Models;

Console.WriteLine("Smartphone Nokia:");
Smartphone nokia = new Nokia("123456789", "Nokia 123", "123456789", 128);
nokia.Ligar();
nokia.InstalarAplicativo("WhatsApp");

Console.WriteLine("\n");

Console.WriteLine("Smartphone Iphone:");
Smartphone iphone = new Iphone("987654321", "Iphone 4", "987654321", 256);
iphone.Ligar();
iphone.InstalarAplicativo("Telegram");