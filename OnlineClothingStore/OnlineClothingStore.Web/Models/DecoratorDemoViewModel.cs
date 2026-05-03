using System.Collections.Generic;

namespace OnlineClothingStore.Models;

public class DecoratorDemoViewModel
{
    public string CustomerName { get; set; } = "Ana";
    public string Email { get; set; } = "ana@bluewear.com";
    public string PhoneNumber { get; set; } = "+37360000000";
    public string DeviceToken { get; set; } = "device-token-123";

    public bool UseEmail { get; set; }
    public bool UseSms { get; set; }
    public bool UsePush { get; set; }

    public string Message { get; set; } = "Comanda ta BlueWear a fost confirmată.";

    public List<string> Channels { get; set; } = new();
    public List<string> DecoratorChain { get; set; } = new();

    public bool WasSent { get; set; }
}